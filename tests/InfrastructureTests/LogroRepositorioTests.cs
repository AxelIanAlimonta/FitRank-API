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

    //obtener lista de logros deberia retornar lista correctamente
    [Fact]
    public async Task ObtenerLogros_DeberiaRetornarListaCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerLogrosDb");
        using var context = new FitRankDbContext(options);
        var logroRepositorioMock = new LogroRepositorioImpl(context);

        var logro1 = new Logro
        {
            Nombre = "Logro 1",
            NombreClave = "logro_1",
            Descripcion = "Descripción del logro 1",
            Imagen = "http://icono.logro/1.png",
            Categoria = "Categoría 1",
            Puntos = 50
        };

        var logro2 = new Logro
        {
            Nombre = "Logro 2",
            NombreClave = "logro_2",
            Descripcion = "Descripción del logro 2",
            Imagen = "http://icono.logro/2.png",
            Categoria = "Categoría 2",
            Puntos = 75
        };

        context.Logros.AddRange(logro1, logro2);
        await context.SaveChangesAsync();

        // Act
        var logros = await logroRepositorioMock.ObtenerTodosLosLogros();

        // Assert
        logros.Should().HaveCount(2);
        logros.Should().ContainEquivalentOf(logro1, options => options.ExcludingMissingMembers());
        logros.Should().ContainEquivalentOf(logro2, options => options.ExcludingMissingMembers());
    }

    //obtener lista vacia
    [Fact]
    public async Task ObtenerLogros_DeberiaRetornarListaVacia()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerLogrosVaciaDb");
        using var context = new FitRankDbContext(options);
        var logroRepositorioMock = new LogroRepositorioImpl(context);

        // Act
        var logros = await logroRepositorioMock.ObtenerTodosLosLogros();

        // Assert
        logros.Should().BeEmpty();
    }

    //obtener logro por id
    [Fact]
    public async Task ObtenerLogroPorId_DeberiaRetornarLogroCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerLogroPorIdDb");
        using var context = new FitRankDbContext(options);
        var logroRepositorioMock = new LogroRepositorioImpl(context);

        var logro = new Logro
        {
            Nombre = "Logro de Prueba",
            NombreClave = "logro_prueba",
            Descripcion = "Descripción del logro de prueba",
            Imagen = "http://icono.logro/prueba.png",
            Categoria = "Categoría de Prueba",
            Puntos = 100
        };

        context.Logros.Add(logro);
        await context.SaveChangesAsync();

        // Act
        var logroObtenido = await logroRepositorioMock.ObtenerLogroPorId(logro.Id);

        // Assert
        logroObtenido.Should().NotBeNull();
        logroObtenido!.Should().BeEquivalentTo(logro, options => options.ExcludingMissingMembers());
    }

    //obtener logro por id inexistente
    [Fact]
    public async Task ObtenerLogroPorId_Inexistente_DeberiaRetornarNull()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerLogroPorIdInexistenteDb");
        using var context = new FitRankDbContext(options);
        var logroRepositorioMock = new LogroRepositorioImpl(context);

        // Act
        var logroObtenido = await logroRepositorioMock.ObtenerLogroPorId(999); // ID inexistente

        // Assert
        logroObtenido.Should().BeNull();
    }

    //actualizar logro deberia actualizar correctamente
    [Fact]
    public async Task ActualizarLogro_DeberiaActualizarCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("ActualizarLogroDb");
        using var context = new FitRankDbContext(options);
        var logroRepositorioMock = new LogroRepositorioImpl(context);

        var logro = new Logro
        {
            Nombre = "Logro de Prueba",
            NombreClave = "logro_prueba",
            Descripcion = "Descripción del logro de prueba",
            Imagen = "http://icono.logro/prueba.png",
            Categoria = "Categoría de Prueba",
            Puntos = 100
        };

        context.Logros.Add(logro);
        await context.SaveChangesAsync();

        // Modificar propiedades del logro
        logro.Nombre = "Logro Actualizado";
        logro.Puntos = 150;

        // Act
        var logroActualizado = await logroRepositorioMock.ActualizarLogro(logro);

        var logroEnDb = await context.Logros.FirstOrDefaultAsync(l => l.Id == logro.Id);

        // Assert
        logroActualizado.Should().NotBeNull();
        logroActualizado!.Should().BeEquivalentTo(logro, options => options.ExcludingMissingMembers());

        logroEnDb.Should().NotBeNull();
        logroEnDb!.Should().BeEquivalentTo(logro, options => options.ExcludingMissingMembers());
    }

    //actualizar logro inexistente deberia retornar null
    [Fact]
    public async Task ActualizarLogro_Inexistente_DeberiaRetornarNull()
    {
        // Arrange
        var options = CreateInMemoryOptions("ActualizarLogroInexistenteDb");
        using var context = new FitRankDbContext(options);
        var logroRepositorioMock = new LogroRepositorioImpl(context);

        var logroInexistente = new Logro
        {
            Id = 999, // ID inexistente
            Nombre = "Logro Inexistente",
            NombreClave = "logro_inexistente",
            Descripcion = "Descripción del logro inexistente",
            Imagen = "http://icono.logro/inexistente.png",
            Categoria = "Categoría Inexistente",
            Puntos = 0
        };

        // Act
        var resultado = await logroRepositorioMock.ActualizarLogro(logroInexistente);

        // Assert
        resultado.Should().BeNull();
    }

    //eliminar logro deberia eliminar correctamente
    [Fact]
    public async Task EliminarLogro_DeberiaEliminarCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("EliminarLogroDb");
        using var context = new FitRankDbContext(options);
        var logroRepositorioMock = new LogroRepositorioImpl(context);

        var logro = new Logro
        {
            Nombre = "Logro de Prueba",
            NombreClave = "logro_prueba",
            Descripcion = "Descripción del logro de prueba",
            Imagen = "http://icono.logro/prueba.png",
            Categoria = "Categoría de Prueba",
            Puntos = 100
        };

        context.Logros.Add(logro);
        await context.SaveChangesAsync();

        // Act
        var resultado = await logroRepositorioMock.EliminarLogro(logro.Id);
        var logroEliminado = await context.Logros.FirstOrDefaultAsync(l => l.Id == logro.Id);

        // Assert
        resultado.Should().BeTrue();
        logroEliminado.Should().BeNull();
    }

}