using FitRank_API.Application.Mappings;
using FitRank_API.Domain.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FitRank_API.Application.DTOs;
using FitRank_API.Domain.Entities;
using FluentAssertions;
using FitRank_API.Application.DTOs.MedidaCorporalDTOs;
using FitRank_API.Application.CasosDeUso.MedidaCorporalCasosDeUso;

namespace CasosDeUsoTests.MedidaCorporalCasosDeUsoTests;

public class ObtenerMedidasPorSocioCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IMedidaCorporalRepositorio> _medidaCorporalRepositorioMock;

    public ObtenerMedidasPorSocioCasoDeUsoTests()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new MedidaCorporalProfile());
        });
        _mapper = mappingConfig.CreateMapper();
        _medidaCorporalRepositorioMock = new Mock<IMedidaCorporalRepositorio>();
    }

    [Fact]
    public async Task Ejecutar_DeberiaObtenerMedidasCorporal_PorSocioExistente()
    {
        // Arrange
        long socioId = 1;

        var medidasCorporal = new List<MedidaCorporal>
        {
            new MedidaCorporal
            {
                Id = 1,
                BrazoDerechoCm = 35.0,
                BrazoIzquierdoCm = 34.5,
                CaderaCm = 95.0,
                CinturaCm = 85.0,
                PechoCm = 100.0,
                PesoKg = 70.0,
                Fecha = new DateTime(2024, 6, 15),
                SocioId = socioId
            },
            new MedidaCorporal
            {
                Id = 2,
                BrazoDerechoCm = 36.0,
                BrazoIzquierdoCm = 35.5,
                CaderaCm = 96.0,
                CinturaCm = 86.0,
                PechoCm = 101.0,
                PesoKg = 71.0,
                Fecha = new DateTime(2024, 7, 15),
                SocioId = socioId
            }
        };

        _medidaCorporalRepositorioMock.Setup(repo => repo.ObtenerPorSocioAsync(socioId))
            .ReturnsAsync(medidasCorporal);

        var casoDeUso = new ObtenerMedidasPorSocioCasoDeUso(_medidaCorporalRepositorioMock.Object, _mapper);

        // Act
        var resultado = await casoDeUso.Ejecutar(socioId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Count().Should().Be(2);

    }

    [Fact]
    public async Task Ejecutar_DeberiaRetornarListaVacia_PorSocioSinMedidasCorporal()
    {
        // Arrange
        long socioId = 999; // Suponiendo que este socio no tiene medidas corporales

        _medidaCorporalRepositorioMock.Setup(repo => repo.ObtenerPorSocioAsync(socioId))
            .ReturnsAsync(new List<MedidaCorporal>());

        var casoDeUso = new ObtenerMedidasPorSocioCasoDeUso(_medidaCorporalRepositorioMock.Object, _mapper);

        // Act
        var resultado = await casoDeUso.Ejecutar(socioId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Count().Should().Be(0);
    }
}