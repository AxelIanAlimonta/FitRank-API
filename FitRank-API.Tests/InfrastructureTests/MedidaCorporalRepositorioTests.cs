using System;
using System.Linq;
using System.Threading.Tasks;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Persistence;
using FitRank_API.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FitRank_API.Tests.InfrastructureTests;

public class MedidaCorporalRepositorioTests
{
    private DbContextOptions<FitRankDbContext> CreateInMemoryOptions(string dbName)
    {
        return new DbContextOptionsBuilder<FitRankDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
    }

    private Socio CrearSocio()
    {
        return new Socio { Nombre = "Socio", Email = "socio@test.com", Nivel = "Intermedio" };
    }

    private MedidaCorporal CrearMedida(long socioId, DateTime? fecha = null)
    {
        return new MedidaCorporal
        {
            SocioId = socioId,
            Fecha = fecha ?? DateTime.UtcNow,
            BrazoDerechoCm = 30,
            BrazoIzquierdoCm = 29,
            PechoCm = 100,
            CinturaCm = 80,
            CaderaCm = 90,
            PesoKg = 75
        };
    }

    [Fact]
    public async Task AgregarAsync_DeberiaAgregarMedida()
    {
        var options = CreateInMemoryOptions("AgregarMedidaDb");
        using var context = new FitRankDbContext(options);
        var repo = new MedidaCorporalRepositorioImpl(context);

        var socio = CrearSocio();
        context.Socios.Add(socio);
        await context.SaveChangesAsync();

        var medida = CrearMedida(socio.Id);

        var resultado = await repo.AgregarAsync(medida);

        resultado.Should().NotBeNull();
        resultado.SocioId.Should().Be(socio.Id);
        (await context.MedidasCorporales.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_DeberiaRetornarMedidaConSocio()
    {
        var options = CreateInMemoryOptions("ObtenerPorIdMedidaDb");
        using var context = new FitRankDbContext(options);
        var repo = new MedidaCorporalRepositorioImpl(context);

        var socio = CrearSocio();
        context.Socios.Add(socio);
        await context.SaveChangesAsync();

        var medida = CrearMedida(socio.Id);
        context.MedidasCorporales.Add(medida);
        await context.SaveChangesAsync();

        var resultado = await repo.ObtenerPorIdAsync(medida.Id);

        resultado.Should().NotBeNull();
        resultado!.Socio.Should().NotBeNull();
        resultado.SocioId.Should().Be(socio.Id);
    }

    [Fact]
    public async Task ObtenerPorSocioAsync_DeberiaRetornarMedidasDelSocio()
    {
        var options = CreateInMemoryOptions("ObtenerPorSocioDb");
        using var context = new FitRankDbContext(options);
        var repo = new MedidaCorporalRepositorioImpl(context);

        var socio = CrearSocio();
        context.Socios.Add(socio);
        await context.SaveChangesAsync();

        var medida1 = CrearMedida(socio.Id, DateTime.UtcNow.AddDays(-1));
        var medida2 = CrearMedida(socio.Id, DateTime.UtcNow);
        context.MedidasCorporales.AddRange(medida1, medida2);
        await context.SaveChangesAsync();

        var resultado = await repo.ObtenerPorSocioAsync(socio.Id);

        resultado.Should().HaveCount(2);
        resultado.First().Fecha.Should().BeAfter(resultado.Last().Fecha);
    }

    [Fact]
    public async Task ActualizarAsync_DeberiaActualizarMedidaExistente()
    {
        var options = CreateInMemoryOptions("ActualizarMedidaDb");
        using var context = new FitRankDbContext(options);
        var repo = new MedidaCorporalRepositorioImpl(context);

        var socio = CrearSocio();
        context.Socios.Add(socio);
        await context.SaveChangesAsync();

        var medida = CrearMedida(socio.Id);
        context.MedidasCorporales.Add(medida);
        await context.SaveChangesAsync();

        medida.PesoKg = 80;
        medida.CinturaCm = 85;
        var actualizado = await repo.ActualizarAsync(medida);

        actualizado.Should().NotBeNull();
        actualizado!.PesoKg.Should().Be(80);
        actualizado.CinturaCm.Should().Be(85);
    }

    [Fact]
    public async Task ActualizarAsync_DeberiaRetornarNullSiNoExiste()
    {
        var options = CreateInMemoryOptions("ActualizarMedidaInexistenteDb");
        using var context = new FitRankDbContext(options);
        var repo = new MedidaCorporalRepositorioImpl(context);

        var medida = CrearMedida(1);
        medida.Id = 999;

        var actualizado = await repo.ActualizarAsync(medida);

        actualizado.Should().BeNull();
    }

    [Fact]
    public async Task EliminarAsync_DeberiaEliminarMedidaSiExiste()
    {
        var options = CreateInMemoryOptions("EliminarMedidaDb");
        using var context = new FitRankDbContext(options);
        var repo = new MedidaCorporalRepositorioImpl(context);

        var socio = CrearSocio();
        context.Socios.Add(socio);
        await context.SaveChangesAsync();

        var medida = CrearMedida(socio.Id);
        context.MedidasCorporales.Add(medida);
        await context.SaveChangesAsync();

        var resultado = await repo.EliminarAsync(medida.Id);

        resultado.Should().BeTrue();
        var enDb = await context.MedidasCorporales.FindAsync(medida.Id);
        enDb.Should().BeNull();
    }

    [Fact]
    public async Task EliminarAsync_DeberiaRetornarFalseSiNoExiste()
    {
        var options = CreateInMemoryOptions("EliminarMedidaInexistenteDb");
        using var context = new FitRankDbContext(options);
        var repo = new MedidaCorporalRepositorioImpl(context);

        var resultado = await repo.EliminarAsync(999);

        resultado.Should().BeFalse();
    }

    [Fact]
    public async Task ObtenerUltimaMedidaPorSocioAsync_DeberiaRetornarLaMasReciente()
    {
        var options = CreateInMemoryOptions("ObtenerUltimaMedidaPorSocioDb");
        using var context = new FitRankDbContext(options);
        var repo = new MedidaCorporalRepositorioImpl(context);

        var socio = CrearSocio();
        context.Socios.Add(socio);
        await context.SaveChangesAsync();

        var medida1 = CrearMedida(socio.Id, DateTime.UtcNow.AddDays(-2));
        var medida2 = CrearMedida(socio.Id, DateTime.UtcNow.AddDays(-1));
        var medida3 = CrearMedida(socio.Id, DateTime.UtcNow);
        context.MedidasCorporales.AddRange(medida1, medida2, medida3);
        await context.SaveChangesAsync();

        var resultado = await repo.ObtenerUltimaMedidaPorSocioAsync(socio.Id);

        resultado.Should().NotBeNull();
        resultado!.Fecha.Date.Should().Be(medida3.Fecha.Date);
    }
}