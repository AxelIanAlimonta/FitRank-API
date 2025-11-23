using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
using System.Collections.Generic;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Persistence;
using FitRank_API.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FitRank_API.Tests.InfrastructureTests;

public class GrupoMuscularRepositorioTests
{
    private DbContextOptions<FitRankDbContext> CreateInMemoryOptions(string dbName)
    {
        return new DbContextOptionsBuilder<FitRankDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
    }

    [Fact]
    public async Task AgregarAsync_DeberiaAgregarGrupoMuscular()
    {
        // Arrange
        var options = CreateInMemoryOptions("AgregarGrupoMuscularDb");
        using var context = new FitRankDbContext(options);
        var repo = new GrupoMuscularRepositorioImpl(context);

        var grupo = new GrupoMuscular
        {
            Nombre = "Pecho",
            Imagen = "pecho.png"
        };

        // Act
        var resultado = await repo.AgregarAsync(grupo);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Nombre.Should().Be("Pecho");
        resultado.Imagen.Should().Be("pecho.png");
    }

    [Fact]
    public async Task ObtenerTodosAsync_DeberiaRetornarTodosLosGrupos()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerTodosGruposDb");
        using var context = new FitRankDbContext(options);
        var repo = new GrupoMuscularRepositorioImpl(context);

        context.GruposMusculares.AddRange(
            new GrupoMuscular { Nombre = "Espalda", Imagen = "espalda.png" },
            new GrupoMuscular { Nombre = "Pierna", Imagen = "pierna.png" }
        );
        await context.SaveChangesAsync();

        // Act
        var resultado = await repo.ObtenerTodosAsync();

        // Assert
        resultado.Should().HaveCount(2);
        resultado.Should().Contain(g => g.Nombre == "Espalda");
        resultado.Should().Contain(g => g.Nombre == "Pierna");
    }

    [Fact]
    public async Task ObtenerPorIdAsync_DeberiaRetornarGrupoSiExiste()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerPorIdGrupoDb");
        using var context = new FitRankDbContext(options);
        var repo = new GrupoMuscularRepositorioImpl(context);

        var grupo = new GrupoMuscular { Nombre = "Brazo", Imagen = "brazo.png" };
        context.GruposMusculares.Add(grupo);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repo.ObtenerPorIdAsync(grupo.Id);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Nombre.Should().Be("Brazo");
    }

    [Fact]
    public async Task ObtenerPorIdAsync_DeberiaRetornarNullSiNoExiste()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerPorIdGrupoInexistenteDb");
        using var context = new FitRankDbContext(options);
        var repo = new GrupoMuscularRepositorioImpl(context);

        // Act
        var resultado = await repo.ObtenerPorIdAsync(999);

        // Assert
        resultado.Should().BeNull();
    }

    [Fact]
    public async Task ActualizarAsync_DeberiaActualizarGrupoExistente()
    {
        // Arrange
        var options = CreateInMemoryOptions("ActualizarGrupoDb");
        using var context = new FitRankDbContext(options);
        var repo = new GrupoMuscularRepositorioImpl(context);

        var grupo = new GrupoMuscular { Nombre = "Original", Imagen = "original.png" };
        context.GruposMusculares.Add(grupo);
        await context.SaveChangesAsync();

        // Act
        grupo.Nombre = "Actualizado";
        grupo.Imagen = "actualizado.png";
        var actualizado = await repo.ActualizarAsync(grupo);

        // Assert
        actualizado.Should().NotBeNull();
        actualizado!.Nombre.Should().Be("Actualizado");
        actualizado.Imagen.Should().Be("actualizado.png");
    }

    [Fact]
    public async Task ActualizarAsync_DeberiaRetornarNullSiNoExiste()
    {
        // Arrange
        var options = CreateInMemoryOptions("ActualizarGrupoInexistenteDb");
        using var context = new FitRankDbContext(options);
        var repo = new GrupoMuscularRepositorioImpl(context);

        var grupo = new GrupoMuscular { Id = 999, Nombre = "NoExiste", Imagen = "noexiste.png" };

        // Act
        var actualizado = await repo.ActualizarAsync(grupo);

        // Assert
        actualizado.Should().BeNull();
    }

    [Fact]
    public async Task EliminarAsync_DeberiaEliminarGrupoSiExiste()
    {
        // Arrange
        var options = CreateInMemoryOptions("EliminarGrupoDb");
        using var context = new FitRankDbContext(options);
        var repo = new GrupoMuscularRepositorioImpl(context);

        var grupo = new GrupoMuscular { Nombre = "Eliminar", Imagen = "eliminar.png" };
        context.GruposMusculares.Add(grupo);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repo.EliminarAsync(grupo.Id);

        // Assert
        resultado.Should().BeTrue();
        var enDb = await context.GruposMusculares.FindAsync(grupo.Id);
        enDb.Should().BeNull();
    }

    [Fact]
    public async Task EliminarAsync_DeberiaRetornarFalseSiNoExiste()
    {
        // Arrange
        var options = CreateInMemoryOptions("EliminarGrupoInexistenteDb");
        using var context = new FitRankDbContext(options);
        var repo = new GrupoMuscularRepositorioImpl(context);

        // Act
        var resultado = await repo.EliminarAsync(999);

        // Assert
        resultado.Should().BeFalse();
    }
}
