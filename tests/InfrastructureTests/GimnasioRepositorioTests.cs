using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Persistence;
using FitRank_API.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FitRank_API.tests.InfrastructureTests;

public class GimnasioRepositorioTests
{
    private DbContextOptions<FitRankDbContext> CreateInMemoryOptions(string dbName)
    {
        return new DbContextOptionsBuilder<FitRankDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
    }

    private async Task<Administrador> SeedData(FitRankDbContext context)
    {
        var administrador = new Administrador
        {
            Nombre = "Admin",
            Email = "admin@example.com",
        };
        context.Administradores.Add(administrador);
        await context.SaveChangesAsync();

        return administrador;
    }

    [Fact]
    public async Task AgregarGimnasio_DeberiaGuardarCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("AgregarGimnasioDb");
        using var context = new FitRankDbContext(options);
        var gimnasioRepositorioMock = new GimnasioRepositorioImpl(context);
        var administrador = await SeedData(context);

        var gimnasio = new Gimnasio
        {
            Nombre = "Gimnasio Test",
            Direccion = "Calle Test 123",
            AdministradorId = administrador.Id
        };

        // Act
        await gimnasioRepositorioMock.AgregarGimnasio(gimnasio);

        // FluentAssert
        var gimnasioEnDb = await context.Gimnasios.FirstOrDefaultAsync(g => g.Id == gimnasio.Id);
        Assert.NotNull(gimnasioEnDb);
        Assert.Equal("Gimnasio Test", gimnasioEnDb!.Nombre);
        Assert.Equal("Calle Test 123", gimnasioEnDb.Direccion);
        Assert.Equal(administrador.Id, gimnasioEnDb.AdministradorId);
    }

    //obtener lista de gimnasios
    [Fact]
    public async Task ObtenerTodosLosGimnasios_DeberiaRetornarListaCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerTodosLosGimnasiosDb");
        using var context = new FitRankDbContext(options);
        var gimnasioRepositorioMock = new GimnasioRepositorioImpl(context);

        context.Gimnasios.AddRange(
            new Gimnasio { Nombre = "Gimnasio 1" },
            new Gimnasio { Nombre = "Gimnasio 2" }
        );
        await context.SaveChangesAsync();

        // Act
        var gimnasios = await gimnasioRepositorioMock.ObtenerTodosLosGimnasios();

        // Assert
        Assert.Equal(2, gimnasios.Count);
        Assert.Contains(gimnasios, g => g.Nombre == "Gimnasio 1");
        Assert.Contains(gimnasios, g => g.Nombre == "Gimnasio 2");
    }

    //obtener lista de gimnasios vacia
    [Fact]
    public async Task ObtenerTodosLosGimnasios_DeberiaRetornarListaVacia()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerTodosLosGimnasiosVaciaDb");
        using var context = new FitRankDbContext(options);
        var gimnasioRepositorioMock = new GimnasioRepositorioImpl(context);

        // Act
        var gimnasios = await gimnasioRepositorioMock.ObtenerTodosLosGimnasios();

        // Assert
        Assert.Empty(gimnasios);
    }

    //obtener gimnasio por id
    [Fact]
    public async Task ObtenerGimnasioPorId_DeberiaRetornarGimnasioCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerGimnasioPorIdDb");
        using var context = new FitRankDbContext(options);
        var gimnasioRepositorioMock = new GimnasioRepositorioImpl(context);

        var gimnasio = new Gimnasio { Nombre = "Gimnasio Test" };
        context.Gimnasios.Add(gimnasio);
        await context.SaveChangesAsync();

        // Act
        var gimnasioObtenido = await gimnasioRepositorioMock.ObtenerGimnasioPorId(gimnasio.Id);

        // Assert
        Assert.NotNull(gimnasioObtenido);
        Assert.Equal("Gimnasio Test", gimnasioObtenido!.Nombre);
    }

    //obtener gimnasio por id inexistente
    [Fact]
    public async Task ObtenerGimnasioPorId_Inexistente_DeberiaRetornarNull()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerGimnasioPorIdInexistenteDb");
        using var context = new FitRankDbContext(options);
        var gimnasioRepositorioMock = new GimnasioRepositorioImpl(context);

        // Act
        var gimnasioObtenido = await gimnasioRepositorioMock.ObtenerGimnasioPorId(999);

        // Assert
        Assert.Null(gimnasioObtenido);
    }

    //actualizar gimnasio
    [Fact]
    public async Task ActualizarGimnasio_DeberiaActualizarCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("ActualizarGimnasioDb");
        using var context = new FitRankDbContext(options);
        var gimnasioRepositorioMock = new GimnasioRepositorioImpl(context);

        var gimnasio = new Gimnasio
        {
            Nombre = "Gimnasio Original",
            Direccion = "Direccion Original"
        };
        context.Gimnasios.Add(gimnasio);
        await context.SaveChangesAsync();

        // Act
        gimnasio.Nombre = "Gimnasio Actualizado";
        gimnasio.Direccion = "Direccion Actualizada";
        var gimnasioActualizado = await gimnasioRepositorioMock.ActualizarGimnasio(gimnasio);

        // FluentAssert
        Assert.NotNull(gimnasioActualizado);
        Assert.Equal("Gimnasio Actualizado", gimnasioActualizado!.Nombre);
        Assert.Equal("Direccion Actualizada", gimnasioActualizado.Direccion);

    }

    //actualizar gimnasio inexistente
    [Fact]
    public async Task ActualizarGimnasio_Inexistente_DeberiaRetornarNull()
    {
        // Arrange
        var options = CreateInMemoryOptions("ActualizarGimnasioInexistenteDb");
        using var context = new FitRankDbContext(options);
        var gimnasioRepositorioMock = new GimnasioRepositorioImpl(context);

        var gimnasio = new Gimnasio
        {
            Id = 999,
            Nombre = "Gimnasio Inexistente",
            Direccion = "Direccion Inexistente"
        };

        // Act
        var gimnasioActualizado = await gimnasioRepositorioMock.ActualizarGimnasio(gimnasio);

        // FluentAssert
        Assert.Null(gimnasioActualizado);
    }

    //eliminar gimnasio
    [Fact]
    public async Task EliminarGimnasio_DeberiaEliminarCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("EliminarGimnasioDb");
        using var context = new FitRankDbContext(options);
        var gimnasioRepositorioMock = new GimnasioRepositorioImpl(context);

        var gimnasio = new Gimnasio { Nombre = "Gimnasio a Eliminar" };
        context.Gimnasios.Add(gimnasio);
        await context.SaveChangesAsync();

        // Act
        var resultado = await gimnasioRepositorioMock.EliminarGimnasio(gimnasio.Id);

        // FluentAssert
        Assert.True(resultado);
        var gimnasioEnDb = await context.Gimnasios.FindAsync(gimnasio.Id);
        Assert.Null(gimnasioEnDb);
    }
}