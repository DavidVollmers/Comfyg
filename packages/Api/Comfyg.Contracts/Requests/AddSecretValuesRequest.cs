using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Comfyg.Contracts.Secrets;

namespace Comfyg.Contracts.Requests;

public class AddSecretValuesRequest
{
    [Required]
    [JsonConverter(
        typeof(ContractConverter<IEnumerable<ISecretValue>, IEnumerable<SecretValue>, IEnumerable<IComfygValue>>))]
    public IEnumerable<ISecretValue> SecretValues { get; set; } = null!;
}