using Comfyg.Timing;

namespace Comfyg;

public sealed class ComfygValuesOptions
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

    internal ComfygValuesOptions(TimeSpan? defaultChangeDetectionInterval = null)
    {
    }

    public ComfygValuesOptions DetectChanges(TimeSpan interval)
    {
        ChangeDetectionInterval = interval;
        return this;
    }

    internal ComfygValuesOptions OverrideChangeDetectionTimer(ITimer timer)
    {
        ChangeDetectionTimerOverride = timer ?? throw new ArgumentNullException(nameof(timer));
        return this;
    }
}
