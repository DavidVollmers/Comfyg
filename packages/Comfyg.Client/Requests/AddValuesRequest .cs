using Comfyg.Store.Contracts;
using Comfyg.Store.Contracts.Requests;

namespace Comfyg.Client.Requests;

internal class AddValuesRequest<T> : IAddValuesRequest<T> where T : IComfygValue
{
    public IEnumerable<T> Values { get; }

    public AddValuesRequest(IEnumerable<T> values)
    {
        Values = values ?? throw new ArgumentNullException(nameof(values));
    }
}
