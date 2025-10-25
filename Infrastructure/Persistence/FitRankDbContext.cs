namespace FitRank_API.Infrastructure.Persistence;

using FitRank_API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class FitRankDbContext : DbContext
{
    public FitRankDbContext(DbContextOptions<FitRankDbContext> options)
        : base(options)
    {
    }
    public DbSet<Persona> Personas { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
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
    public DbSet<Asistencia> Asistencias { get; set; }
    public DbSet<Invitacion> Invitaciones { get; set; }
    public DbSet<Logro> Logros { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // 🔹 Forzar que todos los DateTime se guarden como UTC
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {

                if (property.ClrType == typeof(DateTime))
                {
                    property.SetValueConverter(new ValueConverter<DateTime, DateTime>(
                        v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
                        v => DateTime.SpecifyKind(v, DateTimeKind.Utc)
                    ));
                }
                else if (property.ClrType == typeof(DateTime?))
                {
                    property.SetValueConverter(new ValueConverter<DateTime?, DateTime?>(
                        v => v.HasValue ? (v.Value.Kind == DateTimeKind.Utc ? v.Value : v.Value.ToUniversalTime()) : v,
                        v => v.HasValue ? DateTime.SpecifyKind(v.Value, DateTimeKind.Utc) : v
                    ));
                }
            }
        }

    }
}