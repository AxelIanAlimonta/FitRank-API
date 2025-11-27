using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using FitRank_API.Presentacion.Controllers;
using FitRank_API.Application.CasosDeUso.SolicitudCasosDeUso;
using FitRank_API.Application.DTOs.SolicitudDTO;
using FitRank_API.Domain.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace FitRank_API.tests.ControllersTests;

public class SolicitudRutinaProfesorControllerTests
{
    private readonly SolicitudRutinaProfesorController _controller;
    private readonly Mock<CrearSolicitudRutinaProfesorCasoDeUso> _mockCrear;
    private readonly Mock<ISolicitudRutinaProfesorRepositorio> _mockRepositorio;
    private readonly Mock<TomarSolicitudCasoDeUso> _mockTomar;
    private readonly Mock<FinalizarSolicitudCasoDeUso> _mockFinalizar;
    private readonly Mock<RechazarSolicitudCasoDeUso> _mockRechazar;
    private readonly Mock<TerminarSolicitudCasoDeUso> _mockTerminar;

    public SolicitudRutinaProfesorControllerTests()
    {
        _mockRepositorio = new Mock<ISolicitudRutinaProfesorRepositorio>();
        _mockCrear = new Mock<CrearSolicitudRutinaProfesorCasoDeUso>(_mockRepositorio.Object);
        _mockTomar = new Mock<TomarSolicitudCasoDeUso>(_mockRepositorio.Object);
        _mockFinalizar = new Mock<FinalizarSolicitudCasoDeUso>(_mockRepositorio.Object);
        _mockRechazar = new Mock<RechazarSolicitudCasoDeUso>(_mockRepositorio.Object);
        _mockTerminar = new Mock<TerminarSolicitudCasoDeUso>(_mockRepositorio.Object);

        _controller = new SolicitudRutinaProfesorController(
            _mockCrear.Object,
            _mockRepositorio.Object,
            _mockTomar.Object,
            _mockFinalizar.Object,
            _mockRechazar.Object,
            _mockTerminar.Object
        );
    }

    private void SetupUserClaims(long userId)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };
    }

    #region Crear Tests

    [Fact]
    public async Task Crear_SocioIdCero_RetornaBadRequest()
    {
        var dto = new CrearSolicitudRutinaProfesorDTO();

        var result = await _controller.Crear(0, dto);

        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Crear_SocioIdNegativo_RetornaBadRequest()
    {
        var dto = new CrearSolicitudRutinaProfesorDTO();

        var result = await _controller.Crear(-5, dto);

        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Crear_DtoNulo_RetornaBadRequest()
    {
        var result = await _controller.Crear(1, null!);

        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Crear_Exitoso_RetornaOk()
    {
        var dto = new CrearSolicitudRutinaProfesorDTO
        {
            NombreSocio = "Juan",
            Edad = 30,
            Nivel = "Intermedio"
        };
        _mockCrear.Setup(x => x.EjecutarAsync(dto, 1)).ReturnsAsync(10L);

        var result = await _controller.Crear(1, dto);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task Crear_ExcepcionGenerica_RetornaInternalServerError()
    {
        var dto = new CrearSolicitudRutinaProfesorDTO();
        _mockCrear.Setup(x => x.EjecutarAsync(dto, 1)).ThrowsAsync(new Exception());

        var result = await _controller.Crear(1, dto);

        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region TerminarSolicitud Tests

    [Fact]
    public async Task TerminarSolicitud_IdCero_RetornaBadRequest()
    {
        var result = await _controller.TerminarSolicitud(0);

        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task TerminarSolicitud_IdNegativo_RetornaBadRequest()
    {
        var result = await _controller.TerminarSolicitud(-3);

        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task TerminarSolicitud_Exitoso_RetornaOk()
    {
        _mockTerminar.Setup(x => x.EjecutarAsync(1)).ReturnsAsync(true);

        var result = await _controller.TerminarSolicitud(1);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task TerminarSolicitud_NoEncontrada_RetornaNotFound()
    {
        _mockTerminar.Setup(x => x.EjecutarAsync(999)).ReturnsAsync(false);

        var result = await _controller.TerminarSolicitud(999);

        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task TerminarSolicitud_ExcepcionGenerica_RetornaInternalServerError()
    {
        _mockTerminar.Setup(x => x.EjecutarAsync(1)).ThrowsAsync(new Exception());

        var result = await _controller.TerminarSolicitud(1);

        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ObtenerPendientes Tests

    [Fact]
    public async Task ObtenerPendientes_Exitoso_RetornaOk()
    {
        var solicitudes = new List<SolicitudRutinaProfesorDTO>
        {
            new SolicitudRutinaProfesorDTO { Id = 1, NombreSocio = "Juan" },
            new SolicitudRutinaProfesorDTO { Id = 2, NombreSocio = "María" }
        };
        _mockRepositorio.Setup(x => x.ObtenerPendientesAsync()).ReturnsAsync(solicitudes);

        var result = await _controller.ObtenerPendientes();

        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(solicitudes);
    }

    [Fact]
    public async Task ObtenerPendientes_ExcepcionGenerica_RetornaInternalServerError()
    {
        _mockRepositorio.Setup(x => x.ObtenerPendientesAsync()).ThrowsAsync(new Exception());

        var result = await _controller.ObtenerPendientes();

        var statusCodeResult = result.Result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region TomarSolicitud Tests

    [Fact]
    public async Task TomarSolicitud_DtoNulo_RetornaBadRequest()
    {
        SetupUserClaims(1);

        var result = await _controller.TomarSolicitud(null!);

        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task TomarSolicitud_SolicitudIdCero_RetornaBadRequest()
    {
        SetupUserClaims(1);
        var dto = new TomarSolicitudDTO { SolicitudId = 0 };

        var result = await _controller.TomarSolicitud(dto);

        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task TomarSolicitud_SolicitudIdNegativo_RetornaBadRequest()
    {
        SetupUserClaims(1);
        var dto = new TomarSolicitudDTO { SolicitudId = -5 };

        var result = await _controller.TomarSolicitud(dto);

        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task TomarSolicitud_Exitoso_RetornaOk()
    {
        SetupUserClaims(5);
        var dto = new TomarSolicitudDTO { SolicitudId = 1 };
        _mockTomar.Setup(x => x.EjecutarAsync(1, 5)).ReturnsAsync(true);

        var result = await _controller.TomarSolicitud(dto);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task TomarSolicitud_NoSePudoTomar_RetornaBadRequest()
    {
        SetupUserClaims(5);
        var dto = new TomarSolicitudDTO { SolicitudId = 1 };
        _mockTomar.Setup(x => x.EjecutarAsync(1, 5)).ReturnsAsync(false);

        var result = await _controller.TomarSolicitud(dto);

        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task TomarSolicitud_ExcepcionGenerica_RetornaInternalServerError()
    {
        SetupUserClaims(5);
        var dto = new TomarSolicitudDTO { SolicitudId = 1 };
        _mockTomar.Setup(x => x.EjecutarAsync(1, 5)).ThrowsAsync(new Exception());

        var result = await _controller.TomarSolicitud(dto);

        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region FinalizarSolicitud Tests

    [Fact]
    public async Task FinalizarSolicitud_DtoNulo_RetornaBadRequest()
    {
        var result = await _controller.FinalizarSolicitud(null!);

        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task FinalizarSolicitud_SolicitudIdCero_RetornaBadRequest()
    {
        var dto = new FinalizarSolicitudDTO { SolicitudId = 0, RutinaId = 1 };

        var result = await _controller.FinalizarSolicitud(dto);

        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task FinalizarSolicitud_RutinaIdCero_RetornaBadRequest()
    {
        var dto = new FinalizarSolicitudDTO { SolicitudId = 1, RutinaId = 0 };

        var result = await _controller.FinalizarSolicitud(dto);

        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task FinalizarSolicitud_Exitoso_RetornaOk()
    {
        var dto = new FinalizarSolicitudDTO { SolicitudId = 1, RutinaId = 10, MensajeProfesor = "Completado" };
        _mockFinalizar.Setup(x => x.EjecutarAsync(1, 10, "Completado")).ReturnsAsync(true);

        var result = await _controller.FinalizarSolicitud(dto);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task FinalizarSolicitud_NoSePudoFinalizar_RetornaBadRequest()
    {
        var dto = new FinalizarSolicitudDTO { SolicitudId = 1, RutinaId = 10 };
        _mockFinalizar.Setup(x => x.EjecutarAsync(1, 10, null)).ReturnsAsync(false);

        var result = await _controller.FinalizarSolicitud(dto);

        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task FinalizarSolicitud_ExcepcionGenerica_RetornaInternalServerError()
    {
        var dto = new FinalizarSolicitudDTO { SolicitudId = 1, RutinaId = 10 };
        _mockFinalizar.Setup(x => x.EjecutarAsync(1, 10, null)).ThrowsAsync(new Exception());

        var result = await _controller.FinalizarSolicitud(dto);

        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region RechazarSolicitud Tests

    [Fact]
    public async Task RechazarSolicitud_DtoNulo_RetornaBadRequest()
    {
        SetupUserClaims(1);

        var result = await _controller.RechazarSolicitud(null!);

        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task RechazarSolicitud_SolicitudIdCero_RetornaBadRequest()
    {
        SetupUserClaims(1);
        var dto = new RechazarSolicitudDTO { SolicitudId = 0 };

        var result = await _controller.RechazarSolicitud(dto);

        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task RechazarSolicitud_SolicitudIdNegativo_RetornaBadRequest()
    {
        SetupUserClaims(1);
        var dto = new RechazarSolicitudDTO { SolicitudId = -3 };

        var result = await _controller.RechazarSolicitud(dto);

        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task RechazarSolicitud_Exitoso_RetornaOk()
    {
        SetupUserClaims(5);
        var dto = new RechazarSolicitudDTO { SolicitudId = 1, MensajeProfesor = "No disponible" };
        _mockRechazar.Setup(x => x.EjecutarAsync(1, 5, "No disponible")).ReturnsAsync(true);

        var result = await _controller.RechazarSolicitud(dto);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task RechazarSolicitud_NoSePudoRechazar_RetornaBadRequest()
    {
        SetupUserClaims(5);
        var dto = new RechazarSolicitudDTO { SolicitudId = 1 };
        _mockRechazar.Setup(x => x.EjecutarAsync(1, 5, null)).ReturnsAsync(false);

        var result = await _controller.RechazarSolicitud(dto);

        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task RechazarSolicitud_ExcepcionGenerica_RetornaInternalServerError()
    {
        SetupUserClaims(5);
        var dto = new RechazarSolicitudDTO { SolicitudId = 1 };
        _mockRechazar.Setup(x => x.EjecutarAsync(1, 5, null)).ThrowsAsync(new Exception());

        var result = await _controller.RechazarSolicitud(dto);

        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion
}
