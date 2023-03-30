using System.Text.Json.Serialization;

namespace Comfyg.Store.Contracts.Requests;

/// <summary>
/// Request object used to add Comfyg setting values.
/// </summary>
[JsonConverter(typeof(ContractConverter<IAddSettingValuesRequest, Implementation>))]
public interface IAddSettingValuesRequest : IAddValuesRequest<ISettingValue>
{
    // ReSharper disable once ClassNeverInstantiated.Local
    private class Implementation : IAddSettingValuesRequest
    {
        public IEnumerable<ISettingValue> Values { get; init; } = null!;
    }
}
