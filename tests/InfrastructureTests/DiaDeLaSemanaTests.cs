using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Persistence;
using FitRank_API.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FitRank_API.tests.InfrastructureTests;

public class DiaDeLaSemanaTests
{
    private DbContextOptions<FitRankDbContext> CreateInMemoryOptions(string dbName)
    {
        return new DbContextOptionsBuilder<FitRankDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
    }

    // agregar dia de la semana
    [Fact]
    public async Task AgregarDiaDeLaSemana_DeberiaGuardarCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("AgregarDiaDeLaSemanaDb");
        using var context = new FitRankDbContext(options);
        var diaDeLaSemanaRepositorioMock = new DiaDeLaSemanaRepositorioImpl(context);

        var diaDeLaSemana = new DiaDeLaSemana
        {
            Nombre = "Lunes"
        };

        // Act
        await diaDeLaSemanaRepositorioMock.AgregarDiaDeLaSemanaAsync(diaDeLaSemana);

        // FluentAssert
        var diaEnDb = await context.DiasDeLaSemana.FirstOrDefaultAsync(d => d.Id == diaDeLaSemana.Id);
        diaEnDb.Should().NotBeNull();
        diaEnDb!.Nombre.Should().Be("Lunes");
    }

    //obtener lista de dias de la semana
    [Fact]
    public async Task ObtenerTodosLosDiasDeLaSemana_DeberiaRetornarListaCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerTodosLosDiasDeLaSemanaDb");
        using var context = new FitRankDbContext(options);
        var diaDeLaSemanaRepositorioMock = new DiaDeLaSemanaRepositorioImpl(context);

        var dias = new List<DiaDeLaSemana>
        {
            new DiaDeLaSemana { Nombre = "Lunes" },
            new DiaDeLaSemana { Nombre = "Martes" },
            new DiaDeLaSemana { Nombre = "Miércoles" }
        };

        context.DiasDeLaSemana.AddRange(dias);
        await context.SaveChangesAsync();

        // Act
        var resultado = await diaDeLaSemanaRepositorioMock.ObtenerTodosLosDiasDeLaSemanaAsync();

        // Assert
        resultado.Should().HaveCount(3);
        resultado.Select(d => d.Nombre).Should().Contain(new[] { "Lunes", "Martes", "Miércoles" });
    }

    // obtener lista vacia
    [Fact]
    public async Task ObtenerTodosLosDiasDeLaSemana_SinDatos_DeberiaRetornarListaVacia()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerTodosLosDiasDeLaSemanaVaciaDb");
        using var context = new FitRankDbContext(options);
        var diaDeLaSemanaRepositorioMock = new DiaDeLaSemanaRepositorioImpl(context);

        // Act
        var resultado = await diaDeLaSemanaRepositorioMock.ObtenerTodosLosDiasDeLaSemanaAsync();

        // Assert
        resultado.Should().BeEmpty();
    }

    // obtener dia de la semana por id
    [Fact]
    public async Task ObtenerDiaDeLaSemanaPorId_DeberiaRetornarDiaCorrecto()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerDiaDeLaSemanaPorIdDb");
        using var context = new FitRankDbContext(options);
        var diaDeLaSemanaRepositorioMock = new DiaDeLaSemanaRepositorioImpl(context);

        var diaDeLaSemana = new DiaDeLaSemana
        {
            Nombre = "Miércoles"
        };

        context.DiasDeLaSemana.Add(diaDeLaSemana);
        await context.SaveChangesAsync();

        // Act
        var diaObtenido = await diaDeLaSemanaRepositorioMock.ObtenerDiaDeLaSemanaPorIdAsync(diaDeLaSemana.Id);

        // Assert
        diaObtenido.Should().NotBeNull();
        diaObtenido!.Id.Should().Be(diaDeLaSemana.Id);
        diaObtenido.Nombre.Should().Be("Miércoles");
    }

    //obtener dia de la semana por id inexistente
    [Fact]
    public async Task ObtenerDiaDeLaSemanaPorId_Inexistente_DeberiaRetornarNull()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerDiaDeLaSemanaPorIdInexistenteDb");
        using var context = new FitRankDbContext(options);
        var diaDeLaSemanaRepositorioMock = new DiaDeLaSemanaRepositorioImpl(context);

        // Act
        var diaObtenido = await diaDeLaSemanaRepositorioMock.ObtenerDiaDeLaSemanaPorIdAsync(999);

        // Assert
        diaObtenido.Should().BeNull();
    }

    //actualizar dia de la semana
    [Fact]
    public async Task ActualizarDiaDeLaSemana_DeberiaActualizarCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("ActualizarDiaDeLaSemanaDb");
        using var context = new FitRankDbContext(options);
        var diaDeLaSemanaRepositorioMock = new DiaDeLaSemanaRepositorioImpl(context);

        var diaDeLaSemana = new DiaDeLaSemana
        {
            Nombre = "Jueves"
        };

        context.DiasDeLaSemana.Add(diaDeLaSemana);
        await context.SaveChangesAsync();

        // Act
        diaDeLaSemana.Nombre = "Viernes";
        var diaActualizado = await diaDeLaSemanaRepositorioMock.ActualizarDiaDeLaSemanaAsync(diaDeLaSemana);

        // Assert
        diaActualizado.Should().NotBeNull();
        diaActualizado!.Nombre.Should().Be("Viernes");

        var diaEnDb = await context.DiasDeLaSemana.FirstOrDefaultAsync(d => d.Id == diaDeLaSemana.Id);
        diaEnDb.Should().NotBeNull();
        diaEnDb!.Nombre.Should().Be("Viernes");
    }

    //actualizar dia de la semana inexistente
    [Fact]
    public async Task ActualizarDiaDeLaSemana_Inexistente_DeberiaRetornarNull()
    {
        // Arrange
        var options = CreateInMemoryOptions("ActualizarDiaDeLaSemanaInexistenteDb");
        using var context = new FitRankDbContext(options);
        var diaDeLaSemanaRepositorioMock = new DiaDeLaSemanaRepositorioImpl(context);

        var diaDeLaSemana = new DiaDeLaSemana
        {
            Id = 999,
            Nombre = "Sábado"
        };

        // Act
        var diaActualizado = await diaDeLaSemanaRepositorioMock.ActualizarDiaDeLaSemanaAsync(diaDeLaSemana);

        // Assert
        diaActualizado.Should().BeNull();
    }

    //eliminar dia de la semana
    [Fact]
    public async Task EliminarDiaDeLaSemana_DeberiaEliminarCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("EliminarDiaDeLaSemanaDb");
        using var context = new FitRankDbContext(options);
        var diaDeLaSemanaRepositorioMock = new DiaDeLaSemanaRepositorioImpl(context);

        var diaDeLaSemana = new DiaDeLaSemana
        {
            Nombre = "Domingo"
        };

        context.DiasDeLaSemana.Add(diaDeLaSemana);
        await context.SaveChangesAsync();

        // Act
        var resultado = await diaDeLaSemanaRepositorioMock.EliminarDiaDeLaSemanaAsync(diaDeLaSemana.Id);

        // Assert
        resultado.Should().BeTrue();

        var diaEnDb = await context.DiasDeLaSemana.FirstOrDefaultAsync(d => d.Id == diaDeLaSemana.Id);
        diaEnDb.Should().BeNull();
    }

    //eliminar dia de la semana inexistente
    [Fact]
    public async Task EliminarDiaDeLaSemana_Inexistente_DeberiaRetornarFalse()
    {
        // Arrange
        var options = CreateInMemoryOptions("EliminarDiaDeLaSemanaInexistenteDb");
        using var context = new FitRankDbContext(options);
        var diaDeLaSemanaRepositorioMock = new DiaDeLaSemanaRepositorioImpl(context);

        // Act
        var resultado = await diaDeLaSemanaRepositorioMock.EliminarDiaDeLaSemanaAsync(999);

        // Assert
        resultado.Should().BeFalse();
    }
}