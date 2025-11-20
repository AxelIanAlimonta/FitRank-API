using FitRank_API.Infrastructure.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FitRank_API.Domain.Entities;
using FluentAssertions;
using FitRank_API.Application.CasosDeUso.EjercicioAsignadoCasoDeUso;
using FitRank_API.Application.DTOs.EjercicioAsignadoDTOs;
using FitRank_API.Application.Mappings;

namespace CasosDeUsoTests.EjercicioAsignadoCasoDeUsoTests;

public class AgregarEjercicioAsignadoCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IEjercicioAsignadoRepositorio> _ejercicioAsignadoRepositorioMock;

    public AgregarEjercicioAsignadoCasoDeUsoTests()
    {
        var mappingConfig = new MapperConfiguration(
            mc =>
            {
                mc.AddProfile(new EjercicioAsignadoProfile());
            }
        );
        _mapper = mappingConfig.CreateMapper();
        _ejercicioAsignadoRepositorioMock = new Mock<IEjercicioAsignadoRepositorio>();
    }

    //agregar ejercicio asignado tiene exito
    [Fact]
    public async Task Ejecutar_DeberiaAgregarEjercicioAsignado_CuandoLosDatosSonValidos()
    {
        // Arrange
        var agregarEjercicioAsignadoDTO = new AgregarEjercicioAsignadoDTO
        {
            NumeroEjercicio = 1,
            EjercicioId = 1,
            SesionId = 1,
        };

        var ejercicioAsignadoCreado = new EjercicioAsignado
        {
            Id = 1,
            NumeroEjercicio = agregarEjercicioAsignadoDTO.NumeroEjercicio,
            EjercicioId = agregarEjercicioAsignadoDTO.EjercicioId,
            SesionId = agregarEjercicioAsignadoDTO.SesionId,
        };

        _ejercicioAsignadoRepositorioMock.Setup(repo => repo.AgregarAsync(It.IsAny<EjercicioAsignado>()))
            .ReturnsAsync(ejercicioAsignadoCreado);

        var agregarEjercicioAsignadoCasoDeUso = new AgregarEjercicioAsignadoCasoDeUso(_ejercicioAsignadoRepositorioMock.Object, _mapper);

        // Act
        var resultado = await agregarEjercicioAsignadoCasoDeUso.Ejecutar(agregarEjercicioAsignadoDTO);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(ejercicioAsignadoCreado.Id);
        resultado.NumeroEjercicio.Should().Be(ejercicioAsignadoCreado.NumeroEjercicio);
    }
}

