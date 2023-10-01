using System;

namespace HackYeah.Backend.Keystore.Data;

public sealed class DbCertificate
{
    public Guid Id { get; set; }

    public string CommonName { get; set; }

    public string Thumbprint { get; set; }

    public byte[] Data { get; set; }

    public Guid KeyPairId { get; set; }

    public DbCryptoKeyMaterial KeyPair { get; set; }
}
