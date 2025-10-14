using Microsoft.EntityFrameworkCore;
using FitRank_API.Infrastructure.Persistence;
using System;
using FitRank_API.Application.Services;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Infrastructure.Repositories;
using FitRank_API.Application.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SendGrid;
using System.Text;
using Microsoft.OpenApi.Models;



var builder = WebApplication.CreateBuilder(args);
// Logging expandido
var jwtKeyFromConfig = builder.Configuration["Jwt:Key"];
var qrSecretFromConfig = builder.Configuration["QrSecret"];
var frontendUrlFromConfig = builder.Configuration["FrontendUrl"];
Console.WriteLine("JWT Key length: " + (jwtKeyFromConfig?.Length ?? 0) + " chars");
Console.WriteLine("QrSecret length: " + (qrSecretFromConfig?.Length ?? 0) + " chars");
Console.WriteLine("FrontendUrl: '" + frontendUrlFromConfig + "'");
if (string.IsNullOrEmpty(qrSecretFromConfig) || qrSecretFromConfig.Length < 32)
{
    Console.WriteLine("ERROR: QrSecret is invalid or missing! Check appsettings.json");
}

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<FitRankDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IPersonaRepository, PersonaRepositoryImpl>();
builder.Services.AddScoped<IPersonaService, PersonaServiceImpl>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepositoryImpl>();
builder.Services.AddScoped<IRankingService, RankingServiceImpl>();
builder.Services.AddScoped<IEjercicioRealizado, EjercicioRealizadoService>();
builder.Services.AddScoped<IEjercicioRealizadoRepository, EjercicioRealizadoImpl>();
builder.Services.AddScoped<IUsuarioService, UsuarioServiceImpl>();

builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<ILogroService, LogroServiceImpl>();
builder.Services.AddScoped<ILogroRepositorio, LogroRepositorio>();
builder.Services.AddScoped<ISocioService, SocioServiceImpl>();
builder.Services.AddScoped<ISocioRepositorio, SocioRepositorio>();
builder.Services.AddScoped<IGimnasioService, GimnasioServiceImpl>();
builder.Services.AddScoped<IGimnasioRepositorio, GimnasioRepositorio>();


builder.Services.AddScoped<IPuntuacionDiariaRepository, PuntuacionDiariaImpl>();
builder.Services.AddScoped<CalculoDivisionService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDev",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200") // Angular dev server
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDev",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200") // el origen de tu Angular
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});
builder.Services.AddScoped<IRutinaServicio, RutinaServicioImpl>();
builder.Services.AddScoped<IRutinaRepositorio, RutinaRepositorioImpl>();

// JWT Authentication
var jwtKey = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "tu_secreto_super_seguro_32_chars_minimo");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(jwtKey),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
    });
// SendGrid para emails
builder.Services.AddSingleton<ISendGridClient>(provider =>
    new SendGridClient(builder.Configuration["SendGrid:ApiKey"] ?? "SG.tu_clave")
);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FitRank API", Version = "v1" });
builder.Services.AddScoped<IEjercicioServicio, EjercicioServicioImpl>();
builder.Services.AddScoped<IEjercicioRepositorio, EjercicioRepositorioImpl>();

    // Config para JWT Bearer
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Ejemplo: 'Bearer {tu_token}'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    // Esto aplica el security a TODOS los endpoints
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
      {
          {
              new OpenApiSecurityScheme
              {
                  Reference = new OpenApiReference
                  {
                      Type = ReferenceType.SecurityScheme,
                      Id = "Bearer"
                  }
              },
              Array.Empty<string>()  // Para roles, si querés, agrega {"Admin"}
          }
      });
});

builder.Services.AddAutoMapper(cfg =>
   cfg.AddMaps(typeof(FitRank_API.Application.Mappings.AssemblyMapping).Assembly));

// BORRAR TESTEANDO
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDev",
        policy =>
        {
            policy.WithOrigins("http://localhost:4200") // el origen de tu frontend
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});
//HASTA ACA


var app = builder.Build();
    
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FitRankDbContext>();
    db.Database.Migrate();
}



app.UseCors("AllowAngularDev");

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowAngularDev");


app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
