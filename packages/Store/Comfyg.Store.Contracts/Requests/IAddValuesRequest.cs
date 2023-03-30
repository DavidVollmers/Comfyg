using System.ComponentModel.DataAnnotations;

namespace Comfyg.Store.Contracts.Requests;

/// <summary>
/// Generic request object used to add Comfyg values.
/// </summary>
public interface IAddValuesRequest<out T> where T : IComfygValue
{
    /// <summary>
    /// The values to be added.
    /// </summary>
    [Required]
    IEnumerable<T> Values { get; }
}
