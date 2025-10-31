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



}

