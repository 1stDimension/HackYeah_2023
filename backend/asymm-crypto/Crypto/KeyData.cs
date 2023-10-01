using System;
using System.Text.Json.Serialization;

namespace HackYeah.Backend.AsymmetricCrypto.Crypto;

public readonly struct KeyData
{
    [JsonPropertyName("id")]
    public Guid Id { get; init; }

    [JsonPropertyName("name")]
    public string Name { get; init; }

    [JsonPropertyName("type"), JsonConverter(typeof(JsonStringEnumConverter))]
    public KeyType Type { get; init; }

    [JsonPropertyName("size")]
    public uint Size { get; init; }

    /// <summary>
    /// Gets the PEM string or base64-encoded binary version of the key.
    /// </summary>
    [JsonPropertyName("data")]
    public string Data { get; init; }
}
