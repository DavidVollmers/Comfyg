using Comfyg;

var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    builder.Configuration.AddUserSecrets<Program>();
}

builder.Configuration.AddComfyg(options =>
{
    var connectionString = builder.Configuration["ComfygConnectionString"];
    options.Connect(connectionString);
});

var app = builder.Build();

app.MapControllers();

app.Run();