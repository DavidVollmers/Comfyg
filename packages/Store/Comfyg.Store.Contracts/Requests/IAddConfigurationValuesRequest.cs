using System.Text.Json.Serialization;

namespace Comfyg.Store.Contracts.Requests;

/// <summary>
/// Request object used to add Comfyg configuration values.
/// </summary>
[JsonConverter(typeof(ContractConverter<IAddConfigurationValuesRequest, Implementation>))]
public interface IAddConfigurationValuesRequest : IAddValuesRequest<IConfigurationValue>
{
    // ReSharper disable once ClassNeverInstantiated.Local
    private class Implementation : IAddConfigurationValuesRequest
    {
        public IEnumerable<IConfigurationValue> Values { get; init; } = null!;
    }
}
