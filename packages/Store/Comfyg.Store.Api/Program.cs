﻿using Comfyg.Store.Api;
using Comfyg.Store.Authentication;
using Comfyg.Store.Core;

var builder = WebApplication.CreateBuilder(args);

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