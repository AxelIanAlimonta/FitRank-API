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
using FitRank_API.Application.DTOs.EjercicioDTOs.AgregarEjercicioDTO;

namespace ApplicationTests.CasosDeUsoTests.EjercicioCasosDeUsoTests;

public class AgregarEjercicioCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IEjercicioRepositorio> _ejercicioRepositorioMock;

    public AgregarEjercicioCasoDeUsoTests()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new EjercicioProfile());
        });
        _mapper = mappingConfig.CreateMapper();
        _ejercicioRepositorioMock = new Mock<IEjercicioRepositorio>();
    }

    //agregar ejercicio tiene exito
    [Fact]
    public async Task Ejecutar_DeberiaAgregarEjercicio_CuandoLosDatosSonValidos()
    {
        // Arrange
        var nuevoEjercicioDTO = new AgregarEjercicioDTO
        {
            Nombre = "Nuevo Ejercicio",
            Descripcion = "Descripcion del nuevo ejercicio",
            UrlImagen = "http://imagen.nuevo",
            DuracionEstimada = 25,
            UrlVideo = "http://video.nuevo",
            GrupoMuscularId = 1,
            MaquinaId = null
        };

        var ejercicioAgregado = new Ejercicio
        {
            Id = 1,
            Nombre = nuevoEjercicioDTO.Nombre,
            Descripcion = nuevoEjercicioDTO.Descripcion,
            UrlImagen = nuevoEjercicioDTO.UrlImagen,
            DuracionEstimada = nuevoEjercicioDTO.DuracionEstimada,
            UrlVideo = nuevoEjercicioDTO.UrlVideo,
            GrupoMuscularId = nuevoEjercicioDTO.GrupoMuscularId,
            MaquinaId = nuevoEjercicioDTO.MaquinaId
        };

        _ejercicioRepositorioMock.Setup(repo => repo.AgregarEjercicioAsync(It.IsAny<Ejercicio>()))
            .ReturnsAsync(ejercicioAgregado);

        var agregarEjercicioCasoDeUso = new AgregarEjercicioCasoDeUso(_ejercicioRepositorioMock.Object, _mapper);

        // Act
        var resultado = await agregarEjercicioCasoDeUso.Ejecutar(nuevoEjercicioDTO);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(1);
        resultado.Nombre.Should().Be(nuevoEjercicioDTO.Nombre);
    }
}