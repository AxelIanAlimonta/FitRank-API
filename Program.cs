using Microsoft.EntityFrameworkCore;
using FitRank_API.Infrastructure.Persistence;
using System;
using FitRank_API.Application.Services;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Infrastructure.Repositories;
using FitRank_API.Application.Interfaces;
using FitRank_API.Application.CasosDeUso.SocioCasoDeUso;
using FitRank_API.Application.CasosDeUso.GrupoMuscularCasosDeUso;
using FitRank_API.Application.CasosDeUso.DificultadCasosDeUso;
using FitRank_API.Application.CasosDeUso.SesionRealizadaDeEjercicios;
using FitRank_API.Application.CasosDeUso.ConfiguracionGrupoMuscular;
using FitRank_API.Application.CasosDeUso.EjercicioCasosDeUso;
using FitRank_API.Application.CasosDeUso.RutinaCasosDeUso;
using FitRank_API.Application.CasosDeUso.EjercicioAsignadoCasoDeUso;

var builder = WebApplication.CreateBuilder(args);

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


builder.Services.AddAutoMapper(cfg =>
   cfg.AddMaps(typeof(FitRank_API.Application.Mappings.AssemblyMapping).Assembly));


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<FitRankDbContext>();
    db.Database.Migrate();
}



app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
