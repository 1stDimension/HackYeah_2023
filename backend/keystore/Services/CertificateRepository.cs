using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using HackYeah.Backend.Keystore.Crypto;
using HackYeah.Backend.Keystore.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            Thumbprint = cert.Thumbprint,
            HasPrivateKey = keyPair.Private is not null
        };
    }

    public async Task<IEnumerable<CryptoCert>> GetCertificatesAsync(CancellationToken cancellationToken)
    {
        var dbcerts = await this._db.Certificates
            .Include(x => x.KeyPair)
            .ToListAsync(cancellationToken);

        return dbcerts.Select(x => new CryptoCert()
        {
            Id = x.Id,
            CommonName = x.CommonName,
            Thumbprint = x.Thumbprint,
            HasPrivateKey = x.KeyPair.PrivateKey is not null
        });
    }

    public async Task<string> GetCertificateAsync(Guid id, CancellationToken cancellationToken)
    {
        var dbcert = await this._db.Certificates
            .Include(x => x.KeyPair)
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        if (dbcert is null)
            return default;

        return this.GetCertificate(dbcert);
    }

    public async Task<string> GetCertificateAsync(string thumbprint,  CancellationToken cancellationToken)
    {
        var dbcert = await this._db.Certificates
            .Include(x => x.KeyPair)
            .FirstOrDefaultAsync(x => x.Thumbprint == thumbprint, cancellationToken);

        if (dbcert is null)
            return default;

        return this.GetCertificate(dbcert);
    }

    private string GetCertificate(DbCertificate dbcert)
    {
        var cert = new X509Certificate2(dbcert.Data);
        var certPem = cert.ExportCertificatePem();

        if (dbcert.KeyPair.PrivateKey is null)
            return certPem;

        var certKey = KeyParser.ExportAsymmetricKey(dbcert.KeyPair, true);
        return certPem + "\n" + certKey.Data;
    }
}
