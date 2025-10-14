namespace FitRank_API.Infrastructure.Persistence;

using FitRank_API.Application.DTOs.EjercicioRealizado;
using FitRank_API.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

public class FitRankDbContext : DbContext
{
    public DbSet<Logro> Logros => Set<Logro>();
    public DbSet<SocioRealizaLogro> SocioRealizaLogros => Set<SocioRealizaLogro>();
    public DbSet<Socio> Socios => Set<Socio>();
    public DbSet<Gimnasio> Gimnasios => Set<Gimnasio>();
    public DbSet<GimnasioLogro> GimnasioLogros => Set<GimnasioLogro>();

  
    public DbSet<Persona> Personas { get; set; }
    public DbSet<Ejercicio> Ejercicios { get; set; }

    public DbSet<Maquina> Maquinas { get; set; }
    public DbSet<Rutina> Rutinas { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Asistencia> Asistencias { get; set; }
    public DbSet<PuntuacionDiaria> PuntuacionesDiarias { get; set; }
    public DbSet<Ranking> Rankings { get; set; }

    public DbSet<EjercicioRealizado> EjerciciosRealizados { get; set; }
    public DbSet<ConfiguracionDivision> ConfiguracionesDivisiones { get; set; }
    public DbSet<ConfiguracionDificultad> ConfiguracionesDificultad { get; set; }
    public DbSet<ConfiguracionGrupoMuscular> ConfiguracionesGrupoMuscular { get; set; }

    public DbSet<Invitacion> Invitaciones { get; set; }

    //Rutina Jero
    public DbSet<BloqueRutinaEntity> BloquesRutinas { get; set; }
    public DbSet<BloqueDiaEntity> BloquesDias { get; set; }
    public DbSet<EjercicioBloqueEntity> EjerciciosBloques { get; set; }
    public DbSet<DiaEntity> Dias { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    base.OnModelCreating(modelBuilder);

        // Rutina -> BloqueRutina (1:N)
        modelBuilder.Entity<BloqueRutinaEntity>()
            .HasOne(b => b.Rutina)
            .WithMany(r => r.Bloques)
            .HasForeignKey(b => b.IdRutina)
            .OnDelete(DeleteBehavior.Cascade);

        // BloqueRutina -> BloqueDia (1:N)
        modelBuilder.Entity<BloqueDiaEntity>()
            .HasOne(bd => bd.BloqueRutina)
            .WithMany(b => b.Dias)
            .HasForeignKey(bd => bd.IdBloqueRutina)
            .OnDelete(DeleteBehavior.Cascade);

        // BloqueDia -> Dia (1:1)
        modelBuilder.Entity<BloqueDiaEntity>()
            .HasOne(bd => bd.Dia)
            .WithMany(d => d.BloquesDias)
            .HasForeignKey(bd => bd.IdDia)
            .OnDelete(DeleteBehavior.Restrict);

        // BloqueRutina -> EjercicioBloque (1:N)
        modelBuilder.Entity<EjercicioBloqueEntity>()
            .HasOne(eb => eb.BloqueRutina)
            .WithMany(b => b.Ejercicios)
            .HasForeignKey(eb => eb.IdBloqueRutina)
            .OnDelete(DeleteBehavior.Cascade);

        // Ejercicio -> EjercicioBloque (1:N)
        modelBuilder.Entity<EjercicioBloqueEntity>()
            .HasOne(eb => eb.Ejercicio)
            .WithMany(e => e.EjerciciosBloques)
            .HasForeignKey(eb => eb.IdEjercicio)
            .OnDelete(DeleteBehavior.Restrict);

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
    



    public FitRankDbContext(DbContextOptions<FitRankDbContext> options)
        : base(options)
    {
    }
   }

