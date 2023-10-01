namespace HackYeah.Backend.AsymmetricCrypto.Crypto;

public enum CryptoBlockType : byte
{
    Invalid = 0,
    Header = 1,
    Data = 2,
    EOF = 0xFF,
}
