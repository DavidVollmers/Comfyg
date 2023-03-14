namespace Comfyg.Api;

internal static class WebApplicationExtensions
{
    public static void LogConnectionHint(this WebApplication app)
    {
        try
        {
            var systemClientId = app.Configuration["ComfygSystemClient"];
            var systemClientSecret = app.Configuration["ComfygSystemClientSecret"];
            if (systemClientId == null || systemClientSecret == null)
            {
                app.Logger.LogWarning(
                    "No system client configured. To be able to use all features of the Comfyg API you should configure both ComfygSystemClient and ComfygSystemClientSecret.");
            }
            else
            {
                app.Logger.LogInformation(
                    "You can connect to your local Comfyg API using the following connection string:");
                app.Logger.LogInformation(
                    $"Endpoint={app.Urls.First()};ClientId={systemClientId};ClientSecret={systemClientSecret};");
            }
        }
        catch
        {
            // ignored
        }
    }
}