namespace Comfyg.Timing;

internal interface ITimer : IDisposable
{
    TimeSpan Interval { get; }

    void RegisterCallback(Action callback);
}