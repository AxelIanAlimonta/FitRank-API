using AutoMapper;
using FitRank_API.Application.CasosDeUso.SesionCasosDeUso;
using FitRank_API.Application.DTOs.SesionDTOs;
using FitRank_API.Application.Mappings;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace CasosDeUsoTests.SesionCasosDeUsoTests;

public class AgregarSesionCasoDeUsoTests
{
    private readonly Mock<ISesionRepositorio> _sesionRepositorioMock;
    private readonly IMapper _mapper;
    private readonly AgregarSesionCasoDeUso _agregarSesionCasoDeUso;

    public AgregarSesionCasoDeUsoTests()
    {
        _sesionRepositorioMock = new Mock<ISesionRepositorio>();
        _mapper = new Mapper(new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<SesionProfile>();
        }));
        _agregarSesionCasoDeUso = new AgregarSesionCasoDeUso(_sesionRepositorioMock.Object, _mapper);
    }

    [Fact]
    public async Task AgregarSesionCasoDeUso_CreacionExitosa_RetornaObtenerSesionDTO()
    {
        // Arrange
        var agregarSesionDTO = new AgregarSesionDTO
        {
            NumeroDeSesion = 1,
            Nombre = "Sesión de Prueba",
            RutinaId = 1
        };
        var sesionEntidad = new Sesion
        {
            Id = 1,
            NumeroDeSesion = agregarSesionDTO.NumeroDeSesion,
            Nombre = agregarSesionDTO.Nombre,
            RutinaId = agregarSesionDTO.RutinaId
        };
        _sesionRepositorioMock
            .Setup(repo => repo.AgregarAsync(It.IsAny<Sesion>()))
            .ReturnsAsync(sesionEntidad);
        // Act
        var resultado = await _agregarSesionCasoDeUso.Ejecutar(agregarSesionDTO);
        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(sesionEntidad.Id);
        resultado.NumeroDeSesion.Should().Be(sesionEntidad.NumeroDeSesion);
        resultado.Nombre.Should().Be(sesionEntidad.Nombre);
        resultado.RutinaId.Should().Be(sesionEntidad.RutinaId);
    }

}