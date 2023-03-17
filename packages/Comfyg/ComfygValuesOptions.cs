using Comfyg.Timing;

namespace Comfyg;

public abstract class ComfygValuesOptions<T> where T : ComfygValuesOptions<T>
{
    internal TimeSpan? ChangeDetectionInterval { get; private set; }

    internal ITimer? ChangeDetectionTimerOverride { get; private set; }

    internal ITimer? ChangeDetectionTimer
    {
        get
        {
            if (ChangeDetectionTimerOverride != null) return ChangeDetectionTimerOverride;
            return ChangeDetectionInterval.HasValue ? new TimerImplementation(ChangeDetectionInterval.Value) : null;
        }
    }

    internal ComfygValuesOptions()
    {
    }

    public T DetectChanges(TimeSpan interval)
    {
        ChangeDetectionInterval = interval;
        return (T)this;
    }

    internal T OverrideChangeDetectionTimer(ITimer timer)
    {
        ChangeDetectionTimerOverride = timer ?? throw new ArgumentNullException(nameof(timer));
        return (T)this;
    }
}