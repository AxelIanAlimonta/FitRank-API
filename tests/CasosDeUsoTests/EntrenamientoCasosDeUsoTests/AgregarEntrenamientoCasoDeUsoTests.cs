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

public class AgregarEntrenamientoCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IEntrenamientoRepositorio> _entrenamientoRepositorioMock;

    public AgregarEntrenamientoCasoDeUsoTests()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new EntrenamientoProfile());
        });
        _mapper = mappingConfig.CreateMapper();
        _entrenamientoRepositorioMock = new Mock<IEntrenamientoRepositorio>();
    }

    // [Fact]
    // public async Task AgregarEntrenamiento_CuandoLosDatosSonValidos_RetornaEntrenamientoDTO()
    // {
    //     // Arrange
    //     var nuevoEntrenamientoDTO = new AgregarEntrenamientoDTO
    //     {
    //         SocioId = 1,
    //         Duracion = new DateTime(2023, 1, 22),
    //         Fecha = new DateTime(2023, 1, 22)
    //     };

    //     var entrenamientoAGuardar = new Entrenamiento
    //     {
    //         SocioId = nuevoEntrenamientoDTO.SocioId,
    //         Duracion = nuevoEntrenamientoDTO.Duracion,
    //         Fecha = nuevoEntrenamientoDTO.Fecha
    //     };

    //     var entrenamientoGuardado = new Entrenamiento
    //     {
    //         Id = 1,
    //         SocioId = nuevoEntrenamientoDTO.SocioId,
    //         Duracion = nuevoEntrenamientoDTO.Duracion,
    //         Fecha = nuevoEntrenamientoDTO.Fecha
    //     };

    //     _entrenamientoRepositorioMock
    //         .Setup(repo => repo.AgregarAsync(It.IsAny<Entrenamiento>()))
    //         .ReturnsAsync(entrenamientoGuardado);

    //     var casoDeUso = new AgregarEntrenamientoCasoDeUso(_entrenamientoRepositorioMock.Object, _mapper);

    //     // Act
    //     var resultado = await casoDeUso.Ejecutar(nuevoEntrenamientoDTO);

    //     // Assert
    //     resultado.Should().NotBeNull();
    //     resultado.Id.Should().Be(entrenamientoGuardado.Id);
    //     resultado.SocioId.Should().Be(entrenamientoGuardado.SocioId);
    //     resultado.Duracion.Should().Be(entrenamientoGuardado.Duracion);
    //     resultado.Fecha.Should().Be(entrenamientoGuardado.Fecha);
    // }
}