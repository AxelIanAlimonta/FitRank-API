using System;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Persistence;
using FitRank_API.Infrastructure.Repositorios;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FitRank_API.Tests.InfrastructureTests;

public class UsuarioRepositorioTests
{
    private DbContextOptions<FitRankDbContext> CreateInMemoryOptions(string dbName)
    {
        return new DbContextOptionsBuilder<FitRankDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
    }

    [Fact]
    public async Task AgregarAsync_DeberiaAgregarUsuario()
    {
        var options = CreateInMemoryOptions("AgregarUsuarioDb");
        using var context = new FitRankDbContext(options);
        var repo = new UsuarioRepositorioImpl(context);

        var usuario = new Usuario
        {
            Nombre = "Juan",
            Apellido = "Perez",
            Email = "juan@test.com",
            Rol = "Socio"
        };

        var resultado = await repo.AgregarAsync(usuario);

        resultado.Should().NotBeNull();
        resultado.Email.Should().Be("juan@test.com");
        (await context.Usuarios.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task ExistePorEmailAsync_DeberiaRetornarTrueSiExiste()
    {
        var options = CreateInMemoryOptions("ExistePorEmailDb");
        using var context = new FitRankDbContext(options);
        var repo = new UsuarioRepositorioImpl(context);

        context.Usuarios.Add(new Usuario { Email = "existe@test.com" });
        await context.SaveChangesAsync();

        var existe = await repo.ExistePorEmailAsync("existe@test.com");
        existe.Should().BeTrue();
    }

    [Fact]
    public async Task ObtenerPorEmailAsync_DeberiaRetornarUsuarioCorrecto()
    {
        var options = CreateInMemoryOptions("ObtenerPorEmailDb");
        using var context = new FitRankDbContext(options);
        var repo = new UsuarioRepositorioImpl(context);

        var usuario = new Usuario { Email = "buscar@test.com" };
        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();

        var resultado = await repo.ObtenerPorEmailAsync("buscar@test.com");
        resultado.Should().NotBeNull();
        resultado!.Email.Should().Be("buscar@test.com");
    }

    [Fact]
    public async Task ObtenerPorIdAsync_DeberiaRetornarUsuarioCorrecto()
    {
        var options = CreateInMemoryOptions("ObtenerPorIdDb");
        using var context = new FitRankDbContext(options);
        var repo = new UsuarioRepositorioImpl(context);

        var usuario = new Usuario { Email = "id@test.com" };
        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();

        var resultado = await repo.ObtenerPorIdAsync(usuario.Id);
        resultado.Should().NotBeNull();
        resultado!.Email.Should().Be("id@test.com");
    }

    [Fact]
    public async Task ObtenerPorTokenActivacionAsync_DeberiaRetornarUsuarioCorrecto()
    {
        var options = CreateInMemoryOptions("ObtenerPorTokenDb");
        using var context = new FitRankDbContext(options);
        var repo = new UsuarioRepositorioImpl(context);

        var usuario = new Usuario
        {
            Email = "token@test.com",
            TokenRecuperacion = "token123",
            TokenExpira = DateTime.UtcNow.AddHours(1),
            EsActivado = false
        };
        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();

        var resultado = await repo.ObtenerPorTokenActivacionAsync("token123");
        resultado.Should().NotBeNull();
        resultado!.Email.Should().Be("token@test.com");
    }

    [Fact]
    public async Task ObtenerPorCondicionAsync_DeberiaRetornarUsuarioCorrecto()
    {
        var options = CreateInMemoryOptions("ObtenerPorCondicionDb");
        using var context = new FitRankDbContext(options);
        var repo = new UsuarioRepositorioImpl(context);

        var usuario = new Usuario { Email = "condicion@test.com", Nombre = "Condicion" };
        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();

        Expression<Func<Usuario, bool>> pred = u => u.Nombre == "Condicion";
        var resultado = await repo.ObtenerPorCondicionAsync(pred);
        resultado.Should().NotBeNull();
        resultado!.Email.Should().Be("condicion@test.com");
    }

    [Fact]
    public async Task ActualizarAsync_DeberiaActualizarUsuario()
    {
        var options = CreateInMemoryOptions("ActualizarUsuarioDb");
        using var context = new FitRankDbContext(options);
        var repo = new UsuarioRepositorioImpl(context);

        var usuario = new Usuario { Nombre = "Original", Email = "actualizar@test.com" };
        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();

        usuario.Nombre = "Actualizado";
        usuario.PasswordHash = "nuevohash";
        var actualizado = await repo.ActualizarAsync(usuario);

        actualizado.Should().NotBeNull();
        actualizado!.Nombre.Should().Be("Actualizado");
        actualizado.PasswordHash.Should().Be("nuevohash");
    }

    //actualizar inexistente devuelve null
    [Fact]
    public async Task ActualizarAsync_DeberiaRetornarNullSiNoExiste()
    {
        //Arrange
        var options = CreateInMemoryOptions("ActualizarUsuarioInexistenteDb");
        using var context = new FitRankDbContext(options);
        var repo = new UsuarioRepositorioImpl(context);
        var usuario = new Usuario { Id = 999, Nombre = "Inexistente", Email = "asd@asd.com" };
        //Act
        var actualizado = await repo.ActualizarAsync(usuario);
        //Assert
        actualizado.Should().BeNull();
    }

    [Fact]
    public async Task EliminarAsync_DeberiaEliminarUsuario()
    {
        var options = CreateInMemoryOptions("EliminarUsuarioDb");
        using var context = new FitRankDbContext(options);
        var repo = new UsuarioRepositorioImpl(context);

        var usuario = new Usuario { Email = "eliminar@test.com" };
        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();

        await repo.EliminarAsync(usuario);

        var enDb = await context.Usuarios.FindAsync(usuario.Id);
        enDb.Should().BeNull();
    }

    [Fact]
    public async Task ObtenerTodosAsync_DeberiaRetornarTodosLosUsuarios()
    {
        var options = CreateInMemoryOptions("ObtenerTodosUsuariosDb");
        using var context = new FitRankDbContext(options);
        var repo = new UsuarioRepositorioImpl(context);

        context.Usuarios.AddRange(
            new Usuario { Email = "a@test.com" },
            new Usuario { Email = "b@test.com" }
        );
        await context.SaveChangesAsync();

        var resultado = await repo.ObtenerTodosAsync();
        resultado.Should().HaveCount(2);
    }

    [Fact]
    public async Task ObtenerSocioConGimnasioPorIdAsync_DeberiaRetornarSocioConGimnasio()
    {
        var options = CreateInMemoryOptions("ObtenerSocioConGimnasioDb");
        using var context = new FitRankDbContext(options);
        var repo = new UsuarioRepositorioImpl(context);

        var gimnasio = new Gimnasio { Nombre = "Gym" };
        context.Gimnasios.Add(gimnasio);
        await context.SaveChangesAsync();

        var socio = new Socio { Nombre = "Socio", Email = "socio@test.com", GimnasioId = gimnasio.Id, Nivel = "Intermedio" };
        context.Socios.Add(socio);
        await context.SaveChangesAsync();

        var resultado = await repo.ObtenerSocioConGimnasioPorIdAsync(socio.Id);
        resultado.Should().NotBeNull();
        resultado!.Gimnasio.Should().NotBeNull();
        resultado.Gimnasio!.Nombre.Should().Be("Gym");
    }

    [Fact]
    public async Task ObtenerSociosActivosAsync_DeberiaRetornarSoloSociosActivos()
    {
        var options = CreateInMemoryOptions("ObtenerSociosActivosDb");
        using var context = new FitRankDbContext(options);
        var repo = new UsuarioRepositorioImpl(context);

        var gimnasio = new Gimnasio { Nombre = "Gym" };
        context.Gimnasios.Add(gimnasio);
        await context.SaveChangesAsync();

        var socioActivo = new Socio
        {
            Nombre = "Activo",
            Email = "activo@test.com",
            Estado = "Activo",
            CuotaPagadaHasta = DateTime.UtcNow.AddDays(10),
            GimnasioId = gimnasio.Id,
            Nivel = "Intermedio"
        };
        var socioInactivo = new Socio
        {
            Nombre = "Inactivo",
            Email = "inactivo@test.com",
            Estado = "Inactivo",
            CuotaPagadaHasta = DateTime.UtcNow.AddDays(-10),
            GimnasioId = gimnasio.Id,
            Nivel = "Intermedio"
        };
        context.Socios.AddRange(socioActivo, socioInactivo);
        await context.SaveChangesAsync();

        var resultado = await repo.ObtenerSociosActivosAsync();
        resultado.Should().HaveCount(1);
        resultado.First().Nombre.Should().Be("Activo");
        resultado.First().Gimnasio.Should().NotBeNull();
    }

    [Fact]
    public async Task AgregarAsync_DeberiaAgregarSocioCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("AgregarSocioDb");
        using var context = new FitRankDbContext(options);
        var repo = new UsuarioRepositorioImpl(context);

        var gimnasio = new Gimnasio();
        var socio = new Socio
        {
            Nombre = "Juan",
            Apellido = "Pérez",
            Dni = 12345678,
            NombreUsuario = "juanp",
            Email = "juan@example.com",
            Estado = "Activo",
            Gimnasio = gimnasio,
            Altura = 1.80,
            Peso = 80,
            Nivel = "Intermedio",
            Puntaje = 100
        };

        // Act
        var resultado = await repo.AgregarAsync(socio);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().BeOfType<Socio>();
        resultado.Id.Should().BeGreaterThan(0);

        var socioEnDb = await context.Socios.FirstOrDefaultAsync(s => s.Email == "juan@example.com");
        socioEnDb.Should().NotBeNull();
        socioEnDb!.Nombre.Should().Be("Juan");
        socioEnDb.Gimnasio.Should().NotBeNull();
    }
       
}