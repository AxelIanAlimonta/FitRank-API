using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Persistence;
using FitRank_API.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FitRank_API.tests.InfrastructureTests;

public class AsistenciaRepositorioTests
{
    private DbContextOptions<FitRankDbContext> CreateInMemoryOptions(string dbName)
    {
        return new DbContextOptionsBuilder<FitRankDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
    }

    private async Task<(Usuario, Gimnasio)> SeedData(FitRankDbContext context)
    {
        var gimnasio = new Gimnasio
        {
            Nombre = "Gimnasio de Prueba",
            Direccion = "Calle Falsa 123",
            RazonSocial = "Gimnasio S.A.",
            LogoUrl = "http://logo.url",
            ColorPrincipal = "#FFFFFF",
            ColorSecundario = "#000000",
            Email = "gimnasio.prueba@example.com"
        };
        context.Gimnasios.Add(gimnasio);
        await context.SaveChangesAsync();

        var usuario = new Usuario
        {
            Nombre = "Juan",
            Apellido = "Pérez",
            Email = "juan.perez@example.com"
        };
        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();

        return (usuario, gimnasio);
    }

    //agregar asistencia deberia guardar correctamente
    [Fact]
    public async Task AgregarAsistencia_DeberiaGuardarCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("AgregarAsistenciaDb");
        using var context = new FitRankDbContext(options);
        var asistenciaRepositorioMock = new AsistenciaRepositorioImpl(context);
        var (usuario, gimnasio) = await SeedData(context);

        var asistencia = new Asistencia
        {
            Fecha = DateTime.UtcNow,
            UsuarioId = usuario.Id,
            GimnasioId = gimnasio.Id,
            Presente = true,
            Observaciones = "Asistencia de prueba"
        };

        // Act
        await asistenciaRepositorioMock.AgregarAsync(asistencia);

        var asistenciaGuardada = await context.Asistencias.FirstOrDefaultAsync(a => a.Id == asistencia.Id);

        // FluentAssert
        asistenciaGuardada.Should().NotBeNull();
        asistenciaGuardada!.Fecha.Date.Should().Be(asistencia.Fecha.Date);
        asistenciaGuardada.UsuarioId.Should().Be(asistencia.UsuarioId);
        asistenciaGuardada.GimnasioId.Should().Be(asistencia.GimnasioId);
        asistenciaGuardada.Presente.Should().BeTrue();
    }

    //obtener lista de asistencias
    [Fact]
    public async Task ObtenerListaDeAsistencias_DeberiaRetornarTodasLasAsistencias()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerListaDeAsistenciasDb");
        using var context = new FitRankDbContext(options);
        var asistenciaRepositorioMock = new AsistenciaRepositorioImpl(context);
        var (usuario, gimnasio) = await SeedData(context);

        var asistencia1 = new Asistencia
        {
            Fecha = DateTime.UtcNow.AddDays(-1),
            UsuarioId = usuario.Id,
            GimnasioId = gimnasio.Id,
            Presente = true,
            Observaciones = "Asistencia de prueba 1"

        };

        var asistencia2 = new Asistencia
        {
            Fecha = DateTime.UtcNow,
            UsuarioId = usuario.Id,
            GimnasioId = gimnasio.Id,
            Presente = false,
            Observaciones = "Asistencia de prueba 2"
        };

        context.Asistencias.AddRange(asistencia1, asistencia2);
        await context.SaveChangesAsync();

        // Act
        var asistencias = await asistenciaRepositorioMock.ObtenerTodasAsync();

        // FluentAssert
        asistencias.Should().HaveCount(2);
        asistencias.Should().Contain(a => a.Id == asistencia1.Id);
        asistencias.Should().Contain(a => a.Id == asistencia2.Id);
    }

    //obtener lista vacia
    [Fact]
    public async Task ObtenerListaDeAsistencias_Vacia_DeberiaRetornarListaVacia()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerListaDeAsistenciasVaciaDb");
        using var context = new FitRankDbContext(options);
        var asistenciaRepositorioMock = new AsistenciaRepositorioImpl(context);

        // Act
        var asistencias = await asistenciaRepositorioMock.ObtenerTodasAsync();

        // FluentAssert
        asistencias.Should().BeEmpty();
    }

    //obtener por id
    [Fact]
    public async Task ObtenerAsistenciaPorId_DeberiaRetornarAsistenciaCorrecta()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerAsistenciaPorIdDb");
        using var context = new FitRankDbContext(options);
        var asistenciaRepositorioMock = new AsistenciaRepositorioImpl(context);
        var (usuario, gimnasio) = await SeedData(context);

        var asistencia = new Asistencia
        {
            Fecha = DateTime.UtcNow,
            UsuarioId = usuario.Id,
            GimnasioId = gimnasio.Id,
            Presente = true,
            Observaciones = "Asistencia de prueba"
        };

        context.Asistencias.Add(asistencia);
        await context.SaveChangesAsync();

        // Act
        var asistenciaObtenida = await asistenciaRepositorioMock.ObtenerPorIdAsync(asistencia.Id);

        // FluentAssert
        asistenciaObtenida.Should().NotBeNull();
        asistenciaObtenida!.Id.Should().Be(asistencia.Id);
        asistenciaObtenida.UsuarioId.Should().Be(asistencia.UsuarioId);
        asistenciaObtenida.GimnasioId.Should().Be(asistencia.GimnasioId);
    }

    //obtener por id inexistente
    [Fact]
    public async Task ObtenerAsistenciaPorId_Inexistente_DeberiaRetornarNull()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerAsistenciaPorIdInexistenteDb");
        using var context = new FitRankDbContext(options);
        var asistenciaRepositorioMock = new AsistenciaRepositorioImpl(context);

        // Act
        var asistenciaObtenida = await asistenciaRepositorioMock.ObtenerPorIdAsync(999);

        // FluentAssert
        asistenciaObtenida.Should().BeNull();
    }

    //actualizar asistencia
    [Fact]
    public async Task ActualizarAsistencia_DeberiaActualizarCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("ActualizarAsistenciaDb");
        using var context = new FitRankDbContext(options);
        var asistenciaRepositorioMock = new AsistenciaRepositorioImpl(context);
        var (usuario, gimnasio) = await SeedData(context);

        var asistencia = new Asistencia
        {
            Fecha = DateTime.UtcNow,
            UsuarioId = usuario.Id,
            GimnasioId = gimnasio.Id,
            Presente = true,
            Observaciones = "Asistencia de prueba"
        };

        context.Asistencias.Add(asistencia);
        await context.SaveChangesAsync();

        // Act
        asistencia.Presente = false;
        await asistenciaRepositorioMock.ActualizarAsync(asistencia);

        var asistenciaActualizada = await context.Asistencias.FirstOrDefaultAsync(a => a.Id == asistencia.Id);

        // FluentAssert
        asistenciaActualizada.Should().NotBeNull();
        asistenciaActualizada!.Presente.Should().BeFalse();
    }

    //actualizar asistencia inexistente
    [Fact]
    public async Task ActualizarAsistencia_Inexistente_DeberiaDevolverNull()
    {
        // Arrange
        var options = CreateInMemoryOptions("ActualizarAsistenciaInexistenteDb");
        using var context = new FitRankDbContext(options);
        var asistenciaRepositorioMock = new AsistenciaRepositorioImpl(context);

        var asistenciaInexistente = new Asistencia
        {
            Id = 999,
            Fecha = DateTime.UtcNow,
            UsuarioId = 1,
            GimnasioId = 1,
            Presente = true,
            Observaciones = "Asistencia inexistente"
        };
        // Act
        var resultado = await asistenciaRepositorioMock.ActualizarAsync(asistenciaInexistente);
        // FluentAssert
        resultado.Should().BeNull();
    }

    //eliminar asistencia
    [Fact]
    public async Task EliminarAsistencia_DeberiaEliminarCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("EliminarAsistenciaDb");
        using var context = new FitRankDbContext(options);
        var asistenciaRepositorioMock = new AsistenciaRepositorioImpl(context);
        var (usuario, gimnasio) = await SeedData(context);

        var asistencia = new Asistencia
        {
            Fecha = DateTime.UtcNow,
            UsuarioId = usuario.Id,
            GimnasioId = gimnasio.Id,
            Presente = true,
            Observaciones = "Asistencia de prueba"
        };

        context.Asistencias.Add(asistencia);
        await context.SaveChangesAsync();

        // Act
        await asistenciaRepositorioMock.EliminarAsync(asistencia.Id);

        var asistenciaEliminada = await context.Asistencias.FirstOrDefaultAsync(a => a.Id == asistencia.Id);

        // FluentAssert
        asistenciaEliminada.Should().BeNull();
    }

    //eliminar asistencia inexistente
    [Fact]
    public async Task EliminarAsistencia_Inexistente_DeberiaDevolverFalse()
    {
        // Arrange
        var options = CreateInMemoryOptions("EliminarAsistenciaInexistenteDb");
        using var context = new FitRankDbContext(options);
        var asistenciaRepositorioMock = new AsistenciaRepositorioImpl(context);
        // Act
        var resultado = await asistenciaRepositorioMock.EliminarAsync(999);
        // FluentAssert
        resultado.Should().BeFalse();
    }


    [Fact]
    public async Task ObtenerPorUsuarioYFechaAsync_DeberiaRetornarAsistenciaCorrecta()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerPorUsuarioYFechaDb");
        using var context = new FitRankDbContext(options);
        var repo = new AsistenciaRepositorioImpl(context);
        var (usuario, gimnasio) = await SeedData(context);

        var fecha = DateTime.UtcNow.Date;
        var asistencia = new Asistencia
        {
            Fecha = fecha,
            UsuarioId = usuario.Id,
            GimnasioId = gimnasio.Id,
            Presente = true,
            Observaciones = "Obs"
        };
        context.Asistencias.Add(asistencia);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repo.ObtenerPorUsuarioYFechaAsync(usuario.Id, fecha);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.UsuarioId.Should().Be(usuario.Id);
        resultado.Fecha.Date.Should().Be(fecha);
    }

    [Fact]
    public async Task ObtenerPorUsuarioAsync_DeberiaRetornarTodasLasAsistenciasDelUsuario()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerPorUsuarioDb");
        using var context = new FitRankDbContext(options);
        var repo = new AsistenciaRepositorioImpl(context);
        var (usuario, gimnasio) = await SeedData(context);

        var asistencia1 = new Asistencia
        {
            Fecha = DateTime.UtcNow.AddDays(-1),
            UsuarioId = usuario.Id,
            GimnasioId = gimnasio.Id,
            Presente = true,
            Observaciones = "Obs"
        };
        var asistencia2 = new Asistencia
        {
            Fecha = DateTime.UtcNow,
            UsuarioId = usuario.Id,
            GimnasioId = gimnasio.Id,
            Presente = false,
            Observaciones = "Obs"
        };
        context.Asistencias.AddRange(asistencia1, asistencia2);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repo.ObtenerPorUsuarioAsync(usuario.Id);

        // Assert
        resultado.Should().HaveCount(2);
        resultado.All(a => a.UsuarioId == usuario.Id).Should().BeTrue();
    }

    [Fact]
    public async Task ObtenerPorGimnasioYRangoAsync_DeberiaFiltrarPorFechas()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerPorGimnasioYRangoDb");
        using var context = new FitRankDbContext(options);
        var repo = new AsistenciaRepositorioImpl(context);
        var (usuario, gimnasio) = await SeedData(context);

        var asistencia1 = new Asistencia
        {
            Fecha = new DateTime(2024, 1, 1),
            UsuarioId = usuario.Id,
            GimnasioId = gimnasio.Id,
            Presente = true,
            Observaciones = "Obs1"
        };
        var asistencia2 = new Asistencia
        {
            Fecha = new DateTime(2024, 2, 1),
            UsuarioId = usuario.Id,
            GimnasioId = gimnasio.Id,
            Presente = true,
            Observaciones = "Obs2"
        };
        context.Asistencias.AddRange(asistencia1, asistencia2);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repo.ObtenerPorGimnasioYRangoAsync(gimnasio.Id, new DateTime(2024, 1, 15), new DateTime(2024, 2, 15));

        // Assert
        resultado.Should().HaveCount(1);
        resultado.First().Fecha.Should().Be(new DateTime(2024, 2, 1));
    }

    [Fact]
    public async Task ObtenerTodasConUsuarioAsync_DeberiaIncluirUsuarioYGimnasio()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerTodasConUsuarioDb");
        using var context = new FitRankDbContext(options);
        var repo = new AsistenciaRepositorioImpl(context);
        var (usuario, gimnasio) = await SeedData(context);

        var asistencia = new Asistencia
        {
            Fecha = DateTime.UtcNow,
            UsuarioId = usuario.Id,
            GimnasioId = gimnasio.Id,
            Presente = true,
            Observaciones = "Obs"
        };
        context.Asistencias.Add(asistencia);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repo.ObtenerTodasConUsuarioAsync();

        // Assert
        resultado.Should().NotBeEmpty();
        resultado.First().Usuario.Should().NotBeNull();
    }

    [Fact]
    public async Task ObtenerUltimaAsistenciaPorUsuarioAsync_DeberiaRetornarLaMasReciente()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerUltimaAsistenciaPorUsuarioDb");
        using var context = new FitRankDbContext(options);
        var repo = new AsistenciaRepositorioImpl(context);
        var (usuario, gimnasio) = await SeedData(context);

        var asistencia1 = new Asistencia
        {
            Fecha = DateTime.UtcNow.AddDays(-2),
            UsuarioId = usuario.Id,
            GimnasioId = gimnasio.Id,
            Presente = true,
            Observaciones = "Obs"
        };
        var asistencia2 = new Asistencia
        {
            Fecha = DateTime.UtcNow,
            UsuarioId = usuario.Id,
            GimnasioId = gimnasio.Id,
            Presente = true,
            Observaciones = "Obs"
        };
        context.Asistencias.AddRange(asistencia1, asistencia2);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repo.ObtenerUltimaAsistenciaPorUsuarioAsync(usuario.Id);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Fecha.Date.Should().Be(asistencia2.Fecha.Date);
    }
}