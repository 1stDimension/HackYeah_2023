using System;
using System.Text.Json.Serialization;

namespace HackYeah.Backend.Keystore.Data;

public class CryptoCert
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("cn")]
    public string CommonName { get; set; }

    [JsonPropertyName("thumbprint")]
    public string Thumbprint { get; set; }

    [JsonPropertyName("has_key")]
    public bool HasPrivateKey { get; set; }
}
