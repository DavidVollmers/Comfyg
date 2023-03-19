using Comfyg.Api;
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

    var encryptionKey = builder.Configuration["ComfygAuthenticationEncryptionKey"];
    if (encryptionKey != null) options.UseEncryption(encryptionKey);
    else options.UseAzureKeyVault();
});

builder.Services.AddComfyg(options =>
{
    options.UseAzureTableStorage(builder.Configuration["ComfygSystemAzureTableStorageConnectionString"]);

    var encryptionKey = builder.Configuration["ComfygSystemEncryptionKey"];
    if (encryptionKey != null) options.UseEncryption(encryptionKey);
    else options.UseAzureKeyVault();
});

var app = builder.Build();

app.MapControllers();

app.UseAuthentication();
app.UseAuthorization();

await app.StartAsync().ConfigureAwait(false);

if (app.Environment.IsDevelopment())
{
    app.LogConnectionHint();
}

await app.WaitForShutdownAsync().ConfigureAwait(false);

// Required for integration tests
// https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-8.0#basic-tests-with-the-default-webapplicationfactory
public partial class Program
{
}