using Comfyg.Store.Contracts;
using Comfyg.Store.Contracts.Requests;

namespace Comfyg.Client.Requests;

internal class AddSettingValuesRequest : IAddSettingValuesRequest
{
    public IEnumerable<ISettingValue> Values { get; }

    public AddSettingValuesRequest(IEnumerable<ISettingValue> values)
    {
        Values = values ?? throw new ArgumentNullException(nameof(values));
    }
}
