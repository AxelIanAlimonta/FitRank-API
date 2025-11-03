using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Persistence;
using FitRank_API.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FitRank_API.tests.InfrastructureTests;

public class EjercicioAsignadoRepositorioTests
{
    private DbContextOptions<FitRankDbContext> CreateInMemoryOptions(string dbName)
    {
        return new DbContextOptionsBuilder<FitRankDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
    }

    private async Task<(Sesion, Ejercicio)> SeedData(FitRankDbContext context)
    {

        var Maquina = new Maquina
        {
            Nombre = "Máquina de Prueba",
            GimnasioId = 1,
            UrlImagen = "http://imagen.com/maquina.jpg",
            Qr = "QR12345"
        };
        context.Maquinas.Add(Maquina);
        await context.SaveChangesAsync();

        var GrupoMuscular = new GrupoMuscular
        {
            Nombre = "Grupo Muscular de Prueba"
        };
        context.GruposMusculares.Add(GrupoMuscular);
        await context.SaveChangesAsync();

        var sesion = new Sesion
        {
            NumeroDeSesion = 1,
            Nombre = "Sesión de Prueba",
            RutinaId = 1
        };

        context.Sesiones.Add(sesion);
        await context.SaveChangesAsync();

        var ejercicio = new Ejercicio
        {
            Nombre = "Ejercicio de Prueba",
            Descripcion = "Descripción del ejercicio de prueba",
            MaquinaId = Maquina.Id,
            GrupoMuscularId = GrupoMuscular.Id
        };

        context.Ejercicios.Add(ejercicio);
        await context.SaveChangesAsync();

        return (sesion, ejercicio);
    }

    [Fact]
    public async Task AgregarEjercicioAsignado_DeberiaGuardarCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("AgregarEjercicioAsignadoDb");
        using var context = new FitRankDbContext(options);
        var ejercicioAsignadoRepositorio = new EjercicioAsignadoRepositorioImpl(context);
        var (sesion, ejercicio) = await SeedData(context);

        var ejercicioAsignado = new EjercicioAsignado
        {
            NumeroEjercicio = 1,
            EjercicioId = ejercicio.Id,
            SesionId = sesion.Id
        };

        // Act
        await ejercicioAsignadoRepositorio.AgregarAsync(ejercicioAsignado);

        // Assert con FluentAssertions
        var ejercicioAsignadoGuardado = await context.EjerciciosAsignados
            .Include(ea => ea.Ejercicio)
            .Include(ea => ea.Sesion)
            .FirstOrDefaultAsync(ea => ea.Id == ejercicioAsignado.Id);


        ejercicioAsignadoGuardado.Should().NotBeNull();
        ejercicioAsignadoGuardado!.NumeroEjercicio.Should().Be(1);
        ejercicioAsignadoGuardado.EjercicioId.Should().Be(ejercicio.Id);
        ejercicioAsignadoGuardado.SesionId.Should().Be(sesion.Id);
    }

    //obtener lista de ejercicios asignados
    [Fact]
    public async Task ObtenerEjerciciosAsignados_DeberiaRetornarListaCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerEjerciciosAsignadosDb");
        using var context = new FitRankDbContext(options);
        var (sesion, ejercicio) = await SeedData(context);

        context.EjerciciosAsignados.AddRange(
            new EjercicioAsignado { NumeroEjercicio = 1, EjercicioId = ejercicio.Id, SesionId = sesion.Id },
            new EjercicioAsignado { NumeroEjercicio = 2, EjercicioId = ejercicio.Id, SesionId = sesion.Id }
        );
        await context.SaveChangesAsync();

        var ejercicioAsignadoRepositorio = new EjercicioAsignadoRepositorioImpl(context);

        // Act
        var ejerciciosAsignados = await ejercicioAsignadoRepositorio.ObtenerTodosAsync();

        // Assert
        ejerciciosAsignados.Should().HaveCount(2);
        ejerciciosAsignados[0].Ejercicio.Should().NotBeNull();
        ejerciciosAsignados[0].Sesion.Should().NotBeNull();
    }

    //obtener lista de ejercicios asignados vacia
    [Fact]
    public async Task ObtenerEjerciciosAsignados_Vacia_DeberiaRetornarListaVacia()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerEjerciciosAsignadosVaciaDb");
        using var context = new FitRankDbContext(options);
        var ejercicioAsignadoRepositorio = new EjercicioAsignadoRepositorioImpl(context);

        // Act
        var ejerciciosAsignados = await ejercicioAsignadoRepositorio.ObtenerTodosAsync();

        // Assert
        ejerciciosAsignados.Should().BeEmpty();
    }

    //obtener ejercicio asignado por id
    [Fact]
    public async Task ObtenerEjercicioAsignadoPorId_DeberiaRetornarCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerEjercicioAsignadoPorIdDb");
        using var context = new FitRankDbContext(options);
        var (sesion, ejercicio) = await SeedData(context);

        var ejercicioAsignado = new EjercicioAsignado
        {
            NumeroEjercicio = 1,
            EjercicioId = ejercicio.Id,
            SesionId = sesion.Id
        };
        context.EjerciciosAsignados.Add(ejercicioAsignado);
        await context.SaveChangesAsync();

        var ejercicioAsignadoRepositorio = new EjercicioAsignadoRepositorioImpl(context);

        // Act
        var ejercicioAsignadoObtenido = await ejercicioAsignadoRepositorio.ObtenerPorIdAsync(ejercicioAsignado.Id);

        // Assert
        ejercicioAsignadoObtenido.Should().NotBeNull();
        ejercicioAsignadoObtenido!.NumeroEjercicio.Should().Be(1);
        ejercicioAsignadoObtenido.EjercicioId.Should().Be(ejercicio.Id);
        ejercicioAsignadoObtenido.SesionId.Should().Be(sesion.Id);
    }

    //obtener ejercicio asignado por id inexistente
    [Fact]
    public async Task ObtenerEjercicioAsignadoPorId_Inexistente_DeberiaRetornarNull()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerEjercicioAsignadoPorIdInexistenteDb");
        using var context = new FitRankDbContext(options);
        var ejercicioAsignadoRepositorio = new EjercicioAsignadoRepositorioImpl(context);

        // Act
        var ejercicioAsignadoObtenido = await ejercicioAsignadoRepositorio.ObtenerPorIdAsync(999);

        // Assert
        ejercicioAsignadoObtenido.Should().BeNull();
    }

    //actualizar ejercicio asignado
    [Fact]
    public async Task ActualizarEjercicioAsignado_DeberiaActualizarCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("ActualizarEjercicioAsignadoDb");
        using var context = new FitRankDbContext(options);
        var (sesion, ejercicio) = await SeedData(context);

        var ejercicioAsignado = new EjercicioAsignado
        {
            NumeroEjercicio = 1,
            EjercicioId = ejercicio.Id,
            SesionId = sesion.Id
        };
        context.EjerciciosAsignados.Add(ejercicioAsignado);
        await context.SaveChangesAsync();

        var ejercicioAsignadoRepositorio = new EjercicioAsignadoRepositorioImpl(context);

        // Act
        ejercicioAsignado.NumeroEjercicio = 2;
        var ejercicioAsignadoActualizado = await ejercicioAsignadoRepositorio.ActualizarAsync(ejercicioAsignado);

        // Assert
        ejercicioAsignadoActualizado.Should().NotBeNull();
        ejercicioAsignadoActualizado!.NumeroEjercicio.Should().Be(2);
    }

    //actualizar ejercicio asignado inexistente
    [Fact]
    public async Task ActualizarEjercicioAsignado_Inexistente_DeberiaRetornarNull()
    {
        // Arrange
        var options = CreateInMemoryOptions("ActualizarEjercicioAsignadoInexistenteDb");
        using var context = new FitRankDbContext(options);
        var ejercicioAsignadoRepositorio = new EjercicioAsignadoRepositorioImpl(context);

        var ejercicioAsignado = new EjercicioAsignado
        {
            Id = 999,
            NumeroEjercicio = 1,
            EjercicioId = 1,
            SesionId = 1
        };

        // Act
        var ejercicioAsignadoActualizado = await ejercicioAsignadoRepositorio.ActualizarAsync(ejercicioAsignado);

        // Assert
        ejercicioAsignadoActualizado.Should().BeNull();
    }

    //eliminar ejercicio asignado
    [Fact]
    public async Task EliminarEjercicioAsignado_DeberiaEliminarCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("EliminarEjercicioAsignadoDb");
        using var context = new FitRankDbContext(options);
        var (sesion, ejercicio) = await SeedData(context);

        var ejercicioAsignado = new EjercicioAsignado
        {
            NumeroEjercicio = 1,
            EjercicioId = ejercicio.Id,
            SesionId = sesion.Id
        };
        context.EjerciciosAsignados.Add(ejercicioAsignado);
        await context.SaveChangesAsync();

        var ejercicioAsignadoRepositorio = new EjercicioAsignadoRepositorioImpl(context);

        // Act
        var resultado = await ejercicioAsignadoRepositorio.EliminarAsync(ejercicioAsignado.Id);

        // Assert
        resultado.Should().BeTrue();
        var ejercicioAsignadoEliminado = await context.EjerciciosAsignados.FindAsync(ejercicioAsignado.Id);
        ejercicioAsignadoEliminado.Should().BeNull();
    }

}