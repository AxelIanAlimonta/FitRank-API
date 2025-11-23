using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Persistence;
using FitRank_API.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FitRank_API.Tests.InfrastructureTests;

public class ConfiguracionMuscularTests
{
    private DbContextOptions<FitRankDbContext> CreateInMemoryOptions(string dbName)
    {
        return new DbContextOptionsBuilder<FitRankDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
    }

    [Fact]
    public async Task AgregarAsync_DeberiaAgregarConfiguracion()
    {
        // Arrange
        var options = CreateInMemoryOptions("AgregarConfiguracionDb");
        using var context = new FitRankDbContext(options);
        var repo = new ConfiguracionGrupoMuscularImpl(context);

        var config = new ConfiguracionGrupoMuscular
        {
            MultiplicadorPeso = 1.2,
            MultiplicadorRepeticiones = 1.1,
            GrupoMuscularId = 1,
            FactorProgresion = 0.5
        };

        // Act
        var resultado = await repo.AgregarAsync(config);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.MultiplicadorPeso.Should().Be(1.2);
        resultado.MultiplicadorRepeticiones.Should().Be(1.1);
        resultado.GrupoMuscularId.Should().Be(1);
        resultado.FactorProgresion.Should().Be(0.5);
    }

    [Fact]
    public async Task ObtenerTodosAsync_DeberiaRetornarTodasLasConfiguraciones()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerTodosConfiguracionesDb");
        using var context = new FitRankDbContext(options);
        var repo = new ConfiguracionGrupoMuscularImpl(context);

        context.ConfiguracionesGrupoMuscular.AddRange(
            new ConfiguracionGrupoMuscular { MultiplicadorPeso = 1, MultiplicadorRepeticiones = 1, GrupoMuscularId = 1, FactorProgresion = 0.1 },
            new ConfiguracionGrupoMuscular { MultiplicadorPeso = 2, MultiplicadorRepeticiones = 2, GrupoMuscularId = 2, FactorProgresion = 0.2 }
        );
        await context.SaveChangesAsync();

        // Act
        var resultado = await repo.ObtenerTodosAsync();

        // Assert
        resultado.Should().HaveCount(2);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_DeberiaRetornarConfiguracionSiExiste()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerPorIdConfiguracionDb");
        using var context = new FitRankDbContext(options);
        var repo = new ConfiguracionGrupoMuscularImpl(context);

        var config = new ConfiguracionGrupoMuscular
        {
            MultiplicadorPeso = 1.5,
            MultiplicadorRepeticiones = 1.3,
            GrupoMuscularId = 3,
            FactorProgresion = 0.7
        };
        context.ConfiguracionesGrupoMuscular.Add(config);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repo.ObtenerPorIdAsync(config.Id);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.MultiplicadorPeso.Should().Be(1.5);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_DeberiaRetornarNullSiNoExiste()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerPorIdConfiguracionInexistenteDb");
        using var context = new FitRankDbContext(options);
        var repo = new ConfiguracionGrupoMuscularImpl(context);

        // Act
        var resultado = await repo.ObtenerPorIdAsync(999);

        // Assert
        resultado.Should().BeNull();
    }

    [Fact]
    public async Task ActualizarAsync_DeberiaActualizarConfiguracionExistente()
    {
        // Arrange
        var options = CreateInMemoryOptions("ActualizarConfiguracionDb");
        using var context = new FitRankDbContext(options);
        var repo = new ConfiguracionGrupoMuscularImpl(context);

        var config = new ConfiguracionGrupoMuscular
        {
            MultiplicadorPeso = 1,
            MultiplicadorRepeticiones = 1,
            GrupoMuscularId = 1,
            FactorProgresion = 0.1
        };
        context.ConfiguracionesGrupoMuscular.Add(config);
        await context.SaveChangesAsync();

        // Act
        config.MultiplicadorPeso = 2;
        config.MultiplicadorRepeticiones = 2;
        config.GrupoMuscularId = 2;
        config.FactorProgresion = 0.2;
        var actualizado = await repo.ActualizarAsync(config);

        // Assert
        actualizado.Should().NotBeNull();
        actualizado!.MultiplicadorPeso.Should().Be(2);
        actualizado.MultiplicadorRepeticiones.Should().Be(2);
        actualizado.GrupoMuscularId.Should().Be(2);
        actualizado.FactorProgresion.Should().Be(0.2);
    }

    [Fact]
    public async Task ActualizarAsync_DeberiaRetornarNullSiNoExiste()
    {
        // Arrange
        var options = CreateInMemoryOptions("ActualizarConfiguracionInexistenteDb");
        using var context = new FitRankDbContext(options);
        var repo = new ConfiguracionGrupoMuscularImpl(context);

        var config = new ConfiguracionGrupoMuscular
        {
            Id = 999,
            MultiplicadorPeso = 2,
            MultiplicadorRepeticiones = 2,
            GrupoMuscularId = 2,
            FactorProgresion = 0.2
        };

        // Act
        var actualizado = await repo.ActualizarAsync(config);

        // Assert
        actualizado.Should().BeNull();
    }

    [Fact]
    public async Task EliminarAsync_DeberiaEliminarConfiguracionSiExiste()
    {
        // Arrange
        var options = CreateInMemoryOptions("EliminarConfiguracionDb");
        using var context = new FitRankDbContext(options);
        var repo = new ConfiguracionGrupoMuscularImpl(context);

        var config = new ConfiguracionGrupoMuscular
        {
            MultiplicadorPeso = 1,
            MultiplicadorRepeticiones = 1,
            GrupoMuscularId = 1,
            FactorProgresion = 0.1
        };
        context.ConfiguracionesGrupoMuscular.Add(config);
        await context.SaveChangesAsync();

        // Act
        await repo.EliminarAsync(config.Id);

        // Assert
        var eliminado = await context.ConfiguracionesGrupoMuscular.FindAsync(config.Id);
        eliminado.Should().BeNull();
    }

    [Fact]
    public async Task EliminarAsync_NoDeberiaFallarSiNoExiste()
    {
        // Arrange
        var options = CreateInMemoryOptions("EliminarConfiguracionInexistenteDb");
        using var context = new FitRankDbContext(options);
        var repo = new ConfiguracionGrupoMuscularImpl(context);

        // Act & Assert (no debe lanzar excepción)
        await repo.EliminarAsync(999);
    }
}
