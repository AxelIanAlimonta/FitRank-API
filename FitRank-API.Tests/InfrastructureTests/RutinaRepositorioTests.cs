using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Persistence;
using FitRank_API.Infrastructure.Repositories;
using FitRank_API.Infrastructure.Repositorios;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FitRank_API.tests.InfrastructureTests;

public class RutinaTests
{



    private DbContextOptions<FitRankDbContext> CreateInMemoryOptions(string dbName)
    {
        return new DbContextOptionsBuilder<FitRankDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
    }



    private async Task<(Usuario, Socio)> SeedData(FitRankDbContext context)
    {
        Usuario _usuarioMock = new Usuario
        {
            Nombre = "Test",
            Apellido = "Usuario",
            Email = "test.usuario@example.com",
        };
        Socio _socioMock = new Socio
        {
            Nombre = "Test Socio",
            Apellido = "Usuario",
            Email = "test.socio@example.com",
        };
        context.Usuarios.Add(_usuarioMock);
        context.Socios.Add(_socioMock);
        await context.SaveChangesAsync();

        return (_usuarioMock, _socioMock);
    }

    [Fact]
    public async Task AgregarRutina_DeberíaPersistirRutina()
    {
        // Arrange
        var options = CreateInMemoryOptions("AgregarRutinaTestDb");
        using var context = new FitRankDbContext(options);
        var rutinaRepositorio = new RutinaRepositorioImpl(context);
        var usuarioRepositorio = new UsuarioRepositorioImpl(context);
        var socioRepositorio = new SocioRepositorioImpl(context);

        var nuevaRutina = new Rutina
        {
            Nombre = "Rutina de Fuerza",
            TipoCreacion = "Personalizada",
            Descripcion = "Una rutina enfocada en el desarrollo de la fuerza muscular.",
            Activa = true,
            FechaCreacion = new DateTime(2024, 1, 1),
            SocioId = 1,
            UsuarioId = 1
        };

        // Act
        var resultado = await rutinaRepositorio.AgregarAsync(nuevaRutina);


        // Assert
        resultado.Should().NotBeNull();
        resultado!.Id.Should().BeGreaterThan(0);
        resultado.Nombre.Should().Be("Rutina de Fuerza");
        resultado.Descripcion.Should().Be("Una rutina enfocada en el desarrollo de la fuerza muscular.");
        resultado.Activa.Should().BeTrue();
        resultado.FechaCreacion.Should().Be(new DateTime(2024, 1, 1));
        resultado.SocioId.Should().Be(1);
        resultado.UsuarioId.Should().Be(1);
        resultado.TipoCreacion.Should().Be("Personalizada");
    }

    [Fact]
    public async Task ObtenerPorId_DeberíaRetornarRutinaExistente()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerRutinaPorIdTestDb");
        using var context = new FitRankDbContext(options);
        var repositorio = new RutinaRepositorioImpl(context);
        var (_usuarioMock, _socioMock) = await SeedData(context);

        var rutina = new Rutina
        {
            Nombre = "Rutina de Cardio",
            TipoCreacion = "Estandar",
            Descripcion = "Una rutina para mejorar la resistencia cardiovascular.",
            Activa = true,
            FechaCreacion = new DateTime(2024, 2, 1),
            SocioId = _socioMock.Id,
            UsuarioId = _usuarioMock.Id
        };
        context.Rutinas.Add(rutina);

        await context.SaveChangesAsync();
        // Act
        var resultado = await repositorio.ObtenerPorIdAsync(rutina.Id);
        // Assert
        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(rutina.Id);
        resultado.Nombre.Should().Be("Rutina de Cardio");
        resultado.Descripcion.Should().Be("Una rutina para mejorar la resistencia cardiovascular.");
        resultado.Activa.Should().BeTrue();
        resultado.FechaCreacion.Should().Be(new DateTime(2024, 2, 1));
        resultado.SocioId.Should().Be(_socioMock.Id);
        resultado.UsuarioId.Should().Be(_usuarioMock.Id);
        resultado.TipoCreacion.Should().Be("Estandar");

    }

    [Fact]
    public async Task ObtenerPorId_DeberíaRetornarNullSiNoExiste()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerRutinaPorIdNoExistenteTestDb");
        using var context = new FitRankDbContext(options);
        var repositorio = new RutinaRepositorioImpl(context);
        // Act
        var resultado = await repositorio.ObtenerPorIdAsync(999);
        // Assert
        resultado.Should().BeNull();
    }

    [Fact]
    public async Task EliminarRutina_DeberíaRemoverRutinaExistente()
    {
        // Arrange
        var options = CreateInMemoryOptions("EliminarRutinaTestDb");
        using var context = new FitRankDbContext(options);
        var repositorio = new RutinaRepositorioImpl(context);
        var rutina = new Rutina
        {
            Nombre = "Rutina de Flexibilidad",
            TipoCreacion = "Estandar",
            Descripcion = "Una rutina para mejorar la flexibilidad muscular.",
            Activa = true,
            FechaCreacion = new DateTime(2024, 3, 1),
            SocioId = 3,
            UsuarioId = 3
        };
        context.Rutinas.Add(rutina);
        await context.SaveChangesAsync();
        // Act
        var resultado = await repositorio.EliminarAsync(rutina.Id);
        var rutinaEliminada = await repositorio.ObtenerPorIdAsync(rutina.Id);
        // Assert
        resultado.Should().BeTrue();
        rutinaEliminada.Should().BeNull();
    }

    //eliminar rutina que no existe
    [Fact]
    public async Task EliminarRutina_DeberíaRetornarFalseSiNoExiste()
    {
        // Arrange
        var options = CreateInMemoryOptions("EliminarRutinaNoExistenteTestDb");
        using var context = new FitRankDbContext(options);
        var repositorio = new RutinaRepositorioImpl(context);
        // Act
        var resultado = await repositorio.EliminarAsync(999);
        // Assert
        resultado.Should().BeFalse();
    }

    [Fact]
    public async Task ActualizarRutina_DeberíaModificarRutinaExistente()
    {
        // Arrange
        var options = CreateInMemoryOptions("ActualizarRutinaTestDb");
        using var context = new FitRankDbContext(options);
        var repositorio = new RutinaRepositorioImpl(context);
        var rutina = new Rutina
        {
            Nombre = "Rutina Inicial",
            TipoCreacion = "Estandar",
            Descripcion = "Descripción inicial.",
            Activa = true,
            FechaCreacion = new DateTime(2024, 4, 1),
            SocioId = 4,
            UsuarioId = 4
        };
        context.Rutinas.Add(rutina);
        await context.SaveChangesAsync();
        // Act
        rutina.Nombre = "Rutina Actualizada";
        rutina.Descripcion = "Descripción actualizada.";
        rutina.Activa = false;
        var resultado = await repositorio.ActualizarAsync(rutina);
        // Assert
        resultado.Should().NotBeNull();
        resultado!.Nombre.Should().Be("Rutina Actualizada");
        resultado.Descripcion.Should().Be("Descripción actualizada.");
        resultado.Activa.Should().BeFalse();
    }

    //Actualizar rutina que no existe
    [Fact]
    public async Task ActualizarRutina_DeberíaRetornarNullSiNoExiste()
    {
        // Arrange
        var options = CreateInMemoryOptions("ActualizarRutinaNoExistenteTestDb");
        using var context = new FitRankDbContext(options);
        var repositorio = new RutinaRepositorioImpl(context);
        var rutina = new Rutina
        {
            Id = 999,
            Nombre = "Rutina Inexistente",
            TipoCreacion = "Estandar",
            Descripcion = "Descripción inexistente.",
            Activa = true,
            FechaCreacion = new DateTime(2024, 4, 1),
            SocioId = 4,
            UsuarioId = 4
        };
        // Act
        var resultado = await repositorio.ActualizarAsync(rutina);
        // Assert
        resultado.Should().BeNull();
    }

    [Fact]
    public async Task ObtenerTodasRutinas_DeberíaRetornarTodasLasRutinas()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerTodasRutinasTestDb");
        using var context = new FitRankDbContext(options);
        var repositorio = new RutinaRepositorioImpl(context);

        var (_usuarioMock, _socioMock) = await SeedData(context);

        var rutina1 = new Rutina
        {
            Nombre = "Rutina 1",
            TipoCreacion = "Estandar",
            Descripcion = "Descripción de la rutina 1.",
            Activa = true,
            FechaCreacion = new DateTime(2024, 5, 1),
            SocioId = _socioMock.Id,
            UsuarioId = _usuarioMock.Id
        };
        var rutina2 = new Rutina
        {
            Nombre = "Rutina 2",
            TipoCreacion = "Personalizada",
            Descripcion = "Descripción de la rutina 2.",
            Activa = false,
            FechaCreacion = new DateTime(2024, 6, 1),
            SocioId = _socioMock.Id,
            UsuarioId = _usuarioMock.Id
        };
        context.Rutinas.AddRange(rutina1, rutina2);
        await context.SaveChangesAsync();
        // Act
        var resultado = await repositorio.ObtenerTodasAsync();
        // Assert
        resultado.Should().HaveCount(2);
        resultado.Should().Contain(r => r.Nombre == "Rutina 1");
        resultado.Should().Contain(r => r.Nombre == "Rutina 2");

    }

    //obtener todas las rutinas cuando no hay ninguna
    [Fact]
    public async Task ObtenerTodasRutinas_DeberíaRetornarListaVaciaSiNoHayRutinas()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerTodasRutinasVaciaTestDb");
        using var context = new FitRankDbContext(options);
        var repositorio = new RutinaRepositorioImpl(context);
        // Act
        var resultado = await repositorio.ObtenerTodasAsync();
        // Assert
        resultado.Should().BeEmpty();
    }


}