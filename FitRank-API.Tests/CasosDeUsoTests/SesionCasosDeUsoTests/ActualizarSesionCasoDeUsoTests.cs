using AutoMapper;
using FitRank_API.Application.CasosDeUso.SesionCasosDeUso;
using FitRank_API.Application.DTOs.SesionDTOs;
using FitRank_API.Application.Mappings;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace CasosDeUsoTests.SesionCasosDeUsoTests;

public class ActualizarSesionCasoDeUsoTests
{
    private readonly Mock<ISesionRepositorio> _sesionRepositorioMock;
    private readonly IMapper _mapper;
    private readonly ActualizarSesionCasoDeUso _actualizarSesionCasoDeUso;

    public ActualizarSesionCasoDeUsoTests()
    {
        _sesionRepositorioMock = new Mock<ISesionRepositorio>();
        _mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<SesionProfile>();
        }));
        _actualizarSesionCasoDeUso = new ActualizarSesionCasoDeUso(_sesionRepositorioMock.Object, _mapper);
    }

    //actualizar sesión exitosa
    [Fact]
    public async Task ActualizarSesionCasoDeUso_ActualizacionExitosa_RetornaObtenerSesionDTO()
    {
        // Arrange
        var sesionId = 1L;
        var actualizarSesionDTO = new ActualizarSesionDTO
        {
            Id = sesionId,
            NumeroDeSesion = 2,
            Nombre = "Sesión Actualizada",
            RutinaId = 1
        };
        var sesionEntidadActualizar = new Sesion
        {
            Id = sesionId,
            NumeroDeSesion = actualizarSesionDTO.NumeroDeSesion,
            Nombre = actualizarSesionDTO.Nombre,
            RutinaId = actualizarSesionDTO.RutinaId
        };
        var sesionEntidadActualizada = new Sesion
        {
            Id = sesionId,
            NumeroDeSesion = actualizarSesionDTO.NumeroDeSesion,
            Nombre = actualizarSesionDTO.Nombre,
            RutinaId = actualizarSesionDTO.RutinaId
        };
        _sesionRepositorioMock
            .Setup(repo => repo.ActualizarAsync(It.IsAny<Sesion>()))
            .ReturnsAsync(sesionEntidadActualizada);
        // Act
        var resultado = await _actualizarSesionCasoDeUso.Ejecutar(actualizarSesionDTO);
        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(sesionEntidadActualizada.Id);
        resultado.NumeroDeSesion.Should().Be(sesionEntidadActualizada.NumeroDeSesion);
        resultado.Nombre.Should().Be(sesionEntidadActualizada.Nombre);
        resultado.RutinaId.Should().Be(sesionEntidadActualizada.RutinaId);
    }

    //actualizar sesión inexistente
    [Fact]
    public async Task ActualizarSesionCasoDeUso_SesionInexistente_RetornaNull()
    {
        // Arrange
        var actualizarSesionDTO = new ActualizarSesionDTO
        {
            Id = 999,
            NumeroDeSesion = 2,
            Nombre = "Sesión Inexistente",
            RutinaId = 1
        };
        var sesionEntidadActualizar = new Sesion
        {
            Id = actualizarSesionDTO.Id,
            NumeroDeSesion = actualizarSesionDTO.NumeroDeSesion,
            Nombre = actualizarSesionDTO.Nombre,
            RutinaId = actualizarSesionDTO.RutinaId
        };
        _sesionRepositorioMock
            .Setup(repo => repo.ActualizarAsync(It.IsAny<Sesion>()))
            .ReturnsAsync((Sesion?)null);
        // Act
        var resultado = await _actualizarSesionCasoDeUso.Ejecutar(actualizarSesionDTO);
        // Assert
        resultado.Should().BeNull();
    }

}