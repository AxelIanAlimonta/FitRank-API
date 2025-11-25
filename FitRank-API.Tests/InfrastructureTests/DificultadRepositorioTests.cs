using System.Threading.Tasks;
using System.Collections.Generic;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Persistence;
using FitRank_API.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FitRank_API.Tests.InfrastructureTests;

public class DificultadRepositorioTests
{
    private DbContextOptions<FitRankDbContext> CreateInMemoryOptions(string dbName)
    {
        return new DbContextOptionsBuilder<FitRankDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
    }

    [Fact]
    public async Task AgregarAsync_DeberiaAgregarDificultad()
    {
        // Arrange
        var options = CreateInMemoryOptions("AgregarDificultadDb");
        using var context = new FitRankDbContext(options);
        var repo = new DificultadRepositorioImpl(context);

        var dificultad = new Dificultad
        {
            Descripcion = "Alta"
        };

        // Act
        var resultado = await repo.AgregarAsync(dificultad);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Descripcion.Should().Be("Alta");
    }

    [Fact]
    public async Task ObtenerTodosAsync_DeberiaRetornarTodasLasDificultades()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerTodasDificultadesDb");
        using var context = new FitRankDbContext(options);
        var repo = new DificultadRepositorioImpl(context);

        context.Dificultades.AddRange(
            new Dificultad { Descripcion = "Baja" },
            new Dificultad { Descripcion = "Media" }
        );
        await context.SaveChangesAsync();

        // Act
        var resultado = await repo.ObtenerTodosAsync();

        // Assert
        resultado.Should().HaveCount(2);
        resultado.Should().Contain(d => d.Descripcion == "Baja");
        resultado.Should().Contain(d => d.Descripcion == "Media");
    }

    [Fact]
    public async Task ObtenerPorIdAsync_DeberiaRetornarDificultadSiExiste()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerPorIdDificultadDb");
        using var context = new FitRankDbContext(options);
        var repo = new DificultadRepositorioImpl(context);

        var dificultad = new Dificultad { Descripcion = "Media" };
        context.Dificultades.Add(dificultad);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repo.ObtenerPorIdAsync(dificultad.Id);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Descripcion.Should().Be("Media");
    }

    [Fact]
    public async Task ObtenerPorIdAsync_DeberiaRetornarNullSiNoExiste()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerPorIdDificultadInexistenteDb");
        using var context = new FitRankDbContext(options);
        var repo = new DificultadRepositorioImpl(context);

        // Act
        var resultado = await repo.ObtenerPorIdAsync(999);

        // Assert
        resultado.Should().BeNull();
    }

    [Fact]
    public async Task ActualizarAsync_DeberiaActualizarDificultadExistente()
    {
        // Arrange
        var options = CreateInMemoryOptions("ActualizarDificultadDb");
        using var context = new FitRankDbContext(options);
        var repo = new DificultadRepositorioImpl(context);

        var dificultad = new Dificultad { Descripcion = "Baja" };
        context.Dificultades.Add(dificultad);
        await context.SaveChangesAsync();

        // Act
        dificultad.Descripcion = "Muy Baja";
        var actualizado = await repo.ActualizarAsync(dificultad);

        // Assert
        actualizado.Should().NotBeNull();
        actualizado!.Descripcion.Should().Be("Muy Baja");
    }

    [Fact]
    public async Task ActualizarAsync_DeberiaRetornarNullSiNoExiste()
    {
        // Arrange
        var options = CreateInMemoryOptions("ActualizarDificultadInexistenteDb");
        using var context = new FitRankDbContext(options);
        var repo = new DificultadRepositorioImpl(context);

        var dificultad = new Dificultad { Id = 999, Descripcion = "No existe" };

        // Act
        var actualizado = await repo.ActualizarAsync(dificultad);

        // Assert
        actualizado.Should().BeNull();
    }

    [Fact]
    public async Task EliminarAsync_DeberiaEliminarDificultadSiExiste()
    {
        // Arrange
        var options = CreateInMemoryOptions("EliminarDificultadDb");
        using var context = new FitRankDbContext(options);
        var repo = new DificultadRepositorioImpl(context);

        var dificultad = new Dificultad { Descripcion = "Eliminar" };
        context.Dificultades.Add(dificultad);
        await context.SaveChangesAsync();

        // Act
        await repo.EliminarAsync(dificultad.Id);

        // Assert
        var enDb = await context.Dificultades.FindAsync(dificultad.Id);
        enDb.Should().BeNull();
    }

    [Fact]
    public async Task EliminarAsync_NoDeberiaFallarSiNoExiste()
    {
        // Arrange
        var options = CreateInMemoryOptions("EliminarDificultadInexistenteDb");
        using var context = new FitRankDbContext(options);
        var repo = new DificultadRepositorioImpl(context);

        // Act & Assert (no debe lanzar excepción)
        await repo.EliminarAsync(999);
    }
}