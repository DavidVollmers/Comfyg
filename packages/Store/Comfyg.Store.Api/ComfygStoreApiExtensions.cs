using Comfyg.Store.Authentication;
using Comfyg.Store.Authentication.Abstractions;
using Comfyg.Store.Contracts;
using Comfyg.Store.Core;
using Comfyg.Store.Core.Abstractions;
using Comfyg.Store.Core.Abstractions.Secrets;
using Comfyg.Store.Core.Secrets;

namespace Comfyg.Store.Api;

internal static class ComfygStoreApiExtensions
{
    private const string HealthCheckValue = "COMFYG_HEALTH_CHECK";

    public static WebApplicationBuilder UseComfygStoreApi(this WebApplicationBuilder builder)
    {
        if (builder == null) throw new ArgumentNullException(nameof(builder));

        builder.Configuration.AddEnvironmentVariables("COMFYG_");

        if (builder.Environment.IsDevelopment())
        {
            builder.Configuration.AddUserSecrets<Program>();
        }

        builder.Services.AddComfygAuthentication(options =>
        {
            var tableConnectionString = builder.Configuration["AuthenticationAzureTableStorageConnectionString"];
            if (tableConnectionString != null) options.UseAzureTableStorage(tableConnectionString);

            var blobConnectionString = builder.Configuration["AuthenticationAzureBlobStorageConnectionString"];
            if (blobConnectionString != null) options.UseAzureBlobStorage(blobConnectionString);

            var encryptionKey = builder.Configuration["AuthenticationEncryptionKey"];
            if (encryptionKey != null) options.UseEncryption(encryptionKey);

            var keyVaultUri = builder.Configuration["AuthenticationKeyVaultUri"];
            if (keyVaultUri != null) options.UseAzureKeyVault(keyVaultUri);
        });

        builder.Services.AddComfyg(options =>
        {
            var connectionString = builder.Configuration["SystemAzureTableStorageConnectionString"];
            if (connectionString != null) options.UseAzureTableStorage(connectionString);

            var encryptionKey = builder.Configuration["SystemEncryptionKey"];
            if (encryptionKey != null) options.UseEncryption(encryptionKey);

            var keyVaultUri = builder.Configuration["SystemKeyVaultUri"];
            if (keyVaultUri != null) options.UseAzureKeyVault(keyVaultUri);
        });

        return builder;
    }

    public static async Task CheckHealthAsync(this WebApplication app)
    {
        if (bool.TryParse(app.Configuration["DoHealthCheckOnStartup"], out var doHealthCheckOnStartup) &&
            doHealthCheckOnStartup) return;

        var systemClientId = app.Configuration["SystemClientId"];
        var systemClientSecret = app.Configuration["SystemClientSecret"];
        if (systemClientId == null || systemClientSecret == null) app.Logger.LogWarning("No system client configured.");
        else if (Convert.FromBase64String(systemClientSecret).Length < 16)
            app.Logger.LogError(
                "Your system client secret byte length is smaller than 16 bytes. This will produce errors when trying to connect to your local Comfyg store.");
        else
        {
            app.Logger.LogInformation(
                "You can connect to this Comfyg store using the following connection string:");
            app.Logger.LogInformation(
                $"Endpoint={app.Urls.First()};ClientId={systemClientId};ClientSecret={systemClientSecret};");
        }

        await app.CheckAuthenticationHealthAsync();

        await app.CheckSystemHealthAsync();
    }

    private static Task CheckAuthenticationHealthAsync(this WebApplication app)
    {
        try
        {
            using var scope = app.Services.CreateScope();

            //TODO test client logic
            var _ = scope.ServiceProvider.GetRequiredService<IClientService>();
        }
        catch (Exception exception)
        {
            app.Logger.LogError(exception, "The authentication system of the Comfyg store is not healthy.");
        }

        return Task.CompletedTask;
    }

    private static async Task CheckSystemHealthAsync(this WebApplication app)
    {
        try
        {
            using var scope = app.Services.CreateScope();

            var secretService = scope.ServiceProvider.GetRequiredService<ISecretService>();
            if (secretService is not KeyVaultSecretService)
            {
                var protectedValue = await secretService.ProtectSecretValueAsync(HealthCheckValue);
                var unprotectedValue = await secretService.UnprotectSecretValueAsync(protectedValue);
                if (unprotectedValue != HealthCheckValue)
                    throw new Exception("Secret encryption roundtrip did not succeed.");
            }

            //TODO test value service logic
            var configurationValueService =
                scope.ServiceProvider.GetRequiredService<IValueService<IConfigurationValue>>();
            var secretValueService = scope.ServiceProvider.GetRequiredService<IValueService<ISecretValue>>();
            var settingValueService = scope.ServiceProvider.GetRequiredService<IValueService<ISettingValue>>();
        }
        catch (Exception exception)
        {
            app.Logger.LogError(exception, "The Comfyg store system is not healthy.");
        }
    }
}
