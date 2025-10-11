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
    public DbSet<RutinaEntity> Rutinas { get; set; } // Quiero una tabla en la BDD llamada "Rutinas" para la clase RutinaEntity
    public DbSet<BloqueRutinaEntity> BloquesRutinas { get; set; }
    public DbSet<BloqueDiaEntity> BloquesDias { get; set; }
    public DbSet<EjercicioEntity> Ejercicios { get; set; }
    public DbSet<EjercicioBloqueEntity> EjerciciosBloques { get; set; }
    public DbSet<DiaEntity> Dias { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    { //Mapeo de las clases
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
    }
   }

