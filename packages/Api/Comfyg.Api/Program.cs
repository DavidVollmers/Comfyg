using Comfyg.Authentication;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

builder.Services.AddControllers();

builder.Services.AddComfygAuthentication(options =>
{
    options.UseAzureTableStorage(builder.Configuration["AuthenticationAzureTableStorageConnectionString"]);
    options.UseEncryption(builder.Configuration["AuthenticationEncryptionKey"]);
});

var app = builder.Build();

app.MapControllers();

app.UseAuthentication();
app.UseAuthorization();

app.Run();

// Required for integration tests
// https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-8.0#basic-tests-with-the-default-webapplicationfactory
public partial class Program
{
}