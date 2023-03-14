using Comfyg.Timing;

namespace Comfyg;

public sealed class ComfygOptions
{
    internal string? ConnectionString { get; private set; }

    internal TimeSpan? ConfigurationChangeDetectionInterval { get; private set; }

    internal ITimer? ConfigurationChangeDetectionTimerOverride { get; private set; }

    internal ITimer? ConfigurationChangeDetectionTimer
    {
        get
        {
            if (ConfigurationChangeDetectionTimerOverride != null) return ConfigurationChangeDetectionTimerOverride;
            return ConfigurationChangeDetectionInterval.HasValue
                ? new TimerImplementation(ConfigurationChangeDetectionInterval.Value)
                : null;
        }
    }

    internal HttpClient? HttpClient { get; private set; }

    internal ComfygOptions()
    {
    }

    public ComfygOptions Connect(string connectionString)
    {
        ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        return this;
    }

    public ComfygOptions DetectConfigurationChanges(TimeSpan interval)
    {
        ConfigurationChangeDetectionInterval = interval;
        return this;
    }

    internal ComfygOptions OverrideHttpClient(HttpClient httpClient)
    {
        HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        return this;
    }

    internal ComfygOptions OverrideConfigurationChangeDetectionTimer(ITimer timer)
    {
        ConfigurationChangeDetectionTimerOverride = timer ?? throw new ArgumentNullException(nameof(timer));
        return this;
    }
}