using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Persistence;
using FitRank_API.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FitRank_API.tests.InfrastructureTests;

public class GrupoMuscularRepositorioTest
{
    private DbContextOptions<FitRankDbContext> CreateInMemoryOptions(string dbName)
    {
        return new DbContextOptionsBuilder<FitRankDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
    }

    [Fact]
    public async Task AgregarUsuario_DeberíaPersistirUsuario()
    {
        // Arrange
        var options = CreateInMemoryOptions("AgregarUsuarioTestDb");
        using var context = new FitRankDbContext(options);
        var repositorio = new GrupoMuscularRepositorioImpl(context);

        var nuevoGrupoMuscular = new GrupoMuscular
        {
            Nombre = "Pecho",
            Imagen = "pecho.png"
        };

        // Act
        var resultado = await repositorio.AgregarAsync(nuevoGrupoMuscular);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Id.Should().BeGreaterThan(0);
        resultado.Nombre.Should().Be("Pecho");
        resultado.Imagen.Should().Be("pecho.png");
    }

    [Fact]
    public async Task ObtenerPorId_DeberíaRetornarGrupoMuscularExistente()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerPorIdTestDb");
        using var context = new FitRankDbContext(options);
        var repositorio = new GrupoMuscularRepositorioImpl(context);
        var grupoMuscular = new GrupoMuscular
        {
            Nombre = "Espalda",
            Imagen = "espalda.png"
        };
        context.GruposMusculares.Add(grupoMuscular);
        await context.SaveChangesAsync();
        // Act
        var resultado = await repositorio.ObtenerPorIdAsync(grupoMuscular.Id);
        // Assert
        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(grupoMuscular.Id);
        resultado.Nombre.Should().Be("Espalda");
        resultado.Imagen.Should().Be("espalda.png");
    }

    [Fact]
    public async Task Eliminar_DeberíaRemoverGrupoMuscularExistente()
    {
        // Arrange
        var options = CreateInMemoryOptions("EliminarTestDb");
        using var context = new FitRankDbContext(options);
        var repositorio = new GrupoMuscularRepositorioImpl(context);
        var grupoMuscular = new GrupoMuscular
        {
            Nombre = "Piernas",
            Imagen = "piernas.png"
        };
        context.GruposMusculares.Add(grupoMuscular);
        await context.SaveChangesAsync();
        // Act
        var resultado = await repositorio.EliminarAsync(grupoMuscular.Id);
        var grupoEliminado = await repositorio.ObtenerPorIdAsync(grupoMuscular.Id);
        // Assert
        resultado.Should().BeTrue();
        grupoEliminado.Should().BeNull();
    }

    [Fact]
    public async Task Actualizar_DeberíaModificarGrupoMuscularExistente()
    {
        // Arrange
        var options = CreateInMemoryOptions("ActualizarTestDb");
        using var context = new FitRankDbContext(options);
        var repositorio = new GrupoMuscularRepositorioImpl(context);
        var grupoMuscular = new GrupoMuscular
        {
            Nombre = "Hombros",
            Imagen = "hombros.png"
        };
        context.GruposMusculares.Add(grupoMuscular);
        await context.SaveChangesAsync();
        // Act
        grupoMuscular.Nombre = "Hombros Modificado";
        grupoMuscular.Imagen = "hombros_modificado.png";
        var resultado = await repositorio.ActualizarAsync(grupoMuscular);
        // Assert
        resultado.Should().NotBeNull();
        resultado!.Nombre.Should().Be("Hombros Modificado");
        resultado.Imagen.Should().Be("hombros_modificado.png");
    }

    [Fact]
    public async Task ObtenerTodos_DeberíaRetornarTodosLosGruposMusculares()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerTodosTestDb");
        using var context = new FitRankDbContext(options);
        var repositorio = new GrupoMuscularRepositorioImpl(context);
        context.GruposMusculares.AddRange(
            new GrupoMuscular { Nombre = "Brazos", Imagen = "brazos.png" },
            new GrupoMuscular { Nombre = "Abdomen", Imagen = "abdomen.png" }
        );
        await context.SaveChangesAsync();
        // Act
        var resultado = await repositorio.ObtenerTodosAsync();
        // Assert
        resultado.Should().HaveCount(2);
        resultado.Should().Contain(g => g.Nombre == "Brazos");
        resultado.Should().Contain(g => g.Nombre == "Abdomen");
    }

    [Fact]
    public async Task Eliminar_NoDeberíaRemoverGrupoMuscularInexistente()
    {
        // Arrange
        var options = CreateInMemoryOptions("EliminarInexistenteTestDb");
        using var context = new FitRankDbContext(options);
        var repositorio = new GrupoMuscularRepositorioImpl(context);
        // Act
        var resultado = await repositorio.EliminarAsync(999); // ID inexistente
        // Assert
        resultado.Should().BeFalse();
    }

    [Fact]
    public async Task Actualizar_NoDeberíaModificarGrupoMuscularInexistente()
    {
        // Arrange
        var options = CreateInMemoryOptions("ActualizarInexistenteTestDb");
        using var context = new FitRankDbContext(options);
        var repositorio = new GrupoMuscularRepositorioImpl(context);
        var grupoMuscularInexistente = new GrupoMuscular
        {
            Id = 999, // ID inexistente
            Nombre = "Inexistente",
            Imagen = "inexistente.png"
        };
        // Act
        var resultado = await repositorio.ActualizarAsync(grupoMuscularInexistente);
        // Assert
        resultado.Should().BeNull();
    }

    [Fact]
    public async Task ObtenerPorId_NoDeberíaRetornarGrupoMuscularInexistente()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerPorIdInexistenteTestDb");
        using var context = new FitRankDbContext(options);
        var repositorio = new GrupoMuscularRepositorioImpl(context);
        // Act
        var resultado = await repositorio.ObtenerPorIdAsync(999); // ID inexistente
        // Assert
        resultado.Should().BeNull();
    }
}
