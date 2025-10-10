using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// =======================================================
// 1️⃣ Carrega variáveis do arquivo .env (ambiente local)
// =======================================================
Env.Load();

// =======================================================
// 2️⃣ Configura a conexão com o banco de dados PostgreSQL
// =======================================================
var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
var dbPort = Environment.GetEnvironmentVariable("DB_PORT");
var dbName = Environment.GetEnvironmentVariable("DB_NAME");
var dbUser = Environment.GetEnvironmentVariable("DB_USER");
var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");

var connectionString =
    $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword}";

builder.Services.AddSingleton(connectionString);

// =======================================================
// 3️⃣ Configuração básica do JWT (ainda sem middleware de login)
// =======================================================
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? "default_secret_key";
var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "SisandAirlines";
var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "SisandAirlinesUsers";

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
// 4️⃣ Configuração padrão de serviços
// =======================================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configura Swagger tradicional
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Sisand Airlines API",
        Version = "v1",
        Description = "Sistema de compra de passagens aéreas Curitiba ↔ São Paulo"
    });
});

var app = builder.Build();

// =======================================================
// 5️⃣ Configuração do pipeline HTTP
// =======================================================
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
