using Comfyg;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

builder.Configuration.AddComfyg(options =>
{
    options.Connect(builder.Configuration["ComfygConnectionString"]!);

    // For demonstration purposes...
    options.Settings.DetectChanges(TimeSpan.FromSeconds(10));
});

var app = builder.Build();

app.MapControllers();

app.Run();