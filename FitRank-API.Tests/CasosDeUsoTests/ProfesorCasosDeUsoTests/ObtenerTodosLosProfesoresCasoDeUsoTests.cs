using FitRank_API.Application.Mappings;
using FitRank_API.Domain.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FluentAssertions;
using FitRank_API.Application.CasosDeUso.ProfesorCasosDeUso;
using FitRank_API.Domain.Entities;

namespace CasosDeUsoTests.ProfesorCasosDeUsoTests;

public class ObtenerTodosLosProfesoresCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IProfesorRepositorio> _profesorRepositorioMock;

    public ObtenerTodosLosProfesoresCasoDeUsoTests()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new ProfesorProfile());
        });
        _mapper = mappingConfig.CreateMapper();
        _profesorRepositorioMock = new Mock<IProfesorRepositorio>();
    }

    [Fact]
    public async Task Ejecutar_DeberiaRetornarTodosLosProfesores_CuandoExistenProfesores()
    {
        // Arrange
        var profesoresEnLaBaseDeDatos = new List<Profesor>
        {
            new Profesor 
            { 
                Id = 1, 
                Nombre = "Pedro", 
                Apellido = "López", 
                Dni = 11111111,
                Email = "pedro@ejemplo.com",
                Sexo = "M",
                FechaNacimiento = new DateTime(1980, 1, 1),
                Matricula = "MAT001",
                Sueldo = 50000
            },
            new Profesor 
            { 
                Id = 2, 
                Nombre = "María", 
                Apellido = "Fernández", 
                Dni = 22222222,
                Email = "maria@ejemplo.com",
                Sexo = "F",
                FechaNacimiento = new DateTime(1985, 2, 2),
                Matricula = "MAT002",
                Sueldo = 55000
            },
            new Profesor 
            { 
                Id = 3, 
                Nombre = "Luis", 
                Apellido = "Martínez", 
                Dni = 33333333,
                Email = "luis@ejemplo.com",
                Sexo = "M",
                FechaNacimiento = new DateTime(1990, 3, 3),
                Matricula = "MAT003",
                Sueldo = 60000
            }
        };

        _profesorRepositorioMock.Setup(repo => repo.ObtenerTodosAsync())
            .ReturnsAsync(profesoresEnLaBaseDeDatos);

        var obtenerTodosLosProfesoresCasoDeUso = new ObtenerTodosLosProfesoresCasoDeUso(_profesorRepositorioMock.Object, _mapper);

        // Act
        var resultado = await obtenerTodosLosProfesoresCasoDeUso.Ejecutar();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Count.Should().Be(3);
        resultado[0].Id.Should().Be(1);
        resultado[0].Nombre.Should().Be("Pedro");
        resultado[1].Id.Should().Be(2);
        resultado[1].Nombre.Should().Be("María");
        resultado[2].Id.Should().Be(3);
        resultado[2].Nombre.Should().Be("Luis");
    }

    [Fact]
    public async Task Ejecutar_DeberiaRetornarListaVacia_CuandoNoExistenProfesores()
    {
        // Arrange
        var profesoresEnLaBaseDeDatos = new List<Profesor>();

        _profesorRepositorioMock.Setup(repo => repo.ObtenerTodosAsync())
            .ReturnsAsync(profesoresEnLaBaseDeDatos);

        var obtenerTodosLosProfesoresCasoDeUso = new ObtenerTodosLosProfesoresCasoDeUso(_profesorRepositorioMock.Object, _mapper);

        // Act
        var resultado = await obtenerTodosLosProfesoresCasoDeUso.Ejecutar();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Count.Should().Be(0);
    }
}
