using Comfyg.Store.Authentication.Abstractions;
using Comfyg.Store.Contracts.Requests;
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
        if (User.Identity is not IClientIdentity {Client.IsAsymmetric: true}) return Forbid();

        var doesExist = await _blobService.DoesBlobExistAsync(EncryptionKeyBlobId, cancellationToken);
        if (!doesExist) return NotFound();

        var encryptionKey = await _blobService.DownloadBlobAsync(EncryptionKeyBlobId, cancellationToken);

        return File(encryptionKey, "application/octet-stream");
    }

    [HttpPost("key")]
    public async Task<IActionResult> SetEncryptionKeyAsync([FromForm] ISetEncryptionKeyRequest.Form request,
        CancellationToken cancellationToken = default)
    {
        if (User.Identity is not IClientIdentity {Client.IsAsymmetric: true}) return Forbid();

        var doesExist = await _blobService.DoesBlobExistAsync(EncryptionKeyBlobId, cancellationToken);
        if (doesExist) return Forbid();

        using var stream = new MemoryStream();
        await request.EncryptionKey.CopyToAsync(stream, cancellationToken);

        stream.Position = 0;

        await _blobService.UploadBlobAsync(EncryptionKeyBlobId, stream, cancellationToken: cancellationToken);

        return Ok();
    }
}
