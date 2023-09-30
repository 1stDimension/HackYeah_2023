using System;
using HackYeah.Backend.Keystore.Crypto;

namespace HackYeah.Backend.Keystore.Data;

public sealed class DbCryptoKeyMaterial
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public KeyType Type { get; set; }

    public uint Size { get; set; }

    public byte[] PrivateKey { get; set; }

    public byte[] PublicKey { get; set; }
}
