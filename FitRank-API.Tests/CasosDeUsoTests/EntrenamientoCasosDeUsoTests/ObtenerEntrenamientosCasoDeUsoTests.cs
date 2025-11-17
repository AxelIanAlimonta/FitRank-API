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

public class ObtenerEntrenamientosCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IEntrenamientoRepositorio> _entrenamientoRepositorioMock;

    public ObtenerEntrenamientosCasoDeUsoTests()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new EntrenamientoProfile());
        });
        _mapper = mappingConfig.CreateMapper();
        _entrenamientoRepositorioMock = new Mock<IEntrenamientoRepositorio>();
    }

    //obtener lista
    // [Fact]
    // public async Task ObtenerEntrenamientos_CuandoExistenEntrenamientos_RetornaListaDeEntrenamientoDTO()
    // {
    //     // Arrange
    //     var entrenamientosExistentes = new List<Entrenamiento>
    //     {
    //         new Entrenamiento
    //         {
    //             Id = 1,
    //             SocioId = 1,
    //             Duracion = new DateTime(2023, 1, 22),
    //             Fecha = new DateTime(2023, 1, 22)
    //         },
    //         new Entrenamiento
    //         {
    //             Id = 2,
    //             SocioId = 2,
    //             Duracion = new DateTime(2023, 2, 15),
    //             Fecha = new DateTime(2023, 2, 15)
    //         }
    //     };

    //     _entrenamientoRepositorioMock
    //         .Setup(repo => repo.ObtenerTodosAsync())
    //         .ReturnsAsync(entrenamientosExistentes);

    //     var casoDeUso = new ObtenerEntrenamientosCasoDeUso(_entrenamientoRepositorioMock.Object, _mapper);

    //     // Act
    //     var resultado = await casoDeUso.Ejecutar();

    //     // Assert
    //     resultado.Should().NotBeNull();
    //     resultado.Should().HaveCount(entrenamientosExistentes.Count);
    // }

    //obtener lista vacia
    [Fact]
    public async Task ObtenerEntrenamientos_CuandoNoExistenEntrenamientos_RetornaListaVacia()
    {
        // Arrange
        var entrenamientosExistentes = new List<Entrenamiento>();

        _entrenamientoRepositorioMock
            .Setup(repo => repo.ObtenerTodosAsync())
            .ReturnsAsync(entrenamientosExistentes);

        var casoDeUso = new ObtenerEntrenamientosCasoDeUso(_entrenamientoRepositorioMock.Object, _mapper);

        // Act
        var resultado = await casoDeUso.Ejecutar();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().BeEmpty();
    }
}