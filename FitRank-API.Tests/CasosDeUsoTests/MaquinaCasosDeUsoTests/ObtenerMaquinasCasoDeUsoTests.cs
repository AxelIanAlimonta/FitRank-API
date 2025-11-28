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

public class ObtenerMaquinasCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IMaquinaRepositorio> _maquinaRepositorioMock;

    public ObtenerMaquinasCasoDeUsoTests()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new MaquinaProfile());
        });
        _mapper = mappingConfig.CreateMapper();
        _maquinaRepositorioMock = new Mock<IMaquinaRepositorio>();
    }

    //obtener maquinas exitoso
    [Fact]
    public async Task ObtenerMaquinas_CuandoExistenMaquinas_RetornaListaDeMaquinaDTO()
    {
        // Arrange
        var maquinasExistentes = new List<Maquina>
        {
            new Maquina
            {
                Id = 1,
                GimnasioId = 1,
                Nombre = "Maquina 1",
                UrlImagen = "http://imagen.1",
                Qr = "QR001",
            },
            new Maquina
            {
                Id = 2,
                GimnasioId = 1,
                Nombre = "Maquina 2",
                UrlImagen = "http://imagen.2",
                Qr = "QR002",
            }
        };

        _maquinaRepositorioMock
            .Setup(repo => repo.ObtenerTodasLasMaquinas())
            .ReturnsAsync(maquinasExistentes);

        var obtenerMaquinasCasoDeUso = new ObtenerMaquinasCasoDeUso(_maquinaRepositorioMock.Object, _mapper);

        // Act
        var resultado = await obtenerMaquinasCasoDeUso.Ejecutar();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(maquinasExistentes.Count);

        for (int i = 0; i < maquinasExistentes.Count; i++)
        {
            resultado[i].Id.Should().Be(maquinasExistentes[i].Id);
            resultado[i].GimnasioId.Should().Be(maquinasExistentes[i].GimnasioId);
            resultado[i].Nombre.Should().Be(maquinasExistentes[i].Nombre);
            resultado[i].UrlImagen.Should().Be(maquinasExistentes[i].UrlImagen);
            resultado[i].Qr.Should().Be(maquinasExistentes[i].Qr);
        }
    }


    //obtener maquinas vacio
    [Fact]
    public async Task ObtenerMaquinas_CuandoNoExistenMaquinas_RetornaListaVacia()
    {
        // Arrange
        var maquinasExistentes = new List<Maquina>();

        _maquinaRepositorioMock
            .Setup(repo => repo.ObtenerTodasLasMaquinas())
            .ReturnsAsync(maquinasExistentes);

        var obtenerMaquinasCasoDeUso = new ObtenerMaquinasCasoDeUso(_maquinaRepositorioMock.Object, _mapper);

        // Act
        var resultado = await obtenerMaquinasCasoDeUso.Ejecutar();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().BeEmpty();
    }
}