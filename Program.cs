using FitRank.API.Application.Rutinas.Abstractions;
using FitRank.API.Infrastructure.RulesEngineImpl;
using FitRank_API.Application.CasosDeUso.AdministradorCasosDeUso;
using FitRank_API.Application.CasosDeUso.AsistenciaCasosDeUso;
using FitRank_API.Application.CasosDeUso.ConfiguracionGrupoMuscular;
using FitRank_API.Application.CasosDeUso.DiaDeLaSemanaCasoDeUso;
using FitRank_API.Application.CasosDeUso.DificultadCasosDeUso;
using FitRank_API.Application.CasosDeUso.EjercicioAsignadoCasoDeUso;
using FitRank_API.Application.CasosDeUso.EjercicioCasosDeUso;
using FitRank_API.Application.CasosDeUso.FotoCasosDeUso;
using FitRank_API.Application.CasosDeUso.GimnasioCasosDeUso;
using FitRank_API.Application.CasosDeUso.GrupoMuscularCasosDeUso;
using FitRank_API.Application.CasosDeUso.Invitacion;
using FitRank_API.Application.CasosDeUso.Invitacion.RegistrarInvitacionCasoDeUso;
using FitRank_API.Application.CasosDeUso.JornadaCasosDeUso;
using FitRank_API.Application.CasosDeUso.LogroCasosDeUso;
using FitRank_API.Application.CasosDeUso.MaquinaCasosDeUso;
using FitRank_API.Application.CasosDeUso.MedidaCorporalCasosDeUso;
using FitRank_API.Application.CasosDeUso.NotificacionCasosDeUso;
using FitRank_API.Application.CasosDeUso.ProfesorCasosDeUso;
using FitRank_API.Application.CasosDeUso.PuntajeCasosDeUso;
using FitRank_API.Application.CasosDeUso.RankingCasosDeUso;
using FitRank_API.Application.CasosDeUso.RutinaCasosDeUso;
using FitRank_API.Application.CasosDeUso.SesionCasosDeUso;
using FitRank_API.Application.CasosDeUso.SocioCasoDeUso;
using FitRank_API.Application.CasosDeUso.UsuarioCasosDeUso;
using FitRank_API.Application.Interfaces;
using FitRank_API.Application.Services;
using FitRank_API.Application.UseCases.Actividad;
using FitRank_API.Application.UseCases.Entrenamiento;
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
using System.Text.Json.Serialization;
using FitRank_API.Application.UseCases;
using FitRank_API.Application.CasosDeUso.CalculoPuntajeCasosDeUso;
using FitRank_API.Application.CasosDeUso.EntrenamientoCasosDeUso;
using FitRank_API.Application.CasosDeUso.SerieCasosDeUso;
using FitRank_API.Application.CasosDeUso.Asistencia;
using FitRank_API.Application.CasosDeUso.SolicitudCasosDeUso;
using MercadoPago.Config;
using FitRank_API.Application.CasosDeUso.Ingreso;

using FitRank_API.Application.CasosDeUso.MercadoPago;





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

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
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

builder.Services.AddScoped<IConfiguracionGrupoMuscularRepositorio, ConfiguracionGrupoMuscularImpl>();
builder.Services.AddScoped<ObtenerTodasLasConfiguracionGrupoMuscularCasoDeUso>();
builder.Services.AddScoped<ObtenerConfiguracionGrupoMuscularPorIdCasoDeUso>();
builder.Services.AddScoped<AgregarConfiguracionGrupoMuscularCasoDeUso>();
builder.Services.AddScoped<ActualizarConfiguracionGrupoMuscularCasoDeUso>();
builder.Services.AddScoped<EliminarConfiguracionGrupoMuscularCasoDeUso>();

builder.Services.AddScoped<IRutinaRepositorio, RutinaRepositorioImpl>();
builder.Services.AddScoped<ObtenerTodasLasRutinasCasoDeUso>();
builder.Services.AddScoped<ObtenerRutinaPorIdCasoDeUso>();
builder.Services.AddScoped<AgregarRutinaCasoDeUso>();
builder.Services.AddScoped<ActualizarRutinaCasoDeUso>();
builder.Services.AddScoped<EliminarRutinaCasoDeUso>();
builder.Services.AddScoped<ObtenerRutinaCompletaCasoDeUso>();


builder.Services.AddScoped<ISesionRepositorio, SesionRepositorioImpl>();
builder.Services.AddScoped<ObtenerTodasLasSesionesCasoDeUso>();
builder.Services.AddScoped<ObtenerSesionPorIdCasoDeUso>();
builder.Services.AddScoped<AgregarSesionCasoDeUso>();
builder.Services.AddScoped<ActualizarSesionCasoDeUso>();
builder.Services.AddScoped<EliminarSesionCasoDeUso>();

builder.Services.AddScoped<IEjercicioAsignadoRepositorio, EjercicioAsignadoRepositorioImpl>();
builder.Services.AddScoped<ObtenerEjerciciosAsignadosCasoDeUso>();
builder.Services.AddScoped<AgregarEjercicioAsignadoCasoDeUso>();
builder.Services.AddScoped<ObtenerEjercicioAsignadoPorIdCasoDeUso>();
builder.Services.AddScoped<ActualizarEjercicioAsignadoCasoDeUso>();
builder.Services.AddScoped<EliminarEjercicioAsignadoCasoDeUso>();

builder.Services.AddScoped<IPuntajeRepositorio, PuntajeRepositorioImpl>();
builder.Services.AddScoped<ActualizarPuntajeCasoDeUso>();
builder.Services.AddScoped<AgregarPuntajeCasoDeUso>();
builder.Services.AddScoped<EliminarPuntajeCasoDeUso>();
builder.Services.AddScoped<ObtenerPuntajePorIdCasoDeUso>();
builder.Services.AddScoped<ObtenerTodosLosPuntajeCasoDeUso>();

//CalculoPorPuntajeCasosDeUso
builder.Services.AddScoped<CalcularEstadisticaCombinadaPuntajeSocioCasoDeUso>();
builder.Services.AddScoped<CalcularEstadisticaCorporalSocioCasoDeUso>();
builder.Services.AddScoped<ObtenerPuntajePorGrupoMuscularSocioCasoDeUso>();
builder.Services.AddScoped<ObtenerPuntajeTotalSocioCasoDeUso>();
builder.Services.AddScoped<ObtenerRankingSociosCasoDeUso>();


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
builder.Services.AddScoped<AgregarInvitacionCasoDeUso>();


builder.Services.AddScoped<CrearPreferenciaMercadoPagoCasoDeUso>();
builder.Services.AddScoped<ProcesarPagoMercadoPagoCasoDeUso>();

builder.Services.AddScoped<QrHelper>();


builder.Services.AddScoped<IAsistenciaRepositorio, AsistenciaRepositorioImpl>();
builder.Services.AddScoped<ObtenerAsistenciasPorUsuarioCasoDeUso>();
builder.Services.AddScoped<ValidarQrCasoDeUso>();
builder.Services.AddScoped<ObtenerAsistenciasDetalladasPorUsuarioCasoDeUso>();
builder.Services.AddScoped<ObtenerAsistenciasPorDiaCasoDeUso>();
builder.Services.AddScoped<AgregarAsistenciaCasoDeUso>();
builder.Services.AddScoped<ValidarAsistenciaQrCasoDeUso>();
builder.Services.AddScoped<ObtenerTodasLasAsistenciasCasoDeUso>();
builder.Services.AddScoped<DetectarSociosInactivosCasoDeUso>();

builder.Services.AddScoped<IProfesorRepositorio, ProfesorRepositorioImpl>();
builder.Services.AddScoped<AgregarProfesorCasoDeUso>();
builder.Services.AddScoped<ObtenerTodosLosProfesoresCasoDeUso>();
builder.Services.AddScoped<ObtenerProfesorPorIdCasoDeUso>();
builder.Services.AddScoped<ActualizarProfesorCasoDeUso>();
builder.Services.AddScoped<EliminarProfesorCasoDeUso>();
builder.Services.AddScoped<ObtenerTodosLosProfesoresCasoDeUso>();
builder.Services.AddScoped<ObtenerTodasLasRutinasPorProfesorCasoDeUso>();

builder.Services.AddScoped<IDiaDeLaSemanaRepositorio, DiaDeLaSemanaRepositorioImpl>();
builder.Services.AddScoped<AgregarDiaDeLaSemanaCasoDeUso>();
builder.Services.AddScoped<ObtenerTodosLosDiasDeLaSemanaCasoDeUso>();
builder.Services.AddScoped<ObtenerDiaDeLaSemanaPorIdCasoDeUso>();
builder.Services.AddScoped<ActualizarDiaDeLaSemanaCasoDeUso>();
builder.Services.AddScoped<EliminarDiaDeLaSemanaCasoDeUso>();


builder.Services.AddScoped<IAdministradorRepositorio, AdministradorRepositorioImpl>();

builder.Services.AddScoped<EliminarAdministradorCasoDeUso>();
builder.Services.AddScoped<AgregarUsuarioConInvitacionCasoDeUso>();
builder.Services.AddScoped<ValidarQrCasoDeUso>();
builder.Services.AddScoped<EnviarEmailQrCasoDeUso>();
builder.Services.AddScoped<FallbackEfectivoCasoDeUso>();
builder.Services.AddScoped<AgregarInvitacionCasoDeUso>();
builder.Services.AddScoped<AgregarAdministradorCasoDeUso>();
builder.Services.AddScoped<ObtenerAdministradorCasoDeUso>();

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

builder.Services.AddScoped<IJornadaRepositorio, JornadaRepositorioImpl>();
builder.Services.AddScoped<AgregarJornadaCasoDeUso>();
builder.Services.AddScoped<ObtenerTodasLasJornadasCasoDeUso>();
builder.Services.AddScoped<ObtenerJornadaPorIdCasoDeUso>();
builder.Services.AddScoped<ActualizarJornadaCasoDeUso>();
builder.Services.AddScoped<EliminarJornadaCasoDeUso>();


builder.Services.AddScoped<INotificacionRepositorio, NotificacionRepositorioImpl>();
builder.Services.AddScoped<AgregarNotificacionCasoDeUso>();
builder.Services.AddScoped<ObtenerNotificacionPorUsuarioCasoDeUso>();
builder.Services.AddScoped<RetenerSocioCasoDeUso>();
builder.Services.AddScoped<MarcarNotificacionLeidaCasoDeUso>();


builder.Services.AddScoped<ISerieRepositorio, SerieRepositorioImpl>();
builder.Services.AddScoped<ActualizarSerieCasoDeUso>();
builder.Services.AddScoped<AgregarSerieCasoDeUso>();
builder.Services.AddScoped<EliminarSerieCasoDeUso>();
builder.Services.AddScoped<ObtenerSeriePorIdCasoDeUso>();
builder.Services.AddScoped<ObtenerSeriesCasoDeUso>();




builder.Services.AddScoped<IActividadRepositorio, ActividadRepositorioImpl>();
builder.Services.AddScoped<AgregarActividadCasoDeUso>();
builder.Services.AddScoped<EliminarActividadCasoDeUso>();
builder.Services.AddScoped<ActualizarActividadCasoDeUso>();
builder.Services.AddScoped<ObtenerActividadesCasoDeUso>();
builder.Services.AddScoped<ObtenerActividadPorIdCasoDeUso>();
builder.Services.AddScoped<RegistrarActividadCasoDeUso>();



builder.Services.AddScoped<IEntrenamientoRepositorio, EntrenamientoRepositorioImpl>();
builder.Services.AddScoped<AgregarEntrenamientoCasoDeUso>();
builder.Services.AddScoped<EliminarEntrenamientoCasoDeUso>();
builder.Services.AddScoped<ActualizarEntrenamientoCasoDeUso>();
builder.Services.AddScoped<ObtenerEntrenamientosCasoDeUso>();
builder.Services.AddScoped<ObtenerEntrenamientoPorIdCasoDeUso>();
builder.Services.AddScoped<RegistrarEntrenamientoCasoDeUso>();

builder.Services.AddScoped<IRulesEvaluator, RulesEvaluator>();
builder.Services.AddScoped<IRoutineRulesRunner, RoutineRulesRunner>();
builder.Services.AddScoped<IEjercicioCatalogo, EjercicioCatalogoImpl>();
builder.Services.AddScoped<IRoutineBuilder, RoutineBuilderImpl>();

builder.Services.AddScoped<GenerarRutinaIACasoDeUso>();
builder.Services.AddScoped<ConfirmarRutinaIACasoDeUso>();

builder.Services.AddScoped<ISolicitudRutinaProfesorRepositorio, SolicitudRutinaProfesorRepositorioImpl>();
builder.Services.AddScoped<CrearSolicitudRutinaProfesorCasoDeUso>();
builder.Services.AddScoped<TomarSolicitudCasoDeUso>();
builder.Services.AddScoped<FinalizarSolicitudCasoDeUso>();
builder.Services.AddScoped<RechazarSolicitudCasoDeUso>();
builder.Services.AddScoped<TerminarSolicitudCasoDeUso>();


builder.Services.AddScoped<IIngresoRepositorio, IngresoRepositorio>();
builder.Services.AddScoped<AgregarIngresoCasoDeUso>();
builder.Services.AddScoped<EliminarIngresoCasoDeUso>();
builder.Services.AddScoped<ObtenerIngresosPorGimnasioCasoDeUso>();
builder.Services.AddScoped<ObtenerIngresoPorIdCasoDeUso>();
builder.Services.AddScoped<ObtenerIngresosCasoDeUso>();



/*builder = WebApplication.CreateBuilder(args);


var accessToken = Environment.GetEnvironmentVariable("MERCADOPAGO_ACCESS_TOKEN");

if (string.IsNullOrEmpty(accessToken))
{
    accessToken = builder.Configuration["MercadoPago:AccessToken"];
}


MercadoPagoConfig.AccessToken = accessToken;*/


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

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy =>
        policy.RequireRole("Admin")); // Solo usuarios con rol Admin
});


builder.Services.AddSingleton<ISendGridClient>(provider =>
    new SendGridClient(builder.Configuration["SendGrid:ApiKey"] ?? "SG.tu_clave")
);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "FitRank API", Version = "v1" });

    
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
              Array.Empty<string>()  
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