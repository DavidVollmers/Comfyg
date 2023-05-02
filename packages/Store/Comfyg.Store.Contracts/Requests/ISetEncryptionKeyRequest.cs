using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Comfyg.Store.Contracts.Requests;

public interface ISetEncryptionKeyRequest
{
    [Required] public IFormFile EncryptionKey { get; }

    // Must be public to use with [FromForm]
    // ReSharper disable once ClassNeverInstantiated.Global
    public sealed class Form : ISetEncryptionKeyRequest
    {
        public IFormFile EncryptionKey { get; init; } = null!;
    }
}
