namespace FitRank_API.Infrastructure.Persistence;

using FitRank_API.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public class FitRankDbContext : DbContext
{
    public FitRankDbContext(DbContextOptions<FitRankDbContext> options)
        : base(options)
    {
    }
    public DbSet<Persona> Personas { get; set; }
    public DbSet<Socio> Socios { get; set; }
    public DbSet<GrupoMuscular> GruposMusculares { get; set; }
    public DbSet<Dificultad> Dificultades { get; set; }
    public DbSet<SesionRealizadaDeEjercicios> SesionRealizadaDeEjercicios { get; set; }
    public DbSet<ConfiguracionGrupoMuscular> ConfiguracionesGrupoMuscular { get; set; }
    public DbSet<Ejercicio> Ejercicios { get; set; }
    public DbSet<Rutina> Rutinas { get; set; }
    public DbSet<EjercicioAsignado> EjerciciosAsignados { get; set; }
    public DbSet<SerieAsignada> SeriesAsignadas { get; set; }
    public DbSet<EjercicioRealizado> EjerciciosRealizados { get; set; }
    public DbSet<SerieRealizada> SeriesRealizadas { get; set; }
    public DbSet<Puntaje> Puntajes { get; set; }
    public DbSet<RutinaEjercicio> RutinasEjercicios { get; set; }

}

