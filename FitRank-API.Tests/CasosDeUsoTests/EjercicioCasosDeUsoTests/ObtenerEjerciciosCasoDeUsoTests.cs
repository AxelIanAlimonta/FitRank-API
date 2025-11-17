using FitRank_API.Application.Mappings;
using FitRank_API.Infrastructure.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FitRank_API.Application.DTOs;
using FitRank_API.Domain.Entities;
using FluentAssertions;
using FitRank_API.Application.CasosDeUso.EjercicioCasosDeUso;

namespace CasosDeUsoTests.EjercicioCasosDeUsoTests;

public class ObtenerEjerciciosCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IEjercicioRepositorio> _ejercicioRepositorioMock;

    public ObtenerEjerciciosCasoDeUsoTests()
    {
        _ejercicioRepositorioMock = new Mock<IEjercicioRepositorio>();
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new EjercicioProfile());
        });
        _mapper = mapperConfig.CreateMapper();
    }

    [Fact]
    public async Task Ejecutar_DeberiaRetornarEjercicio_CuandoElEjercicioExiste()
    {
        // Arrange
        var ejercicioId = 1;
        var ejercicio = new Ejercicio
        {
            Id = ejercicioId,
            Nombre = "Nuevo Ejercicio",
            Descripcion = "Descripcion del nuevo ejercicio",
            UrlImagen = "http://imagen.nuevo",
            DuracionEstimada = 25,
            UrlVideo = "http://video.nuevo",
            GrupoMuscularId = 1,
            MaquinaId = null
        };

        _ejercicioRepositorioMock.Setup(repo => repo.ObtenerEjercicioPorIdAsync(ejercicioId))
            .ReturnsAsync(ejercicio);

        var obtenerEjercicioPorIdCasoDeUso = new ObtenerEjercicioPorIdCasoDeUso(_ejercicioRepositorioMock.Object, _mapper);

        // Act
        var resultado = await obtenerEjercicioPorIdCasoDeUso.Ejecutar(ejercicioId);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(ejercicioId);
        resultado.Nombre.Should().Be("Nuevo Ejercicio");
    }

    [Fact]
    public async Task Ejecutar_DeberiaRetornarNull_CuandoElEjercicioNoExiste()
    {
        // Arrange
        var ejercicioId = 999; // ID que no existe

        _ejercicioRepositorioMock.Setup(repo => repo.ObtenerEjercicioPorIdAsync(ejercicioId))
            .ReturnsAsync((Ejercicio?)null);

        var obtenerEjercicioPorIdCasoDeUso = new ObtenerEjercicioPorIdCasoDeUso(_ejercicioRepositorioMock.Object, _mapper);

        // Act
        var resultado = await obtenerEjercicioPorIdCasoDeUso.Ejecutar(ejercicioId);

        // Assert
        resultado.Should().BeNull();
    }
}