namespace Comfyg.Contracts.Responses;

public abstract class GetValuesResponse<T> where T : IComfygValue
{
    public abstract IEnumerable<T> Values { get; }
}