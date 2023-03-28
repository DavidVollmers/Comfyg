using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Comfyg.Store.Contracts;
using Comfyg.Store.Contracts.Requests;
using Comfyg.Store.Contracts.Secrets;

namespace Comfyg.Store.Api.Requests;

public sealed class AddSecretValuesRequest : IAddValuesRequest<ISecretValue>
{
    [Required]
    [JsonConverter(
        typeof(ContractConverter<IEnumerable<ISecretValue>, IEnumerable<SecretValue>, IEnumerable<IComfygValue>>))]
    public IEnumerable<ISecretValue> Values { get; init; } = null!;
}
