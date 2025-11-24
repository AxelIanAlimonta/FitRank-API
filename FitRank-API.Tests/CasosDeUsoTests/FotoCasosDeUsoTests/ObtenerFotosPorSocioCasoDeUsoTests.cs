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

    [Fact]
    public async Task DebeMapearCorrectamenteTodosLosCamposDeCadaFoto()
    {
        // Arrange
        long socioId = 25;
        var fotos = new List<Foto>
        {
            new Foto
            {
                Id = 10,
                Fecha = new DateTime(2024, 1, 15),
                UrlImagen = "https://storage.com/foto1.jpg",
                SocioId = socioId
            },
            new Foto
            {
                Id = 20,
                Fecha = new DateTime(2024, 2, 20),
                UrlImagen = "https://storage.com/foto2.png",
                SocioId = socioId
            }
        };

        _fotoRepositorioMock
            .Setup(repo => repo.ObtenerPorSocioAsync(socioId))
            .ReturnsAsync(fotos);

        var casoDeUso = new ObtenerFotosPorSocioCasoDeUso(_fotoRepositorioMock.Object, _mapper);

        // Act
        var resultado = await casoDeUso.Ejecutar(socioId);

        // Assert
        resultado.Should().HaveCount(2);
        resultado[0].Id.Should().Be(10);
        resultado[0].Fecha.Should().Be(new DateTime(2024, 1, 15));
        resultado[0].UrlImagen.Should().Be("https://storage.com/foto1.jpg");
        resultado[0].SocioId.Should().Be(socioId);
        resultado[1].Id.Should().Be(20);
        resultado[1].Fecha.Should().Be(new DateTime(2024, 2, 20));
        resultado[1].UrlImagen.Should().Be("https://storage.com/foto2.png");
        resultado[1].SocioId.Should().Be(socioId);
    }

    [Fact]
    public async Task DebeLlamarRepositorioConSocioIdCorrecto()
    {
        // Arrange
        long socioId = 777;

        _fotoRepositorioMock
            .Setup(repo => repo.ObtenerPorSocioAsync(socioId))
            .ReturnsAsync(new List<Foto>());

        var casoDeUso = new ObtenerFotosPorSocioCasoDeUso(_fotoRepositorioMock.Object, _mapper);

        // Act
        await casoDeUso.Ejecutar(socioId);

        // Assert
        _fotoRepositorioMock.Verify(repo => repo.ObtenerPorSocioAsync(socioId), Times.Once);
    }

    [Fact]
    public async Task DeberiaRetornarFotosEnElMismoOrdenDelRepositorio()
    {
        // Arrange
        long socioId = 100;
        var fotos = new List<Foto>
        {
            new Foto { Id = 3, Fecha = new DateTime(2024, 3, 1), UrlImagen = "url3", SocioId = socioId },
            new Foto { Id = 1, Fecha = new DateTime(2024, 1, 1), UrlImagen = "url1", SocioId = socioId },
            new Foto { Id = 2, Fecha = new DateTime(2024, 2, 1), UrlImagen = "url2", SocioId = socioId }
        };

        _fotoRepositorioMock
            .Setup(repo => repo.ObtenerPorSocioAsync(socioId))
            .ReturnsAsync(fotos);

        var casoDeUso = new ObtenerFotosPorSocioCasoDeUso(_fotoRepositorioMock.Object, _mapper);

        // Act
        var resultado = await casoDeUso.Ejecutar(socioId);

        // Assert
        resultado.Should().HaveCount(3);
        resultado[0].Id.Should().Be(3);
        resultado[1].Id.Should().Be(1);
        resultado[2].Id.Should().Be(2);
    }

    [Fact]
    public async Task DeberiaRetornarListaConUnaFotoCuandoSocioTieneSoloUnaFoto()
    {
        // Arrange
        long socioId = 5;
        var fotos = new List<Foto>
        {
            new Foto
            {
                Id = 100,
                Fecha = DateTime.UtcNow,
                UrlImagen = "https://example.com/single.jpg",
                SocioId = socioId
            }
        };

        _fotoRepositorioMock
            .Setup(repo => repo.ObtenerPorSocioAsync(socioId))
            .ReturnsAsync(fotos);

        var casoDeUso = new ObtenerFotosPorSocioCasoDeUso(_fotoRepositorioMock.Object, _mapper);

        // Act
        var resultado = await casoDeUso.Ejecutar(socioId);

        // Assert
        resultado.Should().HaveCount(1);
        resultado[0].Id.Should().Be(100);
        resultado[0].UrlImagen.Should().Be("https://example.com/single.jpg");
    }
}