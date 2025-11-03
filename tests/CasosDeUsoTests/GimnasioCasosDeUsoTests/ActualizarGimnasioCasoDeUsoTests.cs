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

public class ActualizarGimnasioCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IGimnasioRepositorio> _gimnasioRepositorioMock;

    public ActualizarGimnasioCasoDeUsoTests()
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

    //actualizar gimnasio tiene exito
    [Fact]
    public async Task Ejecutar_DeberiaActualizarGimnasio_CuandoLosDatosSonValidos()
    {
        //gimnasio existente
        var gimnasioExistente = new Gimnasio
        {
            Id = 1,
            Nombre = "Gimnasio Existente",
            Direccion = "Direccion Existente",
            Telefono = "123456789",
            Email = "test@example.com",
            AdministradorId = 1
        };

        // gimnasio actualizado DTO
        var gimnasioActualizadoDTO = new ActualizarGimnasioDTO
        {
            Id = 1,
            Nombre = "Gimnasio Actualizado",
            Direccion = "Direccion Actualizada",
            Telefono = "987654321",
            Email = "actualizado@example.com",
            AdministradorId = 1
        };

        //gimnasio actualizado entity
        var gimnasioActualizado = new Gimnasio
        {
            Id = gimnasioActualizadoDTO.Id,
            Nombre = gimnasioActualizadoDTO.Nombre,
            Direccion = gimnasioActualizadoDTO.Direccion,
            Telefono = gimnasioActualizadoDTO.Telefono,
            Email = gimnasioActualizadoDTO.Email,
            AdministradorId = gimnasioActualizadoDTO.AdministradorId
        };

        //obtener gimnasio DTO
        var gimnasioDTO = new ObtenerGimnasioDTO
        {
            Id = gimnasioActualizado.Id,
            Nombre = gimnasioActualizado.Nombre,
            Direccion = gimnasioActualizado.Direccion,
            Telefono = gimnasioActualizado.Telefono,
            Email = gimnasioActualizado.Email,
            AdministradorId = gimnasioActualizado.AdministradorId
        };


        _gimnasioRepositorioMock.Setup(repo => repo.ActualizarGimnasio(It.IsAny<Gimnasio>()))
            .ReturnsAsync(gimnasioActualizado);
        var actualizarGimnasioCasoDeUso = new ActualizarGimnasioCasoDeUso(_gimnasioRepositorioMock.Object, _mapper);
        // Act
        var resultado = await actualizarGimnasioCasoDeUso.Ejecutar(gimnasioActualizadoDTO);
        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(gimnasioDTO.Id);
        resultado.Nombre.Should().Be(gimnasioDTO.Nombre);
        resultado.Direccion.Should().Be(gimnasioDTO.Direccion);
    }

    //actualizar gimnasio falla porque no existe
    [Fact]
    public async Task Ejecutar_DeberiaFallarAlActualizarGimnasio_CuandoElGimnasioNoExiste()
    {
        // Arrange
        var gimnasioActualizadoDTO = new ActualizarGimnasioDTO
        {
            Id = 1,
            Nombre = "Gimnasio Actualizado",
            Direccion = "Direccion Actualizada",
            Telefono = "987654321",
            Email = "actualizado@example.com",

            AdministradorId = 1
        };
        _gimnasioRepositorioMock.Setup(repo => repo.ObtenerGimnasioPorId(1))
            .ReturnsAsync((Gimnasio?)null);
        var actualizarGimnasioCasoDeUso = new ActualizarGimnasioCasoDeUso(_gimnasioRepositorioMock.Object, _mapper);
        // Act
        var resultado = await actualizarGimnasioCasoDeUso.Ejecutar(gimnasioActualizadoDTO);
        // Assert
        resultado.Should().BeNull();
    }
}