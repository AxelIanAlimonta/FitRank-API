using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Persistence;
using FitRank_API.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FitRank_API.tests.InfrastructureTests;

public class EntrenamientoRepositorioTests
{
    private DbContextOptions<FitRankDbContext> CreateInMemoryOptions(string dbName)
    {
        return new DbContextOptionsBuilder<FitRankDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
    }

    private async Task<Socio> SeedData(FitRankDbContext context)
    {
        var socio = new Socio
        {
            Nombre = "John Doe",
            Email = "john.doe@example.com",
            Nivel = "Intermedio"
        };
        context.Socios.Add(socio);
        await context.SaveChangesAsync();
        return socio;
    }


    //agregar entrenamiento deberia guardar correctamente
    [Fact]
    public async Task AgregarEntrenamiento_DebeGuardarCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("AgregarEntrenamiento_DebeGuardarCorrectamente");
        using var context = new FitRankDbContext(options);
        var repositorio = new EntrenamientoRepositorioImpl(context);
        var socio = await SeedData(context);

        var entrenamiento = new Entrenamiento
        {
            SocioId = socio.Id,
            Fecha = DateTime.UtcNow,
            Duracion = TimeSpan.FromHours(1),
        };

        // Act
        await repositorio.AgregarAsync(entrenamiento);
        //traer entrenamiento agregado
        var entrenamientos = await context.Entrenamientos
            .Where(e => e.SocioId == socio.Id)
            .ToListAsync();

        // Assert con fluent assertions
        entrenamientos.Should().HaveCount(1);
        entrenamientos.First().Fecha.Should().Be(entrenamiento.Fecha);
        entrenamientos.First().Duracion.Should().Be(entrenamiento.Duracion);
    }

    // obtener lista de entrenamientos deberia retornar lista correctamente
    [Fact]
    public async Task ObtenerEntrenamientos_DebeRetornarListaCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerEntrenamientos_DebeRetornarListaCorrectamente");
        using var context = new FitRankDbContext(options);
        var repositorio = new EntrenamientoRepositorioImpl(context);
        var socio = await SeedData(context);

        var entrenamiento1 = new Entrenamiento
        {
            SocioId = socio.Id,
            Fecha = DateTime.UtcNow,
            Duracion = TimeSpan.FromHours(1),
        };

        var entrenamiento2 = new Entrenamiento
        {
            SocioId = socio.Id,
            Fecha = DateTime.UtcNow.AddDays(-1),
            Duracion = TimeSpan.FromHours(1),
        };

        context.Entrenamientos.AddRange(entrenamiento1, entrenamiento2);
        await context.SaveChangesAsync();

        // Act
        var entrenamientos = await repositorio.ObtenerTodosAsync();

        // Assert con fluent assertions
        entrenamientos.Should().HaveCount(2);
        entrenamientos.Should().Contain(e => e.Id == entrenamiento1.Id);
        entrenamientos.Should().Contain(e => e.Id == entrenamiento2.Id);
    }

    //obtener lista de entrenamientos vacia
    [Fact]
    public async Task ObtenerEntrenamientos_SinEntrenamientos_DebeRetornarListaVacia()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerEntrenamientos_SinEntrenamientos_DebeRetornarListaVacia");
        using var context = new FitRankDbContext(options);
        var repositorio = new EntrenamientoRepositorioImpl(context);
        var socio = await SeedData(context);

        // Act
        var entrenamientos = await repositorio.ObtenerPorSocioAsync(socio.Id);

        // Assert con fluent assertions
        entrenamientos.Should().BeEmpty();
    }

    //obtener entrenamiento por id
    [Fact]
    public async Task ObtenerEntrenamientoPorId_DebeRetornarEntrenamientoCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerEntrenamientoPorId_DebeRetornarEntrenamientoCorrectamente");
        using var context = new FitRankDbContext(options);
        var repositorio = new EntrenamientoRepositorioImpl(context);
        var socio = await SeedData(context);

        var entrenamiento = new Entrenamiento
        {
            SocioId = socio.Id,
            Fecha = DateTime.UtcNow,
            Duracion = TimeSpan.FromHours(1),
        };

        context.Entrenamientos.Add(entrenamiento);
        await context.SaveChangesAsync();

        // Act
        var entrenamientoObtenido = await repositorio.ObtenerPorIdAsync(entrenamiento.Id);

        // Assert con fluent assertions
        entrenamientoObtenido.Should().NotBeNull();
        entrenamientoObtenido!.Id.Should().Be(entrenamiento.Id);
        entrenamientoObtenido.SocioId.Should().Be(socio.Id);
        entrenamientoObtenido.Fecha.Should().Be(entrenamiento.Fecha);
        entrenamientoObtenido.Duracion.Should().Be(entrenamiento.Duracion);
    }

    //obtener entrenamiento por id inexistente
    [Fact]
    public async Task ObtenerEntrenamientoPorId_Inexistente_DebeRetornarNull()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerEntrenamientoPorId_Inexistente_DebeRetornarNull");
        using var context = new FitRankDbContext(options);
        var repositorio = new EntrenamientoRepositorioImpl(context);

        // Act
        var entrenamientoObtenido = await repositorio.ObtenerPorIdAsync(999);

        // Assert con fluent assertions
        entrenamientoObtenido.Should().BeNull();
    }

    //actualizar entrenamiento deberia actualizar correctamente
    [Fact]
    public async Task ActualizarEntrenamiento_DebeActualizarCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("ActualizarEntrenamiento_DebeActualizarCorrectamente");
        using var context = new FitRankDbContext(options);
        var repositorio = new EntrenamientoRepositorioImpl(context);
        var socio = await SeedData(context);

        var entrenamiento = new Entrenamiento
        {
            SocioId = socio.Id,
            Fecha = DateTime.UtcNow,
            Duracion = TimeSpan.FromHours(1),
        };

        context.Entrenamientos.Add(entrenamiento);
        await context.SaveChangesAsync();

        // Modificar datos del entrenamiento
        entrenamiento.Fecha = DateTime.UtcNow.AddDays(1);
        entrenamiento.Duracion = TimeSpan.FromHours(2);

        // Act
        await repositorio.ActualizarAsync(entrenamiento);
        var entrenamientoActualizado = await repositorio.ObtenerPorIdAsync(entrenamiento.Id);

        // Assert con fluent assertions
        entrenamientoActualizado.Should().NotBeNull();
        entrenamientoActualizado!.Fecha.Should().Be(entrenamiento.Fecha);
        entrenamientoActualizado.Duracion.Should().Be(entrenamiento.Duracion);
    }

    //actualizar entrenamiento no existente deberia retornar null
    [Fact]
    public async Task ActualizarEntrenamiento_NoExistente_DebeRetornarNull()
    {
        // Arrange
        var options = CreateInMemoryOptions("ActualizarEntrenamiento_NoExistente_DebeRetornarNull");
        using var context = new FitRankDbContext(options);
        var repositorio = new EntrenamientoRepositorioImpl(context);

        var entrenamiento = new Entrenamiento
        {
            Id = 999,
            SocioId = 1,
            Fecha = DateTime.UtcNow,
            Duracion = TimeSpan.FromHours(1),
        };

        // Act
        var resultado = await repositorio.ActualizarAsync(entrenamiento);

        // Assert con fluent assertions
        resultado.Should().BeNull();
    }

    //eliminar entrenamiento deberia eliminar correctamente
    [Fact]
    public async Task EliminarEntrenamiento_DebeEliminarYDevolverTrue()
    {
        // Arrange
        var options = CreateInMemoryOptions("EliminarEntrenamiento_DebeEliminarYDevolverTrue");
        using var context = new FitRankDbContext(options);
        var repositorio = new EntrenamientoRepositorioImpl(context);
        var socio = await SeedData(context);

        var entrenamiento = new Entrenamiento
        {
            SocioId = socio.Id,
            Fecha = DateTime.UtcNow,
            Duracion = TimeSpan.FromHours(1),
        };

        context.Entrenamientos.Add(entrenamiento);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repositorio.EliminarAsync(entrenamiento.Id);
        var entrenamientoEliminado = await repositorio.ObtenerPorIdAsync(entrenamiento.Id);

        // Assert con fluent assertions
        resultado.Should().BeTrue();
        entrenamientoEliminado.Should().BeNull();

    }

    //eliminar entrenamiento no existente deberia devolver false
    [Fact]
    public async Task EliminarEntrenamiento_NoExistente_DebeDevolverFalse()
    {
        // Arrange
        var options = CreateInMemoryOptions("EliminarEntrenamiento_NoExistente_DebeDevolverFalse");
        using var context = new FitRankDbContext(options);
        var repositorio = new EntrenamientoRepositorioImpl(context);

        // Act
        var resultado = await repositorio.EliminarAsync(999);

        // Assert con fluent assertions
        resultado.Should().BeFalse();
    }


    //obtener socio por id deberia retornar socio correctamente
    [Fact]
    public async Task ObtenerSocioPorId_DebeRetornarSocioCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerSocioPorId_DebeRetornarSocioCorrectamente");
        using var context = new FitRankDbContext(options);
        var repositorio = new EntrenamientoRepositorioImpl(context);
        var socio = await SeedData(context);
        // Act
        var socioObtenido = await repositorio.ObtenerSocioPorIdAsync(socio.Id);
        // Assert con fluent assertions
        socioObtenido.Should().NotBeNull();
        socioObtenido!.Id.Should().Be(socio.Id);
        socioObtenido.Nombre.Should().Be(socio.Nombre);
        socioObtenido.Email.Should().Be(socio.Email);
    }

    //obtener entrenamiento activo por socio id deberia retornar entrenamiento correctamente
    [Fact]
    public async Task ObtenerEntrenamientoActivoPorSocioId_DebeRetornarEntrenamientoCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerEntrenamientoActivoPorSocioId_DebeRetornarEntrenamientoCorrectamente");
        using var context = new FitRankDbContext(options);
        var repositorio = new EntrenamientoRepositorioImpl(context);
        var socio = await SeedData(context);
        var entrenamiento = new Entrenamiento
        {
            SocioId = socio.Id,
            Fecha = DateTime.UtcNow,
            Duracion = TimeSpan.FromHours(1),
        };
        context.Entrenamientos.Add(entrenamiento);
        await context.SaveChangesAsync();
        // Act
        var entrenamientoActivo = await repositorio.ObtenerEntrenamientoActivoPorSocioIdAsync(socio.Id);
        // Assert con fluent assertions
        entrenamientoActivo.Should().NotBeNull();
        entrenamientoActivo!.Id.Should().Be(entrenamiento.Id);
    }

    //obtener historial completo de entrenamientos por socio id deberia retornar lista correctamente
    [Fact]
    public async Task ObtenerHistorialCompletoPorSocioId_DebeRetornarListaCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerHistorialCompletoPorSocioId_DebeRetornarListaCorrectamente");
        using var context = new FitRankDbContext(options);
        var repositorio = new EntrenamientoRepositorioImpl(context);
        var socio = await SeedData(context);
        var entrenamiento1 = new Entrenamiento
        {
            SocioId = socio.Id,
            Fecha = DateTime.UtcNow,
            Duracion = TimeSpan.FromHours(1),
        };
        var entrenamiento2 = new Entrenamiento
        {
            SocioId = socio.Id,
            Fecha = DateTime.UtcNow.AddDays(-1),
            Duracion = TimeSpan.FromHours(1),
        };
        context.Entrenamientos.AddRange(entrenamiento1, entrenamiento2);
        await context.SaveChangesAsync();
        // Act
        var historial = await repositorio.ObtenerHistorialCompletoPorSocioAsync(socio.Id);
        // Assert con fluent assertions
        historial.Should().HaveCount(2);
        historial.Should().Contain(e => e.Id == entrenamiento1.Id);
        historial.Should().Contain(e => e.Id == entrenamiento2.Id);
    }

    //obtener historial por profesor
    [Fact]
    public async Task ObtenerHistorialPorProfesor_DebeRetornarListaCorrectamente()
    {
        // Arrange

        var options = CreateInMemoryOptions("Ejemplo_AddsActividadCorrectamente");
        using var context = new FitRankDbContext(options);
        var repositorio = new EntrenamientoRepositorioImpl(context);
        var socio = await SeedData(context);

        var profesor = new Profesor { Nombre = "Prof", Apellido = "Test", Email = "prof@test.com", Matricula = "M1", Sueldo = 1000 };
        context.Profesores.Add(profesor);
        await context.SaveChangesAsync();

        var rutina = new Rutina { Nombre = "R1", TipoCreacion = "Auto", UsuarioId = profesor.Id };
        context.Rutinas.Add(rutina);
        await context.SaveChangesAsync();

        var sesion = new Sesion { Nombre = "S1", Rutina = rutina };
        context.Sesiones.Add(sesion);
        await context.SaveChangesAsync();

        var ejercicioAsignado = new EjercicioAsignado { Sesion = sesion };
        context.EjerciciosAsignados.Add(ejercicioAsignado);
        await context.SaveChangesAsync();

        var entrenamiento = new Entrenamiento { SocioId = socio.Id, Fecha = DateTime.UtcNow, Duracion = TimeSpan.FromHours(1) };
        context.Entrenamientos.Add(entrenamiento);
        await context.SaveChangesAsync();

        var actividad = new Actividad { Entrenamiento = entrenamiento, EjercicioAsignado = ejercicioAsignado };
        context.Actividades.Add(actividad);
        await context.SaveChangesAsync();
        // Act
        var historial = await repositorio.ObtenerHistorialPorProfesorAsync(profesor.Id, socio.Nombre);
        // Assert con fluent assertions
        historial.Should().HaveCount(1);
        historial.Should().Contain(e => e.Id == entrenamiento.Id);
        historial.First().Actividades.Should().Contain(a => a.Id == actividad.Id);
        historial.First().Actividades.First().EjercicioAsignadoId.Should().Be(ejercicioAsignado.Id);
    }


}