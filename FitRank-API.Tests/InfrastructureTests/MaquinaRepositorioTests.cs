using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Persistence;
using FitRank_API.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FitRank_API.tests.InfrastructureTests;

public class MaquinaRepositorioTests
{
    private DbContextOptions<FitRankDbContext> CreateInMemoryOptions(string dbName)
    {
        return new DbContextOptionsBuilder<FitRankDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
    }

    private async Task<Gimnasio> SeedData(FitRankDbContext context)
    {
        var gimnasio = new Gimnasio
        {
            Nombre = "Gimnasio de Prueba",
            Direccion = "Calle Falsa 123",
        };

        context.Gimnasios.Add(gimnasio);
        await context.SaveChangesAsync();

        return gimnasio;
    }

    //agregar maquina deberia guardar correctamente
    [Fact]
    public async Task AgregarMaquina_DeberiaGuardarCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("AgregarMaquinaDb");
        var maquinaRepositorioMock = new MaquinaRepositorioImpl(new FitRankDbContext(options));
        using var context = new FitRankDbContext(options);
        var gimnasio = await SeedData(context);

        var maquina = new Maquina
        {
            Nombre = "Máquina de Prueba",
            UrlImagen = "http://imagen.maquina/prueba.jpg",
            GimnasioId = gimnasio.Id,
            Qr = "http://imagen.maquina/prueba.jpg"
        };

        // Act
        var maquinaGuardada = await maquinaRepositorioMock.AgregarMaquina(maquina);
        // FluentAssertions
        maquinaGuardada.Should().NotBeNull();
        maquinaGuardada.Should().BeEquivalentTo(maquina, options => options.ExcludingMissingMembers());
    }

    //obtener lista de maquinas
    [Fact]
    public async Task ObtenerMaquinas_DeberiaRetornarListaCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerMaquinasDb");
        using var context = new FitRankDbContext(options);
        var maquinaRepositorioMock = new MaquinaRepositorioImpl(context);
        var gimnasio = await SeedData(context);

        var maquina1 = new Maquina
        {
            Nombre = "Máquina 1",
            UrlImagen = "http://imagen.maquina/1.jpg",
            GimnasioId = gimnasio.Id,
            Qr = "http://imagen.maquina/1.jpg"
        };

        var maquina2 = new Maquina
        {
            Nombre = "Máquina 2",
            UrlImagen = "http://imagen.maquina/2.jpg",
            GimnasioId = gimnasio.Id,
            Qr = "http://imagen.maquina/2.jpg"
        };

        context.Maquinas.AddRange(maquina1, maquina2);
        await context.SaveChangesAsync();

        // Act
        var maquinas = await maquinaRepositorioMock.ObtenerTodasLasMaquinas();

        // FluentAssertions
        maquinas.Should().NotBeNull();
        maquinas.Should().HaveCount(2);
        maquinas.Should().ContainEquivalentOf(maquina1, options => options.ExcludingMissingMembers());
        maquinas.Should().ContainEquivalentOf(maquina2, options => options.ExcludingMissingMembers());
    }

    //obtener maquina por id
    [Fact]
    public async Task ObtenerMaquinaPorId_DeberiaRetornarMaquinaCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerMaquinaPorIdDb");
        using var context = new FitRankDbContext(options);
        var maquinaRepositorioMock = new MaquinaRepositorioImpl(context);
        var gimnasio = await SeedData(context);

        var maquina = new Maquina
        {
            Nombre = "Máquina de Prueba",
            UrlImagen = "http://imagen.maquina/prueba.jpg",
            GimnasioId = gimnasio.Id,
            Qr = "http://imagen.maquina/prueba.jpg"
        };

        context.Maquinas.Add(maquina);
        await context.SaveChangesAsync();

        // Act
        var maquinaObtenida = await maquinaRepositorioMock.ObtenerMaquinaPorId(maquina.Id);

        // FluentAssertions
        maquinaObtenida.Should().NotBeNull();
        maquinaObtenida.Should().BeEquivalentTo(maquina, options => options.ExcludingMissingMembers());
    }

    //obtener maquina por id inexistente deberia retornar null
    [Fact]
    public async Task ObtenerMaquinaPorId_Inexistente_DeberiaRetornarNull()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerMaquinaPorIdInexistenteDb");
        using var context = new FitRankDbContext(options);
        var maquinaRepositorioMock = new MaquinaRepositorioImpl(context);

        // Act
        var maquinaObtenida = await maquinaRepositorioMock.ObtenerMaquinaPorId(999);

        // FluentAssertions
        maquinaObtenida.Should().BeNull();
    }

    //actualizar maquina deberia actualizar correctamente
    [Fact]
    public async Task ActualizarMaquina_DeberiaActualizarCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("ActualizarMaquinaDb");
        using var context = new FitRankDbContext(options);
        var maquinaRepositorioMock = new MaquinaRepositorioImpl(context);
        var gimnasio = await SeedData(context);

        var maquina = new Maquina
        {
            Nombre = "Máquina de Prueba",
            UrlImagen = "http://imagen.maquina/prueba.jpg",
            GimnasioId = gimnasio.Id,
            Qr = "http://imagen.maquina/prueba.jpg"
        };

        context.Maquinas.Add(maquina);
        await context.SaveChangesAsync();

        // Act
        maquina.Nombre = "Máquina Actualizada";
        var maquinaActualizada = await maquinaRepositorioMock.ActualizarMaquina(maquina);

        // FluentAssertions
        maquinaActualizada.Should().NotBeNull();
        maquinaActualizada!.Nombre.Should().Be("Máquina Actualizada");
    }

    //actualizar maquina inexistente deberia retornar null
    [Fact]
    public async Task ActualizarMaquina_Inexistente_DeberiaRetornarNull()
    {
        // Arrange
        var options = CreateInMemoryOptions("ActualizarMaquinaInexistenteDb");
        using var context = new FitRankDbContext(options);
        var maquinaRepositorioMock = new MaquinaRepositorioImpl(context);

        var maquina = new Maquina
        {
            Id = 999,
            Nombre = "Máquina Inexistente",
            UrlImagen = "http://imagen.maquina/inexistente.jpg",
            GimnasioId = 1,
            Qr = "http://imagen.maquina/inexistente.jpg"
        };

        // Act
        var maquinaActualizada = await maquinaRepositorioMock.ActualizarMaquina(maquina);

        // FluentAssertions
        maquinaActualizada.Should().BeNull();
    }

    //eliminar maquina deberia eliminar correctamente
    [Fact]
    public async Task EliminarMaquina_DeberiaEliminarCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("EliminarMaquinaDb");
        using var context = new FitRankDbContext(options);
        var maquinaRepositorioMock = new MaquinaRepositorioImpl(context);
        var gimnasio = await SeedData(context);

        var maquina = new Maquina
        {
            Nombre = "Máquina de Prueba",
            UrlImagen = "http://imagen.maquina/prueba.jpg",
            GimnasioId = gimnasio.Id,
            Qr = "http://imagen.maquina/prueba.jpg"
        };

        context.Maquinas.Add(maquina);
        await context.SaveChangesAsync();

        // Act
        var resultado = await maquinaRepositorioMock.EliminarMaquina(maquina.Id);
        var maquinaEliminada = await maquinaRepositorioMock.ObtenerMaquinaPorId(maquina.Id);

        // FluentAssertions
        resultado.Should().BeTrue();
        maquinaEliminada.Should().BeNull();
    }

    //eliminar maquina inexistente deberia retornar false
    [Fact]
    public async Task EliminarMaquina_Inexistente_DeberiaRetornarFalse()
    {
        // Arrange
        var options = CreateInMemoryOptions("EliminarMaquinaInexistenteDb");
        using var context = new FitRankDbContext(options);
        var maquinaRepositorioMock = new MaquinaRepositorioImpl(context);
        // Act
        var resultado = await maquinaRepositorioMock.EliminarMaquina(999);
        // FluentAssertions
        resultado.Should().BeFalse();
    }
}
