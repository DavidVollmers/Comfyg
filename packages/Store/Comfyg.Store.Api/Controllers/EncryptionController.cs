using Comfyg.Store.Authentication.Abstractions;
using Comfyg.Store.Core.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Comfyg.Store.Api.Controllers;

[Authorize]
[ApiController]
[Route("encryption")]
public class EncryptionController : ControllerBase
{
    private const string EncryptionKeyBlobId = "e2ee.key";
    
    private readonly IBlobService _blobService;

    public EncryptionController(IBlobService blobService)
    {
        _blobService = blobService;
    }
    
    [HttpGet("key")]
    public async Task<IActionResult> GetEncryptionKeyAsync(CancellationToken cancellationToken = default)
    {
        if (User.Identity is not IClientIdentity) return Forbid();

        var doesExist = await _blobService.DoesBlobExistAsync(EncryptionKeyBlobId, cancellationToken);
        if (!doesExist) return NotFound();

        var encryptionKey = await _blobService.DownloadBlobAsync(EncryptionKeyBlobId, cancellationToken);
        
        return File(encryptionKey, "application/octet-stream");
    }
}
