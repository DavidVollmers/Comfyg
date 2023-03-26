namespace Comfyg.Store.Contracts.Requests;

public abstract class AddValuesRequest<T> where T : IComfygValue
{
    public abstract IEnumerable<T> Values { get; init; }

    internal AddValuesRequest() { }
}
