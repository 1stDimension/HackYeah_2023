using System;
using System.Threading;
using System.Threading.Tasks;
using HackYeah.Backend.AsymmetricCrypto.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HackYeah.Backend.AsymmetricCrypto.Controllers;

[ApiController]
[Route("v1")]
public sealed class CryptoController : ControllerBase
{
    private readonly ILogger<CryptoController> _logger;
    private readonly FileEncryptionHandler _handler;

    public CryptoController(ILoggerFactory loggerFactory, FileEncryptionHandler handler)
    {
        this._logger = loggerFactory.CreateLogger<CryptoController>();
        this._handler = handler;
    }

    [HttpPost, Route("encrypt"), Consumes("multipart/form-data")]
    public async Task EncryptAsync(
        [FromForm(Name = "file")] IFormFile inputFile,
        [FromForm(Name = "key")] Guid keyId,
        CancellationToken cancellationToken = default)
    {
        this._logger.LogTrace("ENCRYPT FILE {0}", inputFile.FileName);
        using var input = inputFile.OpenReadStream();

        this.Response.ContentType = "application/octet-stream";
        this.Response.Headers.Add("Content-Disposition", "attachment; filename=\"" + inputFile.FileName + ".e\"");

        await this._handler.EncryptAsync(input, this.Response.Body, keyId, cancellationToken);
    }

    [HttpPost, Route("decrypt"), Consumes("multipart/form-data")]
    public async Task DecryptAsync(
        [FromForm(Name = "file")] IFormFile inputFile,
        [FromForm(Name = "key")] Guid keyId,
        CancellationToken cancellationToken = default)
    {
        this._logger.LogTrace("DECRYPT FILE {0}", inputFile.FileName);
        using var input = inputFile.OpenReadStream();

        this.Response.ContentType = "application/octet-stream";
        this.Response.Headers.Add("Content-Disposition", "attachment; filename=\"" + inputFile.FileName[..inputFile.FileName.LastIndexOf('.')] + "\"");

        await this._handler.DecryptAsync(input, this.Response.Body, keyId, cancellationToken);
    }
}
