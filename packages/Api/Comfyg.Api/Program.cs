using Comfyg.Authentication;
using CoreHelpers.WindowsAzure.Storage.Table;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

builder.Services.AddControllers();

builder.Services.AddSingleton<IStorageContext>(_ =>
{
    var connectionString = builder.Configuration["AzureTableStorageConnectionString"];
    return new StorageContext(connectionString);
});

builder.Services.AddComfygAuthentication();

var app = builder.Build();

app.MapControllers();

app.UseAuthentication();

app.Run();

// Required for integration tests
// https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-8.0#basic-tests-with-the-default-webapplicationfactory
public partial class Program
{
}