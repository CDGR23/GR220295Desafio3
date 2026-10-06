using System.Text;
using GR220295_Desafio3.API.Services;
using GR220295_Desafio3.BLL.Services;
using GR220295_Desafio3.DAL.Data;
using GR220295_Desafio3.DAL.Services;
using GR220295_Desafio3.Entities.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opciones =>
{
    opciones.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "GR220295 - Desafío 3",
        Version = "v1"
    });

    opciones.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Ingrese el token JWT obtenido al iniciar sesión."
    });

    opciones.AddSecurityRequirement(documento => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("bearer", documento)] = []
    });
});

builder.Services.AgregarCapaDatos(builder.Configuration);
builder.Services.AgregarCapaNegocio();

builder.Services.AddIdentityCore<Usuario>(opciones =>
{
    opciones.User.RequireUniqueEmail = true;
    opciones.Password.RequireDigit = true;
    opciones.Password.RequireUppercase = true;
    opciones.Password.RequireLowercase = true;
    opciones.Password.RequireNonAlphanumeric = true;
    opciones.Password.RequiredLength = 8;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddSignInManager();

var claveJwt = builder.Configuration["Jwt:Clave"] ?? throw new InvalidOperationException("No se configuró Jwt:Clave.");
var emisorJwt = builder.Configuration["Jwt:Emisor"] ?? "GR220295_Desafio3.API";
var audienciaJwt = builder.Configuration["Jwt:Audiencia"] ?? "GR220295_Desafio3.MVC";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opciones =>
    {
        opciones.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = emisorJwt,
            ValidAudience = audienciaJwt,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(claveJwt))
        };
    });

builder.Services.AddAuthorization();
builder.Services.AddSingleton<JwtTokenService>();

var app = builder.Build();

using (var alcance = app.Services.CreateScope())
{
    var contexto = alcance.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var administradorUsuarios = alcance.ServiceProvider.GetRequiredService<UserManager<Usuario>>();
    var administradorRoles = alcance.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    await SemillaDatos.InicializarAsync(contexto, administradorUsuarios, administradorRoles);
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

public partial class Program { }
