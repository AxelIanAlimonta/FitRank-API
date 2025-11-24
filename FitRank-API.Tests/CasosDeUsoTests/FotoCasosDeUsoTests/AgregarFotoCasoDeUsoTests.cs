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

public class AgregarFotoCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IFotoRepositorio> _fotoRepositorioMock;

    public AgregarFotoCasoDeUsoTests()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new FotoProfile());
        });
        _mapper = mappingConfig.CreateMapper();
        _fotoRepositorioMock = new Mock<IFotoRepositorio>();
    }

    [Fact]
    public async Task AgregarFoto_CuandoLosDatosSonValidos_RetornaFotoDTO()
    {
        // Arrange
        var nuevaFotoDTO = new AgregarFotoDTO
        {
            Fecha = new DateTime(2023, 1, 1),
            UrlImagen = "http://nueva.foto",
            SocioId = 1
        };

        var fotoAGuardar = new Foto
        {
            Fecha = new DateTime(2023, 1, 1),
            UrlImagen = "http://nueva.foto",
            SocioId = 1
        };


        var fotoGuardada = new Foto
        {
            Fecha = new DateTime(2023, 1, 1),
            UrlImagen = "http://nueva.foto",
            SocioId = 1
        };

        _fotoRepositorioMock.Setup(repo => repo.AgregarAsync(It.IsAny<Foto>()))
            .ReturnsAsync(fotoGuardada);

        var agregarFotoCasoDeUso = new AgregarFotoCasoDeUso(_fotoRepositorioMock.Object, _mapper);

        // Act
        var resultado = await agregarFotoCasoDeUso.Ejecutar(nuevaFotoDTO);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Fecha.Should().Be(nuevaFotoDTO.Fecha);
        resultado.UrlImagen.Should().Be(nuevaFotoDTO.UrlImagen);
        resultado.SocioId.Should().Be(nuevaFotoDTO.SocioId);
    }

    [Fact]
    public async Task DebeMapearCorrectamenteTodosLosCampos()
    {
        // Arrange
        var dto = new AgregarFotoDTO
        {
            Fecha = new DateTime(2024, 6, 15, 10, 30, 0),
            UrlImagen = "https://example.com/foto.jpg",
            SocioId = 123
        };

        _fotoRepositorioMock.Setup(repo => repo.AgregarAsync(It.IsAny<Foto>()))
            .ReturnsAsync((Foto f) => f);

        var casoDeUso = new AgregarFotoCasoDeUso(_fotoRepositorioMock.Object, _mapper);

        // Act
        var resultado = await casoDeUso.Ejecutar(dto);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Fecha.Should().Be(dto.Fecha);
        resultado.UrlImagen.Should().Be(dto.UrlImagen);
        resultado.SocioId.Should().Be(dto.SocioId);
    }

    [Fact]
    public async Task DebeLlamarRepositorioConDatosCorrectos()
    {
        // Arrange
        var dto = new AgregarFotoDTO
        {
            Fecha = DateTime.UtcNow,
            UrlImagen = "https://storage.com/image.png",
            SocioId = 999
        };

        _fotoRepositorioMock.Setup(repo => repo.AgregarAsync(It.IsAny<Foto>()))
            .ReturnsAsync((Foto f) => f);

        var casoDeUso = new AgregarFotoCasoDeUso(_fotoRepositorioMock.Object, _mapper);

        // Act
        await casoDeUso.Ejecutar(dto);

        // Assert
        _fotoRepositorioMock.Verify(repo => repo.AgregarAsync(
            It.Is<Foto>(f => f.SocioId == dto.SocioId && 
                            f.UrlImagen == dto.UrlImagen && 
                            f.Fecha == dto.Fecha)), 
            Times.Once);
    }

    [Fact]
    public async Task DeberiaManteneFechaOriginalDelDTO()
    {
        // Arrange
        var fechaEspecifica = new DateTime(2022, 3, 10, 14, 25, 30);
        var dto = new AgregarFotoDTO
        {
            Fecha = fechaEspecifica,
            UrlImagen = "http://test.com/image.jpg",
            SocioId = 50
        };

        _fotoRepositorioMock.Setup(repo => repo.AgregarAsync(It.IsAny<Foto>()))
            .ReturnsAsync((Foto f) => f);

        var casoDeUso = new AgregarFotoCasoDeUso(_fotoRepositorioMock.Object, _mapper);

        // Act
        var resultado = await casoDeUso.Ejecutar(dto);

        // Assert
        resultado.Fecha.Should().Be(fechaEspecifica);
    }

    [Fact]
    public async Task DeberiaRetornarTipoObtenerFotoDTO()
    {
        // Arrange
        var dto = new AgregarFotoDTO
        {
            Fecha = DateTime.UtcNow,
            UrlImagen = "http://example.com/photo.jpg",
            SocioId = 1
        };

        _fotoRepositorioMock.Setup(repo => repo.AgregarAsync(It.IsAny<Foto>()))
            .ReturnsAsync((Foto f) => f);

        var casoDeUso = new AgregarFotoCasoDeUso(_fotoRepositorioMock.Object, _mapper);

        // Act
        var resultado = await casoDeUso.Ejecutar(dto);

        // Assert
        resultado.Should().BeOfType<ObtenerFotoDTO>();
    }
}