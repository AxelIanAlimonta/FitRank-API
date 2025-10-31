using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Persistence;
using FitRank_API.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FitRank_API.tests.InfrastructureTests;

public class FotoRepositorioTests
{
    private DbContextOptions<FitRankDbContext> GetInMemoryDbOptions(string dbName)
    {
        return new DbContextOptionsBuilder<FitRankDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .EnableSensitiveDataLogging()
            .Options;
    }

    private async Task<Socio> DataSeed(FitRankDbContext context)
    {
        var socio = new Socio
        {
            Nombre = "Pedro",
            Apellido = "Gonzalez",
            Email = "pedro.gonzalez@example.com"
        };
        context.Socios.Add(socio);
        await context.SaveChangesAsync();
        return socio;
    }

    //agregar foto deberia guardar correctamente
    [Fact]
    public async Task AgregarFoto_DeberiaGuardarCorrectamente()
    {
        // Arrange
        var options = GetInMemoryDbOptions("AgregarFotoDb");
        using var context = new FitRankDbContext(options);
        var fotoRepositorioMock = new FotoRepositorioImpl(context);
        var socio = await DataSeed(context);
        var foto = new Foto
        {
            UrlImagen = "http://foto.url/pedro.jpg",
            SocioId = socio.Id,
            Fecha = new DateTime(2024, 1, 1),
        };

        // Act
        await fotoRepositorioMock.AgregarAsync(foto);

        // FluentAssert
        var fotoEnDb = await context.Fotos.FirstOrDefaultAsync(f => f.Id == foto.Id);
        fotoEnDb.Should().NotBeNull();
        fotoEnDb!.UrlImagen.Should().Be("http://foto.url/pedro.jpg");
        fotoEnDb.SocioId.Should().Be(socio.Id);
        fotoEnDb.Fecha.Should().Be(new DateTime(2024, 1, 1));
    }

    // obtener por socio id deberia retornar lista correctamente
    [Fact]
    public async Task ObtenerPorSocioId_DeberiaRetornarListaCorrectamente()
    {
        // Arrange
        var options = GetInMemoryDbOptions("ObtenerFotosPorSocioIdDb");
        using var context = new FitRankDbContext(options);
        var fotoRepositorioMock = new FotoRepositorioImpl(context);
        var socio = await DataSeed(context);

        var foto1 = new Foto
        {
            UrlImagen = "http://foto.url/pedro1.jpg",
            SocioId = socio.Id,
            Fecha = new DateTime(2024, 1, 1),
        };

        var foto2 = new Foto
        {
            UrlImagen = "http://foto.url/pedro2.jpg",
            SocioId = socio.Id,
            Fecha = new DateTime(2024, 1, 1),
        };

        context.Fotos.AddRange(foto1, foto2);
        await context.SaveChangesAsync();

        // Act
        var resultado = await fotoRepositorioMock.ObtenerPorSocioAsync(socio.Id);

        // Assert
        resultado.Should().HaveCount(2);
        resultado.Select(f => f.UrlImagen).Should().Contain(new[] { "http://foto.url/pedro1.jpg", "http://foto.url/pedro2.jpg" });
        resultado.Select(f => f.Fecha).Should().Contain(new[] { new DateTime(2024, 1, 1), new DateTime(2024, 1, 1) });
        resultado.Select(f => f.SocioId).Should().Contain(socio.Id);
    }

    // eliminar foto deberia eliminar correctamente
    [Fact]
    public async Task EliminarFoto_DeberiaEliminarCorrectamente()
    {
        // Arrange
        var options = GetInMemoryDbOptions("EliminarFotoDb");
        using var context = new FitRankDbContext(options);
        var fotoRepositorioMock = new FotoRepositorioImpl(context);
        var socio = await DataSeed(context);

        var foto = new Foto
        {
            UrlImagen = "http://foto.url/pedro_eliminar.jpg",
            SocioId = socio.Id,
            Fecha = new DateTime(2024, 1, 1),
        };

        context.Fotos.Add(foto);
        await context.SaveChangesAsync();

        // Act
        var resultado = await fotoRepositorioMock.EliminarAsync(foto.Id);

        // FluentAssert
        resultado.Should().BeTrue();
        var fotoEnDb = await context.Fotos.FirstOrDefaultAsync(f => f.Id == foto.Id);
        fotoEnDb.Should().BeNull();
    }

    // eliminar foto inexistente deberia retornar false
    [Fact]
    public async Task EliminarFoto_Inexistente_DeberiaRetornarFalse()
    {
        // Arrange
        var options = GetInMemoryDbOptions("EliminarFotoInexistenteDb");
        using var context = new FitRankDbContext(options);
        var fotoRepositorioMock = new FotoRepositorioImpl(context);

        // Act
        var resultado = await fotoRepositorioMock.EliminarAsync(999);

        // FluentAssert
        resultado.Should().BeFalse();
    }
}
