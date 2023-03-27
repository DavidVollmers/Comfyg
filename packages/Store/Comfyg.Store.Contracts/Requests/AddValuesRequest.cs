namespace Comfyg.Store.Contracts.Requests;

/// <summary>
/// Generic request object used to add Comfyg values.
/// </summary>
public abstract class AddValuesRequest<T> where T : IComfygValue
{
    /// <summary>
    /// The values to be added.
    /// </summary>
    public abstract IEnumerable<T> Values { get; init; }

    internal AddValuesRequest() { }
}
