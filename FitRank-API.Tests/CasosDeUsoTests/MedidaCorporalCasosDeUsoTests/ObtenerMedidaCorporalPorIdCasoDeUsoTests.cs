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

public class ObtenerMedidaCorporalPorIdCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IMedidaCorporalRepositorio> _medidaCorporalRepositorioMock;

    public ObtenerMedidaCorporalPorIdCasoDeUsoTests()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new MedidaCorporalProfile());
        });
        _mapper = mappingConfig.CreateMapper();
        _medidaCorporalRepositorioMock = new Mock<IMedidaCorporalRepositorio>();
    }

    [Fact]
    public async Task Ejecutar_DeberiaObtenerMedidaCorporal_PorIdExistente()
    {
        // Arrange
        long medidaCorporalId = 1;

        var medidaCorporalExistente = new MedidaCorporal
        {
            Id = medidaCorporalId,
            BrazoDerechoCm = 36.0,
            BrazoIzquierdoCm = 36.5,
            CaderaCm = 100.0,
            CinturaCm = 90.0,
            PechoCm = 105.0,
            PesoKg = 75.0,
            Fecha = new DateTime(2024, 6, 15),
            SocioId = 1
        };

        _medidaCorporalRepositorioMock.Setup(repo => repo.ObtenerPorIdAsync(medidaCorporalId))
            .ReturnsAsync(medidaCorporalExistente);

        var casoDeUso = new ObtenerMedidaCorporalPorIdCasoDeUso(_medidaCorporalRepositorioMock.Object, _mapper);

        // Act
        var resultado = await casoDeUso.Ejecutar(medidaCorporalId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(medidaCorporalExistente.Id);
        resultado.BrazoDerechoCm.Should().Be(medidaCorporalExistente.BrazoDerechoCm);
        resultado.BrazoIzquierdoCm.Should().Be(medidaCorporalExistente.BrazoIzquierdoCm);
        resultado.CaderaCm.Should().Be(medidaCorporalExistente.CaderaCm);
        resultado.CinturaCm.Should().Be(medidaCorporalExistente.CinturaCm);
        resultado.SocioId.Should().Be(medidaCorporalExistente.SocioId);
        resultado.Fecha.Should().Be(medidaCorporalExistente.Fecha);
    }

    [Fact]
    public async Task Ejecutar_DeberiaRetornarNull_PorIdInexistente()
    {
        // Arrange
        long medidaCorporalIdInexistente = 999;

        _medidaCorporalRepositorioMock.Setup(repo => repo.ObtenerPorIdAsync(medidaCorporalIdInexistente))
            .ReturnsAsync((MedidaCorporal?)null);

        var casoDeUso = new ObtenerMedidaCorporalPorIdCasoDeUso(_medidaCorporalRepositorioMock.Object, _mapper);

        // Act
        var resultado = await casoDeUso.Ejecutar(medidaCorporalIdInexistente);

        // Assert
        resultado.Should().BeNull();
    }
}