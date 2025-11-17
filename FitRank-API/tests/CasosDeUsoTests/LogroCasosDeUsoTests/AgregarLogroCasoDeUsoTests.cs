using FitRank_API.Infrastructure.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FitRank_API.Domain.Entities;
using FluentAssertions;
using FitRank_API.Application.CasosDeUso.LogroCasosDeUso;
using FitRank_API.Application.DTOs.LogroDTOs;
using FitRank_API.Application.Mappings;

namespace CasosDeUsoTests.LogroCasoDeUsoTests;

public class AgregarLogroCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<ILogroRepositorio> _logroRepositorioMock;

    public AgregarLogroCasoDeUsoTests()
    {
        var mappingConfig = new MapperConfiguration(
            mc =>
            {
                mc.AddProfile(new LogroProfile());
            }
        );
        _mapper = mappingConfig.CreateMapper();
        _logroRepositorioMock = new Mock<ILogroRepositorio>();
    }

    [Fact]
    public async Task AgregarLogro_CuandoLosDatosSonValidos_RetornaLogroDTO()
    {
        // Arrange
        var nuevoLogroDTO = new AgregarLogroDTO
        {
            Nombre = "Nuevo Logro",
            Descripcion = "Descripcion del nuevo logro",
            Imagen = "http://imagen.nuevo"
        };

        var logroCreado = new Logro
        {
            Id = 1,
            Nombre = nuevoLogroDTO.Nombre,
            Descripcion = nuevoLogroDTO.Descripcion,
            Imagen = nuevoLogroDTO.Imagen,
        };

        _logroRepositorioMock
            .Setup(repo => repo.AgregarLogro(It.IsAny<Logro>()))
            .ReturnsAsync(logroCreado);

        var agregarLogroCasoDeUso = new AgregarLogroCasoDeUso(
            _logroRepositorioMock.Object,
            _mapper
        );

        // Act
        var resultado = await agregarLogroCasoDeUso.Ejecutar(nuevoLogroDTO);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(logroCreado.Id);
        resultado.Nombre.Should().Be(nuevoLogroDTO.Nombre);
        resultado.Imagen.Should().Be(nuevoLogroDTO.Imagen);
    }
}