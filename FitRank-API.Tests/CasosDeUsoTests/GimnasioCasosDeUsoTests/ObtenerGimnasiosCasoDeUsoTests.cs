using FitRank_API.Domain.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FitRank_API.Domain.Entities;
using FluentAssertions;
using FitRank_API.Application.CasosDeUso.GimnasioCasosDeUso;
using FitRank_API.Application.DTOs.GimnasioDTOs;
using FitRank_API.Application.Mappings;

namespace CasosDeUsoTests.GimnasioCasoDeUsoTests;

public class ObtenerGimnasiosCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IGimnasioRepositorio> _gimnasioRepositorioMock;

    public ObtenerGimnasiosCasoDeUsoTests()
    {
        var mappingConfig = new MapperConfiguration(
            mc =>
            {
                mc.AddProfile(new GimnasioProfile());
            }
        );
        _mapper = mappingConfig.CreateMapper();
        _gimnasioRepositorioMock = new Mock<IGimnasioRepositorio>();
    }

    //obtener lista de gimnasios tiene exito
    [Fact]
    public async Task Ejecutar_DeberiaObtenerListaDeGimnasios_CuandoExistenGimnasios()
    {
        // Arrange
        var gimnasiosExistentes = new List<Gimnasio>
        {
            new Gimnasio
            {
                Id = 1,
                Nombre = "Gimnasio 1",
                Direccion = "Direccion 1",
                Telefono = "123456789",
                Email = "gimnasio1@example.com",
                AdministradorId = 1
            }
        };
        _gimnasioRepositorioMock.Setup(repo => repo.ObtenerTodosLosGimnasios())
            .ReturnsAsync(gimnasiosExistentes);

        // Act
        var obtenerGimnasiosCasoDeUso = new ObtenerGimnasiosCasoDeUso(_gimnasioRepositorioMock.Object, _mapper);
        var resultado = await obtenerGimnasiosCasoDeUso.Ejecutar();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().BeOfType<List<ObtenerGimnasioDTO>>();
        resultado.Count.Should().Be(gimnasiosExistentes.Count);
        resultado[0].Id.Should().Be(gimnasiosExistentes[0].Id);
        resultado[0].Nombre.Should().Be(gimnasiosExistentes[0].Nombre);
        resultado[0].Direccion.Should().Be(gimnasiosExistentes[0].Direccion);
        resultado[0].Telefono.Should().Be(gimnasiosExistentes[0].Telefono);
        resultado[0].Email.Should().Be(gimnasiosExistentes[0].Email);
        resultado[0].AdministradorId.Should().Be(gimnasiosExistentes[0].AdministradorId);
    }

    //obtener lista de gimnasios vacia
    [Fact]
    public async Task Ejecutar_DeberiaRetornarListaVacia_CuandoNoExistenGimnasios()
    {
        // Arrange
        var gimnasiosExistentes = new List<Gimnasio>();
        _gimnasioRepositorioMock.Setup(repo => repo.ObtenerTodosLosGimnasios())
            .ReturnsAsync(gimnasiosExistentes);

        // Act
        var obtenerGimnasiosCasoDeUso = new ObtenerGimnasiosCasoDeUso(_gimnasioRepositorioMock.Object, _mapper);
        var resultado = await obtenerGimnasiosCasoDeUso.Ejecutar();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().BeOfType<List<ObtenerGimnasioDTO>>();
        resultado.Count.Should().Be(0);
    }
}