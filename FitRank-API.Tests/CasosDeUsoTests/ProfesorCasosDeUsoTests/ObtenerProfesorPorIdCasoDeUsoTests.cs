using FitRank_API.Application.Mappings;
using FitRank_API.Infrastructure.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FluentAssertions;
using FitRank_API.Application.CasosDeUso.ProfesorCasosDeUso;
using FitRank_API.Domain.Entities;

namespace CasosDeUsoTests.ProfesorCasosDeUsoTests;

public class ObtenerProfesorPorIdCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IProfesorRepositorio> _profesorRepositorioMock;

    public ObtenerProfesorPorIdCasoDeUsoTests()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new ProfesorProfile());
        });
        _mapper = mappingConfig.CreateMapper();
        _profesorRepositorioMock = new Mock<IProfesorRepositorio>();
    }

    [Fact]
    public async Task Ejecutar_DeberiaObtenerProfesor_CuandoElProfesorExiste()
    {
        // Arrange
        var profesorId = 1L;
        var profesorExistente = new Profesor
        {
            Id = profesorId,
            Nombre = "Carlos",
            Apellido = "García",
            Dni = 87654321,
            Email = "carlos@ejemplo.com",
            Telefono = "111222333",
            Sexo = "M",
            FechaNacimiento = new DateTime(1985, 5, 15),
            Matricula = "MAT123",
            Sueldo = 55000,
            GimnasioId = 1
        };

        _profesorRepositorioMock.Setup(repo => repo.ObtenerPorIdAsync(profesorId))
            .ReturnsAsync(profesorExistente);

        var obtenerProfesorPorIdCasoDeUso = new ObtenerProfesorPorIdCasoDeUso(_profesorRepositorioMock.Object, _mapper);

        // Act
        var resultado = await obtenerProfesorPorIdCasoDeUso.Ejecutar(profesorId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(profesorExistente.Id);
        resultado.Nombre.Should().Be(profesorExistente.Nombre);
        resultado.Apellido.Should().Be(profesorExistente.Apellido);
        resultado.Email.Should().Be(profesorExistente.Email);
        resultado.Matricula.Should().Be(profesorExistente.Matricula);
        resultado.Sueldo.Should().Be(profesorExistente.Sueldo);
    }

    [Fact]
    public async Task Ejecutar_DeberiaRetornarNull_CuandoElProfesorNoExiste()
    {
        // Arrange
        var profesorId = 999L;

        _profesorRepositorioMock.Setup(repo => repo.ObtenerPorIdAsync(profesorId))
            .ReturnsAsync((Profesor?)null);

        var obtenerProfesorPorIdCasoDeUso = new ObtenerProfesorPorIdCasoDeUso(_profesorRepositorioMock.Object, _mapper);

        // Act
        var resultado = await obtenerProfesorPorIdCasoDeUso.Ejecutar(profesorId);

        // Assert
        resultado.Should().BeNull();
    }
}
