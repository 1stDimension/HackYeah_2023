namespace HackYeah.Backend.Keystore.Crypto;

public readonly struct KeyPair
{
    /// <summary>
    /// Gets the DER bytes representing private key of the pair.
    /// </summary>
    public byte[] Private { get; }

    /// <summary>
    /// Gets the DER bytes representing the public key of the pair.
    /// </summary>
    public byte[] Public { get; }

    public KeyPair(byte[] priv, byte[] pub)
    {
        this.Private = priv;
        this.Public = pub;
    }
}
