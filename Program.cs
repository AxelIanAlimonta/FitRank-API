
using FitRank_API.Application.CasosDeUso.AsistenciaCasosDeUso;
using FitRank_API.Application.CasosDeUso.ConfiguracionGrupoMuscular;
using FitRank_API.Application.CasosDeUso.DiaDeLaSemanaCasoDeUso;
using FitRank_API.Application.CasosDeUso.DificultadCasosDeUso;
using FitRank_API.Application.CasosDeUso.EjercicioAsignadoCasoDeUso;
using FitRank_API.Application.CasosDeUso.EjercicioCasosDeUso;
using FitRank_API.Application.CasosDeUso.EjercicioRealizadoCasosDeUso;
using FitRank_API.Application.CasosDeUso.GimnasioCasosDeUso;
using FitRank_API.Application.CasosDeUso.GrupoMuscularCasosDeUso;
using FitRank_API.Application.CasosDeUso.Invitacion;
using FitRank_API.Application.CasosDeUso.Invitacion.RegistrarInvitacionCasoDeUso;
using FitRank_API.Application.CasosDeUso.LogroCasosDeUso;
using FitRank_API.Application.CasosDeUso.ProfesorCasosDeUso;
using FitRank_API.Application.CasosDeUso.MaquinaCasosDeUso;
using FitRank_API.Application.CasosDeUso.PuntajeCasosDeUso;
using FitRank_API.Application.CasosDeUso.RankingCasosDeUso;
using FitRank_API.Application.CasosDeUso.RutinaCasosDeUso;
using FitRank_API.Application.CasosDeUso.RutinaEjerciciosCasosDeUso;
using FitRank_API.Application.CasosDeUso.SerieAsignadaCasoDeUso;
using FitRank_API.Application.CasosDeUso.SerieRealizadaCasosDeUso;
using FitRank_API.Application.CasosDeUso.SesionRealizadaDeEjercicios;
using FitRank_API.Application.CasosDeUso.SocioCasoDeUso;

using FitRank_API.Application.CasosDeUso.UsuarioCasosDeUso;
using FitRank_API.Application.Interfaces;
using FitRank_API.Application.Services;

using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using FitRank_API.Infrastructure.Repositories;
using FitRank_API.Infrastructure.Repositorios;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SendGrid;
using System;
using System.Text;
using FitRank_API.Application.CasosDeUso.AdministradorCasosDeUso;
using FitRank_API.Application.CasosDeUso.MedidaCorporalCasosDeUso;
using FitRank_API.Application.CasosDeUso.FotoCasosDeUso;
using FitRank_API.Application.DTOs.GimnasioDTOs;



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

builder.Services.AddScoped<ISocioRepositorio, SocioRepositorioImpl>();
builder.Services.AddScoped<ObtenerSociosCasoDeUso>();
builder.Services.AddScoped<AgregarSocioCasoDeUso>();
builder.Services.AddScoped<ObtenerSocioPorIdCasoDeUso>();
builder.Services.AddScoped<ActualizarSocioCasoDeUso>();
builder.Services.AddScoped<EliminarSocioCasoDeUso>();

builder.Services.AddScoped<IGrupoMuscularRepositorio, GrupoMuscularRepositorioImpl>();
builder.Services.AddScoped<ObtenerTodosLosGruposMuscularesCasoDeUso>();
builder.Services.AddScoped<ObtenerGrupoMuscularPorIdCasoDeUso>();
builder.Services.AddScoped<AgregarGrupoMuscularCasoDeUso>();
builder.Services.AddScoped<ActualizarGrupoMuscularCasoDeUso>();
builder.Services.AddScoped<EliminarGrupoMuscularCasoDeUso>();

builder.Services.AddScoped<IEjercicioRepositorio, EjercicioRepositorioImpl>();
builder.Services.AddScoped<ObtenerEjerciciosCasoDeUso>();
builder.Services.AddScoped<ObtenerEjercicioPorIdCasoDeUso>();
builder.Services.AddScoped<AgregarEjercicioCasoDeUso>();
builder.Services.AddScoped<ActualizarEjercicioCasoDeUso>();
builder.Services.AddScoped<EliminarEjercicioCasoDeUso>();

builder.Services.AddScoped<IMaquinaRepositorio, MaquinaRepositorioImpl>();
builder.Services.AddScoped<ObtenerMaquinasCasoDeUso>();
builder.Services.AddScoped<ObtenerMaquinaPorIdCasoDeUso>();
builder.Services.AddScoped<AgregarMaquinaCasoDeUso>();
builder.Services.AddScoped<ActualizarMaquinaCasoDeUso>();
builder.Services.AddScoped<EliminarMaquinaCasoDeUso>();

builder.Services.AddScoped<IPersonaRepository, PersonaRepositoryImpl>();
builder.Services.AddScoped<IPersonaService, PersonaServiceImpl>();

builder.Services.AddScoped<IDificultadRepositorio, DificultadRepositorioImpl>();
builder.Services.AddScoped<ObtenerTodasLasDificultadesCasoDeUso>();
builder.Services.AddScoped<ObtenerDificultadPorIdCasoDeUso>();
builder.Services.AddScoped<AgregarDificultadCasoDeUso>();
builder.Services.AddScoped<ActualizarDificultadCasoDeUso>();
builder.Services.AddScoped<EliminarDificultadCasoDeUso>();

builder.Services.AddScoped<ISesionRealizadaDeEjerciciosRepositorio, SesionRealizadaDeEjerciciosRepositorioImpl>();
builder.Services.AddScoped<ObtenerTodasLasSesionesRealizadasDeEjerciciosCasoDeUso>();
builder.Services.AddScoped<ObtenerSesionRealizadaDeEjerciciosPorIdCasoDeUso>();
builder.Services.AddScoped<AgregarSesionRealizadaDeEjerciciosCasoDeUso>();
builder.Services.AddScoped<ActualizarSesionRealizadaDeEjerciciosCasoDeUso>();
builder.Services.AddScoped<EliminarSesionRealizadaDeEjerciciosCasoDeUso>();

builder.Services.AddScoped<IConfiguracionGrupoMuscularRepositorio, ConfiguracionGrupoMuscularImpl>();
builder.Services.AddScoped<ObtenerTodasLasConfiguracionGrupoMuscularCasoDeUso>();
builder.Services.AddScoped<ObtenerConfiguracionGrupoMuscularPorIdCasoDeUso>();
builder.Services.AddScoped<AgregarConfiguracionGrupoMuscularCasoDeUso>();
builder.Services.AddScoped<ActualizarConfiguracionGrupoMuscularCasoDeUso>();
builder.Services.AddScoped<EliminarConfiguracionGrupoMuscularCasoDeUso>();

builder.Services.AddScoped<IRutinaRepositorio, RutinaRepositorioImpl>();
builder.Services.AddScoped<ObtenerTodasLasRutinasCasoDeUso>();
builder.Services.AddScoped<OtenerRutinaPorIdCasoDeUso>();
builder.Services.AddScoped<AgregarRutinaCasoDeUso>();
builder.Services.AddScoped<ActualizarRutinaCasoDeUso>();
builder.Services.AddScoped<EliminarRutinaCasoDeUso>();

builder.Services.AddScoped<IEjercicioAsignadoRepositorio, EjercicioAsignadoRepositorioImpl>();
builder.Services.AddScoped<ObtenerEjerciciosAsignadosCasoDeUso>();
builder.Services.AddScoped<AgregarEjercicioAsignadoCasoDeUso>();
builder.Services.AddScoped<ObtenerEjercicioAsignadoPorIdCasoDeUso>();
builder.Services.AddScoped<ActualizarEjercicioAsignadoCasoDeUso>();
builder.Services.AddScoped<EliminarEjercicioAsignadoCasoDeUso>();

builder.Services.AddScoped<IEjercicioRealizadoRepositorio, EjercicioRealizadoRepositorioImpl>();
builder.Services.AddScoped<ObtenerTodosLosEjercicioRealizadoCasoDeUso>();
builder.Services.AddScoped<ObtenerEjercicioRealizadoPorIdCasoDeUso>();
builder.Services.AddScoped<AgregarEjercicioRealizadoCasoDeUso>();
builder.Services.AddScoped<ActualizarEjercicioRealizadoCasoDeUso>();
builder.Services.AddScoped<EliminarEjercicioRealizadoCasoDeUso>();

builder.Services.AddScoped<ISerieRealizadaRepositorio, SerieRealizadaRepositorioImpl>();
builder.Services.AddScoped<ObtenerTodasLasSerieRealizadaCasoDeUso>();
builder.Services.AddScoped<ObtenerSerieRealizadaPorIdCasoDeUso>();
builder.Services.AddScoped<AgregarSerieRealizadaCasoDeUso>();
builder.Services.AddScoped<ActualizarSerieRealizadaCasoDeUso>();
builder.Services.AddScoped<EliminarSerieRealizadaCasoDeUso>();

builder.Services.AddScoped<IPuntajeRepositorio, PuntajeRepositorioImpl>();
builder.Services.AddScoped<ObtenerTodosLosPuntajeCasoDeUso>();
builder.Services.AddScoped<ObtenerPuntajePorIdCasoDeUso>();
builder.Services.AddScoped<AgregarPuntajeCasoDeUso>();
builder.Services.AddScoped<ActualizarPuntajeCasoDeUso>();
builder.Services.AddScoped<EliminarPuntajeCasoDeUso>();

builder.Services.AddScoped<ISerieAsignadaRepositorio, SerieAsignadaRepositorioImpl>();
builder.Services.AddScoped<ObtenerSeriesAsignadasCasoDeUso>();
builder.Services.AddScoped<ObtenerSerieAsignadaPorIdCasoDeUso>();
builder.Services.AddScoped<AgregarSerieAsignadaCasoDeUso>();
builder.Services.AddScoped<ActualizarSerieAsignadaCasoDeUso>();
builder.Services.AddScoped<EliminarSerieAsignadaCasoDeUso>();

builder.Services.AddScoped<IRutinaEjercicioRepositorio, RutinaEjercicioRepositorioImpl>();
builder.Services.AddScoped<ObtenerTodasRutinasEjerciciosCasoDeUso>();
builder.Services.AddScoped<ObtenerRutinaEjercicioPorIdCasoDeUso>();
builder.Services.AddScoped<AgregarRutinaEjercicioCasoDeUso>();
builder.Services.AddScoped<ActualizarRutinaEjercicioCasoDeUso>();
builder.Services.AddScoped<EliminarRutinaEjercicioCasoDeUso>();

builder.Services.AddScoped<ILogroRepositorio, LogroRepositorioImpl>();
builder.Services.AddScoped<ObtenerLogrosCasoDeUso>();
builder.Services.AddScoped<ObtenerLogroPorIdCasoDeUso>();
builder.Services.AddScoped<AgregarLogroCasoDeUso>();
builder.Services.AddScoped<EliminarLogroCasoDeUso>();
builder.Services.AddScoped<ActualizarLogroCasoDeUso>();

builder.Services.AddScoped<ILogroRepositorio, LogroRepositorioImpl>();
builder.Services.AddScoped<ObtenerGimnasiosCasoDeUso>();
builder.Services.AddScoped<ObtenerGimnasioPorIdCasoDeUso>();
builder.Services.AddScoped<AgregarGimnasioCasoDeUso>();
builder.Services.AddScoped<EliminarGimnasioCasoDeUso>();
builder.Services.AddScoped<ActualizarGimnasioCasoDeUso>();

builder.Services.AddScoped<IRankingRepositorio, RankingRepositorioImpl>();
builder.Services.AddScoped<ObtenerRankingGeneralCasoDeUso>();
builder.Services.AddScoped<ObtenerPosicionPorIdCasoDeUso>();


builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorioImpl>();
builder.Services.AddScoped<LoginUsuarioCasoDeUso>();
builder.Services.AddScoped<RegistrarUsuarioCasoDeUso>();
builder.Services.AddScoped<ValidarTokenActivacionCasoDeUso>();
builder.Services.AddScoped<ActivarCuentaCasoDeUso>();
builder.Services.AddScoped<GenerarTokenCasoDeUso>();
builder.Services.AddScoped<ObtenerUsuarioPorIdCasoDeUso>();
builder.Services.AddScoped<EliminarUsuarioCasoDeUso>();
builder.Services.AddScoped<AgregarUsuarioConInvitacionCasoDeUso>();


builder.Services.AddScoped<IGimnasioRepositorio, GimnasioRepositorioImpl>();


builder.Services.AddScoped<IInvitacionRepositorio, InvitacionRepositorioImpl>();
builder.Services.AddScoped<AgregarUsuarioConInvitacionCasoDeUso>();
builder.Services.AddScoped<EnviarEmailQrCasoDeUso>();
builder.Services.AddScoped<FallbackEfectivoCasoDeUso>();
builder.Services.AddScoped<EliminarInvitacionCasoDeUso>();
builder.Services.AddScoped<ObtenerInvitacionesCasoDeUso>();


builder.Services.AddScoped<QrHelper>();


builder.Services.AddScoped<IAsistenciaRepositorio, AsistenciaRepositorioImpl>();
builder.Services.AddScoped<ObtenerAsistenciasPorUsuarioCasoDeUso>();
builder.Services.AddScoped<ValidarQrCasoDeUso>();
builder.Services.AddScoped<ObtenerAsistenciasDetalladasPorUsuarioCasoDeUso>();
builder.Services.AddScoped<ObtenerAsistenciasPorDiaCasoDeUso>();
builder.Services.AddScoped<AgregarAsistenciaCasoDeUso>();


builder.Services.AddScoped<IProfesorRepositorio, ProfesorRepositorioImpl>();
builder.Services.AddScoped<AgregarProfesorCasoDeUso>();
builder.Services.AddScoped<ObtenerTodosLosProfesoresCasoDeUso>();
builder.Services.AddScoped<ObtenerProfesorPorIdCasoDeUso>();
builder.Services.AddScoped<ActualizarProfesorCasoDeUso>();
builder.Services.AddScoped<EliminarProfesorCasoDeUso>();

builder.Services.AddScoped<IDiaDeLaSemanaRepositorio, DiaDeLaSemanaRepositorioImpl>();
builder.Services.AddScoped<AgregarDiaDeLaSemanaCasoDeUso>();
builder.Services.AddScoped<ObtenerTodosLosDiasDeLaSemanaCasoDeUso>();
builder.Services.AddScoped<ObtenerDiaDeLaSemanaPorIdCasoDeUso>();
builder.Services.AddScoped<ActualizarDiaDeLaSemanaCasoDeUso>();
builder.Services.AddScoped<EliminarDiaDeLaSemanaCasoDeUso>();

builder.Services.AddScoped<IAdministradorRepositorio, AdministradorRepositorioImpl>();
builder.Services.AddScoped<AgregarAdministradorCasoDeUso>();
builder.Services.AddScoped<EliminarAdministradorCasoDeUso>();

builder.Services.AddScoped<IMedidaCorporalRepositorio, MedidaCorporalRepositorioImpl>();
builder.Services.AddScoped<AgregarMedidaCorporalCasoDeUso>();
builder.Services.AddScoped<ObtenerMedidasPorSocioCasoDeUso>();
builder.Services.AddScoped<ObtenerMedidaCorporalPorIdCasoDeUso>();
builder.Services.AddScoped<ActualizarMedidaCorporalCasoDeUso>();
builder.Services.AddScoped<EliminarMedidaCorporalCasoDeUso>();

builder.Services.AddScoped<IFotoRepositorio, FotoRepositorioImpl>();
builder.Services.AddScoped<AgregarFotoCasoDeUso>();
builder.Services.AddScoped<ObtenerFotosPorSocioCasoDeUso>();
builder.Services.AddScoped<EliminarFotoCasoDeUso>();





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
// ✅ Agregar política de Admin
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy =>
        policy.RequireRole("Admin")); // Solo usuarios con rol Admin
});

// SendGrid para emails
builder.Services.AddSingleton<ISendGridClient>(provider =>
    new SendGridClient(builder.Configuration["SendGrid:ApiKey"] ?? "SG.tu_clave")
);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FitRank API", Version = "v1" });

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

app.UseAuthorization();

app.MapControllers();

app.Run();
