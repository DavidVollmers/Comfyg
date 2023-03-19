using Comfyg;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

builder.Configuration.AddComfyg(options => { options.Connect(builder.Configuration["ComfygConnectionString"]!); });

var app = builder.Build();

app.MapControllers();

app.Run();