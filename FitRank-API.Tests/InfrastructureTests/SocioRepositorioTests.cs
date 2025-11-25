using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Persistence;
using FitRank_API.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Tests.InfrastructureTests;

public class SocioRepositorioTests
{
    private DbContextOptions<FitRankDbContext> CreateInMemoryOptions(string dbName)
    {
        return new DbContextOptionsBuilder<FitRankDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
    }

    private async Task<(Socio, Gimnasio)> SeedData(FitRankDbContext context)
    {
        var gimnasio = new Gimnasio
        {
            Nombre = "Gimnasio Test",
            Direccion = "Calle Falsa 123",
        };

        context.Gimnasios.Add(gimnasio);
        await context.SaveChangesAsync();



        var socio = new Socio
        {
            Nombre = "Juan",
            Apellido = "Perez",
            Email = "juanperez@gmail.com",
            Gimnasio = gimnasio,
            FechaRegistro = new DateTime(2023, 1, 1),
            Altura = 1.75,
            Peso = 70,
            Nivel = "Intermedio",
            ParticipaEnRanking = true,
            Puntaje = 1500,
            GimnasioId = gimnasio.Id
        };



        context.Socios.Add(socio);
        await context.SaveChangesAsync();

        var medidaCorporal = new MedidaCorporal
        {
            SocioId = socio.Id,
            Fecha = new DateTime(2023, 1, 2),
            PechoCm = 100,
            CinturaCm = 80,
            CaderaCm = 90,
            BrazoDerechoCm = 30,
            PesoKg = 70,
            BrazoIzquierdoCm = 30
        };
        context.MedidasCorporales.Add(medidaCorporal);
        await context.SaveChangesAsync();

        var entrenamiento = new Entrenamiento
        {
            SocioId = socio.Id,
            Fecha = new DateTime(2023, 1, 3),
        };
        context.Entrenamientos.Add(entrenamiento);
        await context.SaveChangesAsync();

        return (socio, gimnasio);
    }

    //obtener todos los socios exitoso
    [Fact]
    public async Task ObtenerTodosAsync_DeberiaRetornarTodosLosSocios()
    {
        // Arrange
        var options = CreateInMemoryOptions(nameof(ObtenerTodosAsync_DeberiaRetornarTodosLosSocios));
        await using var context = new FitRankDbContext(options);
        await SeedData(context);
        await SeedData(context); // Agregar un segundo socio
        await SeedData(context); // Agregar un tercer socio
        var repo = new SocioRepositorioImpl(context);
        // Act
        var socios = await repo.ObtenerTodosAsync();
        // FluentAssert
        socios.Should().NotBeNull();
        socios.Count.Should().Be(3);
    }

    //obtener todos los socios vacio
    [Fact]
    public async Task ObtenerTodosAsync_DeberiaRetornarListaVaciaSiNoHaySocios()
    {
        // Arrange
        var options = CreateInMemoryOptions(nameof(ObtenerTodosAsync_DeberiaRetornarListaVaciaSiNoHaySocios));
        await using var context = new FitRankDbContext(options);
        var repo = new SocioRepositorioImpl(context);
        // Act
        var socios = await repo.ObtenerTodosAsync();
        // FluentAssert
        socios.Should().NotBeNull();
        socios.Count.Should().Be(0);
    }

    //obtener socio por id exitoso
    [Fact]
    public async Task ObtenerPorIdAsync_DeberiaRetornarSocioExistente()
    {
        // Arrange
        var options = CreateInMemoryOptions(nameof(ObtenerPorIdAsync_DeberiaRetornarSocioExistente));
        await using var context = new FitRankDbContext(options);
        var (socioSemilla, _) = await SeedData(context);
        var repo = new SocioRepositorioImpl(context);
        // Act
        var socio = await repo.ObtenerPorIdAsync(socioSemilla.Id);
        // FluentAssert
        socio.Should().NotBeNull();
        socio!.Id.Should().Be(socioSemilla.Id);
        socio.Nombre.Should().Be(socioSemilla.Nombre);
    }

    //obtener socio por id no existente
    [Fact]
    public async Task ObtenerPorIdAsync_DeberiaRetornarNullSiNoExisteSocio()
    {
        // Arrange
        var options = CreateInMemoryOptions(nameof(ObtenerPorIdAsync_DeberiaRetornarNullSiNoExisteSocio));
        await using var context = new FitRankDbContext(options);
        var repo = new SocioRepositorioImpl(context);
        // Act
        var socio = await repo.ObtenerPorIdAsync(999); // ID inexistente
        // FluentAssert
        socio.Should().BeNull();
    }

    //agregar socio exitoso
    [Fact]
    public async Task AgregarAsync_DeberiaAgregarSocioExitosamente()
    {
        // Arrange
        var options = CreateInMemoryOptions(nameof(AgregarAsync_DeberiaAgregarSocioExitosamente));
        await using var context = new FitRankDbContext(options);
        var (_, gimnasio) = await SeedData(context);
        var repo = new SocioRepositorioImpl(context);
        var nuevoSocio = new Socio
        {
            Nombre = "Maria",
            Apellido = "Lopez",
            Email = "asfasfda@sadfa.com",
            Gimnasio = gimnasio,
            FechaRegistro = new DateTime(2023, 2, 1),
            Altura = 1.65,
            Peso = 60,
            Nivel = "Principiante",
            ParticipaEnRanking = false,
            Puntaje = 1200,
            GimnasioId = gimnasio.Id
        };
        // Act
        var socioAgregado = await repo.AgregarAsync(nuevoSocio);
        // FluentAssert
        socioAgregado.Should().NotBeNull();
        socioAgregado.Id.Should().BeGreaterThan(0);
        socioAgregado.Nombre.Should().Be("Maria");
    }

    //actualizar socio exitoso
    [Fact]
    public async Task ActualizarAsync_DeberiaActualizarSocioExistente()
    {
        // Arrange
        var options = CreateInMemoryOptions(nameof(ActualizarAsync_DeberiaActualizarSocioExistente));
        await using var context = new FitRankDbContext(options);
        var (socioSemilla, _) = await SeedData(context);
        var repo = new SocioRepositorioImpl(context);
        socioSemilla.Nombre = "Juan Actualizado";
        socioSemilla.Peso = 75;
        // Act
        var socioActualizado = await repo.ActualizarAsync(socioSemilla);
        // FluentAssert
        socioActualizado.Should().NotBeNull();
        socioActualizado!.Nombre.Should().Be("Juan Actualizado");
        socioActualizado.Peso.Should().Be(75);
    }

    //actualizar socio no existente
    [Fact]
    public async Task ActualizarAsync_DeberiaRetornarNullSiNoExisteSocio()
    {
        // Arrange
        var options = CreateInMemoryOptions(nameof(ActualizarAsync_DeberiaRetornarNullSiNoExisteSocio));
        await using var context = new FitRankDbContext(options);
        var repo = new SocioRepositorioImpl(context);
        var socioInexistente = new Socio
        {
            Id = 999, // ID inexistente
            Nombre = "Inexistente",
            Apellido = "Inexistente",
            Email = "sasa@fsa.com",
            Altura = 1.80,
            Peso = 80,
            Nivel = "Avanzado",
            ParticipaEnRanking = true,
            Puntaje = 1600
        };
        // Act
        var socioActualizado = await repo.ActualizarAsync(socioInexistente);
        // FluentAssert
        socioActualizado.Should().BeNull();
    }

    //eliminar socio exitoso
    [Fact]
    public async Task EliminarAsync_DeberiaEliminarSocioExistente()
    {
        // Arrange
        var options = CreateInMemoryOptions(nameof(EliminarAsync_DeberiaEliminarSocioExistente));
        await using var context = new FitRankDbContext(options);
        var (socioSemilla, _) = await SeedData(context);
        var repo = new SocioRepositorioImpl(context);
        // Act
        var resultado = await repo.EliminarAsync(socioSemilla.Id);
        var socioEliminado = await repo.ObtenerPorIdAsync(socioSemilla.Id);
        // FluentAssert
        resultado.Should().BeTrue();
        socioEliminado.Should().BeNull();
    }

    //eliminar socio no existente
    [Fact]
    public async Task EliminarAsync_DeberiaRetornarFalseSiNoExisteSocio()
    {
        // Arrange
        var options = CreateInMemoryOptions(nameof(EliminarAsync_DeberiaRetornarFalseSiNoExisteSocio));
        await using var context = new FitRankDbContext(options);
        var repo = new SocioRepositorioImpl(context);
        // Act
        var resultado = await repo.EliminarAsync(999); // ID inexistente
        // FluentAssert
        resultado.Should().BeFalse();
    }

    //obtener socio con medidas exitoso
    [Fact]
    public async Task ObtenerSocioConMedidasAsync_DeberiaRetornarSocioConMedidas()
    {
        // Arrange
        var options = CreateInMemoryOptions(nameof(ObtenerSocioConMedidasAsync_DeberiaRetornarSocioConMedidas));
        await using var context = new FitRankDbContext(options);
        var (socioSemilla, _) = await SeedData(context);
        var repo = new SocioRepositorioImpl(context);
        // Act
        var socio = await repo.ObtenerSocioConMedidasAsync(socioSemilla.Id);
        // FluentAssert
        socio.Should().NotBeNull();
        socio!.MedidasCorporales.Should().NotBeNull();
        socio.MedidasCorporales.Count.Should().Be(1);
    }

    //obtener socio con entrenamientos exitoso
    [Fact]
    public async Task ObtenerSocioConEntrenamientosAsync_DeberiaRetornarSocioConEntrenamientos()
    {
        // Arrange
        var options = CreateInMemoryOptions(nameof(ObtenerSocioConEntrenamientosAsync_DeberiaRetornarSocioConEntrenamientos));
        await using var context = new FitRankDbContext(options);
        var (socioSemilla, _) = await SeedData(context);
        var repo = new SocioRepositorioImpl(context);
        // Act
        var socio = await repo.ObtenerSocioConEntrenamientosAsync(socioSemilla.Id);
        // FluentAssert
        socio.Should().NotBeNull();
        socio!.Entrenamientos.Should().NotBeNull();
        socio.Entrenamientos.Count.Should().Be(1);
    }


    //obtener todos con entrenamientos
    [Fact]
    public async Task ObtenerTodosConEntrenamientosAsync_DeberiaRetornarTodosLosSociosConEntrenamientos()
    {
        // Arrange
        var options = CreateInMemoryOptions(nameof(ObtenerTodosConEntrenamientosAsync_DeberiaRetornarTodosLosSociosConEntrenamientos));
        await using var context = new FitRankDbContext(options);
        await SeedData(context);
        await SeedData(context); // Agregar un segundo socio
        var repo = new SocioRepositorioImpl(context);
        // Act
        var socios = await repo.ObtenerTodosConEntrenamientoAsync();
        // FluentAssert
        socios.Should().NotBeNull();
        socios.Count().Should().Be(2);
        socios.All(s => s.Entrenamientos != null && s.Entrenamientos.Count > 0).Should().BeTrue();
    }

    //obtener todos por gimnasio
    [Fact]
    public async Task ObtenerTodosPorGimnasioAsync_DeberiaRetornarSociosDelGimnasioEspecificado()
    {
        // Arrange
        var options = CreateInMemoryOptions(nameof(ObtenerTodosPorGimnasioAsync_DeberiaRetornarSociosDelGimnasioEspecificado));
        await using var context = new FitRankDbContext(options);
        var (_, gimnasio) = await SeedData(context);
        await SeedData(context); // Agregar un segundo socio al mismo gimnasio

        var socio2 = new Socio
        {
            Nombre = "Carlos",
            Apellido = "Gomez",
            Email = "asdfsa@sdfa.com",
            FechaRegistro = new DateTime(2023, 3, 1),
            Altura = 1.80,
            Peso = 80,
            Nivel = "Avanzado",
            ParticipaEnRanking = true,
            Puntaje = 1600,
            GimnasioId = gimnasio.Id
        };

        var socio3 = new Socio
        {
            Nombre = "Ana",
            Apellido = "Martinez",
            Email = "asdfsa@sdfa.com",
            FechaRegistro = new DateTime(2023, 3, 1),
            Altura = 1.80,
            Peso = 80,
            Nivel = "Avanzado",
            ParticipaEnRanking = true,
            Puntaje = 1600,
            GimnasioId = gimnasio.Id
        };

        context.Socios.AddRange(socio2, socio3);
        await context.SaveChangesAsync();

        var repo = new SocioRepositorioImpl(context);
        // Act
        var socios = await repo.ObtenerTodosPorGimnasio(gimnasio.Id);
        // FluentAssert
        socios.Should().NotBeNull();
        socios.Count().Should().Be(3);

    }


    //cambiar participacion en ranking
    [Fact]
    public async Task CambiarParticipacionEnRankingAsync_DeberiaCambiarEstadoDeParticipacion()
    {
        // Arrange
        var options = CreateInMemoryOptions(nameof(CambiarParticipacionEnRankingAsync_DeberiaCambiarEstadoDeParticipacion));
        await using var context = new FitRankDbContext(options);
        var (socioSemilla, _) = await SeedData(context);
        var repo = new SocioRepositorioImpl(context);
        // Act
        var socioAntes = await repo.ObtenerPorIdAsync(socioSemilla.Id);
        socioAntes!.ParticipaEnRanking.Should().BeTrue();
        await repo.CambiarParticipacionRankingAsync(socioSemilla.Id, false);
        var socioDespues = await repo.ObtenerPorIdAsync(socioSemilla.Id);
        // FluentAssert
        socioDespues!.ParticipaEnRanking.Should().BeFalse();
    }

    //cambiar participacion en ranking socio no existente
    [Fact]
    public async Task CambiarParticipacionEnRankingAsync_DeberiaNoHacerNadaSiNoExisteSocio()
    {
        // Arrange
        var options = CreateInMemoryOptions(nameof(CambiarParticipacionEnRankingAsync_DeberiaNoHacerNadaSiNoExisteSocio));
        await using var context = new FitRankDbContext(options);
        var repo = new SocioRepositorioImpl(context);
        // Act
        Func<Task> act = async () => await repo.CambiarParticipacionRankingAsync(999, false); // ID inexistente
        // FluentAssert
        await act.Should().NotThrowAsync();
    }

    //obtener socios para ranking
    [Fact]
    public async Task ObtenerSociosParaRankingAsync_DeberiaRetornarSoloSociosQueParticipanEnRanking()
    {
        // Arrange
        var options = CreateInMemoryOptions(nameof(ObtenerSociosParaRankingAsync_DeberiaRetornarSoloSociosQueParticipanEnRanking));
        await using var context = new FitRankDbContext(options);
        var gimnasio = new Gimnasio
        {
            Nombre = "Gimnasio Test",
            Direccion = "Calle Falsa 123",
        };
        context.Gimnasios.Add(gimnasio);
        await context.SaveChangesAsync();

        var socio1 = new Socio
        {
            Nombre = "Juan",
            Apellido = "Perez",
            Email = "asdfsa@sdfa.com",
            FechaRegistro = new DateTime(2023, 3, 1),
            Altura = 1.80,
            Peso = 80,
            Nivel = "Avanzado",
            ParticipaEnRanking = true,
            Puntaje = 1600,
            GimnasioId = gimnasio.Id
        };

        var socio2 = new Socio
        {
            Nombre = "Carlos",
            Apellido = "Gomez",
            Email = "asdfsa@sdfa.com",
            FechaRegistro = new DateTime(2023, 3, 1),
            Altura = 1.80,
            Peso = 80,
            Nivel = "Avanzado",
            ParticipaEnRanking = true,
            Puntaje = 1600,
            GimnasioId = gimnasio.Id
        };

        var socio3 = new Socio
        {
            Nombre = "Ana",
            Apellido = "Martinez",
            Email = "asdfsa@sdfa.com",
            FechaRegistro = new DateTime(2023, 3, 1),
            Altura = 1.80,
            Peso = 80,
            Nivel = "Avanzado",
            ParticipaEnRanking = false,
            Puntaje = 1600,
            GimnasioId = gimnasio.Id
        };

        context.Socios.AddRange(socio1, socio2, socio3);
        context.SaveChanges();


        // Act
        var sociosParaRanking = await new SocioRepositorioImpl(context).ObtenerSociosParaRankingAsync(gimnasio.Id);
        // FluentAssert
        sociosParaRanking.Should().NotBeNull();
        sociosParaRanking.Count().Should().Be(2);
    }

    //obtener ranking general
    [Fact]
    public async Task ObtenerRankingGeneralAsync_DeberiaRetornarSociosOrdenadosPorPuntajeDescendente()
    {
        // Arrange
        var options = CreateInMemoryOptions(nameof(ObtenerSociosParaRankingAsync_DeberiaRetornarSoloSociosQueParticipanEnRanking));
        await using var context = new FitRankDbContext(options);
        var gimnasio = new Gimnasio
        {
            Nombre = "Gimnasio Test",
            Direccion = "Calle Falsa 123",
        };
        context.Gimnasios.Add(gimnasio);
        await context.SaveChangesAsync();

        var socio1 = new Socio
        {
            Nombre = "Juan",
            Apellido = "Perez",
            Email = "asdfsa@sdfa.com",
            FechaRegistro = new DateTime(2023, 3, 1),
            Altura = 1.80,
            Peso = 80,
            Nivel = "Avanzado",
            ParticipaEnRanking = true,
            Puntaje = 1600,
            GimnasioId = gimnasio.Id
        };

        var socio2 = new Socio
        {
            Nombre = "Carlos",
            Apellido = "Gomez",
            Email = "asdfsa@sdfa.com",
            FechaRegistro = new DateTime(2023, 3, 1),
            Altura = 1.80,
            Peso = 80,
            Nivel = "Avanzado",
            ParticipaEnRanking = true,
            Puntaje = 1400,
            GimnasioId = gimnasio.Id
        };

        var socio3 = new Socio
        {
            Nombre = "Ana",
            Apellido = "Martinez",
            Email = "asdfsa@sdfa.com",
            FechaRegistro = new DateTime(2023, 3, 1),
            Altura = 1.80,
            Peso = 80,
            Nivel = "Avanzado",
            ParticipaEnRanking = true,
            Puntaje = 1500,
            GimnasioId = gimnasio.Id
        };

        context.Socios.AddRange(socio1, socio2, socio3);
        context.SaveChanges();

        // Act
        var rankingGeneral = await new SocioRepositorioImpl(context).ObtenerRankingGeneralAsync(gimnasio.Id, 2);
        // FluentAssert
        rankingGeneral.Should().NotBeNull();
        rankingGeneral.Count.Should().Be(2);
    }

    //obtener socio y usuario por id
    [Fact]
    public async Task ObtenerSocioYUsuarioPorIdAsync_DeberiaRetornarSocioConUsuario()
    {
        // Arrange
        var options = CreateInMemoryOptions(nameof(ObtenerSocioYUsuarioPorIdAsync_DeberiaRetornarSocioConUsuario));
        await using var context = new FitRankDbContext(options);
        var (socioSemilla, _) = await SeedData(context);
        var repo = new SocioRepositorioImpl(context);
        // Act
        var socio = await repo.ObtenerSocioYUsuarioPorIdAsync(socioSemilla.Id);
        // FluentAssert
        socio.Should().NotBeNull();
        socio!.Id.Should().Be(socioSemilla.Id);
    }
}




