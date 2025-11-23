using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Enums;
using FitRank_API.Infrastructure.Persistence;
using FitRank_API.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FitRank_API.Tests.InfrastructureTests;

public class AmistadRepositorioTests
{
    private DbContextOptions<FitRankDbContext> CreateInMemoryOptions(string dbName)
    {
        return new DbContextOptionsBuilder<FitRankDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
    }

    private async Task<(Socio, Socio)> SeedSocios(FitRankDbContext context)
    {
        var socio1 = new Socio { Nombre = "Juan", Email = "juan@test.com", Nivel = "Nivel" };
        var socio2 = new Socio { Nombre = "Ana", Email = "ana@test.com", Nivel = "Nivel" };
        context.Socios.AddRange(socio1, socio2);
        await context.SaveChangesAsync();
        return (socio1, socio2);
    }

    [Fact]
    public async Task CrearAsync_DeberiaAgregarAmistad()
    {
        // Arrange
        var options = CreateInMemoryOptions("CrearAmistadDb");
        using var context = new FitRankDbContext(options);
        var repo = new AmistadRepositorioImpl(context);
        var (socio1, socio2) = await SeedSocios(context);

        var amistad = new Amistad
        {
            SocioId1 = socio1.Id,
            SocioId2 = socio2.Id,
            Estado = EstadoAmistad.Aceptado,
            SolicitanteId = socio1.Id
        };

        // Act
        var resultado = await repo.CrearAsync(amistad);

        // Assert
        resultado.Should().NotBeNull();
        resultado.SocioId1.Should().Be(socio1.Id);
        resultado.SocioId2.Should().Be(socio2.Id);
        resultado.Estado.Should().Be(EstadoAmistad.Aceptado);
    }

    [Fact]
    public async Task EliminarAsync_DeberiaEliminarAmistad()
    {
        // Arrange
        var options = CreateInMemoryOptions("EliminarAmistadDb");
        using var context = new FitRankDbContext(options);
        var repo = new AmistadRepositorioImpl(context);
        var (socio1, socio2) = await SeedSocios(context);

        var amistad = new Amistad
        {
            SocioId1 = socio1.Id,
            SocioId2 = socio2.Id,
            Estado = EstadoAmistad.Aceptado,
            SolicitanteId = socio1.Id
        };
        context.Amistades.Add(amistad);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repo.EliminarAsync(amistad);

        // Assert
        resultado.Should().BeTrue();
        var enDb = await context.Amistades.FindAsync(amistad.Id);
        enDb.Should().BeNull();
    }


    [Fact]
    public async Task ObtenerPorIdAsync_DeberiaRetornarAmistadSiExiste()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerPorIdAmistadDb");
        using var context = new FitRankDbContext(options);
        var repo = new AmistadRepositorioImpl(context);
        var (socio1, socio2) = await SeedSocios(context);

        var amistad = new Amistad
        {
            SocioId1 = socio1.Id,
            SocioId2 = socio2.Id,
            Estado = EstadoAmistad.Aceptado,
            SolicitanteId = socio1.Id
        };
        context.Amistades.Add(amistad);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repo.ObtenerPorIdAsync(amistad.Id);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(amistad.Id);
    }

    [Fact]
    public async Task ObtenerPorIdDeSociosAsync_DeberiaRetornarAmistadSiExiste()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerPorIdDeSociosDb");
        using var context = new FitRankDbContext(options);
        var repo = new AmistadRepositorioImpl(context);
        var (socio1, socio2) = await SeedSocios(context);

        var amistad = new Amistad
        {
            SocioId1 = socio1.Id,
            SocioId2 = socio2.Id,
            Estado = EstadoAmistad.Aceptado,
            SolicitanteId = socio1.Id
        };
        context.Amistades.Add(amistad);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repo.ObtenerPorIdDeSociosAsync(socio1.Id, socio2.Id);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.SocioId1.Should().Be(socio1.Id);
        resultado.SocioId2.Should().Be(socio2.Id);
    }

    [Fact]
    public async Task ActualizarAsync_DeberiaActualizarAmistad()
    {
        // Arrange
        var options = CreateInMemoryOptions("ActualizarAmistadDb");
        using var context = new FitRankDbContext(options);
        var repo = new AmistadRepositorioImpl(context);
        var (socio1, socio2) = await SeedSocios(context);

        var amistad = new Amistad
        {
            SocioId1 = socio1.Id,
            SocioId2 = socio2.Id,
            Estado = EstadoAmistad.Pendiente,
            SolicitanteId = socio1.Id
        };
        context.Amistades.Add(amistad);
        await context.SaveChangesAsync();

        // Act
        amistad.Estado = EstadoAmistad.Aceptado;
        var actualizado = await repo.ActualizarAsync(amistad);

        // Assert
        actualizado.Should().NotBeNull();
        actualizado.Estado.Should().Be(EstadoAmistad.Aceptado);
    }

    [Fact]
    public async Task ObtenerSolicitudesPendientesAsync_DeberiaRetornarSolicitudesPendientesDeOtros()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerSolicitudesPendientesDb");
        using var context = new FitRankDbContext(options);
        var repo = new AmistadRepositorioImpl(context);
        var (socio1, socio2) = await SeedSocios(context);

        context.Amistades.AddRange(
            new Amistad { SocioId1 = socio1.Id, SocioId2 = socio2.Id, Estado = EstadoAmistad.Pendiente, SolicitanteId = socio2.Id },
            new Amistad { SocioId1 = socio1.Id, SocioId2 = socio2.Id, Estado = EstadoAmistad.Pendiente, SolicitanteId = socio1.Id }
        );
        await context.SaveChangesAsync();

        // Act
        var pendientes = await repo.ObtenerSolicitudesPendientesAsync(socio1.Id);

        // Assert
        pendientes.Should().HaveCount(1);
        pendientes.First().SolicitanteId.Should().Be(socio2.Id);
    }
}