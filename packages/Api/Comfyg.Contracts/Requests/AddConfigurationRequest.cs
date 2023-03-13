using System.ComponentModel.DataAnnotations;
using Comfyg.Contracts.Configuration;

namespace Comfyg.Contracts.Requests;

public sealed class AddConfigurationRequest
{
    [Required] public IConfigurationValue[] ConfigurationValues { get; set; }
}