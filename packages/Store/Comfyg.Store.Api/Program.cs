using System.Reflection;
using Comfyg.Store.Api;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
});

builder.UseComfygStoreApi();

builder.Services.AddSwaggerGen(options =>
{
    // https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/1684
    options.CustomOperationIds(apiDescription =>
        apiDescription.TryGetMethodInfo(out MethodInfo methodInfo) ? methodInfo.Name : null);
});

var app = builder.Build();

app.MapControllers();

app.UseAuthentication();
app.UseAuthorization();

await app.StartAsync();

await app.CheckHealthAsync();

await app.WaitForShutdownAsync();

// Required for integration tests
// https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-8.0#basic-tests-with-the-default-webapplicationfactory
public partial class Program
{
}
