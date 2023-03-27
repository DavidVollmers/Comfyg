using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Comfyg.Store.Contracts.Secrets;

namespace Comfyg.Store.Contracts.Requests;

/// <summary>
/// Request object used to add Comfyg secret values.
/// </summary>
public class AddSecretValuesRequest : AddValuesRequest<ISecretValue>
{
    /// <summary>
    /// The secret values to be added.
    /// </summary>
    [Required]
    [JsonConverter(
        typeof(ContractConverter<IEnumerable<ISecretValue>, IEnumerable<SecretValue>, IEnumerable<IComfygValue>>))]
    public override IEnumerable<ISecretValue> Values { get; init; } = null!;
}
