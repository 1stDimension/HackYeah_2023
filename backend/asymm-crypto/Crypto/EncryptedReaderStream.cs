using System;
using System.Buffers;
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace HackYeah.Backend.AsymmetricCrypto.Crypto;

public class EncryptedReaderStream : Stream, IDisposable
{
    public override bool CanRead
        => true;

    public override bool CanSeek
        => false;

    public override bool CanWrite
        => false;

    public override long Length
        => throw new NotSupportedException();

    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    private readonly Stream _inner;
    private readonly BinaryReader _binary;
    private readonly RSA _rsa;
    private readonly byte[] _tag;
    private readonly MemoryStream _buff;
    private AesGcm _aead;
    private Rfc2898DeriveBytes _kdf;

    private bool _headerRead = false;
    private bool _finalized = false;

    public EncryptedReaderStream(Stream inner, RSA rsa, string aeadTag)
    {
        this._inner = inner;
        this._binary = new(this._inner, Encoding.UTF8, true);
        this._rsa = rsa;
        this._tag = Convert.FromBase64String(aeadTag);
        this._buff = new();
    }

    public async Task ReadHeaderAsync(CancellationToken cancellationToken)
    {
        if (this._headerRead)
            throw new InvalidOperationException("Header is already read.");

        var blockType = (CryptoBlockType)this._binary.ReadByte();
        if (blockType != CryptoBlockType.Header)
            throw new InvalidDataException("Expected header.");

        var blockLength = this._binary.Read7BitEncodedInt64();
        var sigLen = EncryptedWriterStream._signature.Length;
        if (blockLength < sigLen)
            throw new InvalidDataException("Missing signature.");

        var buff = new byte[blockLength];
        var buffRead = 0;
        while (buffRead < buff.Length)
            buffRead += await this._inner.ReadAsync(buff.AsMemory(buffRead), cancellationToken);

        if (!buff.AsSpan(0, sigLen).SequenceEqual(EncryptedWriterStream._signature))
            throw new InvalidDataException("Invalid signature.");

        buff = this._rsa.Decrypt(buff.AsSpan(sigLen), RSAEncryptionPadding.OaepSHA256);
        var eph = JsonSerializer.Deserialize<EphemeralKeyProperties>(buff) ?? throw new InvalidDataException("Invalid e-key data.");
        if (eph.Kdf != typeof(Rfc2898DeriveBytes).Name)
            throw new InvalidDataException("Invalid KDF.");

        this._kdf = new Rfc2898DeriveBytes(eph.KdfInput, eph.KdfSalt, eph.Iterations, HashAlgorithmName.SHA512);
        var key = this._kdf.GetBytes(32);
        eph.Kdf = this._kdf.GetType().Name;
        this._aead = new AesGcm(key);

        this._headerRead = true;
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        if (!this._headerRead)
            throw new InvalidOperationException("Cannot read from the stream before header is read.");

        if (this._buff.Length > 0 && (this._buff.Length > this._buff.Position))
            return this._buff.Read(buffer, offset, count);

        var blockType = (CryptoBlockType)this._binary.ReadByte();
        if (blockType == CryptoBlockType.EOF)
        {
            this._finalized = true;
            return 0;
        }

        if (blockType != CryptoBlockType.Data)
            throw new InvalidDataException("Expected data.");

        var size = this._binary.Read7BitEncodedInt64();
        var nonceLen = this._binary.Read7BitEncodedInt();
        var tagLen = this._binary.Read7BitEncodedInt();
        var cipherLen = size - nonceLen - tagLen;

        var nonce = new byte[nonceLen];
        var tag = new byte[tagLen];
        using var memOwner = MemoryPool<byte>.Shared.Rent((int)cipherLen);
        var mem = memOwner.Memory[..(int)cipherLen];
        using var plainOwner = MemoryPool<byte>.Shared.Rent((int)cipherLen);
        var plain = plainOwner.Memory[..(int)cipherLen];

        var br = 0;
        while (br < cipherLen)
            br += this._inner.Read(mem[br..].Span);

        br = 0;
        while (br < nonceLen)
            br += this._inner.Read(nonce.AsSpan(br));

        br = 0;
        while (br < tagLen)
            br += this._inner.Read(tag.AsSpan(br));

        this._aead.Decrypt(nonce, mem.Span, tag, plain.Span, this._tag);
        this._buff.Position = 0;
        this._buff.SetLength(0);
        this._buff.Write(plain.Span);
        return this._buff.Read(buffer, offset, count);
    }

    public override async Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        => await this.ReadAsync(buffer.AsMemory(offset, count), cancellationToken);

    public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
    {
        if (!this._headerRead)
            throw new InvalidOperationException("Cannot read from the stream before header is read.");

        if (this._buff.Length > 0 && (this._buff.Length > this._buff.Position))
            return this._buff.Read(buffer.Span);

        var blockType = (CryptoBlockType)this._binary.ReadByte();
        if (blockType == CryptoBlockType.EOF)
        {
            this._finalized = true;
            return 0;
        }

        if (blockType != CryptoBlockType.Data)
            throw new InvalidDataException("Expected data.");

        var size = this._binary.Read7BitEncodedInt64();
        var nonceLen = this._binary.Read7BitEncodedInt();
        var tagLen = this._binary.Read7BitEncodedInt();
        var cipherLen = size - nonceLen - tagLen;

        var nonce = new byte[nonceLen];
        var tag = new byte[tagLen];
        using var memOwner = MemoryPool<byte>.Shared.Rent((int)cipherLen);
        var mem = memOwner.Memory[..(int)cipherLen];
        using var plainOwner = MemoryPool<byte>.Shared.Rent((int)cipherLen);
        var plain = plainOwner.Memory[..(int)cipherLen];

        var br = 0;
        while (br < cipherLen)
            br += await this._inner.ReadAsync(mem[br..], cancellationToken);

        br = 0;
        while (br < nonceLen)
            br += await this._inner.ReadAsync(nonce.AsMemory(br), cancellationToken);

        br = 0;
        while (br < tagLen)
            br += await this._inner.ReadAsync(tag.AsMemory(br), cancellationToken);

        this._aead.Decrypt(nonce, mem.Span, tag, plain.Span, this._tag);
        this._buff.Position = 0;
        this._buff.SetLength(0);
        this._buff.Write(plain.Span);
        this._buff.Position = 0;
        return this._buff.Read(buffer.Span);
    }

    public override void Write(byte[] buffer, int offset, int count)
        => throw new NotSupportedException();

    public override void Flush()
        => throw new NotSupportedException();

    public override void SetLength(long value)
        => throw new NotSupportedException();

    public override long Seek(long offset, SeekOrigin origin)
        => throw new NotSupportedException();

    public void Dispose()
    {
        this._aead.Dispose();
        this._kdf.Dispose();
    }
}
