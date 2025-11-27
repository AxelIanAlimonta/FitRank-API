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

public class ObtenerGimnasioPorIdCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IGimnasioRepositorio> _gimnasioRepositorioMock;

    public ObtenerGimnasioPorIdCasoDeUsoTests()
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

    //obtener gimnasio por id tiene exito
    [Fact]
    public async Task Ejecutar_DeberiaObtenerGimnasio_CuandoElGimnasioExiste()
    {
        // Arrange
        var gimnasioExistente = new Gimnasio
        {
            Id = 1,
            Nombre = "Gimnasio Existente",
            Direccion = "Direccion Existente",
            Telefono = "123456789",
            Email = "test@example.com",
            AdministradorId = 1
        };
        _gimnasioRepositorioMock.Setup(repo => repo.ObtenerGimnasioPorId(gimnasioExistente.Id))
            .ReturnsAsync(gimnasioExistente);

        // Act
        var obtenerGimnasioPorIdCasoDeUso = new ObtenerGimnasioPorIdCasoDeUso(_gimnasioRepositorioMock.Object, _mapper);
        var resultado = await obtenerGimnasioPorIdCasoDeUso.Ejecutar(gimnasioExistente.Id);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().BeOfType<ObtenerGimnasioDTO>();
        resultado.Id.Should().Be(gimnasioExistente.Id);
        resultado.Nombre.Should().Be(gimnasioExistente.Nombre);
        resultado.Direccion.Should().Be(gimnasioExistente.Direccion);
        resultado.Telefono.Should().Be(gimnasioExistente.Telefono);
        resultado.Email.Should().Be(gimnasioExistente.Email);
        resultado.AdministradorId.Should().Be(gimnasioExistente.AdministradorId);
    }

    //obtener gimnasio por id no existente
    [Fact]
    public async Task Ejecutar_DeberiaRetornarNull_CuandoElGimnasioNoExiste()
    {
        // Arrange
        var gimnasioIdInexistente = 999;
        _gimnasioRepositorioMock.Setup(repo => repo.ObtenerGimnasioPorId(gimnasioIdInexistente))
            .ReturnsAsync((Gimnasio?)null);

        // Act
        var obtenerGimnasioPorIdCasoDeUso = new ObtenerGimnasioPorIdCasoDeUso(_gimnasioRepositorioMock.Object, _mapper);
        var resultado = await obtenerGimnasioPorIdCasoDeUso.Ejecutar(gimnasioIdInexistente);

        // Assert
        resultado.Should().BeNull();
    }
}