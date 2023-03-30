using System.Text.Json.Serialization;

namespace Comfyg.Store.Contracts.Requests;

/// <summary>
/// Request object used to add Comfyg secret values.
/// </summary>
[JsonConverter(typeof(ContractConverter<IAddSecretValuesRequest, Implementation>))]
public interface IAddSecretValuesRequest : IAddValuesRequest<ISecretValue>
{
    // ReSharper disable once ClassNeverInstantiated.Local
    private class Implementation : IAddSecretValuesRequest
    {
        public IEnumerable<ISecretValue> Values { get; init; } = null!;
    }
}
