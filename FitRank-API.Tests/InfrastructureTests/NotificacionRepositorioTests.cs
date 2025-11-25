using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Persistence;
using FitRank_API.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FitRank_API.tests.InfrastructureTests;

public class NotificacionRepositorioTests
{
    private DbContextOptions<FitRankDbContext> GetInMemoryDbOptions(string dbName)
    {
        return new DbContextOptionsBuilder<FitRankDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .EnableSensitiveDataLogging()
            .Options;
    }

    private async Task<(Usuario, Usuario)> SeedUsuario(FitRankDbContext context)
    {
        var usuarioEmisor = new Usuario
        {
            Nombre = "Carlos",
            Apellido = "Lopez",
            Email = "carlos.lopez@example.com"
        };

        var usuarioReceptor = new Usuario
        {
            Nombre = "Ana",
            Apellido = "Gomez",
            Email = "ana.gomez@example.com"
        };

        context.Usuarios.AddRange(usuarioEmisor, usuarioReceptor);
        await context.SaveChangesAsync();

        return (usuarioEmisor, usuarioReceptor);
    }


    // agregar notificacion deberia guardar correctamente
    [Fact]
    public async Task AgregarNotificacion_DeberiaGuardarCorrectamente()
    {
        // Arrange
        var options = GetInMemoryDbOptions("AgregarNotificacionDb");
        using var context = new FitRankDbContext(options);
        var (usuarioEmisor, usuarioReceptor) = await SeedUsuario(context);
        var notificacionRepositorio = new NotificacionRepositorioImpl(context);

        var notificacion = new Notificacion
        {
            Mensaje = "Tienes una nueva solicitud de amistad",
            FechaEnvio = DateTime.UtcNow,
            UsuarioEmisorId = usuarioEmisor.Id,
            UsuarioReceptorId = usuarioReceptor.Id
        };

        // Act
        await notificacionRepositorio.AgregarAsync(notificacion);

        // FluentAssert
        var notificacionEnDb = await context.Notificaciones.FirstOrDefaultAsync(n => n.Id == notificacion.Id);
        Assert.NotNull(notificacionEnDb);
        Assert.Equal("Tienes una nueva solicitud de amistad", notificacionEnDb!.Mensaje);
        Assert.Equal(usuarioEmisor.Id, notificacionEnDb.UsuarioEmisorId);
        Assert.Equal(usuarioReceptor.Id, notificacionEnDb.UsuarioReceptorId);
    }

    //obtener por usuario receptor
    [Fact]
    public async Task ObtenerNotificacionesPorUsuarioReceptor_DeberiaRetornarListaCorrectamente()
    {
        // Arrange
        var options = GetInMemoryDbOptions("ObtenerNotificacionesPorUsuarioReceptorDb");
        using var context = new FitRankDbContext(options);
        var (usuarioEmisor, usuarioReceptor) = await SeedUsuario(context);
        var notificacionRepositorio = new NotificacionRepositorioImpl(context);

        var notificaciones = new List<Notificacion>
        {
            new Notificacion
            {
                Mensaje = "Solicitud de amistad 1",
                FechaEnvio = DateTime.UtcNow,
                UsuarioEmisorId = usuarioEmisor.Id,
                UsuarioReceptorId = usuarioReceptor.Id
            },
            new Notificacion
            {
                Mensaje = "Solicitud de amistad 2",
                FechaEnvio = DateTime.UtcNow,
                UsuarioEmisorId = usuarioEmisor.Id,
                UsuarioReceptorId = usuarioReceptor.Id
            }
        };

        context.Notificaciones.AddRange(notificaciones);
        await context.SaveChangesAsync();

        // Act
        var resultado = await notificacionRepositorio.ObtenerPorUsuarioAsync(usuarioReceptor.Id);

        // Assert
        resultado.Should().HaveCount(2);
        resultado.Select(n => n.Mensaje).Should().Contain(new[] { "Solicitud de amistad 1", "Solicitud de amistad 2" });
    }

    // marcar como leida deberia actualizar correctamente
    [Fact]
    public async Task MarcarNotificacionComoLeida_DeberiaActualizarCorrectamente()
    {
        // Arrange
        var options = GetInMemoryDbOptions("MarcarNotificacionComoLeidaDb");
        using var context = new FitRankDbContext(options);
        var (usuarioEmisor, usuarioReceptor) = await SeedUsuario(context);
        var notificacionRepositorio = new NotificacionRepositorioImpl(context);

        var notificacion = new Notificacion
        {
            Mensaje = "Tienes una nueva solicitud de amistad",
            FechaEnvio = DateTime.UtcNow,
            UsuarioEmisorId = usuarioEmisor.Id,
            UsuarioReceptorId = usuarioReceptor.Id,
            Leido = false
        };

        context.Notificaciones.Add(notificacion);
        await context.SaveChangesAsync();

        // Act
        await notificacionRepositorio.MarcarComoLeidaAsync(notificacion.Id);

        // FluentAssert
        var notificacionEnDb = await context.Notificaciones.FirstOrDefaultAsync(n => n.Id == notificacion.Id);
        notificacionEnDb.Should().NotBeNull();
        notificacionEnDb!.Leido.Should().BeTrue();
    }


    // desactivar notificacion deberia actualizar correctamente
    [Fact]
    public async Task DesactivarNotificacion_DeberiaActualizarCorrectamente()
    {
        // Arrange
        var options = GetInMemoryDbOptions("DesactivarNotificacionDb");
        using var context = new FitRankDbContext(options);
        var (usuarioEmisor, usuarioReceptor) = await SeedUsuario(context);
        var notificacionRepositorio = new NotificacionRepositorioImpl(context);

        var notificacion = new Notificacion
        {
            Mensaje = "Tienes una nueva solicitud de amistad",
            FechaEnvio = DateTime.UtcNow,
            UsuarioEmisorId = usuarioEmisor.Id,
            UsuarioReceptorId = usuarioReceptor.Id,
            Activa = true
        };

        context.Notificaciones.Add(notificacion);
        await context.SaveChangesAsync();

        // Act
        await notificacionRepositorio.DesactivarAsync(notificacion.Id);

        // FluentAssert
        var notificacionEnDb = await context.Notificaciones.FirstOrDefaultAsync(n => n.Id == notificacion.Id);
        notificacionEnDb.Should().NotBeNull();
        notificacionEnDb!.Activa.Should().BeFalse();
    }

    //obtener por id deberia retornar correctamente
    [Fact]
    public async Task ObtenerNotificacionPorId_DeberiaRetornarCorrectamente()
    {
        // Arrange
        var options = GetInMemoryDbOptions("ObtenerNotificacionPorIdDb");
        using var context = new FitRankDbContext(options);
        var (usuarioEmisor, usuarioReceptor) = await SeedUsuario(context);
        var notificacionRepositorio = new NotificacionRepositorioImpl(context);
        var notificacion = new Notificacion
        {
            Mensaje = "Tienes una nueva solicitud de amistad",
            FechaEnvio = DateTime.UtcNow,
            UsuarioEmisorId = usuarioEmisor.Id,
            UsuarioReceptorId = usuarioReceptor.Id
        };
        context.Notificaciones.Add(notificacion);
        await context.SaveChangesAsync();
        // Act
        var resultado = await notificacionRepositorio.ObtenerPorIdAsync(notificacion.Id);
        // Assert
        resultado.Should().NotBeNull();
        resultado!.Mensaje.Should().Be("Tienes una nueva solicitud de amistad");
        resultado.UsuarioEmisorId.Should().Be(usuarioEmisor.Id);
        resultado.UsuarioReceptorId.Should().Be(usuarioReceptor.Id);
    }

    //actualizar notificacion exitosamente
    [Fact]
    public async Task ActualizarNotificacion_DeberiaActualizarCorrectamente()
    {
        // Arrange
        var options = GetInMemoryDbOptions("ActualizarNotificacionDb");
        using var context = new FitRankDbContext(options);
        var (usuarioEmisor, usuarioReceptor) = await SeedUsuario(context);
        var notificacionRepositorio = new NotificacionRepositorioImpl(context);
        var notificacion = new Notificacion
        {
            Mensaje = "Tienes una nueva solicitud de amistad",
            FechaEnvio = DateTime.UtcNow,
            UsuarioEmisorId = usuarioEmisor.Id,
            UsuarioReceptorId = usuarioReceptor.Id
        };
        context.Notificaciones.Add(notificacion);
        await context.SaveChangesAsync();

        // Act
        notificacion.Mensaje = "Solicitud de amistad actualizada";
        var actualizado = await notificacionRepositorio.ActualizarAsync(notificacion);

        // FluentAssert
        actualizado.Should().NotBeNull();
        actualizado!.Mensaje.Should().Be("Solicitud de amistad actualizada");
        var notificacionEnDb = await context.Notificaciones.FirstOrDefaultAsync(n => n.Id == notificacion.Id);
        notificacionEnDb.Should().NotBeNull();
        notificacionEnDb!.Mensaje.Should().Be("Solicitud de amistad actualizada");
    }

    //actualizar notificacion que no existe deberia retornar null
    [Fact]
    public async Task ActualizarNotificacion_Inexistente_DeberiaRetornarNull()
    {
        // Arrange
        var options = GetInMemoryDbOptions("ActualizarNotificacionInexistenteDb");
        using var context = new FitRankDbContext(options);
        var notificacionRepositorio = new NotificacionRepositorioImpl(context);
        var notificacionInexistente = new Notificacion
        {
            Id = 999, // ID que no existe en la base de datos
            Mensaje = "Notificacion inexistente",
            FechaEnvio = DateTime.UtcNow,
            UsuarioEmisorId = 1,
            UsuarioReceptorId = 2
        };

        // Act
        var resultado = await notificacionRepositorio.ActualizarAsync(notificacionInexistente);

        // Assert
        resultado.Should().BeNull();
    }

    //obtener gimnasio id usuario
    [Fact]
    public async Task ObtenerUsuariosDelGimnasio_DeberiaRetornarListaCorrectamente()
    {
        // Arrange
        var options = GetInMemoryDbOptions("ObtenerUsuariosDelGimnasioDb");
        using var context = new FitRankDbContext(options);
        var gimnasio = new Gimnasio
        {
            Nombre = "Gimnasio Central",
            Direccion = "Calle Falsa 123"
        };
        context.Gimnasios.Add(gimnasio);
        await context.SaveChangesAsync();
        var usuarios = new List<Usuario>
        {
            new Administrador { Nombre = "Usuario1", GimnasioId = gimnasio.Id, Email="aa@aa.aa" },
            new Profesor { Nombre = "Usuario2", GimnasioId = gimnasio.Id, Email="aa@aa.aa"  },
            new Socio { Nombre = "Usuario3", GimnasioId = gimnasio.Id, Email="aa@aa.aa", Nivel="principiante"}
        };
        context.Usuarios.AddRange(usuarios);
        await context.SaveChangesAsync();
        var notificacionRepositorio = new NotificacionRepositorioImpl(context);
        // Act
        var resultado1 = await notificacionRepositorio.ObtenerGimnasioIdDeUsuario(usuarios[0].Id);
        var resultado2 = await notificacionRepositorio.ObtenerGimnasioIdDeUsuario(usuarios[1].Id);
        var resultado3 = await notificacionRepositorio.ObtenerGimnasioIdDeUsuario(usuarios[2].Id);
        // Assert
        resultado1.Should().Be(gimnasio.Id);
        resultado2.Should().Be(gimnasio.Id);
        resultado3.Should().Be(gimnasio.Id);
    }

    //Obtener usuarios del gimnacio
    [Fact]
    public async Task ObtenerUsuariosDelGimnasio_DeberiaRetornarUsuariosCorrectamente()
    {
        // Arrange
        var options = GetInMemoryDbOptions("ObtenerUsuariosDelGimnasioDb2");
        using var context = new FitRankDbContext(options);
        var gimnasio = new Gimnasio
        {
            Nombre = "Gimnasio Central",
            Direccion = "Calle Falsa 123"
        };
        context.Gimnasios.Add(gimnasio);
        await context.SaveChangesAsync();
        var usuarios = new List<Usuario>
        {
            new Administrador { Nombre = "Usuario1", GimnasioId = gimnasio.Id, Email="aa@aa.aa" },
            new Profesor { Nombre = "Usuario2", GimnasioId = gimnasio.Id, Email="aa@aa.aa"  },
            new Socio { Nombre = "Usuario3", GimnasioId = gimnasio.Id, Email="aa@aa.aa", Nivel="principiante"}
        };
        context.Usuarios.AddRange(usuarios);
        await context.SaveChangesAsync();
        var notificacionRepositorio = new NotificacionRepositorioImpl(context);
        // Act
        var resultado = await notificacionRepositorio.ObtenerUsuariosDelGimnasio(gimnasio.Id);
        // Assert
        resultado.Should().HaveCount(3);
        resultado.Select(u => u.Nombre).Should().Contain(new[] { "Usuario1", "Usuario2", "Usuario3" });
    }

    //enciar notificacion global deberia crear notificaciones para todos los usuarios del gimnasio
    [Fact]
    public async Task EnviarNotificacionGlobal_DeberiaCrearNotificacionesParaTodosLosUsuariosDelGimnasio()
    {
        // Arrange
        var options = GetInMemoryDbOptions("EnviarNotificacionGlobalDb");
        using var context = new FitRankDbContext(options);
        var gimnasio = new Gimnasio
        {
            Nombre = "Gimnasio Central",
            Direccion = "Calle Falsa 123"
        };
        context.Gimnasios.Add(gimnasio);
        await context.SaveChangesAsync();
        var usuarios = new List<Usuario>
        {
            new Administrador { Nombre = "Usuario1", GimnasioId = gimnasio.Id, Email="aa@aa.aa" },
            new Profesor { Nombre = "Usuario2", GimnasioId = gimnasio.Id, Email="aa@aa.aa"  },
            new Socio { Nombre = "Usuario3", GimnasioId = gimnasio.Id, Email="aa@aa.aa", Nivel="principiante"}
        };
        context.Usuarios.AddRange(usuarios);
        await context.SaveChangesAsync();
        var notificacionRepositorio = new NotificacionRepositorioImpl(context);
        // Act
        await notificacionRepositorio.EnviarNotificacionGlobal(usuarios[0].Id, "Mantenimiento", "El gimnasio estará cerrado mañana.");
        // Assert
        var notificacionesEnDb = await context.Notificaciones.ToListAsync();
        notificacionesEnDb.Should().HaveCount(3);
        notificacionesEnDb.Select(n => n.Mensaje).Should().AllBeEquivalentTo("El gimnasio estará cerrado mañana.");
    }

    //obtener notificaciones del gimnasio por admin
    [Fact]
    public async Task ObtenerNotificacionesDelGimnasioPorAdmin_DeberiaRetornarCorrectamente()
    {
        // Arrange
        var options = GetInMemoryDbOptions("ObtenerNotificacionesDelGimnasioPorAdminDb");
        using var context = new FitRankDbContext(options);
        var gimnasio = new Gimnasio
        {
            Nombre = "Gimnasio Central",
            Direccion = "Calle Falsa 123"
        };
        context.Gimnasios.Add(gimnasio);
        await context.SaveChangesAsync();
        var admin = new Administrador { Nombre = "Usuario1", GimnasioId = gimnasio.Id, Email = "aa@aa.aa" };
        context.Usuarios.Add(admin);
        await context.SaveChangesAsync();
        var notificaciones = new List<Notificacion>
        {
            new Notificacion
            {
                Mensaje = "Notificacion 1",
                FechaEnvio = DateTime.UtcNow,
                UsuarioEmisorId = admin.Id,
                UsuarioReceptorId = admin.Id
            },
            new Notificacion
            {
                Mensaje = "Notificacion 2",
                FechaEnvio = DateTime.UtcNow,
                UsuarioEmisorId = admin.Id,
                UsuarioReceptorId = admin.Id
            }
        };
        context.Notificaciones.AddRange(notificaciones);
        await context.SaveChangesAsync();

        var notificacionRepositorio = new NotificacionRepositorioImpl(context);

        // Act
        var resultado = await notificacionRepositorio.ObtenerNotificacionesDelGimnasioadmin(admin.Id);

        // Assert
        resultado.Should().HaveCount(2);
        resultado.Select(n => n.Mensaje).Should().Contain(new[] { "Notificacion 1", "Notificacion 2" });
    }

    //obtener notificaciones del gimnasio por admin, no hay notificaciones
    [Fact]
    public async Task ObtenerNotificacionesDelGimnasioPorAdmin_SinNotificaciones_DeberiaRetornarListaVacia()
    {
        // Arrange
        var options = GetInMemoryDbOptions("ObtenerNotificacionesDelGimnasioPorAdminSinNotificacionesDb");
        using var context = new FitRankDbContext(options);
        var gimnasio = new Gimnasio
        {
            Nombre = "Gimnasio Central",
            Direccion = "Calle Falsa 123"
        };
        context.Gimnasios.Add(gimnasio);
        await context.SaveChangesAsync();
        new Administrador { Nombre = "Usuario1", GimnasioId = gimnasio.Id, Email = "aa@aa.aa" };

        var notificacionRepositorio = new NotificacionRepositorioImpl(context);

        // Act
        var resultado = await notificacionRepositorio.ObtenerNotificacionesDelGimnasioadmin(1);

        // Assert
        resultado.Should().BeEmpty();
    }

    //obtener notificaciones del gimnasio por admin, con filtro por usuario
    [Fact]
    public async Task ObtenerNotificacionesDelGimnasio_DeberiaRetornarNotificacionesDeUsuariosIndicados()
    {
        // Arrange
        var options = GetInMemoryDbOptions("ObtenerNotificacionesDelGimnasioDb");
        using var context = new FitRankDbContext(options);

        // Crear usuarios
        var usuario1 = new Usuario { Nombre = "Usuario1", Email = "u1@a.com" };
        var usuario2 = new Usuario { Nombre = "Usuario2", Email = "u2@a.com" };
        var usuario3 = new Usuario { Nombre = "Usuario3", Email = "u3@a.com" };
        context.Usuarios.AddRange(usuario1, usuario2, usuario3);
        await context.SaveChangesAsync();

        // Crear notificaciones
        var notificaciones = new List<Notificacion>
        {
            new Notificacion { Mensaje = "N1", UsuarioEmisorId = usuario1.Id, UsuarioReceptorId = usuario2.Id, FechaEnvio = DateTime.UtcNow },
            new Notificacion { Mensaje = "N2", UsuarioEmisorId = usuario2.Id, UsuarioReceptorId = usuario3.Id, FechaEnvio = DateTime.UtcNow },
            new Notificacion { Mensaje = "N3", UsuarioEmisorId = usuario3.Id, UsuarioReceptorId = usuario1.Id, FechaEnvio = DateTime.UtcNow },
            new Notificacion { Mensaje = "N4", UsuarioEmisorId = 999, UsuarioReceptorId = 998, FechaEnvio = DateTime.UtcNow } // No relevante
        };
        context.Notificaciones.AddRange(notificaciones);
        await context.SaveChangesAsync();

        var repo = new NotificacionRepositorioImpl(context);
        var usuarioIds = new List<long> { usuario1.Id, usuario2.Id };

        // Act
        var resultado = await repo.ObtenerNotificacionesDelGimnasio(usuarioIds);

        // Assert
        resultado.Should().HaveCount(3);
        resultado.Select(n => n.Mensaje).Should().Contain(new[] { "N1", "N2", "N3" });
    }
}

