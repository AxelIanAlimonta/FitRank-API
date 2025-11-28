/*using System.Security.Claims;
using AutoMapper;
using FitRank_API.Application.CasosDeUso.Asistencia;
using FitRank_API.Application.CasosDeUso.AsistenciaCasosDeUso;
using FitRank_API.Application.DTOs.Asistencia;
using FitRank_API.Application.DTOs.QR;
using FitRank_API.Application.DTOs.SocioDTOs;
using FitRank_API.Domain.Interfaces;
using FitRank_API.Presentacion.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace FitRank_API.tests.ControllersTests;

public class AsistenciaControllerTests
{
    private readonly AsistenciaController _controller;
    private readonly Mock<ObtenerAsistenciasPorUsuarioCasoDeUso> _mockObtenerPorUsuario;
    private readonly Mock<ObtenerAsistenciasPorDiaCasoDeUso> _mockObtenerPorDia;
    private readonly Mock<ObtenerAsistenciasDetalladasPorUsuarioCasoDeUso> _mockObtenerDetalladas;
    private readonly Mock<ValidarAsistenciaQrCasoDeUso> _mockValidarQr;
    private readonly Mock<ObtenerTodasLasAsistenciasCasoDeUso> _mockObtenerTodas;
    private readonly Mock<DetectarSociosInactivosCasoDeUso> _mockDetectarInactivos;

    public AsistenciaControllerTests()
    {
        var mockAsistenciaRepo = new Mock<IAsistenciaRepositorio>();
        var mockUsuarioRepo = new Mock<IUsuarioRepositorio>();
        var mockGimnasioRepo = new Mock<IGimnasioRepositorio>();
        var mockMapper = new Mock<IMapper>();
        var mockConfig = new Mock<IConfiguration>();
        var mockHub = new Mock<IHubContext<FitRank_API.Application.Hubs.NotificacionesHub>>();

        _mockObtenerPorUsuario = new Mock<ObtenerAsistenciasPorUsuarioCasoDeUso>(mockAsistenciaRepo.Object, mockMapper.Object);
        _mockObtenerPorDia = new Mock<ObtenerAsistenciasPorDiaCasoDeUso>(mockAsistenciaRepo.Object);
        _mockObtenerDetalladas = new Mock<ObtenerAsistenciasDetalladasPorUsuarioCasoDeUso>(mockAsistenciaRepo.Object, mockUsuarioRepo.Object, mockMapper.Object);
        _mockValidarQr = new Mock<ValidarAsistenciaQrCasoDeUso>(mockUsuarioRepo.Object, mockAsistenciaRepo.Object, mockGimnasioRepo.Object, mockConfig.Object, mockHub.Object);
        _mockObtenerTodas = new Mock<ObtenerTodasLasAsistenciasCasoDeUso>(mockAsistenciaRepo.Object, mockMapper.Object);
        _mockDetectarInactivos = new Mock<DetectarSociosInactivosCasoDeUso>(mockAsistenciaRepo.Object, mockUsuarioRepo.Object);

        _controller = new AsistenciaController(
            _mockObtenerPorUsuario.Object,
            _mockObtenerPorDia.Object,
            _mockObtenerDetalladas.Object,
            _mockValidarQr.Object,
            _mockObtenerTodas.Object,
            _mockDetectarInactivos.Object
        );
    }

    private void SetupAuthenticatedUser(int userId, string role = "Socio")
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, role)
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    #region ObtenerMias Tests

    [Fact]
    public async Task ObtenerMias_Exitoso_RetornaOk()
    {
        // Arrange
        SetupAuthenticatedUser(1, "Socio");
        var asistencias = new List<AsistenciaResponseDTO>
        {
            new AsistenciaResponseDTO { AsistenciaId = 1, Success = true }
        };
        _mockObtenerPorUsuario.Setup(x => x.Ejecutar(1)).ReturnsAsync(asistencias);

        // Act
        var result = await _controller.ObtenerMias();

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(asistencias);
    }

    [Fact]
    public async Task ObtenerMias_SinClaimDeUsuario_RetornaUnauthorized()
    {
        // Arrange
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        // Act
        var result = await _controller.ObtenerMias();

        // Assert
        var unauthorizedResult = result as UnauthorizedObjectResult;
        unauthorizedResult.Should().NotBeNull();
        unauthorizedResult!.StatusCode.Should().Be(401);
    }

    [Fact]
    public async Task ObtenerMias_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        SetupAuthenticatedUser(1, "Socio");
        _mockObtenerPorUsuario.Setup(x => x.Ejecutar(1)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.ObtenerMias();

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ObtenerAsistenciasPorDia Tests

    [Fact]
    public async Task ObtenerAsistenciasPorDia_Exitoso_RetornaOk()
    {
        // Arrange
        SetupAuthenticatedUser(1, "Admin");
        var resultado = new List<AsistenciaPorDiaDTO>();
        _mockObtenerPorDia.Setup(x => x.Ejecutar(1, null, null)).ReturnsAsync(resultado);

        // Act
        var result = await _controller.ObtenerAsistenciasPorDia();

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task ObtenerAsistenciasPorDia_FechaDesdeMayorQueHasta_RetornaBadRequest()
    {
        // Arrange
        SetupAuthenticatedUser(1, "Admin");
        var desde = DateTime.Now.AddDays(5);
        var hasta = DateTime.Now;

        // Act
        var result = await _controller.ObtenerAsistenciasPorDia(desde, hasta);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerAsistenciasPorDia_SinAdminId_RetornaUnauthorized()
    {
        // Arrange
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        // Act
        var result = await _controller.ObtenerAsistenciasPorDia();

        // Assert
        var unauthorizedResult = result as UnauthorizedObjectResult;
        unauthorizedResult.Should().NotBeNull();
        unauthorizedResult!.StatusCode.Should().Be(401);
    }

    [Fact]
    public async Task ObtenerAsistenciasPorDia_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        SetupAuthenticatedUser(1, "Admin");
        _mockObtenerPorDia.Setup(x => x.Ejecutar(It.IsAny<long>(), It.IsAny<DateTime?>(), It.IsAny<DateTime?>()))
            .ThrowsAsync(new Exception());

        // Act
        var result = await _controller.ObtenerAsistenciasPorDia();

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ObtenerTodasLasAsistencias Tests

    [Fact]
    public async Task ObtenerTodasLasAsistencias_Exitoso_RetornaOk()
    {
        // Arrange
        SetupAuthenticatedUser(1, "Admin");
        var asistencias = new List<AsistenciaListadoDTO>();
        _mockObtenerTodas.Setup(x => x.Ejecutar()).ReturnsAsync(asistencias);

        // Act
        var result = await _controller.ObtenerTodasLasAsistencias();

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task ObtenerTodasLasAsistencias_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        SetupAuthenticatedUser(1, "Admin");
        _mockObtenerTodas.Setup(x => x.Ejecutar()).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.ObtenerTodasLasAsistencias();

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ObtenerPorUsuario Tests

    [Fact]
    public async Task ObtenerPorUsuario_Exitoso_RetornaOk()
    {
        // Arrange
        SetupAuthenticatedUser(1, "Admin");
        var asistencias = new List<AsistenciaResponseDTO>();
        _mockObtenerPorUsuario.Setup(x => x.Ejecutar(5)).ReturnsAsync(asistencias);

        // Act
        var result = await _controller.ObtenerPorUsuario(5);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task ObtenerPorUsuario_IdCero_RetornaBadRequest()
    {
        // Arrange
        SetupAuthenticatedUser(1, "Admin");

        // Act
        var result = await _controller.ObtenerPorUsuario(0);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerPorUsuario_IdNegativo_RetornaBadRequest()
    {
        // Arrange
        SetupAuthenticatedUser(1, "Admin");

        // Act
        var result = await _controller.ObtenerPorUsuario(-1);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerPorUsuario_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        SetupAuthenticatedUser(1, "Admin");
        _mockObtenerPorUsuario.Setup(x => x.Ejecutar(It.IsAny<int>())).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.ObtenerPorUsuario(5);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ObtenerAsistenciasDetalladasPorUsuario Tests

    [Fact]
    public async Task ObtenerAsistenciasDetalladasPorUsuario_Exitoso_RetornaOk()
    {
        // Arrange
        SetupAuthenticatedUser(1, "Admin");
        var resultado = new DetalleUsuarioAsistenciaRespuestaDTO { Exito = true };
        _mockObtenerDetalladas.Setup(x => x.Ejecutar(5)).ReturnsAsync(resultado);

        // Act
        var result = await _controller.ObtenerAsistenciasDetalladasPorUsuario(5);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task ObtenerAsistenciasDetalladasPorUsuario_IdCero_RetornaBadRequest()
    {
        // Arrange
        SetupAuthenticatedUser(1, "Admin");

        // Act
        var result = await _controller.ObtenerAsistenciasDetalladasPorUsuario(0);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerAsistenciasDetalladasPorUsuario_IdNegativo_RetornaBadRequest()
    {
        // Arrange
        SetupAuthenticatedUser(1, "Admin");

        // Act
        var result = await _controller.ObtenerAsistenciasDetalladasPorUsuario(-5);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerAsistenciasDetalladasPorUsuario_SocioAccediendoAOtroUsuario_RetornaForbid()
    {
        // Arrange
        SetupAuthenticatedUser(1, "Socio");
        var resultado = new DetalleUsuarioAsistenciaRespuestaDTO { Exito = true };
        _mockObtenerDetalladas.Setup(x => x.Ejecutar(5)).ReturnsAsync(resultado);

        // Act
        var result = await _controller.ObtenerAsistenciasDetalladasPorUsuario(5);

        // Assert
        var forbidResult = result as ForbidResult;
        forbidResult.Should().NotBeNull();
    }

    [Fact]
    public async Task ObtenerAsistenciasDetalladasPorUsuario_SocioAccediendoASuPropio_RetornaOk()
    {
        // Arrange
        SetupAuthenticatedUser(5, "Socio");
        var resultado = new DetalleUsuarioAsistenciaRespuestaDTO { Exito = true };
        _mockObtenerDetalladas.Setup(x => x.Ejecutar(5)).ReturnsAsync(resultado);

        // Act
        var result = await _controller.ObtenerAsistenciasDetalladasPorUsuario(5);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task ObtenerAsistenciasDetalladasPorUsuario_SinClaimDeUsuario_RetornaUnauthorized()
    {
        // Arrange
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        // Act
        var result = await _controller.ObtenerAsistenciasDetalladasPorUsuario(5);

        // Assert
        var unauthorizedResult = result as UnauthorizedObjectResult;
        unauthorizedResult.Should().NotBeNull();
        unauthorizedResult!.StatusCode.Should().Be(401);
    }

    [Fact]
    public async Task ObtenerAsistenciasDetalladasPorUsuario_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        SetupAuthenticatedUser(1, "Admin");
        _mockObtenerDetalladas.Setup(x => x.Ejecutar(It.IsAny<long>())).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.ObtenerAsistenciasDetalladasPorUsuario(5);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ValidarQr Tests

    [Fact]
    public async Task ValidarQr_Exitoso_RetornaOk()
    {
        // Arrange
        SetupAuthenticatedUser(1, "Admin");
        var dto = new QrValidationDTO { QrData = "test-qr-data" };
        var response = new QrValidationResponseDTO { Valido = true, Mensaje = "ok" };
        _mockValidarQr.Setup(x => x.Ejecutar(dto, 1)).ReturnsAsync(response);

        // Act
        var result = await _controller.ValidarQr(dto);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task ValidarQr_DtoNulo_RetornaBadRequest()
    {
        // Arrange
        SetupAuthenticatedUser(1, "Admin");

        // Act
        var result = await _controller.ValidarQr(null);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ValidarQr_ModelStateInvalido_RetornaBadRequest()
    {
        // Arrange
        SetupAuthenticatedUser(1, "Admin");
        _controller.ModelState.AddModelError("QrData", "Requerido");
        var dto = new QrValidationDTO();

        // Act
        var result = await _controller.ValidarQr(dto);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ValidarQr_QrNoValido_RetornaBadRequest()
    {
        // Arrange
        SetupAuthenticatedUser(1, "Admin");
        var dto = new QrValidationDTO { QrData = "test-qr-data" };
        var response = new QrValidationResponseDTO { Valido = false, Mensaje = "QR inválido" };
        _mockValidarQr.Setup(x => x.Ejecutar(dto, 1)).ReturnsAsync(response);

        // Act
        var result = await _controller.ValidarQr(dto);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ValidarQr_SinAdminId_RetornaUnauthorized()
    {
        // Arrange
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };
        var dto = new QrValidationDTO { QrData = "test-qr-data" };

        // Act
        var result = await _controller.ValidarQr(dto);

        // Assert
        var unauthorizedResult = result as UnauthorizedObjectResult;
        unauthorizedResult.Should().NotBeNull();
        unauthorizedResult!.StatusCode.Should().Be(401);
    }

    [Fact]
    public async Task ValidarQr_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        SetupAuthenticatedUser(1, "Admin");
        var dto = new QrValidationDTO { QrData = "test-qr-data" };
        _mockValidarQr.Setup(x => x.Ejecutar(It.IsAny<QrValidationDTO>(), It.IsAny<int>()))
            .ThrowsAsync(new Exception());

        // Act
        var result = await _controller.ValidarQr(dto);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ObtenerSociosInactivos Tests

    [Fact]
    public async Task ObtenerSociosInactivos_Exitoso_RetornaOk()
    {
        // Arrange
        SetupAuthenticatedUser(1, "Admin");
        var resultado = new List<SocioInactivoDTO>();
        _mockDetectarInactivos.Setup(x => x.Ejecutar(5)).ReturnsAsync(resultado);

        // Act
        var result = await _controller.ObtenerSociosInactivos(5);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task ObtenerSociosInactivos_DiasValorPorDefecto_RetornaOk()
    {
        // Arrange
        SetupAuthenticatedUser(1, "Admin");
        var resultado = new List<SocioInactivoDTO>();
        _mockDetectarInactivos.Setup(x => x.Ejecutar(5)).ReturnsAsync(resultado);

        // Act
        var result = await _controller.ObtenerSociosInactivos();

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task ObtenerSociosInactivos_DiasCero_RetornaBadRequest()
    {
        // Arrange
        SetupAuthenticatedUser(1, "Admin");

        // Act
        var result = await _controller.ObtenerSociosInactivos(0);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerSociosInactivos_DiasNegativo_RetornaBadRequest()
    {
        // Arrange
        SetupAuthenticatedUser(1, "Admin");

        // Act
        var result = await _controller.ObtenerSociosInactivos(-5);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerSociosInactivos_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        SetupAuthenticatedUser(1, "Admin");
        _mockDetectarInactivos.Setup(x => x.Ejecutar(It.IsAny<int>())).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.ObtenerSociosInactivos(5);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion
}
*/