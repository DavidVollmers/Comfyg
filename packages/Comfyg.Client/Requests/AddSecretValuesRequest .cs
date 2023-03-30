using Comfyg.Store.Contracts;
using Comfyg.Store.Contracts.Requests;

namespace Comfyg.Client.Requests;

internal class AddSecretValuesRequest : IAddSecretValuesRequest
{
    public IEnumerable<ISecretValue> Values { get; }

    public AddSecretValuesRequest(IEnumerable<ISecretValue> values)
    {
        Values = values ?? throw new ArgumentNullException(nameof(values));
    }
}
