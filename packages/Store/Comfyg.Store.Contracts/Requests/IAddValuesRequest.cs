using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Comfyg.Store.Contracts.Requests;

/// <summary>
/// Generic request object used to add Comfyg values.
/// </summary>
[JsonConverter(typeof(ContractConverter<IAddValuesRequest<IComfygValue>, Implementation>))]
public interface IAddValuesRequest<out T> where T : IComfygValue
{
    /// <summary>
    /// The values to be added.
    /// </summary>
    [Required]
    IEnumerable<T> Values { get; }

    // ReSharper disable once ClassNeverInstantiated.Local
    private class Implementation : IAddValuesRequest<IComfygValue>
    {
        public IEnumerable<IComfygValue> Values { get; init; } = null!;
    }
}
