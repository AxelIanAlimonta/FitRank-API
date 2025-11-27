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

public class ObtenerNotificacionPorUsuarioCasoDeUsoTests
{
    private readonly IMapper _mapper;
    private readonly Mock<INotificacionRepositorio> _notificacionRepositorioMock;

    public ObtenerNotificacionPorUsuarioCasoDeUsoTests()
    {
        var mappingConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new NotificacionProfile());
        });
        _mapper = mappingConfig.CreateMapper();
        _notificacionRepositorioMock = new Mock<INotificacionRepositorio>();
    }

    [Fact]
    public async Task ObtenerNotificacionesPorUsuario_CuandoElUsuarioTieneNotificaciones_RetornaListaDeNotificacionDTO()
    {
        // Arrange
        long usuarioReceptorId = 2;

        var notificacionesEnRepositorio = new List<Notificacion>
        {
            new Notificacion
            {
                Id = 1,
                Mensaje = "Notificación 1",
                Titulo = "Título 1",
                UsuarioEmisorId = 1,
                UsuarioReceptorId = usuarioReceptorId,
                Activa = true,
                Leido = false,
                FechaEnvio = new DateTime(2024, 6, 1)
            },
            new Notificacion
            {
                Id = 2,
                Mensaje = "Notificación 2",
                Titulo = "Título 2",
                UsuarioEmisorId = 3,
                UsuarioReceptorId = usuarioReceptorId,
                Activa = true,
                Leido = true,
                FechaEnvio = new DateTime(2024, 6, 2)
            }
        };

        _notificacionRepositorioMock
            .Setup(repo => repo.ObtenerPorUsuarioAsync(usuarioReceptorId))
            .ReturnsAsync(notificacionesEnRepositorio);

        var casoDeUso = new ObtenerNotificacionPorUsuarioCasoDeUso(_notificacionRepositorioMock.Object, _mapper);

        // Act
        var resultado = await casoDeUso.Ejecutar(usuarioReceptorId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Count().Should().Be(notificacionesEnRepositorio.Count);
        resultado.Should().BeEquivalentTo(_mapper.Map<IEnumerable<ObtenerNotificacionDTO>>(notificacionesEnRepositorio));

    }

    [Fact]
    public async Task ObtenerNotificacionesPorUsuario_CuandoElUsuarioNoTieneNotificaciones_RetornaListaVacia()
    {
        // Arrange
        long usuarioReceptorId = 99;

        _notificacionRepositorioMock
            .Setup(repo => repo.ObtenerPorUsuarioAsync(usuarioReceptorId))
            .ReturnsAsync(new List<Notificacion>());

        var casoDeUso = new ObtenerNotificacionPorUsuarioCasoDeUso(_notificacionRepositorioMock.Object, _mapper);

        // Act
        var resultado = await casoDeUso.Ejecutar(usuarioReceptorId);

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().BeEmpty();
    }
}