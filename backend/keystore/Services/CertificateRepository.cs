using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using HackYeah.Backend.Keystore.Crypto;
using HackYeah.Backend.Keystore.Data;

namespace HackYeah.Backend.Keystore.Services;

public sealed class CertificateRepository
{
    private readonly KeystoreContext _db;

    public CertificateRepository(KeystoreContext db)
    {
        this._db = db;
    }

    public async Task<CryptoCert> AddCertificateAsync(
        Guid id,
        Stream data,
        CancellationToken cancellationToken)
    {
        var cert = await KeyParser.LoadCertificateAsync(data, cancellationToken);
        var keyPair = KeyParser.ExtractKeyPair(cert, out var keyType, out var keySize);
        if (keyPair.Public is null)
            return default;

        var keyMaterial = new DbCryptoKeyMaterial
        {
            Id = Guid.NewGuid(),
            Name = cert.GetNameInfo(X509NameType.SimpleName, false),
            Type = keyType,
            Size = keySize,
            PrivateKey = keyPair.Private,
            PublicKey = keyPair.Public,
        };

        var dbcert = new DbCertificate
        {
            Id = id,
            CommonName = keyMaterial.Name,
            Thumbprint = cert.Thumbprint,
            KeyPair = keyMaterial,
            Data = cert.Export(X509ContentType.Cert)
        };

        await this._db.KeyMaterial.AddAsync(keyMaterial, cancellationToken);
        await this._db.Certificates.AddAsync(dbcert, cancellationToken);
        await this._db.SaveChangesAsync(cancellationToken);

        return new()
        {
            Id = id,
            CommonName = keyMaterial.Name,
            Thumbprint = cert.Thumbprint
        };
    }
}
