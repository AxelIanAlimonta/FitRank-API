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
    public DbSet<Profesor> Profesores { get; set; }
    public DbSet<Gimnasio> Gimnasios { get; set; }
    public DbSet<Maquina> Maquinas { get; set; }
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
    public DbSet<DiaDeLaSemana> DiasDeLaSemana { get; set; }
    public DbSet<LogroSocio> LogrosSocios { get; set; }


    public DbSet<Administrador> Administradores { get; set; }

    public DbSet<Jornada> Jornadas { get; set; }


    public DbSet<MedidaCorporal> MedidasCorporales { get; set; }

    public DbSet<Foto> Fotos { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        ConfigureEntityRelationships(modelBuilder);

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

    private static void ConfigureEntityRelationships(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>().ToTable("Usuarios");
        modelBuilder.Entity<Socio>().ToTable("Socios");
        modelBuilder.Entity<Profesor>().ToTable("Profesores");
    
        modelBuilder.Entity<Administrador>().ToTable("Administradores");

        modelBuilder.Entity<Socio>()
            .HasOne(s => s.Gimnasio)
            .WithMany(g => g.Socios)
            .HasForeignKey(s => s.GimnasioId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Asistencia>()
    .HasOne(a => a.Usuario)
    .WithMany()
    .HasForeignKey(a => a.UsuarioId)
    .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Asistencia>()
            .HasOne(a => a.Gimnasio)
            .WithMany(g => g.Asistencias)
            .HasForeignKey(a => a.GimnasioId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Invitacion>()
    .HasOne(i => i.Gimnasio)
    .WithMany(g => g.Invitaciones)
    .HasForeignKey(i => i.GimnasioId)
    .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Invitacion>()
            .HasOne(i => i.Usuario)
            .WithMany()
            .HasForeignKey(i => i.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Gimnasio>()
      .HasOne(g => g.Administrador)
      .WithOne(a => a.Gimnasio)
      .HasForeignKey<Gimnasio>(g => g.AdministradorId)
      .OnDelete(DeleteBehavior.SetNull);
    }

}