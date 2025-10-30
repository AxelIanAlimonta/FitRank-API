using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Persistence;
using FitRank_API.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FitRank_API.tests.InfrastructureTests;

public class LogroRepositorioTests
{
    private DbContextOptions<FitRankDbContext> CreateInMemoryOptions(string dbName)
    {
        return new DbContextOptionsBuilder<FitRankDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
    }

    // Agregar Logro deberia guardar correctamente
    [Fact]
    public async Task AgregarLogro_DeberiaGuardarCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("AgregarLogroDb");
        var logroRepositorioMock = new LogroRepositorioImpl(new FitRankDbContext(options));
        using var context = new FitRankDbContext(options);

        var logro = new Logro
        {
            Nombre = "Logro de Prueba",
            NombreClave = "logro_prueba",
            Descripcion = "Descripción del logro de prueba",
            Imagen = "http://icono.logro/prueba.png",
            Categoria = "Categoría de Prueba",
            Puntos = 100
        };

        // Act
        await logroRepositorioMock.AgregarLogro(logro);

        var logroGuardado = await context.Logros.FirstOrDefaultAsync(l => l.Id == logro.Id);

        // fluent Assert
        logroGuardado.Should().NotBeNull();
        logroGuardado!.Should().BeEquivalentTo(logro, options => options.ExcludingMissingMembers());
    }

}