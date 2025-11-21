using FitRank_API.Application.Mappings;
using FitRank_API.Infrastructure.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FitRank_API.Domain.Entities;
using FluentAssertions;
using FitRank_API.Application.DTOs.MaquinaDTOs;
using FitRank_API.Application.CasosDeUso.MaquinaCasosDeUso;
using FitRank_API.Application.CasosDeUso.Invitacion;

namespace CasosDeUsoTests.MaquinaCasosDeUsoTests;

public class AgregarMaquinaCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<IMaquinaRepositorio> _maquinaRepositorioMock;
    private readonly Mock<QrHelper> _qrHelperMock;

    public AgregarMaquinaCasoDeUsoTests()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new MaquinaProfile());
        });

        _mapper = mappingConfig.CreateMapper();
        _maquinaRepositorioMock = new Mock<IMaquinaRepositorio>();

        // QrHelper necesita constructor → mock estricto
        _qrHelperMock = new Mock<QrHelper>(MockBehavior.Strict);
    }

    [Fact]
    public async Task AgregarMaquina_CuandoLosDatosSonValidos_RetornaMaquinaDTO()
    {
        // Arrange
        long gimnasioId = 1;

        var agregarMaquinaDTO = new AgregarMaquinaDTO
        {
            Nombre = "Maquina Nueva",
            UrlImagen = "http://imagen.nueva"
        };

        var maquinaAgregada = new Maquina
        {
            Id = 10,
            GimnasioId = gimnasioId,
            Nombre = agregarMaquinaDTO.Nombre,
            UrlImagen = agregarMaquinaDTO.UrlImagen,
            Qr = "PENDIENTE"
        };

        // Mock agregar
        _maquinaRepositorioMock
            .Setup(r => r.AgregarMaquina(It.IsAny<Maquina>()))
            .ReturnsAsync(maquinaAgregada);

        // Mock qr generado
        _qrHelperMock
            .Setup(q => q.GenerarQrDeMaquina(maquinaAgregada.Id))
            .ReturnsAsync("QR-GENERADO-123");

        // Mock actualizar
        _maquinaRepositorioMock
            .Setup(r => r.ActualizarMaquina(It.IsAny<Maquina>()))
            .Returns((Task<Maquina>)Task.CompletedTask);

        var casoDeUso = new AgregarMaquinaCasoDeUso(
            _maquinaRepositorioMock.Object,
            _mapper,
            _qrHelperMock.Object
        );

        // Act
        var resultado = await casoDeUso.Ejecutar(agregarMaquinaDTO, gimnasioId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(maquinaAgregada.Id);
        resultado.GimnasioId.Should().Be(gimnasioId);
        resultado.Nombre.Should().Be(agregarMaquinaDTO.Nombre);
        resultado.UrlImagen.Should().Be(agregarMaquinaDTO.UrlImagen);
        resultado.Qr.Should().Be("QR-GENERADO-123");

    }
}
