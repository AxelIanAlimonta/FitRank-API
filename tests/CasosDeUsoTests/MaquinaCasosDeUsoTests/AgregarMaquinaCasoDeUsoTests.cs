using FitRank_API.Application.Mappings;
using FitRank_API.Infrastructure.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FitRank_API.Domain.Entities;
using FluentAssertions;
using FitRank_API.Application.DTOs.MaquinaDTOs;
using FitRank_API.Application.CasosDeUso.MaquinaCasosDeUso;

namespace CasosDeUsoTests.MaquinaCasosDeUsoTests;

public class AgregarMaquinaCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IMaquinaRepositorio> _maquinaRepositorioMock;

    public AgregarMaquinaCasoDeUsoTests()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new MaquinaProfile());
        });
        _mapper = mappingConfig.CreateMapper();
        _maquinaRepositorioMock = new Mock<IMaquinaRepositorio>();
    }

    //agregar maquina exitoso
    [Fact]
    public async Task AgregarMaquina_CuandoLosDatosSonValidos_RetornaMaquinaDTO()
    {
        // Arrange
        var agregarMaquinaDTO = new AgregarMaquinaDTO
        {
            GimnasioId = 1,
            Nombre = "Maquina Nueva",
            UrlImagen = "http://imagen.nueva",
            Qr = "QR456",
        };

        var maquinaAGregar = new Maquina
        {
            GimnasioId = agregarMaquinaDTO.GimnasioId,
            Nombre = agregarMaquinaDTO.Nombre,
            UrlImagen = agregarMaquinaDTO.UrlImagen,
            Qr = agregarMaquinaDTO.Qr,
        };

        var maquinaAgregada = new Maquina
        {
            Id = 1,
            GimnasioId = agregarMaquinaDTO.GimnasioId,
            Nombre = agregarMaquinaDTO.Nombre,
            UrlImagen = agregarMaquinaDTO.UrlImagen,
            Qr = agregarMaquinaDTO.Qr,
        };

        _maquinaRepositorioMock
            .Setup(repo => repo.AgregarMaquina(It.Is<Maquina>(m =>
                m.GimnasioId == maquinaAGregar.GimnasioId &&
                m.Nombre == maquinaAGregar.Nombre &&
                m.UrlImagen == maquinaAGregar.UrlImagen &&
                m.Qr == maquinaAGregar.Qr)))
            .ReturnsAsync(maquinaAgregada);

        var agregarMaquinaCasoDeUso = new AgregarMaquinaCasoDeUso(_maquinaRepositorioMock.Object, _mapper);

        // Act
        var resultado = await agregarMaquinaCasoDeUso.Ejecutar(agregarMaquinaDTO);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Id.Should().Be(maquinaAgregada.Id);
        resultado.GimnasioId.Should().Be(maquinaAgregada.GimnasioId);
        resultado.Nombre.Should().Be(maquinaAgregada.Nombre);
        resultado.UrlImagen.Should().Be(maquinaAgregada.UrlImagen);
        resultado.Qr.Should().Be(maquinaAgregada.Qr);
    }

}