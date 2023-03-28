using System.ComponentModel.DataAnnotations;
using Comfyg.Store.Contracts;
using Comfyg.Store.Contracts.Requests;

namespace Comfyg.Store.Api.Requests;

public sealed class AddValuesRequest<T> : IAddValuesRequest<T> where T : IComfygValue
{
    [Required] public IEnumerable<T> Values { get; init; } = null!;
}
