using FitRank_API.Application.DTOs.RutinaDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using FitRank_API.Infrastructure.Persistence;
using FitRank_API.Infrastructure.Repositorios;
using FitRank_API.Application.DTOs.EjercicioDTOs;
using FitRank_API.Application.DTOs.SesionDTOs;



namespace FitRank_API.Tests.InfrastructureTests;


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
            Nivel = "Intermedio"
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

    //obtener por socio id
    [Fact]
    public async Task ObtenerRutinasPorSocioId_DeberíaRetornarRutinasDelSocio()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerRutinasPorSocioIdTestDb");
        using var context = new FitRankDbContext(options);
        var repositorio = new RutinaRepositorioImpl(context);
        var (_usuarioMock, _socioMock) = await SeedData(context);
        var rutina1 = new Rutina
        {
            Nombre = "Rutina Socio",
            TipoCreacion = "Estandar",
            Descripcion = "Descripción de la rutina del socio.",
            Activa = true,
            FechaCreacion = new DateTime(2024, 7, 1),
            SocioId = _socioMock.Id,
            UsuarioId = _usuarioMock.Id
        };
        var rutina2 = new Rutina
        {
            Nombre = "Otra Rutina Socio",
            TipoCreacion = "Personalizada",
            Descripcion = "Otra descripción de la rutina del socio.",
            Activa = false,
            FechaCreacion = new DateTime(2024, 8, 1),
            SocioId = _socioMock.Id,
            UsuarioId = _usuarioMock.Id
        };
        context.Rutinas.AddRange(rutina1, rutina2);
        await context.SaveChangesAsync();
        // Act
        var resultado = await repositorio.ObtenerPorSocioIdAsync(_socioMock.Id);
        var rutinasDelSocio = resultado.Where(r => r.SocioId == _socioMock.Id).ToList();
        // Assert
        rutinasDelSocio.Should().HaveCount(2);
        rutinasDelSocio.Should().Contain(r => r.Nombre == "Rutina Socio");
        rutinasDelSocio.Should().Contain(r => r.Nombre == "Otra Rutina Socio");
    }

    [Fact]
    public async Task ObtenerRutinasPorSocioAsync_DeberiaRetornarRutinasConSesionesYEjerciciosAsignados()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerRutinasPorSocioAsyncDb");
        using var context = new FitRankDbContext(options);

        // Crear socio y rutina
        var socio = new Socio { Nombre = "Socio Test", Email = "socio@test.com", Nivel = "Intermedio" };
        context.Socios.Add(socio);
        await context.SaveChangesAsync();

        var rutina = new Rutina
        {
            Nombre = "Rutina Completa",
            TipoCreacion = "Personalizada",
            Descripcion = "Rutina con sesiones y ejercicios",
            Activa = true,
            FechaCreacion = DateTime.UtcNow,
            SocioId = socio.Id,
            UsuarioId = 1
        };
        context.Rutinas.Add(rutina);
        await context.SaveChangesAsync();

        // Crear ejercicio y sesión
        var ejercicio = new Ejercicio { Nombre = "Sentadilla" };
        context.Ejercicios.Add(ejercicio);
        await context.SaveChangesAsync();

        var sesion = new Sesion
        {
            RutinaId = rutina.Id,
            Nombre = "Sesión 1",
            NumeroDeSesion = 1
        };
        context.Sesiones.Add(sesion);
        await context.SaveChangesAsync();

        var ejercicioAsignado = new EjercicioAsignado
        {
            SesionId = sesion.Id,
            EjercicioId = ejercicio.Id,
            NumeroEjercicio = 1
        };
        context.EjerciciosAsignados.Add(ejercicioAsignado);
        await context.SaveChangesAsync();

        var serie = new Serie
        {
            EjercicioAsignadoId = ejercicioAsignado.Id,
            NumeroDeSerie = 1,
            Repeticiones = 10,
            Peso = 50
        };
        context.Series.Add(serie);
        await context.SaveChangesAsync();

        var repo = new RutinaRepositorioImpl(context);

        // Act
        var resultado = await repo.ObtenerRutinasPorSocioAsync(socio.Id);

        // Assert
        resultado.Should().HaveCount(1);
        var rutinaResult = resultado.First();
        rutinaResult.Sesiones.Should().NotBeNullOrEmpty();
        var sesionResult = rutinaResult.Sesiones!.First();
        sesionResult.EjerciciosAsignados.Should().NotBeNullOrEmpty();
        var ejercicioAsignadoResult = sesionResult.EjerciciosAsignados!.First();
        ejercicioAsignadoResult.Series.Should().NotBeNullOrEmpty();
        ejercicioAsignadoResult.Ejercicio.Should().NotBeNull();
        ejercicioAsignadoResult.Ejercicio.Nombre.Should().Be("Sentadilla");
    }

    //obtener todas las rutinas por profesor id
    [Fact]
    public async Task ObtenerTodasLasRutinasPorProfesorIdAsync_DeberiaRetornarRutinasDelProfesor()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerTodasLasRutinasPorProfesorIdTestDb");
        using var context = new FitRankDbContext(options);
        var repositorio = new RutinaRepositorioImpl(context);
        var (_usuarioMock, _socioMock) = await SeedData(context);
        _usuarioMock.Rol = "Profesor";
        var rutina1 = new Rutina
        {
            Nombre = "Rutina Profesor",
            TipoCreacion = "Estandar",
            Descripcion = "Descripción de la rutina del profesor.",
            Activa = true,
            FechaCreacion = new DateTime(2024, 9, 1),
            SocioId = _socioMock.Id,
            UsuarioId = _usuarioMock.Id
        };
        var rutina2 = new Rutina
        {
            Nombre = "Otra Rutina Profesor",
            TipoCreacion = "Personalizada",
            Descripcion = "Otra descripción de la rutina del profesor.",
            Activa = false,
            FechaCreacion = new DateTime(2024, 10, 1),
            SocioId = _socioMock.Id,
            UsuarioId = _usuarioMock.Id
        };
        context.Rutinas.AddRange(rutina1, rutina2);
        await context.SaveChangesAsync();
        // Act
        var resultado = await repositorio.ObtenerTodasLasRutinasPorProfesorIdAsync(_usuarioMock.Id);
        // Assert
        resultado.Should().HaveCount(2);
        resultado.Should().Contain(r => r.Nombre == "Rutina Profesor");
        resultado.Should().Contain(r => r.Nombre == "Otra Rutina Profesor");
    }

    [Fact]
    public async Task ObtenerFavoritasPorSocioAsync_DeberiaRetornarSoloRutinasFavoritasConSesionesYEjercicios()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerFavoritasPorSocioAsyncDb");
        using var context = new FitRankDbContext(options);

        // Crear socio
        var socio = new Socio { Nombre = "Socio Test", Email = "socio@test.com", Nivel = "Intermedio" };
        context.Socios.Add(socio);
        await context.SaveChangesAsync();

        // Crear ejercicios
        var ejercicio = new Ejercicio { Nombre = "Press Banca" };
        context.Ejercicios.Add(ejercicio);
        await context.SaveChangesAsync();

        // Crear rutina favorita
        var rutinaFavorita = new Rutina
        {
            Nombre = "Rutina Favorita",
            TipoCreacion = "Personalizada",
            Descripcion = "Rutina favorita del socio",
            Activa = true,
            FechaCreacion = DateTime.UtcNow,
            SocioId = socio.Id,
            UsuarioId = 1,
            Favorita = true
        };
        context.Rutinas.Add(rutinaFavorita);
        await context.SaveChangesAsync();

        // Crear rutina NO favorita
        var rutinaNoFavorita = new Rutina
        {
            Nombre = "Rutina No Favorita",
            TipoCreacion = "Estandar",
            Descripcion = "Rutina no favorita",
            Activa = true,
            FechaCreacion = DateTime.UtcNow,
            SocioId = socio.Id,
            UsuarioId = 1,
            Favorita = false
        };
        context.Rutinas.Add(rutinaNoFavorita);
        await context.SaveChangesAsync();

        // Crear sesión y ejercicios asignados para la rutina favorita
        var sesion = new Sesion
        {
            RutinaId = rutinaFavorita.Id,
            Nombre = "Sesión 1",
            NumeroDeSesion = 1
        };
        context.Sesiones.Add(sesion);
        await context.SaveChangesAsync();

        var ejercicioAsignado = new EjercicioAsignado
        {
            SesionId = sesion.Id,
            EjercicioId = ejercicio.Id,
            NumeroEjercicio = 1
        };
        context.EjerciciosAsignados.Add(ejercicioAsignado);
        await context.SaveChangesAsync();

        var serie = new Serie
        {
            EjercicioAsignadoId = ejercicioAsignado.Id,
            NumeroDeSerie = 1,
            Repeticiones = 12,
            Peso = 60
        };
        context.Series.Add(serie);
        await context.SaveChangesAsync();

        var repo = new RutinaRepositorioImpl(context);

        // Act
        var resultado = await repo.ObtenerFavoritasPorSocioAsync(socio.Id);

        // Assert
        resultado.Should().HaveCount(1);
        var rutinaResult = resultado.First();
        rutinaResult.Favorita.Should().BeTrue();
        rutinaResult.Nombre.Should().Be("Rutina Favorita");
        rutinaResult.Sesiones.Should().NotBeNullOrEmpty();
        var sesionResult = rutinaResult.Sesiones!.First();
        sesionResult.EjerciciosAsignados.Should().NotBeNullOrEmpty();
        var ejercicioAsignadoResult = sesionResult.EjerciciosAsignados!.First();
        ejercicioAsignadoResult.Series.Should().NotBeNullOrEmpty();
        ejercicioAsignadoResult.Ejercicio.Should().NotBeNull();
        ejercicioAsignadoResult.Ejercicio.Nombre.Should().Be("Press Banca");
    }

    [Fact]
    public async Task MarcarFavoritaAsync_DeberiaMarcarRutinaComoFavorita()
    {
        // Arrange
        var options = CreateInMemoryOptions("MarcarFavoritaAsyncDb");
        using var context = new FitRankDbContext(options);
        var repo = new RutinaRepositorioImpl(context);

        var rutina = new Rutina
        {
            Nombre = "Rutina Test",
            SocioId = 1,
            UsuarioId = 1,
            Favorita = false
        };
        context.Rutinas.Add(rutina);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repo.MarcarFavoritaAsync(rutina.Id, true);

        // Assert
        resultado.Should().BeTrue();
        var rutinaEnDb = await context.Rutinas.FindAsync(rutina.Id);
        rutinaEnDb!.Favorita.Should().BeTrue();
    }

    [Fact]
    public async Task MarcarFavoritaAsync_DeberiaRetornarFalseSiRutinaNoExiste()
    {
        // Arrange
        var options = CreateInMemoryOptions("MarcarFavoritaAsyncNoExisteDb");
        using var context = new FitRankDbContext(options);
        var repo = new RutinaRepositorioImpl(context);

        // Act
        var resultado = await repo.MarcarFavoritaAsync(999, true);

        // Assert
        resultado.Should().BeFalse();
    }

    [Fact]
    public async Task CambiarEstadoRutinaAsync_DeberiaCambiarEstadoDeRutina()
    {
        // Arrange
        var options = CreateInMemoryOptions("CambiarEstadoRutinaAsyncDb");
        using var context = new FitRankDbContext(options);
        var repo = new RutinaRepositorioImpl(context);

        var rutina = new Rutina
        {
            Nombre = "Rutina Test",
            SocioId = 1,
            UsuarioId = 1,
            Activa = true
        };
        context.Rutinas.Add(rutina);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repo.CambiarEstadoRutinaAsync(rutina.Id, false);

        // Assert
        resultado.Should().BeTrue();
        var rutinaEnDb = await context.Rutinas.FindAsync(rutina.Id);
        rutinaEnDb!.Activa.Should().BeFalse();
    }

    [Fact]
    public async Task CambiarEstadoRutinaAsync_DeberiaRetornarFalseSiRutinaNoExiste()
    {
        // Arrange
        var options = CreateInMemoryOptions("CambiarEstadoRutinaAsyncNoExisteDb");
        using var context = new FitRankDbContext(options);
        var repo = new RutinaRepositorioImpl(context);

        // Act
        var resultado = await repo.CambiarEstadoRutinaAsync(999, false);

        // Assert
        resultado.Should().BeFalse();
    }

    
}