using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Persistence;
using FitRank_API.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FitRank_API.tests.InfrastructureTests;

public class SesionRepositorioTests
{

    private DbContextOptions<FitRankDbContext> CreateInMemoryOptions(string dbName)
    {
        return new DbContextOptionsBuilder<FitRankDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
    }

    private async Task<Rutina> SeedData(FitRankDbContext context)
    {
        var rutina = new Rutina
        {
            Nombre = "Rutina de Prueba",
            SocioId = 1,
            UsuarioId = 1
        };

        context.Rutinas.Add(rutina);
        await context.SaveChangesAsync();

        return rutina;
    }

    [Fact]
    public async Task CrearSesion_DeberiaPersistirSesion()
    {
        // Arrange
        var options = CreateInMemoryOptions("CrearSesion_DeberiaPersistirSesion");
        using var context = new FitRankDbContext(options);
        var repositorioMock = new SesionRepositorioImpl(context);

        var rutina = await SeedData(context);

        var sesion = new Sesion
        {
            NumeroDeSesion = 1,
            Nombre = "Sesión de Prueba",
            RutinaId = rutina.Id
        };

        // Act
        await repositorioMock.AgregarAsync(sesion);
        var sesionGuardada = await context.Sesiones.FirstOrDefaultAsync(s => s.Id == sesion.Id);

        // Assert
        sesionGuardada.Should().NotBeNull();
        sesionGuardada!.NumeroDeSesion.Should().Be(sesion.NumeroDeSesion);
        sesionGuardada.Nombre.Should().Be(sesion.Nombre);
        sesionGuardada.RutinaId.Should().Be(sesion.RutinaId);
    }

    [Fact]
    public async Task ObtenerSesion_PorId_DeberiaRetornarSesionCorrecta()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerSesion_PorId_DeberiaRetornarSesionCorrecta");
        using var context = new FitRankDbContext(options);
        var repositorioMock = new SesionRepositorioImpl(context);
        var rutina = await SeedData(context);
        var sesion = new Sesion
        {
            NumeroDeSesion = 2,
            Nombre = "Otra Sesión de Prueba",
            RutinaId = rutina.Id
        };
        context.Sesiones.Add(sesion);
        await context.SaveChangesAsync();

        // Act
        var sesionObtenida = await repositorioMock.ObtenerPorIdAsync(sesion.Id);

        // Assert
        sesionObtenida.Should().NotBeNull();
        sesionObtenida.NumeroDeSesion.Should().Be(sesion.NumeroDeSesion);
        sesionObtenida.Nombre.Should().Be(sesion.Nombre);
        sesionObtenida.RutinaId.Should().Be(sesion.RutinaId);
    }

    [Fact]
    public async Task ObtenerSesion_PorId_NoExistente_DeberiaRetornarNull()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerSesion_PorId_NoExistente_DeberiaRetornarNull");
        using var context = new FitRankDbContext(options);
        var repositorioMock = new SesionRepositorioImpl(context);

        // Act
        var sesionObtenida = await repositorioMock.ObtenerPorIdAsync(999); // ID que no existe

        // Assert
        sesionObtenida.Should().BeNull();
    }

    [Fact]
    public async Task EliminarSesion_DeberiaRemoverSesion()
    {
        // Arrange
        var options = CreateInMemoryOptions("EliminarSesion_DeberiaRemoverSesion");
        using var context = new FitRankDbContext(options);
        var rutina = await SeedData(context);
        var sesion = new Sesion
        {
            NumeroDeSesion = 3,
            Nombre = "Sesión a Eliminar",
            RutinaId = rutina.Id
        };
        context.Sesiones.Add(sesion);
        await context.SaveChangesAsync();

        // Act
        context.Sesiones.Remove(sesion);
        await context.SaveChangesAsync();
        var sesionEliminada = await context.Sesiones.FirstOrDefaultAsync(s => s.Id == sesion.Id);

        // Assert
        Assert.Null(sesionEliminada);
    }

    [Fact]
    public async Task EliminarSesion_NoExistente_DeberiaDevolverFalse()
    {
        // Arrange
        var options = CreateInMemoryOptions("EliminarSesion_NoExistente_NoDeberiaHacerNada");
        using var context = new FitRankDbContext(options);
        var repositorioMock = new SesionRepositorioImpl(context);

        var sesionInexistente = new Sesion
        {
            Id = 999,
            NumeroDeSesion = 4,
            Nombre = "Sesión Inexistente",
            RutinaId = 1
        };

        // Act
        var resultado = await repositorioMock.EliminarAsync(sesionInexistente.Id);

        // Assert
        resultado.Should().BeFalse(); // No se debería haber eliminado nada
    }

    [Fact]
    public async Task ObtenerTodasSesiones_DeberiaRetornarTodasLasSesiones()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerTodasSesiones_DeberiaRetornarTodasLasSesiones");
        using var context = new FitRankDbContext(options);
        var repositorioMock = new SesionRepositorioImpl(context);
        var rutina = await SeedData(context);

        var sesion1 = new Sesion
        {
            NumeroDeSesion = 1,
            Nombre = "Sesión 1",
            RutinaId = rutina.Id
        };
        var sesion2 = new Sesion
        {
            NumeroDeSesion = 2,
            Nombre = "Sesión 2",
            RutinaId = rutina.Id
        };
        context.Sesiones.AddRange(sesion1, sesion2);
        await context.SaveChangesAsync();

        // Act
        var sesionesObtenidas = await repositorioMock.ObtenerTodasAsync();

        // Assert
        sesionesObtenidas.Should().HaveCount(2);
        sesionesObtenidas.Should().ContainSingle(s => s.Nombre == "Sesión 1");
        sesionesObtenidas.Should().ContainSingle(s => s.Nombre == "Sesión 2");
    }

    [Fact]
    public async Task ObtenerTodasSesiones_CuandoNoHaySesiones_DeberiaRetornarListaVacia()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerTodasSesiones_CuandoNoHaySesiones_DeberiaRetornarListaVacia");
        using var context = new FitRankDbContext(options);
        var repositorioMock = new SesionRepositorioImpl(context);

        // Act
        var sesionesObtenidas = await repositorioMock.ObtenerTodasAsync();

        // Assert
        sesionesObtenidas.Should().BeEmpty();
    }

    //actualizar sesion test
    [Fact]
    public async Task ActualizarSesion_DeberiaActualizarSesionExistente()
    {
        // Arrange
        var options = CreateInMemoryOptions("ActualizarSesion_DeberiaActualizarSesionExistente");
        using var context = new FitRankDbContext(options);
        var repositorioMock = new SesionRepositorioImpl(context);
        var rutina = await SeedData(context);
        var sesion = new Sesion
        {
            NumeroDeSesion = 1,
            Nombre = "Sesión Original",
            RutinaId = rutina.Id
        };
        context.Sesiones.Add(sesion);
        await context.SaveChangesAsync();

        // Act
        sesion.Nombre = "Sesión Actualizada";
        await repositorioMock.ActualizarAsync(sesion.Id, sesion);

        // Assert
        var sesionActualizada = await context.Sesiones.FindAsync(sesion.Id);
        sesionActualizada.Should().NotBeNull();
        sesionActualizada.Nombre.Should().Be("Sesión Actualizada");
    }

    [Fact]
    public async Task ActualizarSesion_NoExistente_DeberiaRetornarNull()
    {
        // Arrange
        var options = CreateInMemoryOptions("ActualizarSesion_NoExistente_DeberiaRetornarNull");
        using var context = new FitRankDbContext(options);
        var repositorioMock = new SesionRepositorioImpl(context);

        var sesionInexistente = new Sesion
        {
            Id = 999,
            NumeroDeSesion = 1,
            Nombre = "Sesión Inexistente",
            RutinaId = 1
        };

        // Act
        var resultado = await repositorioMock.ActualizarAsync(sesionInexistente.Id, sesionInexistente);

        // Assert
        resultado.Should().BeNull();
    }

}