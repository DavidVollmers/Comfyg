using System.ComponentModel.DataAnnotations;
using Comfyg.Store.Contracts;

namespace Comfyg.Store.Api.Requests;

public sealed class SetupClientRequest
{
    [Required] public IClient Client { get; init; } = null!;
}
