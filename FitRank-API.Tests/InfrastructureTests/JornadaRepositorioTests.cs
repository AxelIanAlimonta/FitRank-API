using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Persistence;
using FitRank_API.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FitRank_API.tests.InfrastructureTests;

public class JornadaRepositorioTests
{
    private DbContextOptions<FitRankDbContext> CreateInMemoryOptions(string dbName)
    {
        return new DbContextOptionsBuilder<FitRankDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .EnableSensitiveDataLogging()
            .Options;
    }

    //seed
    private async Task<(Profesor, DiaDeLaSemana)> SeedData(FitRankDbContext context)
    {
        var profesor = new Profesor
        {
            Nombre = "Juan Perez",
            Matricula = "ABC123",
            Email = "juan.perez@example.com",
            Sueldo = 50000
        };

        var diaDeLaSemana = new DiaDeLaSemana
        {
            Nombre = "Lunes"
        };

        context.Profesores.Add(profesor);
        context.DiasDeLaSemana.Add(diaDeLaSemana);
        await context.SaveChangesAsync();

        return (profesor, diaDeLaSemana);
    }

    //agregar jornada deberia guardar correctamente
    [Fact]
    public async Task AgregarJornada_DeberiaGuardarCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("AgregarJornadaDb");
        using var context = new FitRankDbContext(options);
        var jornadaRepositorioMock = new JornadaRepositorioImpl(context);
        var (profesor, diaDeLaSemana) = await SeedData(context);

        var jornada = new Jornada
        {
            HoraInicio = new TimeSpan(9, 0, 0),
            HoraFin = new TimeSpan(17, 0, 0),
            ProfesorId = profesor.Id,
            DiaDeLaSemanaId = diaDeLaSemana.Id
        };

        // Act
        await jornadaRepositorioMock.AgregarJornadaAsync(jornada);

        // FluentAssert
        var jornadaEnDb = await context.Jornadas.FirstOrDefaultAsync(j => j.Id == jornada.Id);
        jornadaEnDb.Should().NotBeNull();
        jornadaEnDb.HoraInicio.Should().Be(new TimeSpan(9, 0, 0));
        jornadaEnDb.HoraFin.Should().Be(new TimeSpan(17, 0, 0));
        jornadaEnDb.ProfesorId.Should().Be(profesor.Id);
        jornadaEnDb.DiaDeLaSemanaId.Should().Be(diaDeLaSemana.Id);
    }

    //obtener todas las jornadas
    [Fact]
    public async Task ObtenerTodasLasJornadas_DeberiaRetornarListaCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerTodasLasJornadasDb");
        using var context = new FitRankDbContext(options);
        var jornadaRepositorioMock = new JornadaRepositorioImpl(context);
        var (profesor, diaDeLaSemana) = await SeedData(context);

        context.Jornadas.AddRange(
            new Jornada
            {
                HoraInicio = new TimeSpan(8, 0, 0),
                HoraFin = new TimeSpan(12, 0, 0),
                ProfesorId = profesor.Id,
                DiaDeLaSemanaId = diaDeLaSemana.Id
            },
            new Jornada
            {
                HoraInicio = new TimeSpan(13, 0, 0),
                HoraFin = new TimeSpan(17, 0, 0),
                ProfesorId = profesor.Id,
                DiaDeLaSemanaId = diaDeLaSemana.Id
            }
        );
        await context.SaveChangesAsync();

        // Act
        var jornadas = await jornadaRepositorioMock.ObtenerTodasLasJornadasAsync();

        // FluentAssert
        jornadas.Should().HaveCount(2);
    }

    //obtener todas las jornadas vacia
    [Fact]
    public async Task ObtenerTodasLasJornadas_DeberiaRetornarListaVacia()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerTodasLasJornadasVaciaDb");
        using var context = new FitRankDbContext(options);
        var jornadaRepositorioMock = new JornadaRepositorioImpl(context);

        // Act
        var jornadas = await jornadaRepositorioMock.ObtenerTodasLasJornadasAsync();

        // FluentAssert
        jornadas.Should().BeEmpty();
    }

    //obtener jornada por id
    [Fact]
    public async Task ObtenerJornadaPorId_DeberiaRetornarJornadaCorrecta()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerJornadaPorIdDb");
        using var context = new FitRankDbContext(options);
        var jornadaRepositorioMock = new JornadaRepositorioImpl(context);
        var (profesor, diaDeLaSemana) = await SeedData(context);

        var jornada = new Jornada
        {
            HoraInicio = new TimeSpan(10, 0, 0),
            HoraFin = new TimeSpan(14, 0, 0),
            ProfesorId = profesor.Id,
            DiaDeLaSemanaId = diaDeLaSemana.Id
        };

        context.Jornadas.Add(jornada);
        await context.SaveChangesAsync();

        // Act
        var jornadaObtenida = await jornadaRepositorioMock.ObtenerJornadaPorIdAsync(jornada.Id);

        // FluentAssert
        jornadaObtenida.Should().NotBeNull();
        jornadaObtenida!.Id.Should().Be(jornada.Id);
        jornadaObtenida.ProfesorId.Should().Be(profesor.Id);
        jornadaObtenida.DiaDeLaSemanaId.Should().Be(diaDeLaSemana.Id);
    }

    //obtener jornada por id no existente
    [Fact]
    public async Task ObtenerJornadaPorId_NoExistente_DeberiaRetornarNull()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerJornadaPorIdNoExistenteDb");
        using var context = new FitRankDbContext(options);
        var jornadaRepositorioMock = new JornadaRepositorioImpl(context);

        // Act
        var jornadaObtenida = await jornadaRepositorioMock.ObtenerJornadaPorIdAsync(999);

        // FluentAssert
        jornadaObtenida.Should().BeNull();
    }

    //actualizar jornada
    [Fact]
    public async Task ActualizarJornada_DeberiaActualizarCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("ActualizarJornadaDb");
        using var context = new FitRankDbContext(options);
        var jornadaRepositorioMock = new JornadaRepositorioImpl(context);
        var (profesor, diaDeLaSemana) = await SeedData(context);

        var jornada = new Jornada
        {
            HoraInicio = new TimeSpan(9, 0, 0),
            HoraFin = new TimeSpan(15, 0, 0),
            ProfesorId = profesor.Id,
            DiaDeLaSemanaId = diaDeLaSemana.Id
        };

        context.Jornadas.Add(jornada);
        await context.SaveChangesAsync();

        // Act
        jornada.HoraInicio = new TimeSpan(10, 0, 0);
        jornada.HoraFin = new TimeSpan(16, 0, 0);
        var jornadaActualizada = await jornadaRepositorioMock.ActualizarJornadaAsync(jornada);

        // FluentAssert
        jornadaActualizada.Should().NotBeNull();
        jornadaActualizada!.HoraInicio.Should().Be(new TimeSpan(10, 0, 0));
        jornadaActualizada.HoraFin.Should().Be(new TimeSpan(16, 0, 0));
    }

    //actualizar jornada inexistente
    [Fact]
    public async Task ActualizarJornada_Inexistente_DeberiaRetornarNull()
    {
        // Arrange
        var options = CreateInMemoryOptions("ActualizarJornadaInexistenteDb");
        using var context = new FitRankDbContext(options);
        var jornadaRepositorioMock = new JornadaRepositorioImpl(context);
        var jornadaInexistente = new Jornada
        {
            Id = 999,
            HoraInicio = new TimeSpan(10, 0, 0),
            HoraFin = new TimeSpan(16, 0, 0),
            ProfesorId = 1,
            DiaDeLaSemanaId = 1
        };
        // Act
        var jornadaActualizada = await jornadaRepositorioMock.ActualizarJornadaAsync(jornadaInexistente);
        // FluentAssert
        jornadaActualizada.Should().BeNull();
    }

    //eliminar jornada
    [Fact]
    public async Task EliminarJornada_DeberiaEliminarCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("EliminarJornadaDb");
        using var context = new FitRankDbContext(options);
        var jornadaRepositorioMock = new JornadaRepositorioImpl(context);
        var (profesor, diaDeLaSemana) = await SeedData(context);

        var jornada = new Jornada
        {
            HoraInicio = new TimeSpan(7, 0, 0),
            HoraFin = new TimeSpan(11, 0, 0),
            ProfesorId = profesor.Id,
            DiaDeLaSemanaId = diaDeLaSemana.Id
        };

        context.Jornadas.Add(jornada);
        await context.SaveChangesAsync();

        // Act
        var resultado = await jornadaRepositorioMock.EliminarJornadaAsync(jornada.Id);

        // FluentAssert
        resultado.Should().BeTrue();
        var jornadaEnDb = await context.Jornadas.FindAsync(jornada.Id);
        jornadaEnDb.Should().BeNull();
    }

    //eliminar jornada inexistente
    [Fact]
    public async Task EliminarJornada_Inexistente_DeberiaRetornarFalse()
    {
        // Arrange
        var options = CreateInMemoryOptions("EliminarJornadaInexistenteDb");
        using var context = new FitRankDbContext(options);
        var jornadaRepositorioMock = new JornadaRepositorioImpl(context);

        // Act
        var resultado = await jornadaRepositorioMock.EliminarJornadaAsync(999);

        // FluentAssert
        resultado.Should().BeFalse();
    }

}
