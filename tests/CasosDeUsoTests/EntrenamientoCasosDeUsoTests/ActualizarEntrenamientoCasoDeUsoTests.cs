using FitRank_API.Infrastructure.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FitRank_API.Domain.Entities;
using FluentAssertions;
using FitRank_API.Application.MappingProfiles;
using FitRank_API.Application.DTOs.EntrenamientoDTOs;
using FitRank_API.Application.UseCases.Entrenamiento;

namespace CasosDeUsoTests.EntrenamientoCasosDeUsoTests;

public class ActualizarEntrenamientoCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IEntrenamientoRepositorio> _entrenamientoRepositorioMock;

    public ActualizarEntrenamientoCasoDeUsoTests()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new EntrenamientoProfile());
        });
        _mapper = mappingConfig.CreateMapper();
        _entrenamientoRepositorioMock = new Mock<IEntrenamientoRepositorio>();
    }

    [Fact]
    public async Task ActualizarEntrenamiento_CuandoLosDatosSonValidos_RetornaEntrenamientoDTO()
    {
        // Arrange
        var actualizarEntrenamientoDTO = new ActualizarEntrenamientoDTO
        {
            Id = 1,
            SocioId = 1,
            Duracion = new TimeSpan(2, 0, 0),
            Fecha = new DateTime(2023, 1, 22)
        };

        var entrenamientoExistente = new Entrenamiento
        {
            Id = actualizarEntrenamientoDTO.Id,
            SocioId = 1,
            Duracion = new TimeSpan(1, 0, 0),
            Fecha = new DateTime(2023, 1, 1)
        };

        var entrenamientoActualizado = new Entrenamiento
        {
            Id = actualizarEntrenamientoDTO.Id,
            SocioId = 1,
            Duracion = actualizarEntrenamientoDTO.Duracion,
            Fecha = actualizarEntrenamientoDTO.Fecha
        };

        _entrenamientoRepositorioMock
            .Setup(repo => repo.ObtenerPorIdAsync(actualizarEntrenamientoDTO.Id))
            .ReturnsAsync(entrenamientoExistente);

        _entrenamientoRepositorioMock
            .Setup(repo => repo.ActualizarAsync(It.IsAny<Entrenamiento>()))
            .ReturnsAsync(entrenamientoActualizado);

        var actualizarEntrenamientoCasoDeUso = new ActualizarEntrenamientoCasoDeUso(_entrenamientoRepositorioMock.Object, _mapper);

        // Act
        var resultado = await actualizarEntrenamientoCasoDeUso.Ejecutar(actualizarEntrenamientoDTO);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(entrenamientoActualizado.Id);
        resultado.Fecha.Should().Be(entrenamientoActualizado.Fecha);
        resultado.Duracion.Should().Be(entrenamientoActualizado.Duracion);
        resultado.SocioId.Should().Be(entrenamientoActualizado.SocioId);
    }

    [Fact]
    public async Task ActualizarEntrenamiento_CuandoElEntrenamientoNoExiste_DeberiaRetornarNull()
    {
        // Arrange
        var actualizarEntrenamientoDTO = new ActualizarEntrenamientoDTO
        {
            Id = 1,
            SocioId = 1,
            Duracion = new TimeSpan(2, 0, 0),
            Fecha = new DateTime(2023, 1, 22)
        };

        _entrenamientoRepositorioMock
            .Setup(repo => repo.ObtenerPorIdAsync(actualizarEntrenamientoDTO.Id))
            .ReturnsAsync((Entrenamiento?)null);

        var actualizarEntrenamientoCasoDeUso = new ActualizarEntrenamientoCasoDeUso(_entrenamientoRepositorioMock.Object, _mapper);

        // Act & Assert
        var resultado = await actualizarEntrenamientoCasoDeUso.Ejecutar(actualizarEntrenamientoDTO);
        resultado.Should().BeNull();
    }
}