using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HackYeah.Backend.Keystore.Crypto;
using HackYeah.Backend.Keystore.Data;
using Microsoft.EntityFrameworkCore;

namespace HackYeah.Backend.Keystore.Services;

public sealed class CryptoKeyRepository
{
    private readonly KeystoreContext _db;

    public CryptoKeyRepository(KeystoreContext db)
    {
        this._db = db;
    }

    public async Task<CryptoKey> AddKeyAsync(
        Guid id,
        string name,
        KeyType type,
        uint size,
        Stream material,
        CancellationToken cancellationToken)
    {
        var keyPair = await KeyParser.ParseKeyAsync(material, type, size, cancellationToken);
        if (keyPair.Private is null || keyPair.Public is null)
            return default;

        await this._db.KeyMaterial.AddAsync(new()
        {
            Id = id,
            Name = name,
            Type = type,
            Size = size,
            PublicKey = keyPair.Public,
            PrivateKey = keyPair.Private,
        }, cancellationToken);

        await this._db.SaveChangesAsync(cancellationToken);

        return new()
        {
            Id = id,
            Name = name
        };
    }

    public async Task<IEnumerable<CryptoKey>> GetKeysAsync(CancellationToken cancellationToken)
    {
        var keys = await this._db.KeyMaterial
            .Select(x => new { x.Id, x.Name })
            .ToListAsync(cancellationToken);

        return keys.Select(x => new CryptoKey { Id = x.Id, Name = x.Name });
    }

    public async Task<KeyData> GetKeyAsync(Guid id, CancellationToken cancellationToken)
    {
        var dbKey = await this._db.KeyMaterial.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (dbKey is null)
            return default;

        return KeyParser.ExportKey(dbKey);
    }

    public async Task<KeyData> GetAsymmetricKeyAsync(Guid id, bool @private, CancellationToken cancellationToken)
    {
        var dbKey = await this._db.KeyMaterial.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (dbKey is null)
            return default;

        return KeyParser.ExportAsymmetricKey(dbKey, @private);
    }
}
