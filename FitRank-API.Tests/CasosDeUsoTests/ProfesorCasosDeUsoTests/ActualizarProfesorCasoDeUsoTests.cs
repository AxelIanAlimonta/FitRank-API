using FitRank_API.Application.Mappings;
using FitRank_API.Infrastructure.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FluentAssertions;
using FitRank_API.Application.CasosDeUso.ProfesorCasosDeUso;
using FitRank_API.Application.DTOs.ProfesorDTOs;
using FitRank_API.Domain.Entities;

namespace CasosDeUsoTests.ProfesorCasosDeUsoTests;

public class ActualizarProfesorCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IProfesorRepositorio> _profesorRepositorioMock;

    public ActualizarProfesorCasoDeUsoTests()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new ProfesorProfile());
        });
        _mapper = mappingConfig.CreateMapper();
        _profesorRepositorioMock = new Mock<IProfesorRepositorio>();
    }

    [Fact]
    public async Task Ejecutar_DeberiaActualizarProfesor_CuandoLosDatosSonValidos()
    {
        // Arrange
        var profesorId = 1L;
        var profesorExistente = new Profesor
        {
            Id = profesorId,
            Nombre = "Juan",
            Apellido = "Pérez",
            Dni = 12345678,
            Email = "juan@ejemplo.com",
            Telefono = "123456789",
            Sexo = "M",
            FechaNacimiento = new DateTime(1990, 1, 1),
            Matricula = "MAT001",
            Sueldo = 50000,
            GimnasioId = 1
        };

        var actualizarProfesorDTO = new ActualizarProfesorDTO
        {
            Id = profesorId,
            Nombre = "Juan Actualizado",
            Apellido = "Pérez Actualizado",
            Dni = 12345678,
            Email = "juanactualizado@ejemplo.com",
            Telefono = "987654321",
            Sexo = "M",
            FechaNacimiento = new DateTime(1990, 1, 1),
            Matricula = "MAT002",
            Sueldo = 60000,
            GimnasioId = 2
        };

        _profesorRepositorioMock.Setup(repo => repo.ObtenerPorIdAsync(profesorId))
            .ReturnsAsync(profesorExistente);

        _profesorRepositorioMock.Setup(repo => repo.ActualizarAsync(It.IsAny<Profesor>()))
            .ReturnsAsync((Profesor p) => p);

        var actualizarProfesorCasoDeUso = new ActualizarProfesorCasoDeUso(_profesorRepositorioMock.Object, _mapper);

        // Act
        var resultado = await actualizarProfesorCasoDeUso.Ejecutar(profesorId, actualizarProfesorDTO);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(profesorId);
        resultado.Nombre.Should().Be("Juan Actualizado");
        resultado.Apellido.Should().Be("Pérez Actualizado");
        resultado.Email.Should().Be("juanactualizado@ejemplo.com");
        resultado.Matricula.Should().Be("MAT002");
        resultado.Sueldo.Should().Be(60000);
    }

    [Fact]
    public async Task Ejecutar_DeberiaRetornarNull_CuandoElProfesorNoExiste()
    {
        // Arrange
        var profesorId = 999L;
        var actualizarProfesorDTO = new ActualizarProfesorDTO
        {
            Id = profesorId,
            Nombre = "Inexistente",
            Apellido = "Test",
            Dni = 12345678,
            Email = "test@ejemplo.com",
            Sexo = "M",
            FechaNacimiento = new DateTime(1990, 1, 1),
            Matricula = "MAT999",
            Sueldo = 50000
        };

        _profesorRepositorioMock.Setup(repo => repo.ObtenerPorIdAsync(profesorId))
            .ReturnsAsync((Profesor?)null);

        var actualizarProfesorCasoDeUso = new ActualizarProfesorCasoDeUso(_profesorRepositorioMock.Object, _mapper);

        // Act
        var resultado = await actualizarProfesorCasoDeUso.Ejecutar(profesorId, actualizarProfesorDTO);

        // Assert
        resultado.Should().BeNull();
    }
}
