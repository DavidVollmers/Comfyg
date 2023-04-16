using Comfyg.Timing;

namespace Comfyg;

/// <summary>
/// Options used to configure the behavior of a specific Comfyg value type.
/// </summary>
public sealed class ComfygValuesOptions
{
    internal TimeSpan? ChangeDetectionInterval { get; private set; }

    internal ITimer? ChangeDetectionTimerOverride { get; private set; }

    internal IList<string> Tags { get; } = new List<string>();

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
    /// <returns><see cref="ComfygValuesOptions"/></returns>
    public ComfygValuesOptions DetectChanges(TimeSpan interval)
    {
        ChangeDetectionInterval = interval;
        return this;
    }

    /// <summary>
    /// Loads key-value pairs tagged with the provided tags.
    /// </summary>
    /// <param name="tags">The tags to load key-valur pairs for.</param>
    /// <exception cref="ArgumentNullException">An element of <paramref name="tags"/> is null.</exception>
    /// <returns><see cref="ComfygOptions"/></returns>
    public ComfygValuesOptions LoadTags(params string[] tags)
    {
        foreach (var tag in tags)
        {
            if (tag == null) throw new ArgumentNullException(nameof(tags));
            Tags.Add(tag);
        }

        return this;
    }

    internal ComfygValuesOptions OverrideChangeDetectionTimer(ITimer timer)
    {
        ChangeDetectionTimerOverride = timer ?? throw new ArgumentNullException(nameof(timer));
        return this;
    }
}
