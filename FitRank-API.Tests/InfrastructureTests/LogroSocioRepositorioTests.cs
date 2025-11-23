using System.Threading.Tasks;
using System.Linq;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Persistence;
using FitRank_API.Infrastructure.Repositorios;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FitRank_API.Tests.InfrastructureTests;

public class LogroSocioRepositorioTests
{
    private DbContextOptions<FitRankDbContext> CreateInMemoryOptions(string dbName)
    {
        return new DbContextOptionsBuilder<FitRankDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
    }

    private Logro CrearLogroCompleto()
    {
        return new Logro
        {
            Nombre = "Logro Test",
            NombreClave = "logro_test",
            Descripcion = "Descripción de prueba",
            Categoria = "General",
            Imagen = "logro.png"
        };
    }

    [Fact]
    public async Task CrearAsync_DeberiaAgregarLogroSocio()
    {
        var options = CreateInMemoryOptions("CrearLogroSocioDb");
        using var context = new FitRankDbContext(options);
        var repo = new LogroSocioRepositorio(context);

        var logro = CrearLogroCompleto();
        context.Logros.Add(logro);
        await context.SaveChangesAsync();

        var logroSocio = new LogroSocio
        {
            LogroId = logro.Id,
            GimnasioId = 1,
            SocioId = 1
        };

        var resultado = await repo.CrearAsync(logroSocio);

        resultado.Should().NotBeNull();
        resultado.LogroId.Should().Be(logro.Id);
        resultado.GimnasioId.Should().Be(1);
        resultado.SocioId.Should().Be(1);
        (await context.LogrosSocio.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_DeberiaRetornarLogroSocioConLogro()
    {
        var options = CreateInMemoryOptions("ObtenerPorIdLogroSocioDb");
        using var context = new FitRankDbContext(options);
        var repo = new LogroSocioRepositorio(context);

        var logro = CrearLogroCompleto();
        context.Logros.Add(logro);
        await context.SaveChangesAsync();

        var logroSocio = new LogroSocio
        {
            LogroId = logro.Id,
            GimnasioId = 2,
            SocioId = 2
        };
        context.LogrosSocio.Add(logroSocio);
        await context.SaveChangesAsync();

        var resultado = await repo.ObtenerPorIdAsync(logroSocio.Id);

        resultado.Should().NotBeNull();
        resultado!.Logro.Should().NotBeNull();
        resultado.GimnasioId.Should().Be(2);
        resultado.SocioId.Should().Be(2);
    }

    [Fact]
    public async Task ExisteAsync_DeberiaRetornarTrueSiExiste()
    {
        var options = CreateInMemoryOptions("ExisteLogroSocioDb");
        using var context = new FitRankDbContext(options);
        var repo = new LogroSocioRepositorio(context);

        var logro = CrearLogroCompleto();
        context.Logros.Add(logro);
        await context.SaveChangesAsync();

        var logroSocio = new LogroSocio
        {
            LogroId = logro.Id,
            GimnasioId = 3,
            SocioId = 3
        };
        context.LogrosSocio.Add(logroSocio);
        await context.SaveChangesAsync();

        var existe = await repo.ExisteAsync(logro.Id, 3, 3);

        existe.Should().BeTrue();
    }

    [Fact]
    public async Task ExisteAsync_DeberiaRetornarFalseSiNoExiste()
    {
        var options = CreateInMemoryOptions("NoExisteLogroSocioDb");
        using var context = new FitRankDbContext(options);
        var repo = new LogroSocioRepositorio(context);

        var existe = await repo.ExisteAsync(999, 999, 999);

        existe.Should().BeFalse();
    }

    [Fact]
    public async Task ObtenerPorSocioYGimnasioAsync_DeberiaRetornarLogrosDelSocioEnGimnasio()
    {
        var options = CreateInMemoryOptions("ObtenerPorSocioYGimnasioDb");
        using var context = new FitRankDbContext(options);
        var repo = new LogroSocioRepositorio(context);

        var logro = CrearLogroCompleto();
        context.Logros.Add(logro);
        await context.SaveChangesAsync();

        context.LogrosSocio.AddRange(
            new LogroSocio { LogroId = logro.Id, GimnasioId = 4, SocioId = 4 },
            new LogroSocio { LogroId = logro.Id, GimnasioId = 4, SocioId = 4 }
        );
        await context.SaveChangesAsync();

        var resultado = await repo.ObtenerPorSocioYGimnasioAsync(4, 4);

        resultado.Should().HaveCount(2);
        resultado.All(ls => ls.SocioId == 4 && ls.GimnasioId == 4).Should().BeTrue();

    }

    [Fact]
    public async Task ObtenerPorSocioAsync_DeberiaRetornarTodosLosLogrosDelSocio()
    {
        var options = CreateInMemoryOptions("ObtenerPorSocioDb");
        using var context = new FitRankDbContext(options);
        var repo = new LogroSocioRepositorio(context);

        var logro = CrearLogroCompleto();
        context.Logros.Add(logro);
        await context.SaveChangesAsync();

        context.LogrosSocio.AddRange(
            new LogroSocio { LogroId = logro.Id, GimnasioId = 5, SocioId = 5 },
            new LogroSocio { LogroId = logro.Id, GimnasioId = 6, SocioId = 5 }
        );
        await context.SaveChangesAsync();

        var resultado = await repo.ObtenerPorSocioAsync(5);

        resultado.Should().HaveCount(2);
        resultado.All(ls => ls.SocioId == 5).Should().BeTrue();

    }
}