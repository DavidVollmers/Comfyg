using System.Text.Json.Serialization;
using Comfyg.Contracts.Secrets;

namespace Comfyg.Contracts.Responses;

public sealed class GetSecretValuesResponse
{
    [JsonConverter(
        typeof(ContractConverter<IEnumerable<ISecretValue>, IEnumerable<SecretValue>, IEnumerable<IComfygValue>>))]
    public IEnumerable<ISecretValue> SecretValues { get; }

    public GetSecretValuesResponse(IEnumerable<ISecretValue> secretValues)
    {
        SecretValues = secretValues ?? throw new ArgumentNullException(nameof(secretValues));
    }
}