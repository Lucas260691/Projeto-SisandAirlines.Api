using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SisandAirlines.Application.Services;
using SisandAirlines.Domain.Interfaces;
using SisandAirlines.Infrastructure.UnitOfWork;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// =======================================================
// 1️⃣ Carrega configurações (appsettings + .env + variáveis)
// =======================================================
Env.Load(); // .env local
builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables(); // permite override via Docker/CI/CD

// =======================================================
// 2️⃣ Monta a string de conexão do PostgreSQL
// =======================================================
var dbHost = builder.Configuration["DB_HOST"] ?? "localhost";
var dbPort = builder.Configuration["DB_PORT"] ?? "5432";
var dbName = builder.Configuration["DB_NAME"] ?? "sisand_airlines";
var dbUser = builder.Configuration["DB_USER"] ?? "postgres";
var dbPassword = builder.Configuration["DB_PASSWORD"] ?? "123456";

var connectionString =
    $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword}";

Console.WriteLine($"✅ Conectando em: {connectionString}");

// =======================================================
// 3️⃣ Registra serviços de infraestrutura e aplicação
// =======================================================
builder.Services.AddScoped<IUnitOfWork>(_ => new UnitOfWork(connectionString));
builder.Services.AddScoped<FlightService>();
builder.Services.AddScoped<BookingService>();

// =======================================================
// 4️⃣ Configura autenticação JWT
// =======================================================
var jwtSecret = builder.Configuration["JWT_SECRET"] ?? "default_secret_key";
var jwtIssuer = builder.Configuration["JWT_ISSUER"] ?? "SisandAirlines";
var jwtAudience = builder.Configuration["JWT_AUDIENCE"] ?? "SisandAirlinesUsers";

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

// =======================================================
// 5️⃣ Configurações padrão e Swagger
// =======================================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Sisand Airlines API",
        Version = "v1",
        Description = "Sistema de compra de passagens aéreas Curitiba ↔ São Paulo"
    });
});

// =======================================================
// 6️⃣ Monta o pipeline HTTP
// =======================================================
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Sisand Airlines API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
