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

public class ProfesorRepositorioTests
{
    private DbContextOptions<FitRankDbContext> CreateInMemoryOptions(string dbName)
    {
        return new DbContextOptionsBuilder<FitRankDbContext>()
            .UseInMemoryDatabase(dbName)
            .Options;
    }

    private async Task<(Profesor, Gimnasio)> SeedData(FitRankDbContext context)
    {

        var gimnasio = new Gimnasio
        {
            Nombre = "Gimnasio Test",
            Direccion = "Calle Falsa 123",
            RazonSocial = "Gimnasio S.A.",
            LogoUrl = "http://logo.com/logo.png",
            ColorPrincipal = "#FFFFFF",
            ColorSecundario = "#000000",
            Email = "gymtest@gymt.com",
            Telefono = "123456789",
            Cuil = "20-12345678-9",
        };
        context.Gimnasios.Add(gimnasio);
        await context.SaveChangesAsync();

        var profesor = new Profesor
        {
            Nombre = "Juan",
            Apellido = "Perez",
            Matricula = "MAT123",
            Sueldo = 50000,
            GimnasioId = gimnasio.Id,
            Email = "profe@eml.com"
        };
        context.Profesores.Add(profesor);
        await context.SaveChangesAsync();
        return (profesor, gimnasio);

    }

    //obtener todos los profesores
    [Fact]
    public async Task ObtenerTodosAsync_RetornaTodosLosProfesores()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerTodosAsync_RetornaTodosLosProfesores");
        using var context = new FitRankDbContext(options);
        await SeedData(context);
        await SeedData(context); // Agregar dos profesores
        var repo = new ProfesorRepositorioImpl(context);
        // Act
        var profesores = await repo.ObtenerTodosAsync();
        // FluentAssert
        profesores.Should().HaveCount(2);
    }

    //obtener profesor por id
    [Fact]
    public async Task ObtenerPorIdAsync_RetornaProfesorCorrecto()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerPorIdAsync_RetornaProfesorCorrecto");
        using var context = new FitRankDbContext(options);
        var (profesor, _) = await SeedData(context);
        var repo = new ProfesorRepositorioImpl(context);
        // Act
        var resultado = await repo.ObtenerPorIdAsync(profesor.Id);
        // FluentAssert
        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(profesor.Id);
        resultado.Nombre.Should().Be(profesor.Nombre);
    }

    //obtener profesor por id inexistente
    [Fact]
    public async Task ObtenerPorIdAsync_RetornaNullParaIdInexistente()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerPorIdAsync_RetornaNullParaIdInexistente");
        using var context = new FitRankDbContext(options);
        await SeedData(context);
        var repo = new ProfesorRepositorioImpl(context);
        // Act
        var resultado = await repo.ObtenerPorIdAsync(9999); // ID inexistente
        // FluentAssert
        resultado.Should().BeNull();
    }

    //agregar profesor exitosamente
    [Fact]
    public async Task AgregarAsync_AgregaProfesorExitosamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("AgregarAsync_AgregaProfesorExitosamente");
        using var context = new FitRankDbContext(options);
        var repo = new ProfesorRepositorioImpl(context);
        var nuevoProfesor = new Profesor
        {
            Nombre = "Carlos",
            Apellido = "Lopez",
            Matricula = "MAT456",
            Sueldo = 60000,
            Email = "elmail@gmail.com",
        };
        // Act
        var agregado = await repo.AgregarAsync(nuevoProfesor);
        // FluentAssert
        agregado.Id.Should().BeGreaterThan(0);
        agregado.Nombre.Should().Be("Carlos");
        agregado.Rol.Should().Be("Profesor");
    }

    //actualizar profesor exitosamente
    [Fact]
    public async Task ActualizarAsync_ActualizaProfesorExitosamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("ActualizarAsync_ActualizaProfesorExitosamente");
        using var context = new FitRankDbContext(options);
        var (profesor, _) = await SeedData(context);
        var repo = new ProfesorRepositorioImpl(context);
        // Modificar algunos campos
        profesor.Nombre = "Pedro";
        profesor.Sueldo = 70000;
        // Act
        var actualizado = await repo.ActualizarAsync(profesor);
        // FluentAssert
        actualizado.Should().NotBeNull();
        actualizado!.Nombre.Should().Be("Pedro");
        actualizado.Sueldo.Should().Be(70000);
    }

    //actualizar profesor inexistente
    [Fact]
    public async Task ActualizarAsync_RetornaNullParaProfesorInexistente()
    {
        // Arrange
        var options = CreateInMemoryOptions("ActualizarAsync_RetornaNullParaProfesorInexistente");
        using var context = new FitRankDbContext(options);
        await SeedData(context);
        var repo = new ProfesorRepositorioImpl(context);
        var profesorInexistente = new Profesor
        {
            Id = 9999, // ID inexistente
            Nombre = "Inexistente",
            Apellido = "NoExiste",
            Matricula = "MAT000",
            Sueldo = 0,
            Email = "asdfasfd",
        };
        // Act
        var actualizado = await repo.ActualizarAsync(profesorInexistente);
        // FluentAssert
        actualizado.Should().BeNull();
    }

    //eliminar profesor exitosamente
    [Fact]
    public async Task EliminarAsync_EliminaProfesorExitosamente()
    {
        // Arrange
        var options = CreateInMemoryOptions("EliminarAsync_EliminaProfesorExitosamente");
        using var context = new FitRankDbContext(options);
        var (profesor, _) = await SeedData(context);
        var repo = new ProfesorRepositorioImpl(context);
        // Act
        var eliminado = await repo.EliminarAsync(profesor.Id);
        // FluentAssert
        eliminado.Should().BeTrue();
        var buscado = await repo.ObtenerPorIdAsync(profesor.Id);
        buscado.Should().BeNull();
    }

    //eliminar profesor inexistente
    [Fact]
    public async Task EliminarAsync_RetornaFalseParaProfesorInexistente()
    {
        // Arrange
        var options = CreateInMemoryOptions("EliminarAsync_RetornaFalseParaProfesorInexistente");
        using var context = new FitRankDbContext(options);
        await SeedData(context);
        var repo = new ProfesorRepositorioImpl(context);
        // Act
        var eliminado = await repo.EliminarAsync(9999); // ID inexistente
        // FluentAssert
        eliminado.Should().BeFalse();
    }

    //obtener por id gimnasio
    [Fact]
    public async Task ObtenerPorGimnasioIdAsync_RetornaProfesoresDelGimnasio()
    {
        // Arrange
        var options = CreateInMemoryOptions("ObtenerPorGimnasioIdAsync_RetornaProfesoresDelGimnasio");
        using var context = new FitRankDbContext(options);
        var (profesor1, gimnasio1) = await SeedData(context);
        var (profesor2, gimnasio2) = await SeedData(context); // Otro gimnasio
        var repo = new ProfesorRepositorioImpl(context);
        // Act
        var profesoresGimnasio1 = await repo.ObtenerPorGimnasioAsync(gimnasio1.Id);
        // FluentAssert
        profesoresGimnasio1.Should().HaveCount(1);
        profesoresGimnasio1.First().Id.Should().Be(profesor1.Id);
    }
}