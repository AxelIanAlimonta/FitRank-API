using FitRank_API.Infrastructure.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FitRank_API.Domain.Entities;
using FluentAssertions;
using FitRank_API.Application.Mappings;
using FitRank_API.Application.DTOs.DiaDeLaSemanaDTOs;
using FitRank_API.Application.Mappings;
using FitRank_API.Application.CasosDeUso.DiaDeLaSemanaCasoDeUso;

namespace CasosDeUsoTests.DiaDeLaSemanaCasoDeUsoTests;

public class ActualizarDiaDeLaSemanaCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IDiaDeLaSemanaRepositorio> _diaDeLaSemanaRepositorioMock;

    public ActualizarDiaDeLaSemanaCasoDeUsoTests()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new DiaDeLaSemanaProfile());
        });
        _mapper = mappingConfig.CreateMapper();
        _diaDeLaSemanaRepositorioMock = new Mock<IDiaDeLaSemanaRepositorio>();
    }

    [Fact]
    public async Task ActualizarDiaDeLaSemana_CuandoLosDatosSonValidos_RetornaDiaDeLaSemanaDTO()
    {
        // Arrange
        var actualizarDiaDTO = new ActualizarDiaDeLaSemanaDTO
        {
            Id = 1,
            Nombre = "Lunes"
        };

        var diaExistente = new DiaDeLaSemana
        {
            Id = actualizarDiaDTO.Id,
            Nombre = "Domingo"
        };

        var diaActualizado = new DiaDeLaSemana
        {
            Id = actualizarDiaDTO.Id,
            Nombre = actualizarDiaDTO.Nombre
        };

        _diaDeLaSemanaRepositorioMock
            .Setup(repo => repo.ActualizarDiaDeLaSemanaAsync(It.IsAny<DiaDeLaSemana>()))
            .ReturnsAsync(diaActualizado);

        var actualizarDiaDeLaSemanaCasoDeUso = new ActualizarDiaDeLaSemanaCasoDeUso(_diaDeLaSemanaRepositorioMock.Object, _mapper);

        // Act
        var resultado = await actualizarDiaDeLaSemanaCasoDeUso.Ejecutar(actualizarDiaDTO);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(actualizarDiaDTO.Id);
        resultado.Nombre.Should().Be(actualizarDiaDTO.Nombre);
    }

    //actualizar dia de la semana returna null cuando no se encuentra
    [Fact]
    public async Task ActualizarDiaDeLaSemana_DeberiaRetornarNull_CuandoElDiaNoSeEncuentra()
    {
        // Arrange
        var actualizarDiaDTO = new ActualizarDiaDeLaSemanaDTO
        {
            Id = 1,
            Nombre = "Lunes"
        };
        _diaDeLaSemanaRepositorioMock
            .Setup(repo => repo.ActualizarDiaDeLaSemanaAsync(It.IsAny<DiaDeLaSemana>()))
            .ReturnsAsync((DiaDeLaSemana?)null);
        var actualizarDiaDeLaSemanaCasoDeUso = new ActualizarDiaDeLaSemanaCasoDeUso(_diaDeLaSemanaRepositorioMock.Object, _mapper);
        // Act
        var resultado = await actualizarDiaDeLaSemanaCasoDeUso.Ejecutar(actualizarDiaDTO);
        // Assert
        resultado.Should().BeNull();
    }

    //actualizar dia de la semana falla por dia no encontrado y retorna null
    [Fact]
    public async Task ActualizarDiaDeLaSemana_DeberiaFallar_CuandoElDiaNoExiste()
    {
        // Arrange
        var actualizarDiaDTO = new ActualizarDiaDeLaSemanaDTO
        {
            Id = 99,
            Nombre = "Lunes"
        };

        _diaDeLaSemanaRepositorioMock
            .Setup(repo => repo.ActualizarDiaDeLaSemanaAsync(It.IsAny<DiaDeLaSemana>()))
            .ReturnsAsync((DiaDeLaSemana?)null);

        var actualizarDiaDeLaSemanaCasoDeUso = new ActualizarDiaDeLaSemanaCasoDeUso(_diaDeLaSemanaRepositorioMock.Object, _mapper);

        // Act
        var resultado = await actualizarDiaDeLaSemanaCasoDeUso.Ejecutar(actualizarDiaDTO);

        // Assert
        resultado.Should().BeNull();
    }

}