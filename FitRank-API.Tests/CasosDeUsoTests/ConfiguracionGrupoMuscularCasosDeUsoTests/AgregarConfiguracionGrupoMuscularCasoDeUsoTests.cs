using FitRank_API.Application.Mappings;
using FitRank_API.Infrastructure.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FluentAssertions;
using FitRank_API.Application.CasosDeUso.ConfiguracionGrupoMuscular;
using FitRank_API.Application.DTOs.ConfiguracionGrupoMuscularDTOs;
using FitRank_API.Domain.Entities;

namespace CasosDeUsoTests.ConfiguracionGrupoMuscularCasosDeUsoTests;

public class AgregarConfiguracionGrupoMuscularCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IConfiguracionGrupoMuscularRepositorio> _configuracionRepositorioMock;

    public AgregarConfiguracionGrupoMuscularCasoDeUsoTests()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new ConfiguracionGrupoMuscularProfile());
        });
        _mapper = mappingConfig.CreateMapper();
        _configuracionRepositorioMock = new Mock<IConfiguracionGrupoMuscularRepositorio>();
    }

    [Fact]
    public async Task Ejecutar_DeberiaAgregarConfiguracion_CuandoLosDatosSonValidos()
    {
        // Arrange
        var nuevaConfiguracionDTO = new AgregarConfiguracionGrupoMuscularDTO
        {
            GrupoMuscularId = 1,
            MultiplicadorPeso = 0.5,
            MultiplicadorRepeticiones = 0.3,
            FactorProgresion = 1.1
        };

        var configuracionAgregada = new ConfiguracionGrupoMuscular
        {
            Id = 1,
            GrupoMuscularId = nuevaConfiguracionDTO.GrupoMuscularId,
            MultiplicadorPeso = nuevaConfiguracionDTO.MultiplicadorPeso,
            MultiplicadorRepeticiones = nuevaConfiguracionDTO.MultiplicadorRepeticiones,
            FactorProgresion = nuevaConfiguracionDTO.FactorProgresion
        };

        _configuracionRepositorioMock.Setup(repo => repo.AgregarAsync(It.IsAny<ConfiguracionGrupoMuscular>()))
            .ReturnsAsync(configuracionAgregada);

        var agregarConfiguracionCasoDeUso = new AgregarConfiguracionGrupoMuscularCasoDeUso(_configuracionRepositorioMock.Object, _mapper);

        // Act
        var resultado = await agregarConfiguracionCasoDeUso.Ejecutar(nuevaConfiguracionDTO);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(1);
        resultado.GrupoMuscularId.Should().Be(nuevaConfiguracionDTO.GrupoMuscularId);
        resultado.MultiplicadorPeso.Should().Be(nuevaConfiguracionDTO.MultiplicadorPeso);
        resultado.MultiplicadorRepeticiones.Should().Be(nuevaConfiguracionDTO.MultiplicadorRepeticiones);
        resultado.FactorProgresion.Should().Be(nuevaConfiguracionDTO.FactorProgresion);
    }
}
