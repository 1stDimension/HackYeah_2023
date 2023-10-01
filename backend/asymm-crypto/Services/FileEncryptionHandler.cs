using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using HackYeah.Backend.AsymmetricCrypto.Crypto;
using HackYeah.Backend.AsymmetricCrypto.Data;
using Microsoft.Extensions.Options;

namespace HackYeah.Backend.AsymmetricCrypto.Services;

public class FileEncryptionHandler
{
    private readonly Config _config;
    private readonly HttpClient _http;

    public FileEncryptionHandler(IOptions<Config> config)
    {
        this._config = config.Value;
        this._http = new();
    }

    public async Task EncryptAsync(Stream input, Stream output, Guid keyId, CancellationToken cancellationToken)
    {
        using var req = new HttpRequestMessage(HttpMethod.Get, new UriBuilder(this._config.KeyStore) { Path = "/v1/keys/" + keyId.ToString() + "/public" }.Uri);
        using var res = await this._http.SendAsync(req, cancellationToken);
        using var rsa = RSA.Create();

        var kdata = await res.Content.ReadFromJsonAsync<KeyData>(options: null, cancellationToken);
        if (kdata.Type != KeyType.RSA)
            throw new InvalidOperationException("Invalid key type.");

        rsa.ImportFromPem(kdata.Data);
        var enc = new EncryptedWriterStream(output, rsa, this._config.Tag);

        var kdfInput = new byte[64];
        var kdfSalt = new byte[32];
        RandomNumberGenerator.Fill(kdfInput);
        RandomNumberGenerator.Fill(kdfSalt);
        await enc.WriteHeaderAsync(new() { KdfInput = kdfInput, KdfSalt = kdfSalt, Iterations = RandomNumberGenerator.GetInt32(10000, 15000) }, cancellationToken);

        await input.CopyToAsync(enc, cancellationToken);
        enc.FinalizeStream();
    }

    public async Task DecryptAsync(Stream input, Stream output, Guid keyId, CancellationToken cancellationToken = default)
    {
        using var req = new HttpRequestMessage(HttpMethod.Get, new UriBuilder(this._config.KeyStore) { Path = "/v1/keys/" + keyId.ToString() + "/private" }.Uri);
        using var res = await this._http.SendAsync(req, cancellationToken);
        using var rsa = RSA.Create();

        var kdata = await res.Content.ReadFromJsonAsync<KeyData>(options: null, cancellationToken);
        if (kdata.Type != KeyType.RSA)
            throw new InvalidOperationException("Invalid key type.");

        rsa.ImportFromPem(kdata.Data);
        var enc = new EncryptedReaderStream(input, rsa, this._config.Tag);

        await enc.ReadHeaderAsync(cancellationToken);
        await enc.CopyToAsync(output, cancellationToken);
    }
}
