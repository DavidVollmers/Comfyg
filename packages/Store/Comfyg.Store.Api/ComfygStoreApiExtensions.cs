using Comfyg.Store.Authentication;
using Comfyg.Store.Core;

namespace Comfyg.Store.Api;

// needed for E2E tests
internal static class ComfygStoreApiExtensions
{
    public static WebApplicationBuilder UseComfygStoreApi(this WebApplicationBuilder builder)
    {
        if (builder == null) throw new ArgumentNullException(nameof(builder));

        builder.Configuration.AddEnvironmentVariables("COMFYG_");

        if (builder.Environment.IsDevelopment())
        {
            builder.Configuration.AddUserSecrets<Program>();
        }

        builder.Services.AddControllers();

        builder.Services.AddComfygAuthentication(options =>
        {
            options.UseAzureTableStorage(builder.Configuration["AuthenticationAzureTableStorageConnectionString"]);

            var encryptionKey = builder.Configuration["AuthenticationEncryptionKey"];
            if (encryptionKey != null) options.UseEncryption(encryptionKey);
            else options.UseAzureKeyVault();
        });

        builder.Services.AddComfyg(options =>
        {
            options.UseAzureTableStorage(builder.Configuration["SystemAzureTableStorageConnectionString"]);

            var encryptionKey = builder.Configuration["SystemEncryptionKey"];
            if (encryptionKey != null) options.UseEncryption(encryptionKey);
            else options.UseAzureKeyVault();
        });

        return builder;
    }

    public static void LogConnectionHint(this WebApplication app)
    {
        try
        {
            var systemClientId = app.Configuration["SystemClientId"];
            var systemClientSecret = app.Configuration["SystemClientSecret"];
            if (systemClientId == null || systemClientSecret == null)
            {
                app.Logger.LogWarning(
                    "No system client configured. To be able to use all Comfyg store features you should configure both SystemClientId and SystemClientSecret.");
                return;
            }

            if (Convert.FromBase64String(systemClientSecret).Length < 16)
            {
                app.Logger.LogError(
                    "Your system client secret byte length is smaller than 16 bytes. This will produce errors when trying to connect to your local Comfyg store.");
                return;
            }

            app.Logger.LogInformation(
                "You can connect to your local Comfyg store using the following connection string:");
            app.Logger.LogInformation(
                $"Endpoint={app.Urls.First()};ClientId={systemClientId};ClientSecret={systemClientSecret};");
        }
        catch
        {
            // ignored
        }
    }
}
