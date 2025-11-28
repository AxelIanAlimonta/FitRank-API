using FitRank_API.Domain.Interfaces;
using Moq;
using AutoMapper;
using Xunit;
using FitRank_API.Domain.Entities;
using FluentAssertions;
using FitRank_API.Application.DTOs.NotificacionDTOs;
using FitRank_API.Application.CasosDeUso.NotificacionCasosDeUso;
using FitRank_API.Application.Mappings;

namespace CasosDeUsoTests.NotificacionCasosDeUsoTests;

public class AgregarNotificacionCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<INotificacionRepositorio> _notificacionRepositorioMock;

    public AgregarNotificacionCasoDeUsoTests()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new NotificacionProfile());
        });
        _mapper = mappingConfig.CreateMapper();
        _notificacionRepositorioMock = new Mock<INotificacionRepositorio>();
    }

    [Fact]
    public async Task AgregarNotificacion_CuandoLosDatosSonValidos_RetornaNotificacionDTO()
    {
        // Arrange
        var nuevaNotificacionDTO = new AgregarNotificacionDTO
        {
           
            Mensaje = "Nueva notificación",
            Titulo = "Notificación",
            UsuarioEmisorId = 1,
            UsuarioReceptorId = 2,
            
        };

        var notificacionAGuardar = new Notificacion
        {
            Id = 0,
            Mensaje = "Nueva notificación",
            Titulo = "Notificación",
            UsuarioEmisorId = 1,
            UsuarioReceptorId = 2,
            Activa = true,
            Leido = false,
            FechaEnvio = new DateTime(2024, 6, 1)
        };

        var notificacionGuardada = new Notificacion
        {
            Id = 0,
            Mensaje = "Nueva notificación",
            Titulo = "Notificación",
            UsuarioEmisorId = 1,
            UsuarioReceptorId = 2,
            Activa = true,
            Leido = false,
            FechaEnvio = new DateTime(2024, 6, 1)
        };

        _notificacionRepositorioMock
            .Setup(repo => repo.AgregarAsync(It.IsAny<Notificacion>()))
            .ReturnsAsync(notificacionGuardada);

        var agregarNotificacionCasoDeUso = new AgregarNotificacionCasoDeUso(_notificacionRepositorioMock.Object, _mapper);

        // Act
        var resultado = await agregarNotificacionCasoDeUso.Ejecutar(nuevaNotificacionDTO);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Id.Should().Be(notificacionGuardada.Id);
        resultado.Mensaje.Should().Be(nuevaNotificacionDTO.Mensaje);
        resultado.Titulo.Should().Be(nuevaNotificacionDTO.Titulo);
        resultado.UsuarioEmisorId.Should().Be(nuevaNotificacionDTO.UsuarioEmisorId);
        resultado.UsuarioReceptorId.Should().Be(nuevaNotificacionDTO.UsuarioReceptorId);
     
    }
}