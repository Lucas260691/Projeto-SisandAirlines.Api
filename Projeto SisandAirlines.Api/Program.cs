using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SisandAirlines.Application.Services;
using SisandAirlines.Domain.Interfaces;
using SisandAirlines.Infrastructure.UnitOfWork;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// =======================================================
// 1️⃣ Carrega configurações (appsettings + .env + variáveis de ambiente)
// =======================================================
Env.Load(); // Lê variáveis do .env local

builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables(); // Permite sobrescrever configs via Docker/CI/CD

// =======================================================
// 2️⃣ Configuração de conexão com PostgreSQL
// =======================================================
var dbHost = builder.Configuration["DB_HOST"] ?? "localhost";
var dbPort = builder.Configuration["DB_PORT"] ?? "5432";
var dbName = builder.Configuration["DB_NAME"] ?? "sisand_airlines";
var dbUser = builder.Configuration["DB_USER"] ?? "postgres";
var dbPassword = builder.Configuration["DB_PASSWORD"] ?? "123456";

var connectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword}";

Console.WriteLine($"✅ Conectando ao banco: {connectionString}");

// =======================================================
// 3️⃣ Serviços de infraestrutura e aplicação
// =======================================================
builder.Services.AddScoped<IUnitOfWork>(_ => new UnitOfWork(connectionString));
builder.Services.AddScoped<IFlightScheduler, FlightService>();
builder.Services.AddScoped<FlightService>();
builder.Services.AddScoped<BookingService>();
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<PaymentService>();
builder.Services.AddHostedService<SisandAirlines.Infrastructure.BackgroundJobs.FlightSchedulerJob>();
// =======================================================
// 4️⃣ Configuração do JWT
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
// 5️⃣ Configuração de CORS dinâmica
// =======================================================
var corsOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? new[] { "http://localhost:4200" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(corsOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// =======================================================
// 6️⃣ Swagger e Controllers
// =======================================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Sisand Airlines API",
        Version = "v1",
        Description = "Sistema de compra de passagens aéreas Curitiba ↔ São Paulo"
    });
});

// =======================================================
// 7️⃣ Construção do pipeline HTTP
// =======================================================
var app = builder.Build();

app.UseCors("AllowFrontend"); // 🔥 Aplica CORS antes da autenticação

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
