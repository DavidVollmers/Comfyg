using System.Reflection;
using System.Text.RegularExpressions;
using Comfyg.Store.Api;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

var assembly = Assembly.GetExecutingAssembly().GetName();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true;
});

builder.UseComfygStoreApi();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = assembly.Version!.ToString(),
        Title = "Comfyg Store API",
        Description = "Comfyg Store API to manage and retrieve key-value pairs.",
        Contact = new OpenApiContact
        {
            Name = "David Vollmers",
            Email = "david@vollmers.org",
            Url = new Uri("http://david.vollmers.org")
        },
        License = new OpenApiLicense
        {
            Name = "MIT",
            Url = new Uri("https://github.com/DavidVollmers/Comfyg/blob/main/LICENSE.txt")
        }
    });

    var documentationFile = $"{assembly.Name}.xml";
    options.IncludeXmlComments(Path.Join(AppContext.BaseDirectory, documentationFile));
    
    // https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/1684
    options.CustomOperationIds(apiDescription =>
    {
        if (!apiDescription.TryGetMethodInfo(out MethodInfo methodInfo)) return null;

        var name = methodInfo.Name;
        if (name.EndsWith("Async")) name = name[..^"Async".Length];
        return Regex.Replace(name, "([A-Z])", " $1", RegexOptions.Compiled).Trim();
    });
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
