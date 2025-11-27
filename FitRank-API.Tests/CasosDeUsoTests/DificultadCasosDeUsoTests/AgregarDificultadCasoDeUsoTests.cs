using FitRank_API.Application.Mappings;
using FitRank_API.Domain.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FluentAssertions;
using FitRank_API.Application.CasosDeUso.DificultadCasosDeUso;
using FitRank_API.Application.DTOs.DificultadDTOs;
using FitRank_API.Domain.Entities;

namespace CasosDeUsoTests.DificultadCasosDeUsoTests;

public class AgregarDificultadCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IDificultadRepositorio> _dificultadRepositorioMock;

    public AgregarDificultadCasoDeUsoTests()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new DificultadProfile());
        });
        _mapper = mappingConfig.CreateMapper();
        _dificultadRepositorioMock = new Mock<IDificultadRepositorio>();
    }

    [Fact]
    public async Task Ejecutar_DeberiaAgregarDificultad_CuandoLosDatosSonValidos()
    {
        // Arrange
        var nuevaDificultadDTO = new AgregarDificultadDTO
        {
            Descripcion = "Principiante"
        };

        var dificultadAgregada = new Dificultad
        {
            Id = 1,
            Descripcion = nuevaDificultadDTO.Descripcion
        };

        _dificultadRepositorioMock.Setup(repo => repo.AgregarAsync(It.IsAny<Dificultad>()))
            .ReturnsAsync(dificultadAgregada);

        var agregarDificultadCasoDeUso = new AgregarDificultadCasoDeUso(_dificultadRepositorioMock.Object, _mapper);

        // Act
        var resultado = await agregarDificultadCasoDeUso.Ejecutar(nuevaDificultadDTO);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(1);
        resultado.Descripcion.Should().Be(nuevaDificultadDTO.Descripcion);
    }
}
