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

public class ObtenerMaquinaPorIdCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IMaquinaRepositorio> _maquinaRepositorioMock;

    public ObtenerMaquinaPorIdCasoDeUsoTests()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new MaquinaProfile());
        });
        _mapper = mappingConfig.CreateMapper();
        _maquinaRepositorioMock = new Mock<IMaquinaRepositorio>();
    }

    //obtener maquina por id exitoso
    [Fact]
    public async Task ObtenerMaquinaPorId_CuandoElIdExiste_RetornaMaquinaDTO()
    {
        // Arrange
        var maquinaId = 1;

        var maquinaExistente = new Maquina
        {
            Id = maquinaId,
            GimnasioId = 1,
            Nombre = "Maquina Existente",
            UrlImagen = "http://imagen.existente",
            Qr = "QR123",
        };

        _maquinaRepositorioMock
            .Setup(repo => repo.ObtenerMaquinaPorId(maquinaId))
            .ReturnsAsync(maquinaExistente);

        var obtenerMaquinaPorIdCasoDeUso = new ObtenerMaquinaPorIdCasoDeUso(_maquinaRepositorioMock.Object, _mapper);

        // Act
        var resultado = await obtenerMaquinaPorIdCasoDeUso.Ejecutar(maquinaId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(maquinaExistente.Id);
        resultado.GimnasioId.Should().Be(maquinaExistente.GimnasioId);
        resultado.Nombre.Should().Be(maquinaExistente.Nombre);
        resultado.UrlImagen.Should().Be(maquinaExistente.UrlImagen);
        resultado.Qr.Should().Be(maquinaExistente.Qr);
    }

    //obtener maquina por id no existente
    [Fact]
    public async Task ObtenerMaquinaPorId_CuandoElIdNoExiste_RetornaNull()
    {
        // Arrange
        var maquinaId = 999; // ID que no existe

        _maquinaRepositorioMock
            .Setup(repo => repo.ObtenerMaquinaPorId(maquinaId))
            .ReturnsAsync((Maquina?)null);

        var obtenerMaquinaPorIdCasoDeUso = new ObtenerMaquinaPorIdCasoDeUso(_maquinaRepositorioMock.Object, _mapper);

        // Act
        var resultado = await obtenerMaquinaPorIdCasoDeUso.Ejecutar(maquinaId);

        // Assert
        resultado.Should().BeNull();
    }
}