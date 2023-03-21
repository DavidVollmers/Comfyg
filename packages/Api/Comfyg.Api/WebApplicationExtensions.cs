namespace Comfyg.Api;

internal static class WebApplicationExtensions
{
    public static void LogConnectionHint(this WebApplication app)
    {
        try
        {
            var systemClientId = app.Configuration["SystemClientId"];
            var systemClientSecret = app.Configuration["SystemClientSecret"];
            if (systemClientId == null || systemClientSecret == null)
            {
                app.Logger.LogWarning(
                    "No system client configured. To be able to use all features of the Comfyg API you should configure both SystemClientId and SystemClientSecret.");
                return;
            }

            if (Convert.FromBase64String(systemClientSecret).Length < 16)
            {
                app.Logger.LogError(
                    "Your system client secret byte length is smaller than 16 bytes. This will produce errors when trying to connect to your local Comfyg API.");
                return;
            }

            app.Logger.LogInformation(
                "You can connect to your local Comfyg API using the following connection string:");
            app.Logger.LogInformation(
                $"Endpoint={app.Urls.First()};ClientId={systemClientId};ClientSecret={systemClientSecret};");
        }
        catch
        {
            // ignored
        }
    }
}