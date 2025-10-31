using FitRank_API.Application.Mappings;
using FitRank_API.Infrastructure.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FitRank_API.Application.DTOs;
using FitRank_API.Domain.Entities;
using FluentAssertions;
using FitRank_API.Application.DTOs.MedidaCorporalDTOs;
using FitRank_API.Application.CasosDeUso.MedidaCorporalCasosDeUso;

namespace CasosDeUsoTests.MedidaCorporalCasosDeUsoTests;

public class ActualizarMedidaCorporalCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IMedidaCorporalRepositorio> _medidaCorporalRepositorioMock;

    public ActualizarMedidaCorporalCasoDeUsoTests()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new MedidaCorporalProfile());
        });
        _mapper = mappingConfig.CreateMapper();
        _medidaCorporalRepositorioMock = new Mock<IMedidaCorporalRepositorio>();
    }

    //actualizar medida corporal exitoso
    [Fact]
    public async Task ActualizarMedidaCorporal_CuandoLosDatosSonValidos_RetornaMedidaCorporalDTO()
    {
        // Arrange
        var actualizarMedidaCorporalDTO = new ActualizarMedidaCorporalDTO
        {
            Id = 1,
            BrazoDerechoCm = 35.0,
            BrazoIzquierdoCm = 34.5,
            CaderaCm = 95.0,
            CinturaCm = 85.0,
            PechoCm = 100.0,
            PesoKg = 70.0,
            Fecha = new DateTime(2024, 6, 15),
            SocioId = 1
        };

        var medidaCorporalExistente = new MedidaCorporal
        {
            Id = 1,
            BrazoDerechoCm = 36.0,
            BrazoIzquierdoCm = 36.5,
            CaderaCm = 100.0,
            CinturaCm = 90.0,
            PechoCm = 105.0,
            PesoKg = 75.0,
            Fecha = new DateTime(2024, 6, 15),
            SocioId = 1
        };

        var medidaCorporalActualizada = new MedidaCorporal
        {
            Id = actualizarMedidaCorporalDTO.Id,
            BrazoDerechoCm = actualizarMedidaCorporalDTO.BrazoDerechoCm,
            BrazoIzquierdoCm = actualizarMedidaCorporalDTO.BrazoIzquierdoCm,
            CaderaCm = actualizarMedidaCorporalDTO.CaderaCm,
            CinturaCm = actualizarMedidaCorporalDTO.CinturaCm,
            PechoCm = actualizarMedidaCorporalDTO.PechoCm,
            PesoKg = actualizarMedidaCorporalDTO.PesoKg,
            Fecha = actualizarMedidaCorporalDTO.Fecha,
            SocioId = actualizarMedidaCorporalDTO.SocioId
        };

        _medidaCorporalRepositorioMock
            .Setup(repo => repo.ActualizarAsync(It.IsAny<MedidaCorporal>()))
            .ReturnsAsync(medidaCorporalActualizada);

        var actualizarMedidaCorporalCasoDeUso = new ActualizarMedidaCorporalCasoDeUso(_medidaCorporalRepositorioMock.Object, _mapper);

        // Act
        var resultado = await actualizarMedidaCorporalCasoDeUso.Ejecutar(actualizarMedidaCorporalDTO);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(medidaCorporalActualizada.Id);
        resultado.BrazoDerechoCm.Should().Be(actualizarMedidaCorporalDTO.BrazoDerechoCm);
        resultado.BrazoIzquierdoCm.Should().Be(actualizarMedidaCorporalDTO.BrazoIzquierdoCm);
        resultado.CaderaCm.Should().Be(actualizarMedidaCorporalDTO.CaderaCm);
    }

    //actualizar medida corporal no existente
    [Fact]
    public async Task ActualizarMedidaCorporal_CuandoLaMedidaCorporalNoExiste_RetornaNull()
    {
        // Arrange
        var actualizarMedidaCorporalDTO = new ActualizarMedidaCorporalDTO
        {
            Id = 99,
            BrazoDerechoCm = 35.0,
            BrazoIzquierdoCm = 34.5,
            CaderaCm = 95.0,
            CinturaCm = 85.0,
            PechoCm = 100.0,
            PesoKg = 70.0,
            Fecha = new DateTime(2024, 6, 15),
            SocioId = 1
        };

        _medidaCorporalRepositorioMock
            .Setup(repo => repo.ActualizarAsync(It.IsAny<MedidaCorporal>()))
            .ReturnsAsync((MedidaCorporal?)null);

        var actualizarMedidaCorporalCasoDeUso = new ActualizarMedidaCorporalCasoDeUso(_medidaCorporalRepositorioMock.Object, _mapper);

        // Act
        var resultado = await actualizarMedidaCorporalCasoDeUso.Ejecutar(actualizarMedidaCorporalDTO);

        // Assert
        resultado.Should().BeNull();
    }
}