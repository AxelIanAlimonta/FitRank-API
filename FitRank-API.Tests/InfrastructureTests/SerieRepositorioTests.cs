using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Persistence;
using FitRank_API.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FitRank_API.Tests.InfrastructureTests;

public class SerieRepositorioTests
{
    private DbContextOptions<FitRankDbContext> CreateInMemoryOptions(string dbName)
    {
        return new DbContextOptionsBuilder<FitRankDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
    }

    private async Task<EjercicioAsignado> SeedData(FitRankDbContext context)
    {
        // var ejercicio = new Ejercicio
        // {
        //     Nombre = "Ejercicio de Prueba",
        //     Descripcion = "Descripción del ejercicio de prueba",
        // };

        // context.Ejercicios.Add(ejercicio);
        // await context.SaveChangesAsync();

        // var sesion = new Sesion
        // {
        //     NumeroDeSesion = 1,
        //     Nombre = "Sesión de Prueba",
        //     RutinaId = 1
        // };

        // context.Sesiones.Add(sesion);
        // await context.SaveChangesAsync();

        var ejercicioAsignado = new EjercicioAsignado
        {
            NumeroEjercicio = 1,
        };

        context.EjerciciosAsignados.Add(ejercicioAsignado);
        await context.SaveChangesAsync();

        return ejercicioAsignado;
    }

    // agregar serie deberia guardar correctamente
    [Fact]
    public async Task AgregarSerie_DeberiaGuardarCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("AgregarSerieDb");
        using var context = new FitRankDbContext(options);
        var serieRepositorioMock = new SerieRepositorioImpl(context);
        var ejercicioAsignado = await SeedData(context);

        var serie = new Serie
        {
            NumeroDeSerie = 1,
            Repeticiones = 10,
            Peso = 50.0,
            EjercicioAsignadoId = ejercicioAsignado.Id
        };

        // Act
        await serieRepositorioMock.AgregarAsync(serie);

        // Assert con FluentAssertions
        var serieGuardada = await context.Series.FirstOrDefaultAsync(s => s.Id == serie.Id);
        serieGuardada.Should().NotBeNull();
        serieGuardada!.NumeroDeSerie.Should().Be(1);
        serieGuardada.Repeticiones.Should().Be(10);
        serieGuardada.Peso.Should().Be(50.0);
        serieGuardada.EjercicioAsignadoId.Should().Be(ejercicioAsignado.Id);
    }

    // obtener lista de series
    [Fact]
    public async Task ObtenerSeries_DeberiaRetornarListaCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerSeriesDb");
        using var context = new FitRankDbContext(options);
        var ejercicioAsignado = await SeedData(context);

        context.Series.AddRange(
            new Serie { NumeroDeSerie = 1, Repeticiones = 10, Peso = 50.0, EjercicioAsignadoId = ejercicioAsignado.Id },
            new Serie { NumeroDeSerie = 2, Repeticiones = 12, Peso = 55.0, EjercicioAsignadoId = ejercicioAsignado.Id }
        );
        await context.SaveChangesAsync();

        var serieRepositorio = new SerieRepositorioImpl(context);

        // Act
        var series = await serieRepositorio.ObtenerTodasAsync();

        // Assert
        series.Should().HaveCount(2);
        series.Should().Contain(s => s.NumeroDeSerie == 1 && s.Repeticiones == 10 && s.Peso == 50.0);
        series.Should().Contain(s => s.NumeroDeSerie == 2 && s.Repeticiones == 12 && s.Peso == 55.0);
    }

    // lista de series vacia
    [Fact]
    public async Task ObtenerSeries_DeberiaRetornarListaVacia()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerSeriesVaciaDb");
        using var context = new FitRankDbContext(options);
        var serieRepositorio = new SerieRepositorioImpl(context);

        // Act
        var series = await serieRepositorio.ObtenerTodasAsync();

        // Assert
        series.Should().BeEmpty();
    }

    // obtener serie por id
    [Fact]
    public async Task ObtenerSeriePorId_DeberiaRetornarSerieCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerSeriePorIdDb");
        using var context = new FitRankDbContext(options);
        var ejercicioAsignado = await SeedData(context);

        var serie = new Serie
        {
            NumeroDeSerie = 1,
            Repeticiones = 10,
            Peso = 50.0,
            EjercicioAsignadoId = ejercicioAsignado.Id
        };
        context.Series.Add(serie);
        await context.SaveChangesAsync();

        var serieRepositorio = new SerieRepositorioImpl(context);

        // Act
        var serieObtenida = await serieRepositorio.ObtenerPorIdAsync(serie.Id);

        // Assert
        serieObtenida.Should().NotBeNull();
        serieObtenida!.NumeroDeSerie.Should().Be(1);
        serieObtenida.Repeticiones.Should().Be(10);
        serieObtenida.Peso.Should().Be(50.0);
        serieObtenida.EjercicioAsignadoId.Should().Be(ejercicioAsignado.Id);
    }

    // obtener serie por id no existente
    [Fact]
    public async Task ObtenerSeriePorId_NoExistente_DeberiaRetornarNull()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerSeriePorIdNoExistenteDb");
        using var context = new FitRankDbContext(options);
        var serieRepositorio = new SerieRepositorioImpl(context);

        // Act
        var serieObtenida = await serieRepositorio.ObtenerPorIdAsync(999); // ID que no existe

        // Assert
        serieObtenida.Should().BeNull();
    }

    // actualizar serie
    [Fact]
    public async Task ActualizarSerie_DeberiaActualizarCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("ActualizarSerieDb");
        using var context = new FitRankDbContext(options);
        var ejercicioAsignado = await SeedData(context);

        var serie = new Serie
        {
            NumeroDeSerie = 1,
            Repeticiones = 10,
            Peso = 50.0,
            EjercicioAsignadoId = ejercicioAsignado.Id
        };
        context.Series.Add(serie);
        await context.SaveChangesAsync();

        var serieRepositorio = new SerieRepositorioImpl(context);

        // Act
        serie.NumeroDeSerie = 2;
        serie.Repeticiones = 12;
        serie.Peso = 55.0;
        await serieRepositorio.ActualizarAsync(serie);

        // Assert
        var serieActualizada = await context.Series.FirstOrDefaultAsync(s => s.Id == serie.Id);
        serieActualizada.Should().NotBeNull();
        serieActualizada!.NumeroDeSerie.Should().Be(2);
        serieActualizada.Repeticiones.Should().Be(12);
        serieActualizada.Peso.Should().Be(55.0);
    }

    // actualizar serie no existente
    [Fact]
    public async Task ActualizarSerie_NoExistente_DeberiaDevolverNull()
    {
        // Arrange
        var options = CreateInMemoryOptions("ActualizarSerieNoExistenteDb");
        using var context = new FitRankDbContext(options);
        var serieRepositorio = new SerieRepositorioImpl(context);

        var serie = new Serie
        {
            Id = 999, // ID que no existe
            NumeroDeSerie = 1,
            Repeticiones = 10,
            Peso = 50.0,
            EjercicioAsignadoId = 1
        };

        // Act
        Func<Task> act = async () => await serieRepositorio.ActualizarAsync(serie);

        // Assert
        await act.Should().NotThrowAsync();
    }

    // eliminar serie
    [Fact]
    public async Task EliminarSerie_DeberiaEliminarCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("EliminarSerieDb");
        using var context = new FitRankDbContext(options);
        var ejercicioAsignado = await SeedData(context);

        var serie = new Serie
        {
            NumeroDeSerie = 1,
            Repeticiones = 10,
            Peso = 50.0,
            EjercicioAsignadoId = ejercicioAsignado.Id
        };
        context.Series.Add(serie);
        await context.SaveChangesAsync();

        var serieRepositorio = new SerieRepositorioImpl(context);

        // Act
        await serieRepositorio.EliminarAsync(serie.Id);

        // Assert
        var serieEliminada = await context.Series.FirstOrDefaultAsync(s => s.Id == serie.Id);
        serieEliminada.Should().BeNull();
    }

    // eliminar serie no existente
    [Fact]
    public async Task EliminarSerie_NoExistente_DeberiaNoHacerNada()
    {
        // Arrange
        var options = CreateInMemoryOptions("EliminarSerieNoExistenteDb");
        using var context = new FitRankDbContext(options);
        var serieRepositorio = new SerieRepositorioImpl(context);
        // Act
        Func<Task> act = async () => await serieRepositorio.EliminarAsync(999); // ID que no existe
        // Assert
        await act.Should().NotThrowAsync();
    }

    // obtener por ejercicio con series
    [Fact]
    public async Task ObtenerPorEjercicioAsync_ConSeries_DeberiaRetornarSoloLasSeriesDelEjercicio()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerPorEjercicioDb");
        using var context = new FitRankDbContext(options);
        var repo = new SerieRepositorioImpl(context);

        var ejercicio1 = new EjercicioAsignado { Id = 1, NumeroEjercicio = 1, EjercicioId = 1, SesionId = 1 };
        var ejercicio2 = new EjercicioAsignado { Id = 2, NumeroEjercicio = 2, EjercicioId = 2, SesionId = 1 };

        var serie1 = new Serie { Id = 1, EjercicioAsignadoId = 1, EjercicioAsignado = ejercicio1, Actividades = new List<Actividad>() };
        var serie2 = new Serie { Id = 2, EjercicioAsignadoId = 1, EjercicioAsignado = ejercicio1, Actividades = new List<Actividad>() };
        var serie3 = new Serie { Id = 3, EjercicioAsignadoId = 2, EjercicioAsignado = ejercicio2, Actividades = new List<Actividad>() };

        context.Series.AddRange(serie1, serie2, serie3);
        await context.SaveChangesAsync();

        // Act
        var resultado = (await repo.ObtenerPorEjercicioAsync(1)).ToList();

        // Assert
        resultado.Should().HaveCount(2);
        resultado.Should().ContainEquivalentOf(serie1, options => options.ExcludingMissingMembers());
        resultado.Should().ContainEquivalentOf(serie2, options => options.ExcludingMissingMembers());
        resultado.Should().NotContain(s => s.Id == 3);
    }

    // obtener por ejercicio sin series
    [Fact]
    public async Task ObtenerPorEjercicioAsync_SinSeries_DeberiaRetornarListaVacia()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerPorEjercicioVacioDb");
        using var context = new FitRankDbContext(options);
        var repo = new SerieRepositorioImpl(context);

        // Act
        var resultado = await repo.ObtenerPorEjercicioAsync(99);

        // Assert
        resultado.Should().BeEmpty();
    }
}