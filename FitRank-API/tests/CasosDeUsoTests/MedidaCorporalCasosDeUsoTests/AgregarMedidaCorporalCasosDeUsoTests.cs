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

public class AgregarMedidaCorporalCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IMedidaCorporalRepositorio> _medidaCorporalRepositorioMock;

    public AgregarMedidaCorporalCasoDeUsoTests()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new MedidaCorporalProfile());
        });
        _mapper = mappingConfig.CreateMapper();
        _medidaCorporalRepositorioMock = new Mock<IMedidaCorporalRepositorio>();
    }

    //agregar medida corporal exitoso
    [Fact]
    public async Task Ejecutar_DeberiaAgregarMedidaCorporal_CuandoLosDatosSonValidos()
    {
        // Arrange
        var nuevoMedidaCorporalDTO = new AgregarMedidaCorporalDTO
        {
            BrazoDerechoCm = 35.0,
            BrazoIzquierdoCm = 34.5,
            CaderaCm = 95.0,
            CinturaCm = 85.0,
            PechoCm = 100.0,
            PesoKg = 70.0,
            Fecha = new DateTime(2024, 6, 15),
            SocioId = 1
        };

        var medidaCorporalAgregada = new MedidaCorporal
        {
            Id = 1,
            BrazoDerechoCm = nuevoMedidaCorporalDTO.BrazoDerechoCm,
            BrazoIzquierdoCm = nuevoMedidaCorporalDTO.BrazoIzquierdoCm,
            CaderaCm = nuevoMedidaCorporalDTO.CaderaCm,
            CinturaCm = nuevoMedidaCorporalDTO.CinturaCm,
            PechoCm = nuevoMedidaCorporalDTO.PechoCm,
            PesoKg = nuevoMedidaCorporalDTO.PesoKg,
            Fecha = nuevoMedidaCorporalDTO.Fecha,
            SocioId = nuevoMedidaCorporalDTO.SocioId
        };

        _medidaCorporalRepositorioMock.Setup(repo => repo.AgregarAsync(It.IsAny<MedidaCorporal>()))
            .ReturnsAsync(medidaCorporalAgregada);

        var agregarMedidaCorporalCasoDeUso = new AgregarMedidaCorporalCasoDeUso(_medidaCorporalRepositorioMock.Object, _mapper);

        // Act
        var resultado = await agregarMedidaCorporalCasoDeUso.Ejecutar(nuevoMedidaCorporalDTO);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(medidaCorporalAgregada.Id);
        resultado.BrazoDerechoCm.Should().Be(nuevoMedidaCorporalDTO.BrazoDerechoCm);
        resultado.BrazoIzquierdoCm.Should().Be(nuevoMedidaCorporalDTO.BrazoIzquierdoCm);
        resultado.CaderaCm.Should().Be(nuevoMedidaCorporalDTO.CaderaCm);
        resultado.CinturaCm.Should().Be(nuevoMedidaCorporalDTO.CinturaCm);
        resultado.PechoCm.Should().Be(nuevoMedidaCorporalDTO.PechoCm);
        resultado.PesoKg.Should().Be(nuevoMedidaCorporalDTO.PesoKg);
    }
}