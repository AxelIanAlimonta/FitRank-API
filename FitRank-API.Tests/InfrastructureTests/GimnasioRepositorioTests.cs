using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Persistence;
using FitRank_API.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace FitRank_API.tests.InfrastructureTests;

public class GimnasioRepositorioTests
{
    private DbContextOptions<FitRankDbContext> CreateInMemoryOptions(string dbName)
    {
        return new DbContextOptionsBuilder<FitRankDbContext>()
            .UseInMemoryDatabase(databaseName: dbName)
            .Options;
    }

    private async Task<Administrador> SeedData(FitRankDbContext context)
    {
        var administrador = new Administrador
        {
            Nombre = "Admin",
            Email = "admin@example.com",
        };
        context.Administradores.Add(administrador);
        await context.SaveChangesAsync();

        return administrador;
    }

    [Fact]
    public async Task AgregarGimnasio_DeberiaGuardarCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("AgregarGimnasioDb");
        using var context = new FitRankDbContext(options);
        var gimnasioRepositorioMock = new GimnasioRepositorioImpl(context);
        var administrador = await SeedData(context);

        var gimnasio = new Gimnasio
        {
            Nombre = "Gimnasio Test",
            Direccion = "Calle Test 123",
            AdministradorId = administrador.Id
        };

        // Act
        await gimnasioRepositorioMock.AgregarGimnasio(gimnasio);

        // FluentAssert
        var gimnasioEnDb = await context.Gimnasios.FirstOrDefaultAsync(g => g.Id == gimnasio.Id);
        Assert.NotNull(gimnasioEnDb);
        Assert.Equal("Gimnasio Test", gimnasioEnDb!.Nombre);
        Assert.Equal("Calle Test 123", gimnasioEnDb.Direccion);
        Assert.Equal(administrador.Id, gimnasioEnDb.AdministradorId);
    }

    //obtener lista de gimnasios
    [Fact]
    public async Task ObtenerTodosLosGimnasios_DeberiaRetornarListaCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerTodosLosGimnasiosDb");
        using var context = new FitRankDbContext(options);
        var gimnasioRepositorioMock = new GimnasioRepositorioImpl(context);

        context.Gimnasios.AddRange(
            new Gimnasio { Nombre = "Gimnasio 1" },
            new Gimnasio { Nombre = "Gimnasio 2" }
        );
        await context.SaveChangesAsync();

        // Act
        var gimnasios = await gimnasioRepositorioMock.ObtenerTodosLosGimnasios();

        // Assert
        Assert.Equal(2, gimnasios.Count);
        Assert.Contains(gimnasios, g => g.Nombre == "Gimnasio 1");
        Assert.Contains(gimnasios, g => g.Nombre == "Gimnasio 2");
    }

    //obtener lista de gimnasios vacia
    [Fact]
    public async Task ObtenerTodosLosGimnasios_DeberiaRetornarListaVacia()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerTodosLosGimnasiosVaciaDb");
        using var context = new FitRankDbContext(options);
        var gimnasioRepositorioMock = new GimnasioRepositorioImpl(context);

        // Act
        var gimnasios = await gimnasioRepositorioMock.ObtenerTodosLosGimnasios();

        // Assert
        Assert.Empty(gimnasios);
    }

    //obtener gimnasio por id
    [Fact]
    public async Task ObtenerGimnasioPorId_DeberiaRetornarGimnasioCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerGimnasioPorIdDb");
        using var context = new FitRankDbContext(options);
        var gimnasioRepositorioMock = new GimnasioRepositorioImpl(context);

        var gimnasio = new Gimnasio { Nombre = "Gimnasio Test" };
        context.Gimnasios.Add(gimnasio);
        await context.SaveChangesAsync();

        // Act
        var gimnasioObtenido = await gimnasioRepositorioMock.ObtenerGimnasioPorId(gimnasio.Id);

        // Assert
        Assert.NotNull(gimnasioObtenido);
        Assert.Equal("Gimnasio Test", gimnasioObtenido!.Nombre);
    }

    //obtener gimnasio por id inexistente
    [Fact]
    public async Task ObtenerGimnasioPorId_Inexistente_DeberiaRetornarNull()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerGimnasioPorIdInexistenteDb");
        using var context = new FitRankDbContext(options);
        var gimnasioRepositorioMock = new GimnasioRepositorioImpl(context);

        // Act
        var gimnasioObtenido = await gimnasioRepositorioMock.ObtenerGimnasioPorId(999);

        // Assert
        Assert.Null(gimnasioObtenido);
    }

    //actualizar gimnasio
    [Fact]
    public async Task ActualizarGimnasio_DeberiaActualizarCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("ActualizarGimnasioDb");
        using var context = new FitRankDbContext(options);
        var gimnasioRepositorioMock = new GimnasioRepositorioImpl(context);

        var gimnasio = new Gimnasio
        {
            Nombre = "Gimnasio Original",
            Direccion = "Direccion Original"
        };
        context.Gimnasios.Add(gimnasio);
        await context.SaveChangesAsync();

        // Act
        gimnasio.Nombre = "Gimnasio Actualizado";
        gimnasio.Direccion = "Direccion Actualizada";
        var gimnasioActualizado = await gimnasioRepositorioMock.ActualizarGimnasio(gimnasio);

        // FluentAssert
        Assert.NotNull(gimnasioActualizado);
        Assert.Equal("Gimnasio Actualizado", gimnasioActualizado!.Nombre);
        Assert.Equal("Direccion Actualizada", gimnasioActualizado.Direccion);

    }

    //actualizar gimnasio inexistente
    [Fact]
    public async Task ActualizarGimnasio_Inexistente_DeberiaRetornarNull()
    {
        // Arrange
        var options = CreateInMemoryOptions("ActualizarGimnasioInexistenteDb");
        using var context = new FitRankDbContext(options);
        var gimnasioRepositorioMock = new GimnasioRepositorioImpl(context);

        var gimnasio = new Gimnasio
        {
            Id = 999,
            Nombre = "Gimnasio Inexistente",
            Direccion = "Direccion Inexistente"
        };

        // Act
        var gimnasioActualizado = await gimnasioRepositorioMock.ActualizarGimnasio(gimnasio);

        // FluentAssert
        Assert.Null(gimnasioActualizado);
    }

    //eliminar gimnasio
    [Fact]
    public async Task EliminarGimnasio_DeberiaEliminarCorrectamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("EliminarGimnasioDb");
        using var context = new FitRankDbContext(options);
        var gimnasioRepositorioMock = new GimnasioRepositorioImpl(context);

        var gimnasio = new Gimnasio { Nombre = "Gimnasio a Eliminar" };
        context.Gimnasios.Add(gimnasio);
        await context.SaveChangesAsync();

        // Act
        var resultado = await gimnasioRepositorioMock.EliminarGimnasio(gimnasio.Id);

        // FluentAssert
        Assert.True(resultado);
        var gimnasioEnDb = await context.Gimnasios.FindAsync(gimnasio.Id);
        Assert.Null(gimnasioEnDb);
    }

    //elimminar gimnasio inexistente
    [Fact]
    public async Task EliminarGimnasio_Inexistente_DeberiaRetornarFalse()
    {
        // Arrange
        var options = CreateInMemoryOptions("EliminarGimnasioInexistenteDb");
        using var context = new FitRankDbContext(options);
        var gimnasioRepositorioMock = new GimnasioRepositorioImpl(context);
        // Act
        var resultado = await gimnasioRepositorioMock.EliminarGimnasio(999);
        // FluentAssert
        Assert.False(resultado);
    }

    [Fact]
    public async Task ObtenerPorAdministradorIdAsync_DeberiaRetornarGimnasioSiExiste()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerPorAdministradorIdAsyncDb");
        using var context = new FitRankDbContext(options);
        var repo = new GimnasioRepositorioImpl(context);

        var admin = new Administrador { Nombre = "Admin", Email = "admin@test.com" };
        context.Administradores.Add(admin);
        await context.SaveChangesAsync();

        var gimnasio = new Gimnasio
        {
            Nombre = "Gimnasio Admin",
            Direccion = "Calle 123",
            AdministradorId = admin.Id
        };
        context.Gimnasios.Add(gimnasio);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repo.ObtenerPorAdministradorIdAsync(admin.Id);

        // Assert
        Assert.NotNull(resultado);
        Assert.Equal("Gimnasio Admin", resultado!.Nombre);
        Assert.Equal(admin.Id, resultado.AdministradorId);
        Assert.NotNull(resultado.Administrador);
        Assert.Equal("Admin", resultado.Administrador!.Nombre);
    }

    [Fact]
    public async Task ObtenerPorAdministradorIdAsync_DeberiaRetornarNullSiNoExisteGimnasio()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerPorAdministradorIdAsyncVacioDb");
        using var context = new FitRankDbContext(options);
        var repo = new GimnasioRepositorioImpl(context);

        var admin = new Administrador { Nombre = "Admin", Email = "admin@test.com" };
        context.Administradores.Add(admin);
        await context.SaveChangesAsync();

        // Act
        var resultado = await repo.ObtenerPorAdministradorIdAsync(admin.Id);

        // Assert
        Assert.Null(resultado);
    }

    [Fact]
    public void ObtenerGimnasioIdPorUsuario_DeberiaRetornarIdParaAdministrador()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerGimnasioIdPorUsuarioAdminDb");
        using var context = new FitRankDbContext(options);
        var repo = new GimnasioRepositorioImpl(context);

        var admin = new Administrador { Nombre = "Admin", Email = "admin@test.com" };
        context.Administradores.Add(admin);
        context.SaveChanges();

        var gimnasio = new Gimnasio { Nombre = "Gimnasio Admin", AdministradorId = admin.Id };
        context.Gimnasios.Add(gimnasio);
        context.SaveChanges();

        // Act
        var resultado = repo.ObtenerGimnasioIdPorUsuario(admin.Id);

        // Assert
        Assert.Equal(gimnasio.Id, resultado);
    }

    [Fact]
    public void ObtenerGimnasioIdPorUsuario_DeberiaRetornarIdParaProfesor()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerGimnasioIdPorUsuarioProfesorDb");
        using var context = new FitRankDbContext(options);
        var repo = new GimnasioRepositorioImpl(context);

        var gimnasio = new Gimnasio { Nombre = "Gimnasio Profesor" };
        context.Gimnasios.Add(gimnasio);
        context.SaveChanges();

        var profesor = new Profesor { Nombre = "Profesor", Email = "prof@test.com", GimnasioId = gimnasio.Id };
        context.Profesores.Add(profesor);
        context.SaveChanges();

        // Act
        var resultado = repo.ObtenerGimnasioIdPorUsuario(profesor.Id);

        // Assert
        Assert.Equal(gimnasio.Id, resultado);
    }

    [Fact]
    public void ObtenerGimnasioIdPorUsuario_DeberiaRetornarIdParaSocio()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerGimnasioIdPorUsuarioSocioDb");
        using var context = new FitRankDbContext(options);
        var repo = new GimnasioRepositorioImpl(context);

        var gimnasio = new Gimnasio { Nombre = "Gimnasio Socio" };
        context.Gimnasios.Add(gimnasio);
        context.SaveChanges();

        var socio = new Socio { Nombre = "Socio", Email = "socio@test.com", GimnasioId = gimnasio.Id, Nivel = "Principiante" };
        context.Socios.Add(socio);
        context.SaveChanges();

        // Act
        var resultado = repo.ObtenerGimnasioIdPorUsuario(socio.Id);

        // Assert
        Assert.Equal(gimnasio.Id, resultado);
    }

    [Fact]
    public void ObtenerGimnasioIdPorUsuario_DeberiaRetornarNullSiUsuarioNoExiste()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerGimnasioIdPorUsuarioInexistenteDb");
        using var context = new FitRankDbContext(options);
        var repo = new GimnasioRepositorioImpl(context);

        // Act
        var resultado = repo.ObtenerGimnasioIdPorUsuario(999);

        // Assert
        Assert.Null(resultado);
    }

    [Fact]
    public async Task ActualizarPersonalizacion_DeberiaActualizarColoresYLogo()
    {
        // Arrange
        var options = CreateInMemoryOptions("ActualizarPersonalizacionDb");
        using var context = new FitRankDbContext(options);
        var repo = new GimnasioRepositorioImpl(context);

        var gimnasio = new Gimnasio
        {
            Nombre = "Gimnasio Test",
            ColorPrincipal = "Rojo",
            ColorSecundario = "Azul",
            LogoUrl = "logo_viejo.png"
        };
        context.Gimnasios.Add(gimnasio);
        await context.SaveChangesAsync();

        // Act
        var actualizado = await repo.ActualizarPersonalizacion(
            gimnasio.Id, "Verde", "Amarillo", "logo_nuevo.png");

        // Assert
        Assert.NotNull(actualizado);
        Assert.Equal("Verde", actualizado!.ColorPrincipal);
        Assert.Equal("Amarillo", actualizado.ColorSecundario);
        Assert.Equal("logo_nuevo.png", actualizado.LogoUrl);
    }

    [Fact]
    public async Task ActualizarPersonalizacion_DeberiaActualizarSoloColoresSiLogoEsNullOVacio()
    {
        // Arrange
        var options = CreateInMemoryOptions("ActualizarPersonalizacionSoloColoresDb");
        using var context = new FitRankDbContext(options);
        var repo = new GimnasioRepositorioImpl(context);

        var gimnasio = new Gimnasio
        {
            Nombre = "Gimnasio Test",
            ColorPrincipal = "Rojo",
            ColorSecundario = "Azul",
            LogoUrl = "logo_viejo.png"
        };
        context.Gimnasios.Add(gimnasio);
        await context.SaveChangesAsync();

        // Act
        var actualizado = await repo.ActualizarPersonalizacion(
            gimnasio.Id, "Negro", "Blanco", null);

        // Assert
        Assert.NotNull(actualizado);
        Assert.Equal("Negro", actualizado!.ColorPrincipal);
        Assert.Equal("Blanco", actualizado.ColorSecundario);
        Assert.Equal("logo_viejo.png", actualizado.LogoUrl);
    }

    [Fact]
    public async Task ActualizarPersonalizacion_DeberiaRetornarNullSiNoExisteGimnasio()
    {
        // Arrange
        var options = CreateInMemoryOptions("ActualizarPersonalizacionInexistenteDb");
        using var context = new FitRankDbContext(options);
        var repo = new GimnasioRepositorioImpl(context);

        // Act
        var actualizado = await repo.ActualizarPersonalizacion(999, "Verde", "Amarillo", "logo.png");

        // Assert
        Assert.Null(actualizado);
    }
}