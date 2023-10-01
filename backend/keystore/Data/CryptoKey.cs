using System;
using System.Text.Json.Serialization;

namespace HackYeah.Backend.Keystore.Data;

public class CryptoKey
{
    [JsonPropertyName("id")]
    public Guid Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
}
