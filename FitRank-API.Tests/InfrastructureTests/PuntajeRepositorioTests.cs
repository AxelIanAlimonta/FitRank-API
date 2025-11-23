using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Persistence;
using FitRank_API.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FitRank_API.Tests.InfrastructureTests;

public class PuntajeRepositorioTests
{
    private DbContextOptions<FitRankDbContext> CreateInMemoryOptions(string dbName)
    {
        return new DbContextOptionsBuilder<FitRankDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
    }

    [Fact]
    public async Task AgregarAsync_DeberiaAgregarPuntaje()
    {
        // Arrange
        var options = CreateInMemoryOptions("AgregarPuntajeDb");
        using var context = new FitRankDbContext(options);
        var repo = new PuntajeRepositorioImpl(context);

        var puntaje = new Puntaje
        {
            SocioId = 1,
            Motivo = "Victoria",
            Fecha = DateTime.UtcNow,
            Valor = 100
        };

        // Act
        var resultado = await repo.AgregarAsync(puntaje);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Motivo.Should().Be("Victoria");
        resultado.Valor.Should().Be(100);
    }

    [Fact]
    public async Task ObtenerTodasAsync_DeberiaRetornarTodosLosPuntajes()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerTodosPuntajesDb");
        using var context = new FitRankDbContext(options);
        var repo = new PuntajeRepositorioImpl(context);

        context.Puntajes.AddRange(
            new Puntaje { SocioId = 1, Motivo = "A", Fecha = DateTime.UtcNow, Valor = 10 },
            new Puntaje { SocioId = 2, Motivo = "B", Fecha = DateTime.UtcNow, Valor = 20 }
        );
        await context.SaveChangesAsync();

        // Act
        var resultado = await repo.ObtenerTodasAsync();

        // Assert
        resultado.Should().HaveCount(2);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_DeberiaRetornarPuntajeSiExiste()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerPorIdPuntajeDb");
        using var context = new FitRankDbContext(options);
        var repo = new PuntajeRepositorioImpl(context);

        var puntaje = new Puntaje
        {
            SocioId = 1,
            Motivo = "Motivo",
            Fecha = DateTime.UtcNow,
            Valor = 50
        };
        context.Puntajes.Add(puntaje);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repo.ObtenerPorIdAsync(puntaje.Id);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Motivo.Should().Be("Motivo");
    }

    [Fact]
    public async Task ObtenerPorIdAsync_DeberiaRetornarNullSiNoExiste()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerPorIdPuntajeInexistenteDb");
        using var context = new FitRankDbContext(options);
        var repo = new PuntajeRepositorioImpl(context);

        // Act
        var resultado = await repo.ObtenerPorIdAsync(999);

        // Assert
        resultado.Should().BeNull();
    }

    [Fact]
    public async Task ActualizarAsync_DeberiaActualizarPuntajeExistente()
    {
        // Arrange
        var options = CreateInMemoryOptions("ActualizarPuntajeDb");
        using var context = new FitRankDbContext(options);
        var repo = new PuntajeRepositorioImpl(context);

        var puntaje = new Puntaje
        {
            SocioId = 1,
            Motivo = "Inicial",
            Fecha = DateTime.UtcNow,
            Valor = 10
        };
        context.Puntajes.Add(puntaje);
        await context.SaveChangesAsync();

        // Act
        puntaje.Motivo = "Actualizado";
        puntaje.Valor = 99;
        var actualizado = await repo.ActualizarAsync(puntaje);

        // Assert
        actualizado.Should().NotBeNull();
        actualizado!.Motivo.Should().Be("Actualizado");
        actualizado.Valor.Should().Be(99);
    }

    [Fact]
    public async Task ActualizarAsync_DeberiaRetornarNullSiNoExiste()
    {
        // Arrange
        var options = CreateInMemoryOptions("ActualizarPuntajeInexistenteDb");
        using var context = new FitRankDbContext(options);
        var repo = new PuntajeRepositorioImpl(context);

        var puntaje = new Puntaje
        {
            Id = 999,
            SocioId = 1,
            Motivo = "No existe",
            Fecha = DateTime.UtcNow,
            Valor = 1
        };

        // Act
        var actualizado = await repo.ActualizarAsync(puntaje);

        // Assert
        actualizado.Should().BeNull();
    }

    [Fact]
    public async Task EliminarAsync_DeberiaEliminarPuntajeSiExiste()
    {
        // Arrange
        var options = CreateInMemoryOptions("EliminarPuntajeDb");
        using var context = new FitRankDbContext(options);
        var repo = new PuntajeRepositorioImpl(context);

        var puntaje = new Puntaje
        {
            SocioId = 1,
            Motivo = "Eliminar",
            Fecha = DateTime.UtcNow,
            Valor = 5
        };
        context.Puntajes.Add(puntaje);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repo.EliminarAsync(puntaje.Id);

        // Assert
        resultado.Should().BeTrue();
        var enDb = await context.Puntajes.FindAsync(puntaje.Id);
        enDb.Should().BeNull();
    }

    [Fact]
    public async Task EliminarAsync_DeberiaRetornarFalseSiNoExiste()
    {
        // Arrange
        var options = CreateInMemoryOptions("EliminarPuntajeInexistenteDb");
        using var context = new FitRankDbContext(options);
        var repo = new PuntajeRepositorioImpl(context);

        // Act
        var resultado = await repo.EliminarAsync(999);

        // Assert
        resultado.Should().BeFalse();
    }
}
