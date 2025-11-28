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

    private void SetupAuthenticatedUser(int userId)
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        }, "mock"));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    #region EnviarSolicitud Tests

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

        SetupAuthenticatedUser(1);

        // Act
        var resultado = await _controller.EnviarSolicitud(dto);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(resultadoEsperado);
    }

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
        
        SetupAuthenticatedUser(1);

        // Act
        var resultado = await _controller.EnviarSolicitud(dto);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.StatusCode.Should().Be(400);
        badRequestResult.Value.Should().BeEquivalentTo(resultadoEsperado);
    }

    [Fact]
    public async Task EnviarSolicitud_DeberiaRetornarBadRequest_CuandoDtoEsNulo()
    {
        // Arrange
        SetupAuthenticatedUser(1);

        // Act
        var resultado = await _controller.EnviarSolicitud(null!);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task EnviarSolicitud_DeberiaRetornarBadRequest_CuandoModelStateEsInvalido()
    {
        // Arrange
        var dto = new EnviarSolicitudAmistadDTO { DestinatarioId = 2 };
        SetupAuthenticatedUser(1);
        _controller.ModelState.AddModelError("DestinatarioId", "Requerido");

        // Act
        var resultado = await _controller.EnviarSolicitud(dto);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task EnviarSolicitud_DeberiaRetornarUnauthorized_CuandoNoHayClaimDeUsuario()
    {
        // Arrange
        var dto = new EnviarSolicitudAmistadDTO { DestinatarioId = 2 };
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        // Act
        var resultado = await _controller.EnviarSolicitud(dto);

        // Assert
        var unauthorizedResult = resultado as UnauthorizedObjectResult;
        unauthorizedResult.Should().NotBeNull();
        unauthorizedResult.StatusCode.Should().Be(401);
    }

    [Fact]
    public async Task EnviarSolicitud_DeberiaRetornarInternalServerError_CuandoOcurreExcepcion()
    {
        // Arrange
        var dto = new EnviarSolicitudAmistadDTO { DestinatarioId = 2 };
        SetupAuthenticatedUser(1);
        _mockEnviarSolicitudAmistad
            .Setup(caso => caso.Ejecutar(It.IsAny<EnviarSolicitudAmistadDTO>()))
            .ThrowsAsync(new Exception("Error inesperado"));

        // Act
        var resultado = await _controller.EnviarSolicitud(dto);

        // Assert
        var statusCodeResult = resultado as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult.StatusCode.Should().Be(500);
    }

    #endregion

    #region ObtenerSolicitudesPendientes Tests

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

        SetupAuthenticatedUser(usuarioId);

        // Act
        var resultado = await _controller.ObtenerSolicitudesPendientes();

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(solicitudesEsperadas);
    }

    [Fact]
    public async Task ObtenerSolicitudesPendientes_DeberiaRetornarUnauthorized_CuandoNoHayClaimDeUsuario()
    {
        // Arrange
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        // Act
        var resultado = await _controller.ObtenerSolicitudesPendientes();

        // Assert
        var unauthorizedResult = resultado as UnauthorizedObjectResult;
        unauthorizedResult.Should().NotBeNull();
        unauthorizedResult.StatusCode.Should().Be(401);
    }

    [Fact]
    public async Task ObtenerSolicitudesPendientes_DeberiaRetornarInternalServerError_CuandoOcurreExcepcion()
    {
        // Arrange
        SetupAuthenticatedUser(1);
        _mockObtenerSolicitudesPendientes
            .Setup(caso => caso.Ejecutar(It.IsAny<int>()))
            .ThrowsAsync(new Exception("Error inesperado"));

        // Act
        var resultado = await _controller.ObtenerSolicitudesPendientes();

        // Assert
        var statusCodeResult = resultado as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult.StatusCode.Should().Be(500);
    }

    #endregion

    #region AceptarSolicitud Tests

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

        SetupAuthenticatedUser(1);

        // Act
        var resultado = await _controller.AceptarSolicitud(aceptarSolicitudAmistadDto.AmistadId);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(resultadoEsperado);
    }

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

        SetupAuthenticatedUser(1);

        // Act
        var resultado = await _controller.AceptarSolicitud(aceptarSolicitudAmistadDto.AmistadId);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.StatusCode.Should().Be(400);
        badRequestResult.Value.Should().BeEquivalentTo(resultadoEsperado);
    }

    [Fact]
    public async Task AceptarSolicitud_DeberiaRetornarBadRequest_CuandoAmistadIdEsCero()
    {
        // Arrange
        SetupAuthenticatedUser(1);

        // Act
        var resultado = await _controller.AceptarSolicitud(0);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task AceptarSolicitud_DeberiaRetornarBadRequest_CuandoAmistadIdEsNegativo()
    {
        // Arrange
        SetupAuthenticatedUser(1);

        // Act
        var resultado = await _controller.AceptarSolicitud(-5);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task AceptarSolicitud_DeberiaRetornarUnauthorized_CuandoNoHayClaimDeUsuario()
    {
        // Arrange
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        // Act
        var resultado = await _controller.AceptarSolicitud(1);

        // Assert
        var unauthorizedResult = resultado as UnauthorizedObjectResult;
        unauthorizedResult.Should().NotBeNull();
        unauthorizedResult.StatusCode.Should().Be(401);
    }

    [Fact]
    public async Task AceptarSolicitud_DeberiaRetornarInternalServerError_CuandoOcurreExcepcion()
    {
        // Arrange
        SetupAuthenticatedUser(1);
        _mockAceptarSolicitud
            .Setup(caso => caso.Ejecutar(It.IsAny<AceptarSolicitudAmistadDTO>()))
            .ThrowsAsync(new Exception("Error inesperado"));

        // Act
        var resultado = await _controller.AceptarSolicitud(1);

        // Assert
        var statusCodeResult = resultado as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult.StatusCode.Should().Be(500);
    }

    #endregion

    #region EliminarAmigo Tests

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

        SetupAuthenticatedUser(1);

        // Act
        var resultado = await _controller.EliminarAmigo(eliminarDto.AmigoId);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(new { Completado = true, Mensaje = "Amistad eliminada correctamente." });
    }

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

        SetupAuthenticatedUser(1);

        // Act
        var resultado = await _controller.EliminarAmigo(eliminarDto.AmigoId);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.StatusCode.Should().Be(400);
        badRequestResult.Value.Should().BeEquivalentTo(new { Completado = false, Mensaje = "No se pudo eliminar la amistad." });
    }

    [Fact]
    public async Task EliminarAmigo_DeberiaRetornarBadRequest_CuandoAmigoIdEsCero()
    {
        // Arrange
        SetupAuthenticatedUser(1);

        // Act
        var resultado = await _controller.EliminarAmigo(0);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task EliminarAmigo_DeberiaRetornarBadRequest_CuandoAmigoIdEsNegativo()
    {
        // Arrange
        SetupAuthenticatedUser(1);

        // Act
        var resultado = await _controller.EliminarAmigo(-3);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task EliminarAmigo_DeberiaRetornarUnauthorized_CuandoNoHayClaimDeUsuario()
    {
        // Arrange
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        // Act
        var resultado = await _controller.EliminarAmigo(2);

        // Assert
        var unauthorizedResult = resultado as UnauthorizedObjectResult;
        unauthorizedResult.Should().NotBeNull();
        unauthorizedResult.StatusCode.Should().Be(401);
    }

    [Fact]
    public async Task EliminarAmigo_DeberiaRetornarInternalServerError_CuandoOcurreExcepcion()
    {
        // Arrange
        SetupAuthenticatedUser(1);
        _mockEliminar
            .Setup(caso => caso.Ejecutar(It.IsAny<EliminarAmigoDTO>()))
            .ThrowsAsync(new Exception("Error inesperado"));

        // Act
        var resultado = await _controller.EliminarAmigo(2);

        // Assert
        var statusCodeResult = resultado as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult.StatusCode.Should().Be(500);
    }

    #endregion

    #region ObtenerAmigos Tests

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

        SetupAuthenticatedUser(usuarioId);

        // Act
        var resultado = await _controller.ObtenerAmigos();

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(amigosEsperados);
    }

    [Fact]
    public async Task ObtenerAmigos_DeberiaRetornarUnauthorized_CuandoNoHayClaimDeUsuario()
    {
        // Arrange
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        // Act
        var resultado = await _controller.ObtenerAmigos();

        // Assert
        var unauthorizedResult = resultado as UnauthorizedObjectResult;
        unauthorizedResult.Should().NotBeNull();
        unauthorizedResult.StatusCode.Should().Be(401);
    }

    [Fact]
    public async Task ObtenerAmigos_DeberiaRetornarInternalServerError_CuandoOcurreExcepcion()
    {
        // Arrange
        SetupAuthenticatedUser(1);
        _mockObtenerPorId
            .Setup(caso => caso.Ejecutar(It.IsAny<long>()))
            .ThrowsAsync(new Exception("Error inesperado"));

        // Act
        var resultado = await _controller.ObtenerAmigos();

        // Assert
        var statusCodeResult = resultado as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult.StatusCode.Should().Be(500);
    }

    #endregion
}