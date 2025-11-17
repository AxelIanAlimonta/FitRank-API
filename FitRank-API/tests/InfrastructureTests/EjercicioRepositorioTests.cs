using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Persistence;
using FitRank_API.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FitRank_API.tests.InfrastructureTests;

public class EjercicioRepositorioTests
{
    private DbContextOptions<FitRankDbContext> CreateInMemoryOptions(string dbName)
    {
        return new DbContextOptionsBuilder<FitRankDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
    }

    private async Task<GrupoMuscular> SeedData(FitRankDbContext context)
    {
        var grupoMuscular = new GrupoMuscular
        {
            Nombre = "Pecho",
            Imagen = "pecho.png"
        };

        context.GruposMusculares.Add(grupoMuscular);
        await context.SaveChangesAsync();
        return grupoMuscular;
    }

    [Fact]
    public async Task AgregarEjercicio_DeberiaGuardarCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("AgregarEjercicioDb");
        using var context = new FitRankDbContext(options);
        var grupoMuscular = await SeedData(context);
        var ejercicioRepositorio = new EjercicioRepositorioImpl(context);

        var ejercicio = new Ejercicio
        {
            Nombre = "Press de banca",
            Descripcion = "Ejercicio para fortalecer el pecho",
            GrupoMuscularId = grupoMuscular.Id
        };

        // Act
        await ejercicioRepositorio.AgregarEjercicioAsync(ejercicio);

        // Assert
        var ejercicioGuardado = await context.Ejercicios.FirstOrDefaultAsync(e => e.Id == ejercicio.Id);
        ejercicioGuardado.Should().NotBeNull();
        ejercicioGuardado!.Nombre.Should().Be("Press de banca");
        ejercicioGuardado.GrupoMuscularId.Should().Be(grupoMuscular.Id);
    }

    //obtener lista de ejercicios
    [Fact]
    public async Task ObtenerEjercicios_DeberiaRetornarListaCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerEjerciciosDb");
        using var context = new FitRankDbContext(options);
        var grupoMuscular = await SeedData(context);

        context.Ejercicios.AddRange(
            new Ejercicio { Nombre = "Press de banca", GrupoMuscularId = grupoMuscular.Id },
            new Ejercicio { Nombre = "Flexiones", GrupoMuscularId = grupoMuscular.Id }
        );
        await context.SaveChangesAsync();

        var ejercicioRepositorio = new EjercicioRepositorioImpl(context);

        // Act
        var ejercicios = await ejercicioRepositorio.ObtenerEjerciciosAsync();

        // Assert
        ejercicios.Should().HaveCount(2);
        ejercicios.Should().Contain(e => e.Nombre == "Press de banca");
        ejercicios.Should().Contain(e => e.Nombre == "Flexiones");
    }

    //lista de ejercicios vacia
    [Fact]
    public async Task ObtenerEjercicios_SinEjercicios_DeberiaRetornarListaVacia()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerEjerciciosVaciaDb");
        using var context = new FitRankDbContext(options);
        var ejercicioRepositorio = new EjercicioRepositorioImpl(context);

        // Act
        var ejercicios = await ejercicioRepositorio.ObtenerEjerciciosAsync();

        // Assert
        ejercicios.Should().BeEmpty();
    }

    //obtener ejercicio por id
    [Fact]
    public async Task ObtenerEjercicioPorId_DeberiaRetornarEjercicioCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerEjercicioPorIdDb");
        using var context = new FitRankDbContext(options);
        var grupoMuscular = await SeedData(context);

        var ejercicio = new Ejercicio
        {
            Nombre = "Press de banca",
            GrupoMuscularId = grupoMuscular.Id
        };
        context.Ejercicios.Add(ejercicio);
        await context.SaveChangesAsync();

        var ejercicioRepositorio = new EjercicioRepositorioImpl(context);

        // Act
        var ejercicioObtenido = await ejercicioRepositorio.ObtenerEjercicioPorIdAsync(ejercicio.Id);

        // Assert
        ejercicioObtenido.Should().NotBeNull();
        ejercicioObtenido!.Nombre.Should().Be("Press de banca");
    }

    //obtener ejercicio por id no existente
    [Fact]
    public async Task ObtenerEjercicioPorId_NoExistente_DeberiaRetornarNull()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerEjercicioPorIdNoExistenteDb");
        using var context = new FitRankDbContext(options);
        var ejercicioRepositorio = new EjercicioRepositorioImpl(context);

        // Act
        var ejercicioObtenido = await ejercicioRepositorio.ObtenerEjercicioPorIdAsync(999);

        // Assert
        ejercicioObtenido.Should().BeNull();
    }

    //actualizar ejercicio
    [Fact]
    public async Task ActualizarEjercicio_DeberiaActualizarCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("ActualizarEjercicioDb");
        using var context = new FitRankDbContext(options);
        var grupoMuscular = await SeedData(context);

        var ejercicio = new Ejercicio
        {
            Nombre = "Press de banca",
            GrupoMuscularId = grupoMuscular.Id
        };
        context.Ejercicios.Add(ejercicio);
        await context.SaveChangesAsync();

        var ejercicioRepositorio = new EjercicioRepositorioImpl(context);

        // Act
        ejercicio.Nombre = "Press de banca inclinado";
        var ejercicioActualizado = await ejercicioRepositorio.ActualizarEjercicioAsync(ejercicio);

        // Assert
        ejercicioActualizado.Should().NotBeNull();
        ejercicioActualizado!.Nombre.Should().Be("Press de banca inclinado");
    }

    //actualizar ejercicio no existente
    [Fact]
    public async Task ActualizarEjercicio_NoExistente_DeberiaRetornarNull()
    {
        // Arrange
        var options = CreateInMemoryOptions("ActualizarEjercicioNoExistenteDb");
        using var context = new FitRankDbContext(options);
        var ejercicioRepositorio = new EjercicioRepositorioImpl(context);

        var ejercicio = new Ejercicio
        {
            Id = 999,
            Nombre = "Ejercicio inexistente",
            GrupoMuscularId = 1
        };

        // Act
        var ejercicioActualizado = await ejercicioRepositorio.ActualizarEjercicioAsync(ejercicio);

        // Assert
        ejercicioActualizado.Should().BeNull();
    }

    //eliminar ejercicio
    [Fact]
    public async Task EliminarEjercicio_DeberiaEliminarCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("EliminarEjercicioDb");
        using var context = new FitRankDbContext(options);
        var grupoMuscular = await SeedData(context);

        var ejercicio = new Ejercicio
        {
            Nombre = "Press de banca",
            GrupoMuscularId = grupoMuscular.Id
        };
        context.Ejercicios.Add(ejercicio);
        await context.SaveChangesAsync();

        var ejercicioRepositorio = new EjercicioRepositorioImpl(context);

        // Act
        var resultado = await ejercicioRepositorio.EliminarEjercicioAsync(ejercicio.Id);

        // Assert
        resultado.Should().BeTrue();
        var ejercicioEliminado = await context.Ejercicios.FindAsync(ejercicio.Id);
        ejercicioEliminado.Should().BeNull();
    }
}