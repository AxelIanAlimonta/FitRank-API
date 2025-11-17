using FitRank_API.Application.CasosDeUso.GrupoMuscularCasosDeUso;
using FitRank_API.Application.Mappings;
using FitRank_API.Infrastructure.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FitRank_API.Application.DTOs;
using FitRank_API.Domain.Entities;
using FluentAssertions;



namespace CasosDeUsoTests.GrupoMuscularCasosDeUsoTests;

public class AgregarGrupoMuscularCasoDeUsoTests
{
    private readonly Mock<IGrupoMuscularRepositorio> _grupoMuscularRepositorioMock;
    private readonly IMapper _mapper;
    private readonly AgregarGrupoMuscularCasoDeUso _agregarGrupoMuscularCasoDeUso;

    public AgregarGrupoMuscularCasoDeUsoTests()
    {
        _grupoMuscularRepositorioMock = new Mock<IGrupoMuscularRepositorio>();

        var mapperConfig = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<GrupoMuscularProfile>();
        });
        _mapper = mapperConfig.CreateMapper();

        _agregarGrupoMuscularCasoDeUso = new AgregarGrupoMuscularCasoDeUso(_grupoMuscularRepositorioMock.Object, _mapper);
    }

    [Fact]
    public void AgregarGrupoMuscularCasoDeUso_CreacionExitosa_RetornaGrupoMuscularDTO()
    {
        // Arrange
        var agregarGrupoMuscularDTO = new AgregarGrupoMuscularDTO
        {
            Nombre = "Pecho",
            Imagen = "imagen_url"
        };
        var grupoMuscularEntidad = new GrupoMuscular
        {
            Id = 1,
            Nombre = agregarGrupoMuscularDTO.Nombre,
            Imagen = agregarGrupoMuscularDTO.Imagen
        };
        _grupoMuscularRepositorioMock
            .Setup(repo => repo.AgregarAsync(It.IsAny<GrupoMuscular>()))
            .ReturnsAsync(grupoMuscularEntidad);
        // Act
        var resultado = _agregarGrupoMuscularCasoDeUso.Ejecutar(agregarGrupoMuscularDTO).Result;
        // Assert con FluentAssertions
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(grupoMuscularEntidad.Id);
        resultado.Nombre.Should().Be(grupoMuscularEntidad.Nombre);
        resultado.Imagen.Should().Be(grupoMuscularEntidad.Imagen);
    }
}
