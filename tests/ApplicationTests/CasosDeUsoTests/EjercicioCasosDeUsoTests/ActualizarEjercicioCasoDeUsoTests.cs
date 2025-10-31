using FitRank_API.Application.Mappings;
using FitRank_API.Infrastructure.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FitRank_API.Application.DTOs;
using FitRank_API.Domain.Entities;
using FluentAssertions;
using FitRank_API.Application.DTOs.EjercicioDTOs.ActualizarEjercicioDTO;
using FitRank_API.Application.CasosDeUso.EjercicioCasosDeUso;

namespace ApplicationTests.CasosDeUsoTests.EjercicioCasosDeUsoTests;

public class ActualizarEjercicioCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IEjercicioRepositorio> _ejercicioRepositorioMock;

    public ActualizarEjercicioCasoDeUsoTests()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new EjercicioProfile());
        });
        _mapper = mappingConfig.CreateMapper();
        _ejercicioRepositorioMock = new Mock<IEjercicioRepositorio>();
    }

    //actualizar ejercicio tiene exito
    [Fact]
    public async Task Ejecutar_DeberiaActualizarEjercicio_CuandoLosDatosSonValidos()
    {
        // Arrange
        var ejercicioExistente = new Ejercicio
        {
            Id = 1,
            Nombre = "Ejercicio Original",
            Descripcion = "Descripcion Original",
            UrlImagen = "http://imagen.original",
            DuracionEstimada = 30,
            UrlVideo = "http://video.original",
            GrupoMuscularId = 1,
            MaquinaId = null
        };

        var ejercicioActualizadoDTO = new ActualizarEjercicioDTO
        {
            Id = 1,
            Nombre = "Ejercicio Actualizado",
            Descripcion = "Descripcion Actualizada",
            UrlImagen = "http://imagen.actualizada",
            DuracionEstimada = 45,
            UrlVideo = "http://video.actualizado",
            GrupoMuscularId = 2,
            MaquinaId = 1
        };

        _ejercicioRepositorioMock.Setup(repo => repo.ObtenerEjercicioPorIdAsync(1))
            .ReturnsAsync(ejercicioExistente);

        _ejercicioRepositorioMock.Setup(repo => repo.ActualizarEjercicioAsync(It.IsAny<Ejercicio>()))
            .ReturnsAsync((Ejercicio e) => e);

        var actualizarEjercicioCasoDeUso = new ActualizarEjercicioCasoDeUso(_ejercicioRepositorioMock.Object, _mapper);

        // Act
        var resultado = await actualizarEjercicioCasoDeUso.Ejecutar(ejercicioActualizadoDTO);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(1);
        resultado.Nombre.Should().Be("Ejercicio Actualizado");
        resultado.Descripcion.Should().Be("Descripcion Actualizada");
        resultado.UrlImagen.Should().Be("http://imagen.actualizada");
        resultado.DuracionEstimada.Should().Be(45);
        resultado.UrlVideo.Should().Be("http://video.actualizado");
        resultado.GrupoMuscularId.Should().Be(2);
        resultado.MaquinaId.Should().Be(1);
    }

    //actualizar ejercicio falla porque el ejercicio no existe
    [Fact]
    public async Task Ejecutar_DeberiaRetornarNull_CuandoElEjercicioNoExiste()
    {
        // Arrange
        var ejercicioActualizadoDTO = new ActualizarEjercicioDTO
        {
            Id = 99,
            Nombre = "Ejercicio No Existente",
            Descripcion = "Descripcion No Existente",
            UrlImagen = "http://imagen.noexistente",
            DuracionEstimada = 45,
            UrlVideo = "http://video.noexistente",
            GrupoMuscularId = 2,
            MaquinaId = 1
        };

        _ejercicioRepositorioMock.Setup(repo => repo.ObtenerEjercicioPorIdAsync(99))
            .ReturnsAsync((Ejercicio?)null);

        var actualizarEjercicioCasoDeUso = new ActualizarEjercicioCasoDeUso(_ejercicioRepositorioMock.Object, _mapper);

        // Act
        var resultado = await actualizarEjercicioCasoDeUso.Ejecutar(ejercicioActualizadoDTO);

        // Assert
        resultado.Should().BeNull();
    }


}