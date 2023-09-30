using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;

namespace HackYeah.Backend.Keystore.Controllers.v1;

[ApiController, Route("/v1/certificates")]
public sealed class CertificatesController : ControllerBase
{
    private readonly ILogger<CertificatesController> _logger;
    
    public CertificatesController(ILoggerFactory loggerFactory)
    {
        this._logger = loggerFactory.CreateLogger<CertificatesController>();
    }

    [HttpPost, Route(""), Consumes("multipart/form-data")]
    public async Task<ActionResult> AddCertificateAsync(
        [BindRequired, FromForm(Name = "data")] IFormFile certificateFile,
        CancellationToken cancellationToken = default)
    {
        return this.NotFound();
    }
}
