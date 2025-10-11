namespace FitRank_API.Infrastructure.Persistence;

using FitRank_API.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public class FitRankDbContext : DbContext
{
    public DbSet<Logro> Logros => Set<Logro>();
    public DbSet<SocioRealizaLogro> SocioRealizaLogros => Set<SocioRealizaLogro>();
    public DbSet<Socio> Socios => Set<Socio>();
    public DbSet<Gimnasio> Gimnasios => Set<Gimnasio>();
    public DbSet<GimnasioLogro> GimnasioLogros => Set<GimnasioLogro>();

    public FitRankDbContext(DbContextOptions<FitRankDbContext> options)
        : base(options)
    {
    }

    public DbSet<Persona> Personas { get; set; }
}

