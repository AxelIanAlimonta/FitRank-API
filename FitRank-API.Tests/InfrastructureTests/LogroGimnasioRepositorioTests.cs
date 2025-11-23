using System.Threading.Tasks;
using System.Linq;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Persistence;
using FitRank_API.Infrastructure.Repositorios;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FitRank_API.Tests.InfrastructureTests;

public class LogroGimnasioRepositorioTests
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
    public async Task CrearAsync_DeberiaAgregarLogroGimnasio()
    {
        var options = CreateInMemoryOptions("CrearLogroGimnasioDb");
        using var context = new FitRankDbContext(options);
        var repo = new LogroGimnasioRepositorio(context);

        var logro = CrearLogroCompleto();
        context.Logros.Add(logro);
        await context.SaveChangesAsync();

        var entidad = new LogroGimnasio
        {
            GimnasioId = 1,
            LogroId = logro.Id
        };

        var resultado = await repo.CrearAsync(entidad);

        resultado.Should().NotBeNull();
        resultado.GimnasioId.Should().Be(1);
        resultado.LogroId.Should().Be(logro.Id);
        (await context.LogrosGimnasio.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_DeberiaRetornarLogroGimnasioConLogro()
    {
        var options = CreateInMemoryOptions("ObtenerPorIdLogroGimnasioDb");
        using var context = new FitRankDbContext(options);
        var repo = new LogroGimnasioRepositorio(context);

        var logro = CrearLogroCompleto();
        context.Logros.Add(logro);
        await context.SaveChangesAsync();

        var entidad = new LogroGimnasio
        {
            GimnasioId = 2,
            LogroId = logro.Id
        };
        context.LogrosGimnasio.Add(entidad);
        await context.SaveChangesAsync();

        var resultado = await repo.ObtenerPorIdAsync(entidad.Id);

        resultado.Should().NotBeNull();
        resultado!.Logro.Should().NotBeNull();
        resultado.GimnasioId.Should().Be(2);
    }

    [Fact]
    public async Task ObtenerPorGimnasioYLogroAsync_DeberiaRetornarCorrecto()
    {
        var options = CreateInMemoryOptions("ObtenerPorGimnasioYLogroDb");
        using var context = new FitRankDbContext(options);
        var repo = new LogroGimnasioRepositorio(context);

        var logro = CrearLogroCompleto();
        context.Logros.Add(logro);
        await context.SaveChangesAsync();

        var entidad = new LogroGimnasio
        {
            GimnasioId = 3,
            LogroId = logro.Id
        };
        context.LogrosGimnasio.Add(entidad);
        await context.SaveChangesAsync();

        var resultado = await repo.ObtenerPorGimnasioYLogroAsync(3, logro.Id);

        resultado.Should().NotBeNull();
        resultado!.LogroId.Should().Be(logro.Id);
        resultado.GimnasioId.Should().Be(3);
    }

    [Fact]
    public async Task ObtenerPorGimnasioAsync_DeberiaRetornarTodosLosLogrosDelGimnasio()
    {
        var options = CreateInMemoryOptions("ObtenerPorGimnasioDb");
        using var context = new FitRankDbContext(options);
        var repo = new LogroGimnasioRepositorio(context);

        var logro = CrearLogroCompleto();
        context.Logros.Add(logro);
        await context.SaveChangesAsync();

        context.LogrosGimnasio.AddRange(
            new LogroGimnasio { GimnasioId = 4, LogroId = logro.Id },
            new LogroGimnasio { GimnasioId = 4, LogroId = logro.Id }
        );
        await context.SaveChangesAsync();

        var resultado = await repo.ObtenerPorGimnasioAsync(4);

        resultado.Should().HaveCount(2);
        resultado.All(lg => lg.GimnasioId == 4).Should().BeTrue();
    }

    [Fact]
    public async Task ActualizarAsync_DeberiaActualizarLogroGimnasioExistente()
    {
        var options = CreateInMemoryOptions("ActualizarLogroGimnasioDb");
        using var context = new FitRankDbContext(options);
        var repo = new LogroGimnasioRepositorio(context);

        var logro = CrearLogroCompleto();
        context.Logros.Add(logro);
        await context.SaveChangesAsync();

        var entidad = new LogroGimnasio
        {
            GimnasioId = 5,
            LogroId = logro.Id
        };
        context.LogrosGimnasio.Add(entidad);
        await context.SaveChangesAsync();

        entidad.GimnasioId = 6;
        var actualizado = await repo.ActualizarAsync(entidad);

        actualizado.Should().NotBeNull();
        actualizado!.GimnasioId.Should().Be(6);
    }

    [Fact]
    public async Task ActualizarAsync_DeberiaRetornarNullSiNoExiste()
    {
        var options = CreateInMemoryOptions("ActualizarLogroGimnasioInexistenteDb");
        using var context = new FitRankDbContext(options);
        var repo = new LogroGimnasioRepositorio(context);

        var entidad = new LogroGimnasio
        {
            Id = 999,
            GimnasioId = 7,
            LogroId = 1
        };

        var actualizado = await repo.ActualizarAsync(entidad);

        actualizado.Should().BeNull();
    }
}