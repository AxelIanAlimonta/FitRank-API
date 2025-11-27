using FitRank_API.Domain.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FitRank_API.Domain.Entities;
using FluentAssertions;
using FitRank_API.Application.Mappings;
using FitRank_API.Application.DTOs.DiaDeLaSemanaDTOs;
using FitRank_API.Application.Mappings;
using FitRank_API.Application.CasosDeUso.DiaDeLaSemanaCasoDeUso;

namespace CasosDeUsoTests.DiaDeLaSemanaCasoDeUsoTests;

public class AgregarDiaDeLaSemanaCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IDiaDeLaSemanaRepositorio> _diaDeLaSemanaRepositorioMock;

    public AgregarDiaDeLaSemanaCasoDeUsoTests()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new DiaDeLaSemanaProfile());
        });
        _mapper = mappingConfig.CreateMapper();
        _diaDeLaSemanaRepositorioMock = new Mock<IDiaDeLaSemanaRepositorio>();
    }

    [Fact]
    public async Task AgregarDiaDeLaSemana_CuandoLosDatosSonValidos_RetornaDiaDeLaSemanaDTO()
    {
        // Arrange
        var nuevoDiaDTO = new AgregarDiaDeLaSemanaDTO
        {
            Nombre = "Lunes"
        };

        var diaCreado = new DiaDeLaSemana
        {
            Id = 1,
            Nombre = nuevoDiaDTO.Nombre
        };

        _diaDeLaSemanaRepositorioMock
            .Setup(repo => repo.AgregarDiaDeLaSemanaAsync(It.IsAny<DiaDeLaSemana>()))
            .ReturnsAsync(diaCreado);

        var agregarDiaDeLaSemanaCasoDeUso = new AgregarDiaDeLaSemanaCasoDeUso(_diaDeLaSemanaRepositorioMock.Object, _mapper);

        // Act
        var resultado = await agregarDiaDeLaSemanaCasoDeUso.Ejecutar(nuevoDiaDTO);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(diaCreado.Id);
        resultado.Nombre.Should().Be(diaCreado.Nombre);
    }
}