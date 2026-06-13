using Microsoft.EntityFrameworkCore;
using EnviosRapidosGT.API.Data;
using EnviosRapidosGT.API.Services;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new()
    {
        Title = "Envíos Rápidos GT - API",
        Version = "v1",
        Description = "API REST para gestión de envíos y paquetería - Envíos Rápidos GT"
    });
});

// Database - SQLite
var dbPath = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=EnviosRapidosGT.db";
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(dbPath));

// DI
builder.Services.AddScoped<IEnvioService, EnvioService>();

// CORS para Render
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var app = builder.Build();

// Auto-migrate on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Envíos Rápidos GT API v1");
    c.RoutePrefix = string.Empty; // Swagger en raíz
});

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();

// Needed for xUnit integration tests
public partial class Program { }
