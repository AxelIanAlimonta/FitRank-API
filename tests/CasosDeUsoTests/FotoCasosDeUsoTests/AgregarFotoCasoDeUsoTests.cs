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
}