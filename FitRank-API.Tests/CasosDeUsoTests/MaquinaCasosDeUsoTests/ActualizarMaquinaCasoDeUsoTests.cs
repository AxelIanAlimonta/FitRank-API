using FitRank_API.Application.Mappings;
using FitRank_API.Domain.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FitRank_API.Domain.Entities;
using FluentAssertions;
using FitRank_API.Application.DTOs.MaquinaDTOs;
using FitRank_API.Application.CasosDeUso.MaquinaCasosDeUso;

namespace CasosDeUsoTests.MaquinaCasosDeUsoTests;

public class ActualizarMaquinaCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IMaquinaRepositorio> _maquinaRepositorioMock;

    public ActualizarMaquinaCasoDeUsoTests()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new MaquinaProfile());
        });
        _mapper = mappingConfig.CreateMapper();
        _maquinaRepositorioMock = new Mock<IMaquinaRepositorio>();
    }

    //actualizar maquina exitoso
    [Fact]
    public async Task ActualizarMaquina_CuandoLosDatosSonValidos_RetornaMaquinaDTO()
    {
        // Arrange
        var actualizarMaquinaDTO = new ActualizarMaquinaDTO
        {
            Id = 1,
            GimnasioId = 1,
            Nombre = "Maquina Original",
            UrlImagen = "http://imagen.original",
            Qr = "QR123",
        };

        var maquinaExistente = new Maquina
        {
            Id = actualizarMaquinaDTO.Id,
            GimnasioId = 1,
            Nombre = "Maquina Original",
            UrlImagen = "http://imagen.original",
            Qr = "QR123",

        };

        var maquinaActualizada = new Maquina
        {
            Id = actualizarMaquinaDTO.Id,
            GimnasioId = actualizarMaquinaDTO.GimnasioId,
            Nombre = actualizarMaquinaDTO.Nombre,
            UrlImagen = actualizarMaquinaDTO.UrlImagen,
            Qr = actualizarMaquinaDTO.Qr,
        };

        _maquinaRepositorioMock
            .Setup(repo => repo.ActualizarMaquina(It.IsAny<Maquina>()))
            .ReturnsAsync(maquinaActualizada);

        var actualizarMaquinaCasoDeUso = new ActualizarMaquinaCasoDeUso(_maquinaRepositorioMock.Object, _mapper);

        // Act
        var resultado = await actualizarMaquinaCasoDeUso.Ejecutar(actualizarMaquinaDTO);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(maquinaActualizada.Id);
        resultado.GimnasioId.Should().Be(maquinaActualizada.GimnasioId);
        resultado.Nombre.Should().Be(maquinaActualizada.Nombre);
        resultado.UrlImagen.Should().Be(maquinaActualizada.UrlImagen);
        resultado.Qr.Should().Be(maquinaActualizada.Qr);
    }

    //actualizar maquina no existente
    [Fact]
    public async Task ActualizarMaquina_CuandoLaMaquinaNoExiste_RetornaNull()
    {
        // Arrange
        var actualizarMaquinaDTO = new ActualizarMaquinaDTO
        {
            Id = 99,
            GimnasioId = 1,
            Nombre = "Maquina No Existente",
            UrlImagen = "http://imagen.noexistente",
            Qr = "QR999",
        };

        _maquinaRepositorioMock
            .Setup(repo => repo.ActualizarMaquina(It.IsAny<Maquina>()))
            .ReturnsAsync((Maquina?)null);

        var actualizarMaquinaCasoDeUso = new ActualizarMaquinaCasoDeUso(_maquinaRepositorioMock.Object, _mapper);

        // Act
        var resultado = await actualizarMaquinaCasoDeUso.Ejecutar(actualizarMaquinaDTO);

        // Assert
        resultado.Should().BeNull();
    }

}