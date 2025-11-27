using System.Threading.Tasks;
using AutoMapper;
using FitRank_API.Application.CasosDeUso.AmistadCasosDeUso;
using FitRank_API.Application.DTOs;
using FitRank_API.Application.DTOs;
using FitRank_API.Application.DTOs.AmistadDTOs;
using FitRank_API.Application.UseCases;
using FitRank_API.Controllers;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using FitRank_API.Presentacion.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Xunit;

namespace FitRank_API.tests.ControllersTests;

public class AmistadControllerTests
{

    private readonly AmistadController _controller;
    private readonly Mock<EliminarAmigoCasoDeUso> _mockEliminar;
    private readonly Mock<AceptarSolicitudAmistadCasoDeUso> _mockAceptarSolicitud;
    private readonly Mock<ObtenerSolicitudesPendientesCasoDeUso> _mockObtenerSolicitudesPendientes;
    private readonly Mock<ObtenerAmigosCasoDeUso> _mockObtenerPorId;
    private readonly Mock<EnviarSolicitudAmistadCasoDeUso> _mockEnviarSolicitudAmistad;

    public AmistadControllerTests()
    {
        var mockAmistadRepositorio = new Mock<IAmistadRepositorio>();
        var mockUsuarioRepositorio = new Mock<IUsuarioRepositorio>();
        var mockMapper = new Mock<IMapper>();

        _mockEliminar = new Mock<EliminarAmigoCasoDeUso>(mockAmistadRepositorio.Object);
        _mockObtenerPorId = new Mock<ObtenerAmigosCasoDeUso>(mockAmistadRepositorio.Object, mockMapper.Object);
        _mockEnviarSolicitudAmistad = new Mock<EnviarSolicitudAmistadCasoDeUso>(mockAmistadRepositorio.Object, mockUsuarioRepositorio.Object);
        _mockAceptarSolicitud = new Mock<AceptarSolicitudAmistadCasoDeUso>(mockAmistadRepositorio.Object);
        _mockObtenerSolicitudesPendientes = new Mock<ObtenerSolicitudesPendientesCasoDeUso>(mockAmistadRepositorio.Object);

        _controller = new AmistadController(
            _mockEnviarSolicitudAmistad.Object,
            _mockObtenerPorId.Object,
            _mockObtenerSolicitudesPendientes.Object,
            _mockAceptarSolicitud.Object,
            _mockEliminar.Object
        );

    }

    //enviar solicitud amistad exitoso
    [Fact]
    public async Task EnviarSolicitud_DeberiaRetornarOk_CuandoSolicitudEsExitosa()
    {
        // Arrange
        var dto = new EnviarSolicitudAmistadDTO
        {
            SolicitanteId = 1,
            DestinatarioId = 2
        };

        var resultadoEsperado = new AmistadDTO
        {
            Completado = true,
            Mensaje = "Solicitud de amistad enviada exitosamente.",
            AmistadId = 1,
            SocioId1 = 1,
            SocioId2 = 2,
            SolicitanteId = 1,
            Estado = "Pendiente"
        };

        _mockEnviarSolicitudAmistad
            .Setup(caso => caso.Ejecutar(It.IsAny<EnviarSolicitudAmistadDTO>()))
            .ReturnsAsync(resultadoEsperado);

        // Simular usuario autenticado
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "1")
        }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        // Act
        var resultado = await _controller.EnviarSolicitud(dto);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(resultadoEsperado);
    }

    //bad request al enviar solicitud de amistad
    [Fact]
    public async Task EnviarSolicitud_DeberiaRetornarBadRequest_CuandoSolicitudFalla()
    {
        // Arrange
        var dto = new EnviarSolicitudAmistadDTO
        {
            SolicitanteId = 1,
            DestinatarioId = 2
        };
        var resultadoEsperado = new AmistadDTO
        {
            Completado = false,
            Mensaje = "Error al enviar la solicitud de amistad."
        };
        _mockEnviarSolicitudAmistad
            .Setup(caso => caso.Ejecutar(It.IsAny<EnviarSolicitudAmistadDTO>()))
            .ReturnsAsync(resultadoEsperado);
        // Simular usuario autenticado
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "1")
        }, "mock"));
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
        // Act
        var resultado = await _controller.EnviarSolicitud(dto);
        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.StatusCode.Should().Be(400);
        badRequestResult.Value.Should().BeEquivalentTo(resultadoEsperado);
    }

    // obtener solicitudes pendientes exitoso
    [Fact]
    public async Task ObtenerSolicitudesPendientes_DeberiaRetornarOk_CuandoExitoso()
    {
        // Arrange
        var usuarioId = 1;
        var solicitudesEsperadas = new List<SolicitudAmistadDTO>
        {
            new SolicitudAmistadDTO
            {
                AmistadId = 1,
                RemitenteId = 2,
                RemitenteNombreUsuario = "usuario2",
                RemitenteNombre = "Usuario Dos",
                RemitentePuntaje = 1500.0
            },
            new SolicitudAmistadDTO
            {
                AmistadId = 2,
                RemitenteId = 3,
                RemitenteNombreUsuario = "usuario3",
                RemitenteNombre = "Usuario Tres",
                RemitentePuntaje = 1600.0
            }
        };
        _mockObtenerSolicitudesPendientes
            .Setup(caso => caso.Ejecutar(usuarioId))
            .ReturnsAsync(solicitudesEsperadas);
        // Simular usuario autenticado
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuarioId.ToString())
        }, "mock"));
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
        // Act
        var resultado = await _controller.ObtenerSolicitudesPendientes();
        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(solicitudesEsperadas);
    }

    //aceptar solicitud amistad exitoso
    [Fact]
    public async Task AceptarSolicitud_DeberiaRetornarOk_CuandoExitoso()
    {
        // Arrange
        var aceptarSolicitudAmistadDto = new AceptarSolicitudAmistadDTO
        {
            SocioId = 2,
            AmistadId = 1
        };

        var resultadoEsperado = new AmistadDTO
        {
            Completado = true,
            Mensaje = "Solicitud de amistad aceptada exitosamente.",
            AmistadId = 1,
            SocioId1 = 1,
            SocioId2 = 2,
            SolicitanteId = 2,
            Estado = "Aceptada"
        };

        _mockAceptarSolicitud
            .Setup(caso => caso.Ejecutar(It.IsAny<AceptarSolicitudAmistadDTO>()))
            .ReturnsAsync(resultadoEsperado);

        // Simular usuario autenticado
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "1")
        }, "mock"));
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
        // Act
        var resultado = await _controller.AceptarSolicitud(aceptarSolicitudAmistadDto.AmistadId);
        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(resultadoEsperado);
    }

    //aceptar solicitud amistad falla
    [Fact]
    public async Task AceptarSolicitud_DeberiaRetornarBadRequest_CuandoFalla()
    {
        // Arrange
        var aceptarSolicitudAmistadDto = new AceptarSolicitudAmistadDTO
        {
            SocioId = 2,
            AmistadId = 1
        };
        var resultadoEsperado = new AmistadDTO
        {
            Completado = false,
            Mensaje = "Error al aceptar la solicitud de amistad."
        };
        _mockAceptarSolicitud
            .Setup(caso => caso.Ejecutar(It.IsAny<AceptarSolicitudAmistadDTO>()))
            .ReturnsAsync(resultadoEsperado);
        // Simular usuario autenticado
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "1")
        }, "mock"));
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
        // Act
        var resultado = await _controller.AceptarSolicitud(aceptarSolicitudAmistadDto.AmistadId);
        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.StatusCode.Should().Be(400);
        badRequestResult.Value.Should().BeEquivalentTo(resultadoEsperado);
    }

    //eliminar amigo exitoso
    [Fact]
    public async Task EliminarAmigo_DeberiaRetornarOk_CuandoExitoso()
    {
        // Arrange
        var eliminarDto = new EliminarAmigoDTO
        {
            SocioId = 1,
            AmigoId = 2
        };

        _mockEliminar.Setup(caso => caso.Ejecutar(It.IsAny<EliminarAmigoDTO>()))
            .ReturnsAsync(true);

        // Simular usuario autenticado
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "1")
        }, "mock"));
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        // Act
        var resultado = await _controller.EliminarAmigo(eliminarDto.AmigoId);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(new { Completado = true, Mensaje = "Amistad eliminada correctamente." });
    }


    //eliminar amigo falla
    [Fact]
    public async Task EliminarAmigo_DeberiaRetornarBadRequest_SiFalla()
    {
        // Arrange
        var eliminarDto = new EliminarAmigoDTO
        {
            SocioId = 1,
            AmigoId = 2
        };

        _mockEliminar.Setup(caso => caso.Ejecutar(It.IsAny<EliminarAmigoDTO>()))
            .ReturnsAsync(false);

        // Simular usuario autenticado
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "1")
        }, "mock"));
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        // Act
        var resultado = await _controller.EliminarAmigo(eliminarDto.AmigoId);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.StatusCode.Should().Be(400);
        badRequestResult.Value.Should().BeEquivalentTo(new { Completado = false, Mensaje = "No se pudo eliminar la amistad." });
    }

    //obtener amigos exitoso
    [Fact]
    public async Task ObtenerAmigos_DeberiaRetornarOk_CuandoExitoso()
    {
        // Arrange
        var usuarioId = 1;
        var amigosEsperados = new List<AmigoDTO>
        {
            new AmigoDTO
            {
                Id = 2,
                NombreUsuario = "usuario2",
                Nombre = "Usuario Dos",
                Puntaje = 1500.0

            },
            new AmigoDTO
            {
                Id = 3,
                NombreUsuario = "usuario3",
                Nombre = "Usuario Tres",
                Puntaje = 1600.0
            }
        };
        _mockObtenerPorId
            .Setup(caso => caso.Ejecutar(usuarioId))
            .ReturnsAsync(amigosEsperados);
        // Simular usuario autenticado
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, usuarioId.ToString())
        }, "mock"));
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
        // Act
        var resultado = await _controller.ObtenerAmigos();
        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(amigosEsperados);
    }

}