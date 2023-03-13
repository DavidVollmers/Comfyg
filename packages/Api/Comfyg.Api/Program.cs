using Comfyg.Authentication;
using Comfyg.Core;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}
else
{
    builder.Configuration.AddEnvironmentVariables(nameof(Comfyg));
}

builder.Services.AddControllers();

builder.Services.AddComfygAuthentication(options =>
{
    options.UseAzureTableStorage(builder.Configuration["ComfygAuthenticationAzureTableStorageConnectionString"]);
    options.UseEncryption(builder.Configuration["ComfygAuthenticationEncryptionKey"]);
});

builder.Services.AddComfyg(options =>
{
    options.UseAzureTableStorage(builder.Configuration["ComfygSystemAzureTableStorageConnectionString"]);
});

var app = builder.Build();

app.MapControllers();

app.UseAuthentication();
app.UseAuthorization();

await app.StartAsync().ConfigureAwait(false);

if (app.Environment.IsDevelopment())
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
        app.Logger.LogInformation("You can connect to your local Comfyg API using the following connection string:");
        app.Logger.LogInformation(
            $"Endpoint={app.Urls.First()};ClientId={systemClientId};ClientSecret={systemClientSecret};");
    }
}

await app.WaitForShutdownAsync().ConfigureAwait(false);

// Required for integration tests
// https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-8.0#basic-tests-with-the-default-webapplicationfactory
public partial class Program
{
}