using System.Text.Json.Serialization;

namespace HackYeah.Backend.AsymmetricCrypto.Crypto;

public sealed class EphemeralKeyProperties
{
    [JsonPropertyName("k")]
    public byte[] KdfInput { get; set; }

    [JsonPropertyName("s")]
    public byte[] KdfSalt { get; set; }

    [JsonPropertyName("d")]
    public string Kdf { get; set; }

    [JsonPropertyName("i")]
    public int Iterations { get; set; }

    [JsonPropertyName("t")]
    public byte[] AuthenticationTag { get; set; }
}
