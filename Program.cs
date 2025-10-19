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

builder.Services.AddScoped<IPersonaRepository, PersonaRepositoryImpl>();
builder.Services.AddScoped<IPersonaService, PersonaServiceImpl>();

builder.Services.AddScoped<IDificultadRepositorio, DificultadRepositorioImpl>();
// Add other repositories and use cases as needed
builder.Services.AddScoped<ObtenerTodasLasDificultadesCasoDeUso>();
builder.Services.AddScoped<ObtenerDificultadPorIdCasoDeUso>();
builder.Services.AddScoped<AgregarDificultadCasoDeUso>();
builder.Services.AddScoped<ActualizarDificultadCasoDeUso>();
builder.Services.AddScoped<EliminarDificultadCasoDeUso>();


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
