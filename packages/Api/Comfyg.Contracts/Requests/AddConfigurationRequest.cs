using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Comfyg.Contracts.Configuration;

namespace Comfyg.Contracts.Requests;

public sealed class AddConfigurationRequest
{
    [Required]
    [JsonConverter(typeof(ContractConverter<IConfigurationValue[], ConfigurationValue[]>))]
    public IConfigurationValue[] ConfigurationValues { get; set; } = null!;
}