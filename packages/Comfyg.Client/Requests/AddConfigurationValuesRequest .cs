using Comfyg.Store.Contracts;
using Comfyg.Store.Contracts.Requests;

namespace Comfyg.Client.Requests;

internal class AddConfigurationValuesRequest : IAddConfigurationValuesRequest
{
    public IEnumerable<IConfigurationValue> Values { get; }

    public AddConfigurationValuesRequest(IEnumerable<IConfigurationValue> values)
    {
        Values = values ?? throw new ArgumentNullException(nameof(values));
    }
}
