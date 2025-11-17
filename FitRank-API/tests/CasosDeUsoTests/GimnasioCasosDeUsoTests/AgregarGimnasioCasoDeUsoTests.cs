using FitRank_API.Infrastructure.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FitRank_API.Domain.Entities;
using FluentAssertions;
using FitRank_API.Application.CasosDeUso.GimnasioCasosDeUso;
using FitRank_API.Application.DTOs.GimnasioDTOs;
using FitRank_API.Application.Mappings;

namespace CasosDeUsoTests.GimnasioCasoDeUsoTests;

public class AgregarGimnasioCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IGimnasioRepositorio> _gimnasioRepositorioMock;
    private readonly Mock<IAdministradorRepositorio> _adminRepositorioMock;

    public AgregarGimnasioCasoDeUsoTests()
    {
        var mappingConfig = new MapperConfiguration(
            mc =>
            {
                mc.AddProfile(new GimnasioProfile());
            }
        );
        _mapper = mappingConfig.CreateMapper();
        _gimnasioRepositorioMock = new Mock<IGimnasioRepositorio>();
        _adminRepositorioMock = new Mock<IAdministradorRepositorio>();
    }

    [Fact]
    public async Task Ejecutar_DeberiaAgregarGimnasio_CuandoLosDatosSonValidos()
    {
        // Arrange
        var nuevoGimnasioDTO = new AgregarGimnasioDTO
        {
            Nombre = "Gimnasio Nuevo",
            Direccion = "Direccion Nueva",
            Telefono = "987654321",
            Email = "nuevo@example.com",
            AdministradorId = 1
        };

        var gimnasioAgregado = new Gimnasio
        {
            Id = 1,
            Nombre = nuevoGimnasioDTO.Nombre,
            Direccion = nuevoGimnasioDTO.Direccion,
            Telefono = nuevoGimnasioDTO.Telefono,
            Email = nuevoGimnasioDTO.Email,
            AdministradorId = nuevoGimnasioDTO.AdministradorId
        };

        _gimnasioRepositorioMock.Setup(repo => repo.AgregarGimnasio(It.IsAny<Gimnasio>()))
            .ReturnsAsync(gimnasioAgregado);

        var agregarGimnasioCasoDeUso = new AgregarGimnasioCasoDeUso(_gimnasioRepositorioMock.Object, _mapper, _adminRepositorioMock.Object);

        // Act
        var resultado = await agregarGimnasioCasoDeUso.Ejecutar(nuevoGimnasioDTO);
        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().BeGreaterThan(0);
        resultado.Nombre.Should().Be(nuevoGimnasioDTO.Nombre);
        resultado.Direccion.Should().Be(nuevoGimnasioDTO.Direccion);
        resultado.Telefono.Should().Be(nuevoGimnasioDTO.Telefono);
        resultado.Email.Should().Be(nuevoGimnasioDTO.Email);

    }
}