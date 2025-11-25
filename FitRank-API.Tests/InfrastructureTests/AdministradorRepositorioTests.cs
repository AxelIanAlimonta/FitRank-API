using System.Threading.Tasks;
using System.Linq;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Persistence;
using FitRank_API.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FitRank_API.Tests.InfrastructureTests;

public class AdministradorRepositorioTests
{
    private DbContextOptions<FitRankDbContext> CreateInMemoryOptions(string dbName)
    {
        return new DbContextOptionsBuilder<FitRankDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
    }

    [Fact]
    public async Task AgregarAsync_DeberiaAgregarAdministrador()
    {
        var options = CreateInMemoryOptions("AgregarAdministradorDb");
        using var context = new FitRankDbContext(options);
        var repo = new AdministradorRepositorioImpl(context);

        var admin = new Administrador { Nombre = "Admin", Email = "admin@test.com" };
        var resultado = await repo.AgregarAsync(admin);

        resultado.Should().NotBeNull();
        resultado.Nombre.Should().Be("Admin");
        (await context.Administradores.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task EliminarAsync_DeberiaEliminarAdministrador()
    {
        var options = CreateInMemoryOptions("EliminarAdministradorDb");
        using var context = new FitRankDbContext(options);
        var repo = new AdministradorRepositorioImpl(context);

        var admin = new Administrador { Nombre = "Admin", Email = "admin@test.com" };
        context.Administradores.Add(admin);
        await context.SaveChangesAsync();

        await repo.EliminarAsync(admin);

        var enDb = await context.Administradores.FindAsync(admin.Id);
        enDb.Should().BeNull();
    }

    [Fact]
    public async Task ObtenerPorIdAsync_DeberiaRetornarAdministradorCorrecto()
    {
        var options = CreateInMemoryOptions("ObtenerPorIdAdministradorDb");
        using var context = new FitRankDbContext(options);
        var repo = new AdministradorRepositorioImpl(context);

        var admin = new Administrador { Nombre = "Admin", Email = "admin@test.com" };
        context.Administradores.Add(admin);
        await context.SaveChangesAsync();

        var resultado = await repo.ObtenerPorIdAsync(admin.Id);

        resultado.Should().NotBeNull();
        resultado!.Nombre.Should().Be("Admin");
    }

    [Fact]
    public async Task ObtenerTodosAsync_DeberiaRetornarTodosLosAdministradores()
    {
        var options = CreateInMemoryOptions("ObtenerTodosAdministradoresDb");
        using var context = new FitRankDbContext(options);
        var repo = new AdministradorRepositorioImpl(context);

        context.Administradores.AddRange(
            new Administrador { Nombre = "Admin1", Email = "a1@test.com" },
            new Administrador { Nombre = "Admin2", Email = "a2@test.com" }
        );
        await context.SaveChangesAsync();

        var resultado = await repo.ObtenerTodosAsync();

        resultado.Should().HaveCount(2);
        resultado.Any(a => a.Nombre == "Admin1").Should().BeTrue();
        resultado.Any(a => a.Nombre == "Admin2").Should().BeTrue();
    }

    [Fact]
    public async Task ObtenerPorAdministradorIdAsync_DeberiaRetornarGimnasioCorrecto()
    {
        var options = CreateInMemoryOptions("ObtenerPorAdministradorIdDb");
        using var context = new FitRankDbContext(options);
        var repo = new AdministradorRepositorioImpl(context);

        var admin = new Administrador { Nombre = "Admin", Email = "admin@test.com" };
        context.Administradores.Add(admin);
        await context.SaveChangesAsync();

        var gimnasio = new Gimnasio { Nombre = "Gym", AdministradorId = admin.Id };
        context.Gimnasios.Add(gimnasio);
        await context.SaveChangesAsync();

        var resultado = await repo.ObtenerPorAdministradorIdAsync(admin.Id);

        resultado.Should().NotBeNull();
        resultado!.Nombre.Should().Be("Gym");
        resultado.AdministradorId.Should().Be(admin.Id);
    }

    [Fact]
    public async Task ActualizarAsync_DeberiaActualizarAdministrador()
    {
        var options = CreateInMemoryOptions("ActualizarAdministradorDb");
        using var context = new FitRankDbContext(options);
        var repo = new AdministradorRepositorioImpl(context);

        var admin = new Administrador { Nombre = "Original", Email = "admin@test.com" };
        context.Administradores.Add(admin);
        await context.SaveChangesAsync();

        admin.Nombre = "Actualizado";
        await repo.ActualizarAsync(admin);

        var actualizado = await context.Administradores.FindAsync(admin.Id);
        actualizado.Should().NotBeNull();
        actualizado!.Nombre.Should().Be("Actualizado");
    }

    [Fact]
    public async Task ObtenerTodosPorGimnasio_DeberiaRetornarAdministradoresDelGimnasio()
    {
        var options = CreateInMemoryOptions("ObtenerTodosPorGimnasioDb");
        using var context = new FitRankDbContext(options);
        var repo = new AdministradorRepositorioImpl(context);

        var gimnasio = new Gimnasio { Nombre = "Gym" };
        context.Gimnasios.Add(gimnasio);
        await context.SaveChangesAsync();

        var admin1 = new Administrador { Nombre = "Admin1", GimnasioId = gimnasio.Id, Email = "asd@asd.com" };
        var admin2 = new Administrador { Nombre = "Admin2", GimnasioId = gimnasio.Id, Email = "asd@asd.com" };
        var admin3 = new Administrador { Nombre = "Admin3", GimnasioId = gimnasio.Id + 1, Email = "asd@asd.com" };
        context.Administradores.AddRange(admin1, admin2, admin3);
        await context.SaveChangesAsync();

        var resultado = await repo.ObtenerTodosPorGimnasio(gimnasio.Id);

        resultado.Should().HaveCount(2);
        resultado.Any(a => a.Nombre == "Admin1").Should().BeTrue();
        resultado.Any(a => a.Nombre == "Admin2").Should().BeTrue();
    }
}