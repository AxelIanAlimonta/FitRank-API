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

public class ObtenerEjerciciosAsignadosCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IEjercicioAsignadoRepositorio> _ejercicioAsignadoRepositorioMock;

    public ObtenerEjerciciosAsignadosCasoDeUsoTests()
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

    //obtener ejercicios asignados tiene exito
    [Fact]
    public async Task Ejecutar_DeberiaObtenerEjerciciosAsignados_CuandoExistenEjerciciosAsignados()
    {
        // Arrange
        var ejerciciosAsignadosExistentes = new List<EjercicioAsignado>
        {
            new EjercicioAsignado
            {
                Id = 1,
                NumeroEjercicio = 1,
                EjercicioId = 1,
                SesionId = 1,
            },
            new EjercicioAsignado
            {
                Id = 2,
                NumeroEjercicio = 2,
                EjercicioId = 2,
                SesionId = 1,
            }
        };

        _ejercicioAsignadoRepositorioMock.Setup(repo => repo.ObtenerTodosAsync())
            .ReturnsAsync(ejerciciosAsignadosExistentes);

        var obtenerEjerciciosAsignadosCasoDeUso = new ObtenerEjerciciosAsignadosCasoDeUso(_ejercicioAsignadoRepositorioMock.Object, _mapper);

        // Act
        var resultado = await obtenerEjerciciosAsignadosCasoDeUso.Ejecutar();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Count().Should().Be(ejerciciosAsignadosExistentes.Count);
    }   
}