using Comfyg.Store.Api;

var builder = WebApplication.CreateBuilder(args);

builder.UseComfygStoreApi();

var app = builder.Build();

app.MapControllers();

app.UseAuthentication();
app.UseAuthorization();

await app.StartAsync();

if (app.Environment.IsDevelopment())
{
    app.LogConnectionHint();
}

await app.WaitForShutdownAsync();

// Required for integration tests
// https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-8.0#basic-tests-with-the-default-webapplicationfactory
public partial class Program
{
}
