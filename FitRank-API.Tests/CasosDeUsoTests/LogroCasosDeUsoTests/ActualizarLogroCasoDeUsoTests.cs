using FitRank_API.Domain.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FitRank_API.Domain.Entities;
using FluentAssertions;
using FitRank_API.Application.CasosDeUso.LogroCasosDeUso;
using FitRank_API.Application.DTOs.LogroDTOs;
using FitRank_API.Application.Mappings;

namespace CasosDeUsoTests.LogroCasoDeUsoTests;

public class ActualizarLogroCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<ILogroRepositorio> _logroRepositorioMock;

    public ActualizarLogroCasoDeUsoTests()
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
    public async Task ActualizarLogro_CuandoLosDatosSonValidos_RetornaLogroDTO()
    {
        // Arrange
        var actualizarLogroDTO = new ActualizarLogroDTO
        {
            Id = 1,
            Nombre = "Nombre Actualizado",
            Descripcion = "Descripcion Actualizada",
            Imagen = "http://imagen.actualizada"
        };

        var logroExistente = new Logro
        {
            Id = actualizarLogroDTO.Id,
            Nombre = "Nombre Original",
            Descripcion = "Descripcion Original",
            Imagen = "http://imagen.original",
        };

        var logroActualizado = new Logro
        {
            Id = actualizarLogroDTO.Id,
            Nombre = "Nombre Actualizado",
            Descripcion = "Descripcion Actualizada",
            Imagen = "http://imagen.actualizada",
        };

        _logroRepositorioMock
            .Setup(repo => repo.ActualizarLogro(It.IsAny<Logro>()))
            .ReturnsAsync(logroActualizado);

        var actualizarLogroCasoDeUso = new ActualizarLogroCasoDeUso(
            _logroRepositorioMock.Object,
            _mapper
        );

        // Act
        var resultado = await actualizarLogroCasoDeUso.Ejecutar(actualizarLogroDTO);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(logroActualizado.Id);
        resultado.Nombre.Should().Be(actualizarLogroDTO.Nombre);
        resultado.Imagen.Should().Be(actualizarLogroDTO.Imagen);
    }

    [Fact]
    public async Task ActualizarLogro_CuandoLosDatosSonInvalidos_RetornaNull()
    {
        // Arrange
        var actualizarLogroDTO = new ActualizarLogroDTO
        {
            Id = 99, // Suponiendo que este ID no existe
            Nombre = "Nombre No Existente",
            Descripcion = "Descripcion No Existente",
            Imagen = "http://imagen.noexistente"
        };

        _logroRepositorioMock
            .Setup(repo => repo.ActualizarLogro(It.IsAny<Logro>()))
            .ReturnsAsync((Logro)null); // Simula que no se encuentra el logro

        var actualizarLogroCasoDeUso = new ActualizarLogroCasoDeUso(
            _logroRepositorioMock.Object,
            _mapper
        );

        // Act
        var resultado = await actualizarLogroCasoDeUso.Ejecutar(actualizarLogroDTO);

        // Assert
        resultado.Should().BeNull();
    }
}