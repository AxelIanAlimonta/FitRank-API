using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Persistence;
using FitRank_API.Infrastructure.Repositories;
using FitRank_API.Application.DTOs.RankingDTOs;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FitRank_API.Tests.InfrastructureTests;

public class RankingRepositorioTests
{
    private DbContextOptions<FitRankDbContext> CreateInMemoryOptions(string dbName)
    {
        return new DbContextOptionsBuilder<FitRankDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
    }

    private async Task SeedSociosYPuntajes(FitRankDbContext context)
    {
        var socios = new List<Socio>
        {
            new Socio { Nombre = "Juan", Apellido = "Perez", Email = "juan@test.com",Nivel = "Principiante" },
            new Socio { Nombre = "Ana", Apellido = "Gomez", Email = "ana@test.com",Nivel = "Principiante" },
            new Socio { Nombre = "Luis", Apellido = "Lopez", Email = "luis@test.com",Nivel = "Principiante" }
        };
        context.Socios.AddRange(socios);
        await context.SaveChangesAsync();

        var puntajes = new List<Puntaje>
        {
            new Puntaje { SocioId = socios[0].Id, Valor = 100,Motivo = "Motivo" },
            new Puntaje { SocioId = socios[1].Id, Valor = 200,Motivo = "Motivo" },
            new Puntaje { SocioId = socios[2].Id, Valor = 120,Motivo = "Motivo" }
        };
        context.Puntajes.AddRange(puntajes);
        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task ObtenerTopSociosAsync_DeberiaRetornarSociosOrdenadosPorPuntaje()
    {
        // Arrange
        var options = CreateInMemoryOptions("RankingTopSociosDb");
        using var context = new FitRankDbContext(options);
        await SeedSociosYPuntajes(context);
        var repo = new RankingRepositorioImpl(context);

        // Act
        var resultado = await repo.ObtenerTopSociosAsync(2);

        // Assert
        resultado.Should().HaveCount(2);
        resultado[0].NombreCompleto.Should().Be("Ana Gomez");
        resultado[0].PuntajeTotal.Should().Be(200);
        resultado[1].NombreCompleto.Should().Be("Luis Lopez");
        resultado[1].PuntajeTotal.Should().Be(120);
    }

    [Fact]
    public async Task ObtenerPosicionPorIdAsync_DeberiaRetornarPosicionCorrecta()
    {
        // Arrange
        var options = CreateInMemoryOptions("RankingPosicionDb");
        using var context = new FitRankDbContext(options);
        await SeedSociosYPuntajes(context);
        var repo = new RankingRepositorioImpl(context);

        var socioJuan = await context.Socios.FirstAsync(s => s.Nombre == "Juan");
        var socioAna = await context.Socios.FirstAsync(s => s.Nombre == "Ana");
        var socioLuis = await context.Socios.FirstAsync(s => s.Nombre == "Luis");

        // Act
        var posicionJuan = await repo.ObtenerPosicionPorIdAsync(socioJuan.Id);
        var posicionAna = await repo.ObtenerPosicionPorIdAsync(socioAna.Id);
        var posicionLuis = await repo.ObtenerPosicionPorIdAsync(socioLuis.Id);

        // Assert
        posicionAna.Should().NotBeNull();
        posicionAna!.Posicion.Should().Be(1);
        posicionAna.NombreCompleto.Should().Be("Ana Gomez");
        posicionAna.PuntajeTotal.Should().Be(200);

        posicionLuis.Should().NotBeNull();
        posicionLuis!.Posicion.Should().Be(2);
        posicionLuis.NombreCompleto.Should().Be("Luis Lopez");
        posicionLuis.PuntajeTotal.Should().Be(120);

        posicionJuan.Should().NotBeNull();
        posicionJuan!.Posicion.Should().Be(3);
        posicionJuan.NombreCompleto.Should().Be("Juan Perez");
        posicionJuan.PuntajeTotal.Should().Be(100);
    }
}
