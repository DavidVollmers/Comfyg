using Comfyg.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddComfygAuthentication();

var app = builder.Build();

app.MapControllers();

app.UseAuthentication();

app.Run();