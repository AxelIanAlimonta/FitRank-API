using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Persistence;
using FitRank_API.Infrastructure.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FitRank_API.Tests.InfrastructureTests;

public class InvitacionRepositorioTests
{
    private DbContextOptions<FitRankDbContext> CreateInMemoryOptions(string dbName)
    {
        return new DbContextOptionsBuilder<FitRankDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
    }

    [Fact]
    public async Task AgregarAsync_DeberiaAgregarInvitacion()
    {
        // Arrange
        var options = CreateInMemoryOptions("AgregarInvitacionDb");
        using var context = new FitRankDbContext(options);
        var repo = new InvitacionRepositorioImpl(context);

        var invitacion = new Invitacion
        {
            Email = "test@email.com",
            Estado = "Pendiente",
            GimnasioId = 1,
            CreadaEn = DateTime.UtcNow
        };

        // Act
        var resultado = await repo.AgregarAsync(invitacion);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Email.Should().Be("test@email.com");
        resultado.Estado.Should().Be("Pendiente");
    }

    [Fact]
    public async Task AgregarAsync_NoAgregaSiYaExiste()
    {
        // Arrange
        var options = CreateInMemoryOptions("AgregarInvitacionExistenteDb");
        using var context = new FitRankDbContext(options);
        var repo = new InvitacionRepositorioImpl(context);

        var invitacion = new Invitacion
        {
            Email = "existente@email.com",
            Estado = "Pendiente",
            GimnasioId = 1,
            CreadaEn = DateTime.UtcNow
        };
        context.Invitaciones.Add(invitacion);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repo.AgregarAsync(invitacion);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(invitacion.Id);
    }

    [Fact]
    public async Task ActualizarAsync_DeberiaActualizarInvitacionExistente()
    {
        // Arrange
        var options = CreateInMemoryOptions("ActualizarInvitacionDb");
        using var context = new FitRankDbContext(options);
        var repo = new InvitacionRepositorioImpl(context);

        var invitacion = new Invitacion
        {
            Email = "update@email.com",
            Estado = "Pendiente",
            GimnasioId = 1,
            CreadaEn = DateTime.UtcNow
        };
        context.Invitaciones.Add(invitacion);
        await context.SaveChangesAsync();

        // Act
        invitacion.Estado = "Aceptada";
        invitacion.MetodoPago = "Efectivo";
        invitacion.MpPaymentId = "123";
        var actualizado = await repo.ActualizarAsync(invitacion);

        // Assert
        actualizado.Should().NotBeNull();
        actualizado!.Estado.Should().Be("Aceptada");
        actualizado.MetodoPago.Should().Be("Efectivo");
        actualizado.MpPaymentId.Should().Be("123");
    }

    [Fact]
    public async Task ActualizarAsync_DeberiaRetornarNullSiNoExiste()
    {
        // Arrange
        var options = CreateInMemoryOptions("ActualizarInvitacionInexistenteDb");
        using var context = new FitRankDbContext(options);
        var repo = new InvitacionRepositorioImpl(context);

        var invitacion = new Invitacion
        {
            Id = 999,
            Email = "noexiste@email.com",
            Estado = "Pendiente",
            GimnasioId = 1,
            CreadaEn = DateTime.UtcNow
        };

        // Act
        var actualizado = await repo.ActualizarAsync(invitacion);

        // Assert
        actualizado.Should().BeNull();
    }

    [Fact]
    public async Task Eliminar_DeberiaEliminarInvitacionExistente()
    {
        // Arrange
        var options = CreateInMemoryOptions("EliminarInvitacionDb");
        using var context = new FitRankDbContext(options);
        var repo = new InvitacionRepositorioImpl(context);

        var invitacion = new Invitacion
        {
            Email = "eliminar@email.com",
            Estado = "Pendiente",
            GimnasioId = 1,
            CreadaEn = DateTime.UtcNow
        };
        context.Invitaciones.Add(invitacion);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repo.Eliminar(invitacion.Id);

        // Assert
        resultado.Should().BeTrue();
        var enDb = await context.Invitaciones.FindAsync(invitacion.Id);
        enDb.Should().BeNull();
    }

    [Fact]
    public async Task Eliminar_DeberiaRetornarFalseSiNoExiste()
    {
        // Arrange
        var options = CreateInMemoryOptions("EliminarInvitacionInexistenteDb");
        using var context = new FitRankDbContext(options);
        var repo = new InvitacionRepositorioImpl(context);

        // Act
        var resultado = await repo.Eliminar(999);

        // Assert
        resultado.Should().BeFalse();
    }

    [Fact]
    public async Task ObtenerPorIdAsync_DeberiaRetornarInvitacionConGimnasio()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerPorIdInvitacionDb");
        using var context = new FitRankDbContext(options);
        var repo = new InvitacionRepositorioImpl(context);

        var gimnasio = new Gimnasio { Nombre = "Gym" };
        context.Gimnasios.Add(gimnasio);
        await context.SaveChangesAsync();

        var invitacion = new Invitacion
        {
            Email = "byid@email.com",
            Estado = "Pendiente",
            GimnasioId = gimnasio.Id,
            CreadaEn = DateTime.UtcNow
        };
        context.Invitaciones.Add(invitacion);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repo.ObtenerPorIdAsync(invitacion.Id);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Gimnasio.Should().NotBeNull();
        resultado.Gimnasio!.Nombre.Should().Be("Gym");
    }

    [Fact]
    public async Task ObtenerPorIdYEstadoAsync_DeberiaRetornarCorrecto()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerPorIdYEstadoInvitacionDb");
        using var context = new FitRankDbContext(options);
        var repo = new InvitacionRepositorioImpl(context);

        var invitacion = new Invitacion
        {
            Email = "byidestado@email.com",
            Estado = "Pendiente",
            GimnasioId = 1,
            CreadaEn = DateTime.UtcNow
        };
        context.Invitaciones.Add(invitacion);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repo.ObtenerPorIdYEstadoAsync(invitacion.Id, "Pendiente");

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Estado.Should().Be("Pendiente");
    }

    [Fact]
    public async Task ObtenerTodasAsync_DeberiaRetornarSoloDelGimnasio()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerTodasInvitacionesDb");
        using var context = new FitRankDbContext(options);
        var repo = new InvitacionRepositorioImpl(context);

        var gimnasio1 = new Gimnasio { Nombre = "Gym1" };
        var gimnasio2 = new Gimnasio { Nombre = "Gym2" };
        context.Gimnasios.AddRange(gimnasio1, gimnasio2);
        await context.SaveChangesAsync();

        context.Invitaciones.AddRange(
            new Invitacion { Email = "a@email.com", Estado = "Pendiente", GimnasioId = gimnasio1.Id, CreadaEn = DateTime.UtcNow },
            new Invitacion { Email = "b@email.com", Estado = "Pendiente", GimnasioId = gimnasio1.Id, CreadaEn = DateTime.UtcNow },
            new Invitacion { Email = "c@email.com", Estado = "Pendiente", GimnasioId = gimnasio2.Id, CreadaEn = DateTime.UtcNow }
        );
        await context.SaveChangesAsync();

        // Act
        var resultado = await repo.ObtenerTodasAsync(gimnasio1.Id);

        // Assert
        resultado.Should().HaveCount(2);
        resultado.All(i => i.GimnasioId == gimnasio1.Id).Should().BeTrue();
    }

    [Fact]
    public async Task ObtenerPorEmailAsync_DeberiaRetornarInvitacionConGimnasioYUsuario()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerPorEmailInvitacionDb");
        using var context = new FitRankDbContext(options);
        var repo = new InvitacionRepositorioImpl(context);

        var gimnasio = new Gimnasio { Nombre = "Gym" };
        var socio = new Socio { Nombre = "User", Email = "user@email.com", Nivel = "Principiante" };
        context.Gimnasios.Add(gimnasio);
        context.Socios.Add(socio);
        await context.SaveChangesAsync();

        var invitacion = new Invitacion
        {
            Email = "user@email.com",
            Estado = "Pendiente",
            GimnasioId = gimnasio.Id,
            UsuarioId = socio.Id, // Ahora es un Socio
            CreadaEn = DateTime.UtcNow
        };
        context.Invitaciones.Add(invitacion);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repo.ObtenerPorEmailAsync("user@email.com");

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Gimnasio.Should().NotBeNull();
        resultado.Usuario.Should().NotBeNull();
        resultado.Usuario!.Nombre.Should().Be("User");
    }
}
