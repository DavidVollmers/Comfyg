using Comfyg.Timing;

namespace Comfyg;

/// <summary>
/// Options used to configure the behavior of a specific Comfyg value type.
/// </summary>
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

    /// <summary>
    /// Set the interval in which change detection is performed.
    /// </summary>
    /// <param name="interval">The interval in which change detection is performed.</param>
    /// <returns>The provided <see cref="ComfygValuesOptions"/>.</returns>
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
