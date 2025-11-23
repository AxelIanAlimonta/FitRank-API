using FitRank_API.Infrastructure.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FitRank_API.Domain.Entities;
using FluentAssertions;
using FitRank_API.Application.Mappings;
using FitRank_API.Application.DTOs.ActividadDTOs;
using FitRank_API.Application.UseCases.Actividad;

namespace CasosDeUsoTests.ActividadCasosDeUsoTests;

public class ObtenerActividadesCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IActividadRepositorio> _actividadRepositorioMock;

    public ObtenerActividadesCasoDeUsoTests()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new ActividadProfile());
        });
        _mapper = mappingConfig.CreateMapper();
        _actividadRepositorioMock = new Mock<IActividadRepositorio>();
    }


    [Fact]
    public async Task ObtenerActividades_CuandoExistenActividades_RetornaListaDeActividadDTO()
    {
        // Arrange
        var actividadesEnLaBaseDeDatos = new List<Actividad>
        {
            new Actividad
            {
                Id = 1,
                Repeticiones = 10,
                Peso = 50.5,
                Punto = 20.0,
                EjercicioAsignadoId = 2,
                EntrenamientoId = 3,
                SerieId = 4
            },
            new Actividad
            {
                Id = 2,
                Repeticiones = 15,
                Peso = 60.0,
                Punto = 25.0,
                EjercicioAsignadoId = 3,
                EntrenamientoId = 4,
                SerieId = 5
            }
        };

        _actividadRepositorioMock
            .Setup(repo => repo.ObtenerTodasAsync())
            .ReturnsAsync(actividadesEnLaBaseDeDatos);

        var obtenerActividadesCasoDeUso = new ObtenerActividadesCasoDeUso(_actividadRepositorioMock.Object, _mapper);

        // Act
        var resultado = await obtenerActividadesCasoDeUso.Ejecutar();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Count().Should().Be(actividadesEnLaBaseDeDatos.Count);
        for (int i = 0; i < actividadesEnLaBaseDeDatos.Count; i++)
        {
            resultado.ElementAt(i).Id.Should().Be(actividadesEnLaBaseDeDatos[i].Id);
            resultado.ElementAt(i).Repeticiones.Should().Be(actividadesEnLaBaseDeDatos[i].Repeticiones);
            resultado.ElementAt(i).Peso.Should().Be(actividadesEnLaBaseDeDatos[i].Peso);
            resultado.ElementAt(i).Punto.Should().Be(actividadesEnLaBaseDeDatos[i].Punto);
            resultado.ElementAt(i).EjercicioAsignadoId.Should().Be(actividadesEnLaBaseDeDatos[i].EjercicioAsignadoId);
            resultado.ElementAt(i).EntrenamientoId.Should().Be(actividadesEnLaBaseDeDatos[i].EntrenamientoId);
            resultado.ElementAt(i).SerieId.Should().Be(actividadesEnLaBaseDeDatos[i].SerieId);
        }
    }

    [Fact]
    public async Task ObtenerActividades_CuandoNoExistenActividades_RetornaListaVacia()
    {
        // Arrange
        var actividadesEnLaBaseDeDatos = new List<Actividad>();

        _actividadRepositorioMock
            .Setup(repo => repo.ObtenerTodasAsync())
            .ReturnsAsync(actividadesEnLaBaseDeDatos);

        var obtenerActividadesCasoDeUso = new ObtenerActividadesCasoDeUso(_actividadRepositorioMock.Object, _mapper);

        // Act
        var resultado = await obtenerActividadesCasoDeUso.Ejecutar();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Count().Should().Be(0);
    }

}