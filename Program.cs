using ApiJobfy.Data;
using ApiJobfy.Services;
using ApiJobfy.models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Add DbContext with InMemory database for demo
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("ApiDb"));

// Add controllers
builder.Services.AddControllers();

// Add AutoMapper if needed (not added now for simplicity)

// Add custom services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IAuthService, AuthService>();

// Configure Authentication with JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = builder.Configuration["JwtSettings:SecretKey"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // For dev only
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

// Add authorization policy (RBAC)
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
    options.AddPolicy("FuncionarioPolicy", policy => policy.RequireRole("Funcionario", "Admin"));
    options.AddPolicy("CandidatoPolicy", policy => policy.RequireRole("Candidato"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseRouting();
app.MapGet("/", () => "API funcionando!"); // Esta linha mapeia a raiz "/".

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
