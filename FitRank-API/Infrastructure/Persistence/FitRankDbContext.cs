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
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Socio> Socios { get; set; }
    public DbSet<Profesor> Profesores { get; set; }
    public DbSet<Gimnasio> Gimnasios { get; set; }
    public DbSet<Maquina> Maquinas { get; set; }
    public DbSet<GrupoMuscular> GruposMusculares { get; set; }
    public DbSet<Dificultad> Dificultades { get; set; }

    public DbSet<ConfiguracionGrupoMuscular> ConfiguracionesGrupoMuscular { get; set; }
    public DbSet<Ejercicio> Ejercicios { get; set; }
    public DbSet<Rutina> Rutinas { get; set; }
    public DbSet<Sesion> Sesiones { get; set; }
    public DbSet<EjercicioAsignado> EjerciciosAsignados { get; set; }
    public DbSet<Puntaje> Puntajes { get; set; }

    public DbSet<Asistencia> Asistencias { get; set; }
    public DbSet<Invitacion> Invitaciones { get; set; }
    public DbSet<Logro> Logros { get; set; }
    public DbSet<DiaDeLaSemana> DiasDeLaSemana { get; set; }
    public DbSet<LogroSocio> LogrosSocios { get; set; }


    public DbSet<Administrador> Administradores { get; set; }

    public DbSet<Jornada> Jornadas { get; set; }


    public DbSet<MedidaCorporal> MedidasCorporales { get; set; }

    public DbSet<Foto> Fotos { get; set; }

    public DbSet<Notificacion> Notificaciones { get; set; }

    public DbSet<Entrenamiento> Entrenamientos { get; set; }

    public DbSet<Serie> Series { get; set; }
    public DbSet<SolicitudRutinaProfesor> SolicitudesRutinaProfesor { get; set; }

    public DbSet<Actividad> Actividades { get; set; }

    public DbSet<Ingreso> Ingresos { get; set; }

    public DbSet<Valoracion> Valoraciones { get; set; }
    public DbSet<Amistad> Amistades { get; set; }
    public DbSet<LogroGimnasio> LogrosGimnasio { get; set; }
    public DbSet<LogroSocio> LogrosSocio { get; set; }
    public DbSet<Reporte> Reportes { get; set; }
    public DbSet<BatallaPunto> Batallas { get; set; }

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
        modelBuilder.Entity<Logro>()
            .HasIndex(l => l.NombreClave)
            .IsUnique();

        // índice único por gimnasio + logro
        modelBuilder.Entity<LogroGimnasio>()
            .HasIndex(lg => new { lg.GimnasioId, lg.LogroId })
            .IsUnique();

        // índice único para evitar duplicar logro de socio
        modelBuilder.Entity<LogroSocio>()
            .HasIndex(ls => new { ls.LogroId, ls.GimnasioId, ls.SocioId })
            .IsUnique();
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

        modelBuilder.Entity<Gimnasio>()
     .HasMany(g => g.Profesores)
     .WithOne(p => p.Gimnasio)
     .HasForeignKey(p => p.GimnasioId)
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

        // Rutina → Usuario creador
        modelBuilder.Entity<Rutina>()
            .HasOne(r => r.Usuario)
            .WithMany(u => u.RutinasCreadas) // colección en Usuario que representa rutinas creadas
            .HasForeignKey(r => r.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        // Rutina → Socio asignado
        modelBuilder.Entity<Rutina>()
            .HasOne(r => r.Socio)
            .WithMany(s => s.RutinasAsignadas) // colección en Socio que representa rutinas asignadas
            .HasForeignKey(r => r.SocioId)
            .OnDelete(DeleteBehavior.Restrict);

        // Sesiones → Rutina
        modelBuilder.Entity<Sesion>()
            .HasOne(s => s.Rutina)
            .WithMany(r => r.Sesiones) // colección en Rutina de todas sus sesiones
            .HasForeignKey(s => s.RutinaId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Ejercicio>()
            .HasOne(e => e.GrupoMuscular)
            .WithMany(g => g.Ejercicios)
            .HasForeignKey(e => e.GrupoMuscularId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Gimnasio>()
      .HasOne(g => g.Administrador)
      .WithOne(a => a.Gimnasio)
      .HasForeignKey<Gimnasio>(g => g.AdministradorId)
      .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Notificacion>(entity =>
        {
            entity.HasKey(n => n.Id);

            entity.HasOne(n => n.UsuarioEmisor)
                  .WithMany(u => u.NotificacionesEnviadas)
                  .HasForeignKey(n => n.UsuarioEmisorId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(n => n.UsuarioReceptor)
                  .WithMany(u => u.NotificacionesRecibidas)
                  .HasForeignKey(n => n.UsuarioReceptorId)
                  .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Asistencia>(entity =>
            {
                entity.HasKey(a => a.Id);

                entity.HasOne(a => a.Usuario)
                      .WithMany(u => u.Asistencias)
                      .HasForeignKey(a => a.UsuarioId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .IsRequired();

                entity.HasOne(a => a.Gimnasio)
                      .WithMany(g => g.Asistencias)
                      .HasForeignKey(a => a.GimnasioId)
                      .OnDelete(DeleteBehavior.Cascade)
                      .IsRequired();
            });


        });
        modelBuilder.Entity<Valoracion>()
      .HasOne(v => v.Emisor)
      .WithMany(u => u.ValoracionesEnviadas)
      .HasForeignKey(v => v.EmisorId)
      .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Valoracion>()
            .HasOne(v => v.Receptor)
            .WithMany(u => u.ValoracionesRecibidas)
            .HasForeignKey(v => v.ReceptorId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Valoracion>()
            .HasOne(v => v.Rutina)
            .WithMany(r => r.Valoraciones)
            .HasForeignKey(v => v.RutinaId)
            .OnDelete(DeleteBehavior.SetNull);

    }

}