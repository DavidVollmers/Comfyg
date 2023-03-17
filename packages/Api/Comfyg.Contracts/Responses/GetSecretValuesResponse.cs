using System.Text.Json.Serialization;
using Comfyg.Contracts.Secrets;

namespace Comfyg.Contracts.Responses;

public sealed class GetSecretValuesResponse : GetValuesResponse<ISecretValue>
{
    [JsonConverter(
        typeof(ContractConverter<IEnumerable<ISecretValue>, IEnumerable<SecretValue>, IEnumerable<IComfygValue>>))]
    public override IEnumerable<ISecretValue> Values { get; }

    public GetSecretValuesResponse(IEnumerable<ISecretValue> secretValues)
    {
        Values = secretValues ?? throw new ArgumentNullException(nameof(secretValues));
    }
}