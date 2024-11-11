using capa_datos.Clases.Middlewares;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
Env.Load();
builder.Configuration.AddEnvironmentVariables();

builder.Services.AddControllers();

var app = builder.Build();

app.UseMiddleware<AuthMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
