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
    public DbSet<Ejercicio> Ejercicios { get; set; }

    public DbSet<Maquina> Maquinas { get; set; }
    public DbSet<Rutina> Rutinas { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Asistencia> Asistencias { get; set; }
    public DbSet<PuntuacionDiaria> PuntuacionesDiarias { get; set; }
    public DbSet<Ranking> Rankings { get; set; }

}

