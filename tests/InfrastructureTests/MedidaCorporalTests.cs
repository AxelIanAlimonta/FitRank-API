using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Persistence;
using FitRank_API.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FitRank_API.tests.InfrastructureTests;

public class MedidaCorporalRepositorioTests
{
    private DbContextOptions<FitRankDbContext> GetInMemoryDbOptions(string dbName)
    {
        return new DbContextOptionsBuilder<FitRankDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .EnableSensitiveDataLogging()
            .Options;
    }

    private async Task<Socio> SeedSocio(FitRankDbContext context)
    {
        var socio = new Socio
        {
            Nombre = "Luis",
            Apellido = "Martinez",
            Email = "luis.martinez@example.com"
        };

        context.Socios.Add(socio);
        await context.SaveChangesAsync();

        return socio;
    }

    // agregar medida corporal deberia guardar correctamente
    [Fact]
    public async Task AgregarMedidaCorporal_DeberiaGuardarCorrectamente()
    {
        // Arrange
        var options = GetInMemoryDbOptions("AgregarMedidaCorporalDb");
        using var context = new FitRankDbContext(options);
        var MedidaCorporalRepositorioMock = new MedidaCorporalRepositorioImpl(context);
        var socio = await SeedSocio(context);

        var medidaCorporal = new MedidaCorporal
        {
            Fecha = DateTime.UtcNow,
            CaderaCm = 90.0,
            PechoCm = 100.0,
            PesoKg = 75.5,
            SocioId = socio.Id
        };

        // Act
        await MedidaCorporalRepositorioMock.AgregarAsync(medidaCorporal);

        // FluentAssert
        var medidaCorporalEnDb = await context.MedidasCorporales.FirstOrDefaultAsync(m => m.Id == medidaCorporal.Id);
        Assert.NotNull(medidaCorporalEnDb);
        Assert.Equal(medidaCorporal.CaderaCm, medidaCorporalEnDb.CaderaCm);
        Assert.Equal(medidaCorporal.PechoCm, medidaCorporalEnDb.PechoCm);
        Assert.Equal(medidaCorporal.PesoKg, medidaCorporalEnDb.PesoKg);
        Assert.Equal(medidaCorporal.SocioId, medidaCorporalEnDb.SocioId);
    }

    //actualizar medida corporal deberia actualizar correctamente
    [Fact]
    public async Task ActualizarMedidaCorporal_DeberiaActualizarCorrectamente()
    {
        // Arrange
        var options = GetInMemoryDbOptions("ActualizarMedidaCorporalDb");
        using var context = new FitRankDbContext(options);
        var MedidaCorporalRepositorioMock = new MedidaCorporalRepositorioImpl(context);
        var socio = await SeedSocio(context);

        var medidaCorporal = new MedidaCorporal
        {
            Fecha = DateTime.UtcNow,
            CaderaCm = 90.0,
            PechoCm = 100.0,
            PesoKg = 75.5,
            SocioId = socio.Id
        };

        context.MedidasCorporales.Add(medidaCorporal);
        await context.SaveChangesAsync();

        // Act
        medidaCorporal.PesoKg = 78.0;
        await MedidaCorporalRepositorioMock.ActualizarAsync(medidaCorporal);

        // FluentAssert
        var medidaCorporalEnDb = await context.MedidasCorporales.FirstOrDefaultAsync(m => m.Id == medidaCorporal.Id);
        Assert.NotNull(medidaCorporalEnDb);
        Assert.Equal(78.0, medidaCorporalEnDb!.PesoKg);
    }

    //eliminar medida corporal deberia eliminar correctamente
    [Fact]
    public async Task EliminarMedidaCorporal_DeberiaEliminarCorrectamente()
    {
        // Arrange
        var options = GetInMemoryDbOptions("EliminarMedidaCorporalDb");
        using var context = new FitRankDbContext(options);
        var MedidaCorporalRepositorioMock = new MedidaCorporalRepositorioImpl(context);
        var socio = await SeedSocio(context);

        var medidaCorporal = new MedidaCorporal
        {
            Fecha = DateTime.UtcNow,
            CaderaCm = 90.0,
            PechoCm = 100.0,
            PesoKg = 75.5,
            SocioId = socio.Id
        };

        context.MedidasCorporales.Add(medidaCorporal);
        await context.SaveChangesAsync();

        // Act
        var resultado = await MedidaCorporalRepositorioMock.EliminarAsync(medidaCorporal.Id);

        // FluentAssert
        Assert.True(resultado);
        var medidaCorporalEnDb = await context.MedidasCorporales.FirstOrDefaultAsync(m => m.Id == medidaCorporal.Id);
        Assert.Null(medidaCorporalEnDb);
    }

    //eliminar medida corporal inexistente deberia devolver false
    [Fact]
    public async Task EliminarMedidaCorporal_Inexistente_DeberiaDevolverFalse()
    {
        // Arrange
        var options = GetInMemoryDbOptions("EliminarMedidaCorporalInexistenteDb");
        using var context = new FitRankDbContext(options);
        var MedidaCorporalRepositorioMock = new MedidaCorporalRepositorioImpl(context);

        // Act
        var resultado = await MedidaCorporalRepositorioMock.EliminarAsync(999); // ID inexistente

        // FluentAssert
        Assert.False(resultado);

    }

}