using System;
using System.Threading;
using System.Threading.Tasks;
using HackYeah.Backend.Keystore.Data;
using HackYeah.Backend.Keystore.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace HackYeah.Backend.Keystore.Controllers.v1;

[ApiController, Route("/v1/certificates")]
public sealed class CertificatesController : ControllerBase
{
    private readonly ILogger<CertificatesController> _logger;
    private readonly CertificateRepository _repo;
    
    public CertificatesController(ILoggerFactory loggerFactory, CertificateRepository repo)
    {
        this._logger = loggerFactory.CreateLogger<CertificatesController>();
        this._repo = repo;
    }

    [HttpPost, Route(""), Consumes("multipart/form-data")]
    public async Task<ActionResult<CryptoCert>> AddCertificateAsync(
        [BindRequired, FromForm(Name = "data")] IFormFile certificateFile,
        CancellationToken cancellationToken = default)
    {
        try
        {
            this._logger.LogTrace("ADD CERT");
            using var stream = certificateFile.OpenReadStream();
            var cryptoCert = await this._repo.AddCertificateAsync(Guid.NewGuid(), stream, cancellationToken);

            if (cryptoCert is null)
                return this.BadRequest();

            return this.Ok(cryptoCert);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Exception occured while adding cert");
            return this.BadRequest();
        }
    }

    [HttpGet, Route("{id}")]
    public async Task<ActionResult> GetCertificateAsync(Guid id, CancellationToken cancellationToken)
    {
        return this.NotFound();
    }

    [HttpGet, Route("{thumbprint}")]
    public async Task<ActionResult> GetCertificateByThumbprintAsync(string thumbprint, CancellationToken cancellationToken)
    {
        return this.NotFound();
    }
}
