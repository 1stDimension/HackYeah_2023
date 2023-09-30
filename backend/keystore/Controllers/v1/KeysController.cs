using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using HackYeah.Backend.Keystore.Crypto;
using HackYeah.Backend.Keystore.Data;
using HackYeah.Backend.Keystore.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace HackYeah.Backend.Keystore.Controllers;

[ApiController, Route("/v1/keys")]
public sealed class KeysController : ControllerBase
{
    private readonly CryptoKeyRepository _repo;
    private readonly ILogger<KeysController> _logger;

    public KeysController(ILoggerFactory loggerFactory, CryptoKeyRepository repo)
    {
        this._logger = loggerFactory.CreateLogger<KeysController>();
        this._repo = repo;
    }

    [HttpPost, Route(""), Consumes("multipart/form-data")]
    public async Task<ActionResult<CryptoKey>> AddKeyAsync(
        [FromForm(Name = "name")] string keyName,
        [FromForm(Name = "size")] uint keySize,
        [FromForm(Name = "type")] KeyType keyType,
        [FromForm(Name = "data")] IFormFile keyMaterial,
        CancellationToken cancellationToken = default)
    {
        try
        {
            using var stream = keyMaterial.OpenReadStream();
            var cryptoKey = await this._repo.AddKeyAsync(
            id: Guid.NewGuid(),
            name: keyName,
            type: keyType,
            size: keySize,
            material: stream,
            cancellationToken);

            if (cryptoKey.Name is null)
                return this.BadRequest();

            return this.Ok(cryptoKey);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Exception occured while adding key");
            return this.BadRequest();
        }
    }

    [HttpGet, Route("")]
    public async Task<ActionResult<IEnumerable<CryptoKey>>> GetKeysAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return this.Ok(await this._repo.GetKeysAsync(cancellationToken));
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Exception occured while enumerating keys");
            return this.BadRequest();
        }
    }

    [HttpGet, Route("{id}")]
    public async Task<ActionResult<CryptoKey>> GetKeyAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var key = await this._repo.GetKeyAsync(id, cancellationToken);
            if (key.Name is null)
                return this.NotFound();

            return this.Ok(key);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Exception occured while getting key {0}", id);
            return this.BadRequest();
        }
    }

    [HttpGet, Route("{id}/public")]
    public async Task<ActionResult<CryptoKey>> GetPublicKeyAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var key = await this._repo.GetAsymmetricKeyAsync(id, false, cancellationToken);
            if (key.Name is null)
                return this.NotFound();

            return this.Ok(key);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Exception occured while getting pub key {0}", id);
            return this.BadRequest();
        }
    }

    [HttpGet, Route("{id}/private")]
    public async Task<ActionResult<CryptoKey>> GetPrivateKeyAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var key = await this._repo.GetAsymmetricKeyAsync(id, true, cancellationToken);
            if (key.Name is null)
                return this.NotFound();

            return this.Ok(key);
        }
        catch (Exception ex)
        {
            this._logger.LogError(ex, "Exception occured while getting priv key {0}", id);
            return this.BadRequest();
        }
    }
}
