using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Comfyg.Store.Contracts.Configuration;

namespace Comfyg.Store.Contracts.Requests;

/// <summary>
/// Request object used to add Comfyg configuration values.
/// </summary>
public sealed class AddConfigurationValuesRequest : AddValuesRequest<IConfigurationValue>
{
    /// <summary>
    /// The configuration values to be added.
    /// </summary>
    [Required]
    [JsonConverter(
        typeof(ContractConverter<IEnumerable<IConfigurationValue>, IEnumerable<ConfigurationValue>,
            IEnumerable<IComfygValue>>))]
    public override IEnumerable<IConfigurationValue> Values { get; init; } = null!;
}
