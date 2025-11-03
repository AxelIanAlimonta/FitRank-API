using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Persistence;
using FitRank_API.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FitRank_API.tests.InfrastructureTests;

public class ActividadRepositorioTests
{

    private DbContextOptions<FitRankDbContext> CreateInMemoryOptions(string dbName)
    {
        return new DbContextOptionsBuilder<FitRankDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
    }

    private async Task<(Serie, EjercicioAsignado, Entrenamiento)> SeedData(FitRankDbContext context)
    {
        var ejercicioAsignado = new EjercicioAsignado
        {
            NumeroEjercicio = 1,
        };

        context.EjerciciosAsignados.Add(ejercicioAsignado);
        await context.SaveChangesAsync();

        var serie = new Serie
        {
            NumeroDeSerie = 1,
            Repeticiones = 10,
            Peso = 50,
            EjercicioAsignadoId = ejercicioAsignado.Id
        };

        context.Series.Add(serie);
        await context.SaveChangesAsync();

        var entrenamiento = new Entrenamiento
        {
            Fecha = DateTime.UtcNow,
            SocioId = 1,
        };

        context.Entrenamientos.Add(entrenamiento);
        await context.SaveChangesAsync();

        return (serie, ejercicioAsignado, entrenamiento);
    }

    //agregar actividad deberia guardar correctamente
    [Fact]
    public async Task AgregarActividad_DeberiaGuardarCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("AgregarActividadDb");
        using var context = new FitRankDbContext(options);
        var actividadRepositorioMock = new ActividadRepositorioImpl(context);
        var (serie, ejercicioAsignado, entrenamiento) = await SeedData(context);

        var actividad = new Actividad
        {
            Repeticiones = 10,
            Peso = 50.0,
            Punto = 8.5,

        };

        // Act
        await actividadRepositorioMock.AgregarAsync(actividad);
        var actividadGuardada = await context.Actividades.FirstOrDefaultAsync(a => a.Id == actividad.Id);

        // Assert con FluentAssertions
        actividadGuardada.Should().NotBeNull();
        actividadGuardada!.Duracion.Should().Be(actividad.Duracion);
        actividadGuardada.Repeticiones.Should().Be(actividad.Repeticiones);
        actividadGuardada.Peso.Should().Be(actividad.Peso);
        actividadGuardada.Punto.Should().Be(actividad.Punto);
    }

    //obtener lista de actividades
    [Fact]
    public async Task ObtenerActividades_DeberiaRetornarListaCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerActividadesDb");
        using var context = new FitRankDbContext(options);
        var (serie, ejercicioAsignado, entrenamiento) = await SeedData(context);

        context.Actividades.AddRange(
            new Actividad { Repeticiones = 10, Peso = 50.0, Punto = 8.5, SerieId = serie.Id, EjercicioAsignadoId = ejercicioAsignado.Id, EntrenamientoId = entrenamiento.Id },
            new Actividad { Repeticiones = 12, Peso = 55.0, Punto = 9.0, SerieId = serie.Id, EjercicioAsignadoId = ejercicioAsignado.Id, EntrenamientoId = entrenamiento.Id }
        );
        await context.SaveChangesAsync();

        var actividadRepositorio = new ActividadRepositorioImpl(context);

        // Act
        var actividades = await actividadRepositorio.ObtenerTodasAsync();

        // Assert
        actividades.Should().HaveCount(2);
    }

    //obtener lista de actividades vacia
    [Fact]
    public async Task ObtenerActividades_Vacia_DeberiaRetornarListaVacia()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerActividadesVaciaDb");
        using var context = new FitRankDbContext(options);

        var actividadRepositorio = new ActividadRepositorioImpl(context);

        // Act
        var actividades = await actividadRepositorio.ObtenerTodasAsync();

        // Assert
        actividades.Should().BeEmpty();
    }

    //obtener actividad por id
    [Fact]
    public async Task ObtenerActividadPorId_DeberiaRetornarActividadCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerActividadPorIdDb");
        using var context = new FitRankDbContext(options);
        var (serie, ejercicioAsignado, entrenamiento) = await SeedData(context);

        var actividad = new Actividad
        {
            Repeticiones = 10,
            Peso = 50.0,
            Punto = 8.5,
            EntrenamientoId = entrenamiento.Id,
            SerieId = serie.Id,
            EjercicioAsignadoId = ejercicioAsignado.Id
        };
        context.Actividades.Add(actividad);
        await context.SaveChangesAsync();

        var actividadRepositorio = new ActividadRepositorioImpl(context);

        // Act
        var actividadObtenida = await actividadRepositorio.ObtenerPorIdAsync(actividad.Id);

        // Assert
        actividadObtenida.Should().NotBeNull();
        actividadObtenida!.Repeticiones.Should().Be(10);
        actividadObtenida.Peso.Should().Be(50.0);
        actividadObtenida.Punto.Should().Be(8.5);
    }

    //obtener actividad por id no existente
    [Fact]
    public async Task ObtenerActividadPorId_NoExistente_DeberiaRetornarNull()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerActividadPorIdNoExistenteDb");
        using var context = new FitRankDbContext(options);

        var actividadRepositorio = new ActividadRepositorioImpl(context);

        // Act
        var actividadObtenida = await actividadRepositorio.ObtenerPorIdAsync(999);

        // Assert
        actividadObtenida.Should().BeNull();
    }

    //actyualizar actividad
    [Fact]
    public async Task ActualizarActividad_DeberiaActualizarCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("ActualizarActividadDb");
        using var context = new FitRankDbContext(options);
        var (serie, ejercicioAsignado, entrenamiento) = await SeedData(context);

        var actividad = new Actividad
        {
            Repeticiones = 10,
            Peso = 50.0,
            Punto = 8.5
        };
        context.Actividades.Add(actividad);
        await context.SaveChangesAsync();

        var actividadRepositorio = new ActividadRepositorioImpl(context);

        // Act
        actividad.Repeticiones = 12;
        actividad.Peso = 55.0;
        actividad.Punto = 9.0;
        await actividadRepositorio.ActualizarAsync(actividad);

        // Assert
        var actividadActualizada = await context.Actividades.FindAsync(actividad.Id);
        actividadActualizada.Should().NotBeNull();
        actividadActualizada!.Repeticiones.Should().Be(12);
        actividadActualizada.Peso.Should().Be(55.0);
        actividadActualizada.Punto.Should().Be(9.0);
    }

    //actualizar actividad no existente
    [Fact]
    public async Task ActualizarActividad_NoExistente_DeberiaRetornarNull()
    {
        // Arrange
        var options = CreateInMemoryOptions("ActualizarActividad_NoExistente_Db");
        using var context = new FitRankDbContext(options);

        var actividadRepositorio = new ActividadRepositorioImpl(context);

        var actividadInexistente = new Actividad
        {
            Id = 999,
            Repeticiones = 10,
            Peso = 50.0,
            Punto = 8.5
        };

        // Act
        var resultado = await actividadRepositorio.ActualizarAsync(actividadInexistente);

        // Assert
        resultado.Should().BeNull();
    }

    //eliminar actividad exitosa
    [Fact]
    public async Task EliminarActividad_Exitosa_DeberiaEliminarCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("EliminarActividad_Exitosa_Db");
        using var context = new FitRankDbContext(options);
        var (serie, ejercicioAsignado, entrenamiento) = await SeedData(context);

        var actividad = new Actividad
        {
            Repeticiones = 10,
            Peso = 50.0,
            Punto = 8.5
        };
        context.Actividades.Add(actividad);
        await context.SaveChangesAsync();

        var actividadRepositorio = new ActividadRepositorioImpl(context);

        // Act
        var resultado = await actividadRepositorio.EliminarAsync(actividad.Id);

        // Assert
        resultado.Should().BeTrue();
        var actividadEliminada = await context.Actividades.FindAsync(actividad.Id);
        actividadEliminada.Should().BeNull();
    }

    //eliminar actividad no existente
    [Fact]
    public async Task EliminarActividad_NoExistente_DeberiaRetornarFalse()
    {
        // Arrange
        var options = CreateInMemoryOptions("EliminarActividad_NoExistente_Db");
        using var context = new FitRankDbContext(options);

        var actividadRepositorio = new ActividadRepositorioImpl(context);

        // Act
        var resultado = await actividadRepositorio.EliminarAsync(999);

        // Assert
        resultado.Should().BeFalse();
    }
}