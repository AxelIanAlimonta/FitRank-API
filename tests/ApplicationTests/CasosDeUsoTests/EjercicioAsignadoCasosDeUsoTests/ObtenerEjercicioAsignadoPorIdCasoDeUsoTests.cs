using FitRank_API.Infrastructure.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FitRank_API.Domain.Entities;
using FluentAssertions;
using FitRank_API.Application.CasosDeUso.EjercicioAsignadoCasoDeUso;
using FitRank_API.Application.DTOs.EjercicioAsignadoDTOs;
using FitRank_API.Application.Mappings;

namespace ApplicationTests.CasosDeUsoTests.EjercicioAsignadoCasoDeUsoTests;

public class ObtenerEjercicioAsignadoPorIdCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IEjercicioAsignadoRepositorio> _ejercicioAsignadoRepositorioMock;

    public ObtenerEjercicioAsignadoPorIdCasoDeUsoTests()
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

    //obtener ejercicio asignado por id tiene exito
    [Fact]
    public async Task Ejecutar_DeberiaObtenerEjercicioAsignado_CuandoElEjercicioAsignadoExiste()
    {
        // Arrange
        var ejercicioAsignadoId = 1;
        var ejercicioAsignadoExistente = new EjercicioAsignado
        {
            Id = ejercicioAsignadoId,
            NumeroEjercicio = 1,
            EjercicioId = 1,
            SesionId = 1,
        };

        _ejercicioAsignadoRepositorioMock.Setup(repo => repo.ObtenerPorIdAsync(ejercicioAsignadoId))
            .ReturnsAsync(ejercicioAsignadoExistente);

        var obtenerEjercicioAsignadoPorIdCasoDeUso = new ObtenerEjercicioAsignadoPorIdCasoDeUso(_ejercicioAsignadoRepositorioMock.Object, _mapper);

        // Act
        var resultado = await obtenerEjercicioAsignadoPorIdCasoDeUso.Ejecutar(ejercicioAsignadoId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(ejercicioAsignadoExistente.Id);
        resultado.NumeroEjercicio.Should().Be(ejercicioAsignadoExistente.NumeroEjercicio);
    }

    //obtener ejercicio asignado por id falla porque no existe
    [Fact]
    public async Task Ejecutar_DeberiaFallarAlObtenerEjercicioAsignado_CuandoElEjercicioAsignadoNoExiste()
    {
        // Arrange
        var ejercicioAsignadoId = 999; // ID que no existe

        _ejercicioAsignadoRepositorioMock.Setup(repo => repo.ObtenerPorIdAsync(ejercicioAsignadoId))
            .ReturnsAsync((EjercicioAsignado?)null);

        var obtenerEjercicioAsignadoPorIdCasoDeUso = new ObtenerEjercicioAsignadoPorIdCasoDeUso(_ejercicioAsignadoRepositorioMock.Object, _mapper);

        // Act
        var resultado = await obtenerEjercicioAsignadoPorIdCasoDeUso.Ejecutar(ejercicioAsignadoId);

        // Assert
        resultado.Should().BeNull();
    }
}