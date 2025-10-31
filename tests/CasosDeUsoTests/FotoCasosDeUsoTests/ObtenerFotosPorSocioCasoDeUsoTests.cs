using FitRank_API.Application.Mappings;
using FitRank_API.Infrastructure.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FluentAssertions;
using FitRank_API.Application.CasosDeUso.FotoCasosDeUso;
using FitRank_API.Application.DTOs.FotoDTOs;
using FitRank_API.Domain.Entities;

namespace CasosDeUsoTests.FotoCasosDeUsoTests;

public class ObtenerFotosPorSocioCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IFotoRepositorio> _fotoRepositorioMock;

    public ObtenerFotosPorSocioCasoDeUsoTests()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new FotoProfile());
        });
        _mapper = mappingConfig.CreateMapper();
        _fotoRepositorioMock = new Mock<IFotoRepositorio>();
    }

    [Fact]
    public async Task ObtenerFotosPorSocio_CuandoExistenFotos_RetornaListaDeFotoDTO()
    {
        // Arrange
        int socioId = 1;
        var fotosEnLaBaseDeDatos = new List<Foto>
        {
            new Foto
            {
                Id = 1,
                Fecha = new DateTime(2023, 1, 1),
                UrlImagen = "http://foto1.url",
                SocioId = socioId
            },
            new Foto
            {
                Id = 2,
                Fecha = new DateTime(2023, 2, 1),
                UrlImagen = "http://foto2.url",
                SocioId = socioId
            }
        };

        _fotoRepositorioMock
            .Setup(repo => repo.ObtenerPorSocioAsync(socioId))
            .ReturnsAsync(fotosEnLaBaseDeDatos);

        var obtenerFotosPorSocioCasoDeUso = new ObtenerFotosPorSocioCasoDeUso(_fotoRepositorioMock.Object, _mapper);

        // Act
        var resultado = await obtenerFotosPorSocioCasoDeUso.Ejecutar(socioId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(2);
        resultado[0].Id.Should().Be(1);
        resultado[0].UrlImagen.Should().Be("http://foto1.url");
        resultado[1].Id.Should().Be(2);
        resultado[1].UrlImagen.Should().Be("http://foto2.url");
    }

    [Fact]
    public async Task ObtenerFotosPorSocio_CuandoNoExistenFotos_RetornaListaVacia()
    {
        // Arrange
        int socioId = 99;

        _fotoRepositorioMock
            .Setup(repo => repo.ObtenerPorSocioAsync(socioId))
            .ReturnsAsync(new List<Foto>());

        var obtenerFotosPorSocioCasoDeUso = new ObtenerFotosPorSocioCasoDeUso(_fotoRepositorioMock.Object, _mapper);

        // Act
        var resultado = await obtenerFotosPorSocioCasoDeUso.Ejecutar(socioId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().BeEmpty();
    }
}