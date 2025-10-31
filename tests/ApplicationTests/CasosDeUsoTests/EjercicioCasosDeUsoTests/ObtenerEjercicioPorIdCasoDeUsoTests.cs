using FitRank_API.Application.Mappings;
using FitRank_API.Infrastructure.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FitRank_API.Application.DTOs;
using FitRank_API.Domain.Entities;
using FluentAssertions;
using FitRank_API.Application.CasosDeUso.EjercicioCasosDeUso;

namespace ApplicationTests.CasosDeUsoTests.EjercicioCasosDeUsoTests;

public class ObtenerEjercicioPorIdCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IEjercicioRepositorio> _ejercicioRepositorioMock;

    public ObtenerEjercicioPorIdCasoDeUsoTests()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new EjercicioProfile());
        });
        _mapper = mappingConfig.CreateMapper();
        _ejercicioRepositorioMock = new Mock<IEjercicioRepositorio>();
    }

    //obtener ejercicio por id tiene exito
    [Fact]
    public async Task Ejecutar_DeberiaObtenerEjercicio_CuandoElEjercicioExiste()
    {
        // Arrange
        var ejercicioId = 1;
        var ejercicioExistente = new Ejercicio
        {
            Id = ejercicioId,
            Nombre = "Ejercicio Existente",
            Descripcion = "Descripcion del ejercicio existente",
            UrlImagen = "http://imagen.existente",
            DuracionEstimada = 30,
            UrlVideo = "http://video.existente",
            GrupoMuscularId = 1,
            MaquinaId = null
        };

        _ejercicioRepositorioMock.Setup(repo => repo.ObtenerEjercicioPorIdAsync(ejercicioId))
            .ReturnsAsync(ejercicioExistente);

        var obtenerEjercicioPorIdCasoDeUso = new ObtenerEjercicioPorIdCasoDeUso(_ejercicioRepositorioMock.Object, _mapper);

        // Act
        var resultado = await obtenerEjercicioPorIdCasoDeUso.Ejecutar(ejercicioId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(ejercicioExistente.Id);
        resultado.Nombre.Should().Be(ejercicioExistente.Nombre);
    }

    //obtener ejercicio por id falla porque no existe
    [Fact]
    public async Task Ejecutar_DeberiaFallarAlObtenerEjercicio_CuandoElEjercicioNoExiste()
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