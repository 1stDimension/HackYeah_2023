using Microsoft.AspNetCore.Mvc;

namespace HackYeah.Backend.AsymmetricCrypto.Controllers;

[ApiController]
[Route("v1")]
public sealed class CryptoController : ControllerBase
{
    [HttpPost, Route("encrypt"), Consumes("multipart/form-data")]
    public async Task EncryptAsync(
        [FromForm(Name = "file")] IFormFile inputFile,
        [FromForm(Name = "key")] IFormFile key,
        [FromForm(Name = "keyPassword")] byte[] keyPassword)
    {

    }
}
