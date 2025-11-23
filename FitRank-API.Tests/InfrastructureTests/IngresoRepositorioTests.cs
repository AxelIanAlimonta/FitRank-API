using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Persistence;
using FitRank_API.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FitRank_API.Tests.InfrastructureTests;

public class IngresoRepositorioTests
{
    private DbContextOptions<FitRankDbContext> CreateInMemoryOptions(string dbName)
    {
        return new DbContextOptionsBuilder<FitRankDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
    }

    private async Task<(Usuario, Gimnasio)> SeedUsuarioYGimnasio(FitRankDbContext context)
    {
        var gimnasio = new Gimnasio { Nombre = "Gimnasio Test" };
        context.Gimnasios.Add(gimnasio);
        await context.SaveChangesAsync();

        var usuario = new Usuario { Nombre = "Usuario Test", Email = "usuario@test.com" };
        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();

        return (usuario, gimnasio);
    }

    [Fact]
    public async Task AgregarAsync_DeberiaAgregarIngreso()
    {
        // Arrange
        var options = CreateInMemoryOptions("AgregarIngresoDb");
        using var context = new FitRankDbContext(options);
        var repo = new IngresoRepositorio(context);
        var (usuario, gimnasio) = await SeedUsuarioYGimnasio(context);

        var ingreso = new Ingreso
        {
            UsuarioId = usuario.Id,
            GimnasioId = gimnasio.Id,
            Fecha = DateTime.UtcNow
        };

        // Act
        await repo.AgregarAsync(ingreso);
        await repo.GuardarCambiosAsync();

        // Assert
        var enDb = await context.Ingresos.FirstOrDefaultAsync(i => i.Id == ingreso.Id);
        enDb.Should().NotBeNull();
        enDb!.UsuarioId.Should().Be(usuario.Id);
        enDb.GimnasioId.Should().Be(gimnasio.Id);
    }

    [Fact]
    public async Task ObtenerTodosAsync_DeberiaRetornarTodosLosIngresosConRelaciones()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerTodosIngresosDb");
        using var context = new FitRankDbContext(options);
        var repo = new IngresoRepositorio(context);
        var (usuario, gimnasio) = await SeedUsuarioYGimnasio(context);

        var ingreso1 = new Ingreso { UsuarioId = usuario.Id, GimnasioId = gimnasio.Id, Fecha = DateTime.UtcNow.AddDays(-1) };
        var ingreso2 = new Ingreso { UsuarioId = usuario.Id, GimnasioId = gimnasio.Id, Fecha = DateTime.UtcNow };
        context.Ingresos.AddRange(ingreso1, ingreso2);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repo.ObtenerTodosAsync();

        // Assert
        resultado.Should().HaveCount(2);
        resultado.All(i => i.Usuario != null).Should().BeTrue();
        resultado.All(i => i.Gimnasio != null).Should().BeTrue();


    }

    [Fact]
    public async Task ObtenerPorGimnasioAsync_DeberiaRetornarSoloIngresosDelGimnasio()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerPorGimnasioDb");
        using var context = new FitRankDbContext(options);
        var repo = new IngresoRepositorio(context);
        var (usuario, gimnasio) = await SeedUsuarioYGimnasio(context);

        var otroGimnasio = new Gimnasio { Nombre = "Otro Gimnasio" };
        context.Gimnasios.Add(otroGimnasio);
        await context.SaveChangesAsync();

        var ingreso1 = new Ingreso { UsuarioId = usuario.Id, GimnasioId = gimnasio.Id, Fecha = DateTime.UtcNow };
        var ingreso2 = new Ingreso { UsuarioId = usuario.Id, GimnasioId = otroGimnasio.Id, Fecha = DateTime.UtcNow };
        context.Ingresos.AddRange(ingreso1, ingreso2);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repo.ObtenerPorGimnasioAsync(gimnasio.Id);

        // Assert
        resultado.Should().HaveCount(1);
        resultado.First().GimnasioId.Should().Be(gimnasio.Id);
    }

    [Fact]
    public async Task ObtenerPorIdAsync_DeberiaRetornarIngresoConRelaciones()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerPorIdIngresoDb");
        using var context = new FitRankDbContext(options);
        var repo = new IngresoRepositorio(context);
        var (usuario, gimnasio) = await SeedUsuarioYGimnasio(context);

        var ingreso = new Ingreso { UsuarioId = usuario.Id, GimnasioId = gimnasio.Id, Fecha = DateTime.UtcNow };
        context.Ingresos.Add(ingreso);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repo.ObtenerPorIdAsync(ingreso.Id);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Usuario.Should().NotBeNull();
        resultado.Gimnasio.Should().NotBeNull();
    }

    [Fact]
    public async Task EliminarAsync_DeberiaEliminarIngreso()
    {
        // Arrange
        var options = CreateInMemoryOptions("EliminarIngresoDb");
        using var context = new FitRankDbContext(options);
        var repo = new IngresoRepositorio(context);
        var (usuario, gimnasio) = await SeedUsuarioYGimnasio(context);

        var ingreso = new Ingreso { UsuarioId = usuario.Id, GimnasioId = gimnasio.Id, Fecha = DateTime.UtcNow };
        context.Ingresos.Add(ingreso);
        await context.SaveChangesAsync();

        // Act
        await repo.EliminarAsync(ingreso);

        // Assert
        var enDb = await context.Ingresos.FindAsync(ingreso.Id);
        enDb.Should().BeNull();
    }
}