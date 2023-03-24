namespace Comfyg.Timing;

internal class TimerImplementation : ITimer
{
    private readonly Timer _timer;
    private readonly IList<Action> _callbacks = new List<Action>();

    public TimeSpan Interval { get; }

    public TimerImplementation(TimeSpan interval)
    {
        Interval = interval;

        _timer = new Timer(Callback!, null, interval, interval);
    }

    private void Callback(object state)
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

    public void Dispose()
    {
        _timer.Dispose();
        _callbacks.Clear();
    }
}
