using FitRank_API.Application.CasosDeUso.GrupoMuscularCasosDeUso;
using FitRank_API.Application.Mappings;
using FitRank_API.Domain.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FitRank_API.Application.DTOs;
using FitRank_API.Domain.Entities;
using FluentAssertions;
using FitRank_API.Application.DTOs.GrupoMuscularDTOs;


namespace CasosDeUsoTests.GrupoMuscularCasosDeUsoTests;

public class ActualizarGrupoMuscularCasoDeUsoTests
{
    private readonly Mock<IGrupoMuscularRepositorio> _grupoMuscularRepositorioMock;
    private readonly IMapper _mapper;
    private readonly ActualizarGrupoMuscularCasoDeUso _actualizarGrupoMuscularCasoDeUso;
    public ActualizarGrupoMuscularCasoDeUsoTests()
    {
        _grupoMuscularRepositorioMock = new Mock<IGrupoMuscularRepositorio>();
        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<GrupoMuscularProfile>();
        });
        _mapper = mapperConfig.CreateMapper();
        _actualizarGrupoMuscularCasoDeUso = new ActualizarGrupoMuscularCasoDeUso(_grupoMuscularRepositorioMock.Object, _mapper);
    }

    [Fact]
    public async Task ActualizarGrupoMuscularCasoDeUso_ActualizacionExitosa_RetornaGrupoMuscularDTO()
    {
        // Arrange
        var grupoMuscularDTO = new ObtenerGrupoMuscularDTO
        {
            Id = 1,
            Nombre = "Pecho",
            Imagen = "imagen_url"
        };

        var actualizarGrupoMuscularDTO = new ActualizarGrupoMuscularDTO
        {
            Id = 1,
            Nombre = "Pecho",
            Imagen = "imagen_url"
        };

        var grupoMuscularEntidad = new GrupoMuscular
        {
            Id = 1,
            Nombre = "Pecho",
            Imagen = "imagen_url"
        };
        _grupoMuscularRepositorioMock
            .Setup(repo => repo.ActualizarAsync(It.IsAny<GrupoMuscular>()))
            .ReturnsAsync(grupoMuscularEntidad);
        // Act
        var resultado = await _actualizarGrupoMuscularCasoDeUso.Ejecutar(actualizarGrupoMuscularDTO);
        // Assert con FluentAssertions
        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(grupoMuscularDTO.Id);
        resultado.Nombre.Should().Be(grupoMuscularDTO.Nombre);
        resultado.Imagen.Should().Be(grupoMuscularDTO.Imagen);
    }

    [Fact]
    public async Task ActualizarGrupoMuscularCasoDeUso_ActualizacionFalla_RetornaNull()
    {
        // Arrange

        var actualizarGrupoMuscularDTO = new ActualizarGrupoMuscularDTO
        {
            Id = 2,
            Nombre = "Brazos",
            Imagen = "imagen_url"
        };
        _grupoMuscularRepositorioMock
            .Setup(repo => repo.ActualizarAsync(It.IsAny<GrupoMuscular>()))
            .ReturnsAsync((GrupoMuscular?)null);
        // Act
        var resultado = await _actualizarGrupoMuscularCasoDeUso.Ejecutar(actualizarGrupoMuscularDTO);
        // Assert
        resultado.Should().BeNull();
    }
}
