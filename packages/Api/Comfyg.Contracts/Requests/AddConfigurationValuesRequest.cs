using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Comfyg.Contracts.Configuration;

namespace Comfyg.Contracts.Requests;

public sealed class AddConfigurationValuesRequest
{
    [Required]
    [JsonConverter(
        typeof(ContractConverter<IEnumerable<IConfigurationValue>, IEnumerable<ConfigurationValue>,
            IEnumerable<IComfygValue>>))]
    public IEnumerable<IConfigurationValue> ConfigurationValues { get; set; } = null!;
}