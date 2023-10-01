using System.ComponentModel.DataAnnotations;

namespace HackYeah.Backend.AsymmetricCrypto.Data;

public sealed class Config
{
    /// <summary>
    /// Gets or sets the URL for the keystore service.
    /// </summary>
    [Required]
    public string KeyStore { get; set; }

    /// <summary>
    /// Gets or sets the AEAD tag to use when encrypting data.
    /// </summary>
    [Required]
    public string Tag { get; set; }
}
