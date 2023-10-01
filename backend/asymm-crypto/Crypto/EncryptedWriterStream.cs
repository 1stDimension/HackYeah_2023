using System;
using System.Buffers;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace HackYeah.Backend.AsymmetricCrypto.Crypto;

public sealed class EncryptedWriterStream : Stream, IDisposable
{
    internal static readonly byte[] _signature = new byte [] { 0x45, 0x4D, 0x5A, 0x49, 0x30, 0x37, 0x36, 0x37 };

    public override bool CanRead
        => false;

    public override bool CanSeek
        => false;

    public override bool CanWrite
        => true;

    public override long Length
        => throw new NotSupportedException();

    public override long Position
    {
        get => throw new NotSupportedException();
        set => throw new NotSupportedException();
    }

    private readonly Stream _inner;
    private readonly BinaryWriter _binary;
    private readonly RSA _rsa;
    private readonly byte[] _tag;
    private AesGcm _aead;
    private Rfc2898DeriveBytes _kdf;

    private bool _headerWritten = false;
    private bool _finalized = false;

    public EncryptedWriterStream(Stream inner, RSA rsa, string aeadTag)
    {
        this._inner = inner;
        this._binary = new(this._inner, Encoding.UTF8, true);
        this._rsa = rsa;
        this._tag = Convert.FromBase64String(aeadTag);
    }

    public void WriteHeader(EphemeralKeyProperties eph)
    {
        if (this._headerWritten)
            throw new InvalidOperationException("Header is already written.");

        this._kdf = new Rfc2898DeriveBytes(eph.KdfInput, eph.KdfSalt, eph.Iterations, HashAlgorithmName.SHA512);
        var key = this._kdf.GetBytes(32);
        eph.Kdf = this._kdf.GetType().Name;
        this._aead = new AesGcm(key);

        using (var ms = new MemoryStream())
        {
            JsonSerializer.Serialize(ms, eph);
            var buff = ms.GetBuffer();
            var spBuff = buff.AsSpan(0, (int)ms.Length);
            buff = this._rsa.Encrypt(spBuff, RSAEncryptionPadding.OaepSHA256);

            this._binary.Write((byte)CryptoBlockType.Header);
            this._binary.Write7BitEncodedInt64(_signature.Length + buff.Length);
            this._inner.Write(_signature);
            this._inner.Write(buff);
        }

        this._headerWritten = true;
    }

    public async Task WriteHeaderAsync(EphemeralKeyProperties eph, CancellationToken cancellationToken)
    {
        if (this._headerWritten)
            throw new InvalidOperationException("Header is already written.");

        this._kdf = new Rfc2898DeriveBytes(eph.KdfInput, eph.KdfSalt, eph.Iterations, HashAlgorithmName.SHA512);
        var key = this._kdf.GetBytes(32);
        eph.Kdf = this._kdf.GetType().Name;
        this._aead = new AesGcm(key);

        using (var ms = new MemoryStream())
        {
            JsonSerializer.Serialize(ms, eph);
            var buff = ms.ToArray();
            buff = this._rsa.Encrypt(buff, RSAEncryptionPadding.OaepSHA256);

            this._binary.Write((byte)CryptoBlockType.Header);
            this._binary.Write7BitEncodedInt64(_signature.Length + buff.Length);
            await this._inner.WriteAsync(_signature, cancellationToken);
            await this._inner.WriteAsync(buff, cancellationToken);
        }

        this._headerWritten = true;
    }

    public void FinalizeStream() 
        => this._binary.Write((byte)CryptoBlockType.EOF);

    public override int Read(byte[] buffer, int offset, int count)
        => throw new NotSupportedException();

    public override void Write(byte[] buffer, int offset, int count)
    {
        if (!this._headerWritten)
            throw new InvalidOperationException("Cannot write to a stream before header is written.");

        this._inner.WriteByte((byte)CryptoBlockType.Data);
        var nonce = new byte[AesGcm.NonceByteSizes.MaxSize];
        var tag = new byte[AesGcm.TagByteSizes.MaxSize];
        using var memOwner = MemoryPool<byte>.Shared.Rent(buffer.Length);
        var mem = memOwner.Memory[..buffer.Length];

        this._aead.Encrypt(nonce, buffer, mem.Span, tag, this._tag);

        var size = mem.Length + nonce.Length + tag.Length;
        this._binary.Write7BitEncodedInt64(size);
        this._binary.Write7BitEncodedInt(nonce.Length);
        this._binary.Write7BitEncodedInt(tag.Length);
        this._inner.WriteAsync(mem);
        this._inner.WriteAsync(nonce);
        this._inner.WriteAsync(tag);
    }

    public override async Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        => await this.WriteAsync(buffer.AsMemory(offset, count), cancellationToken);

    public override async ValueTask WriteAsync(ReadOnlyMemory<byte> buffer, CancellationToken cancellationToken = default)
    {
        if (!this._headerWritten)
            throw new InvalidOperationException("Cannot write to a stream before header is written.");

        this._inner.WriteByte((byte)CryptoBlockType.Data);
        var nonce = new byte[AesGcm.NonceByteSizes.MaxSize];
        var tag = new byte[AesGcm.TagByteSizes.MaxSize];
        using var memOwner = MemoryPool<byte>.Shared.Rent(buffer.Length);
        var mem = memOwner.Memory[..buffer.Length];

        this._aead.Encrypt(nonce, buffer.Span, mem.Span, tag, this._tag);

        var size = mem.Length + nonce.Length + tag.Length;
        this._binary.Write7BitEncodedInt64(size);
        this._binary.Write7BitEncodedInt(nonce.Length);
        this._binary.Write7BitEncodedInt(tag.Length);
        await this._inner.WriteAsync(mem, cancellationToken);
        await this._inner.WriteAsync(nonce, cancellationToken);
        await this._inner.WriteAsync(tag, cancellationToken);
    }

    public override void Flush()
        => this._inner.Flush();

    public override Task FlushAsync(CancellationToken cancellationToken) 
        => this._inner.FlushAsync(cancellationToken);

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
