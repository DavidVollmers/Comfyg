using Comfyg.Timing;

namespace Comfyg.Tests;

public class TestTimerImplementation : ITimer
{
    private readonly IList<Action> _callbacks = new List<Action>();
    
    public TimeSpan Interval => TimeSpan.Zero;
    
    public void Dispose()
    {
    }

    public void Callback()
    {
        foreach (var callback in _callbacks)
        {
            callback();
        }
    }

    public void RegisterCallback(Action callback)
    {
        _callbacks.Add(callback);
    }
}