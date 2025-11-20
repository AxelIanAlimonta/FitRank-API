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
}