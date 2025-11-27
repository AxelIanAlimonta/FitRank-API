using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using AutoMapper;
using FitRank_API.Domain.Interfaces;
using FitRank_API.Presentacion.Controllers;
using FitRank_API.Application.CasosDeUso.NotificacionCasoDeUso;
using FitRank_API.Application.CasosDeUso.NotificacionCasosDeUso;
using FitRank_API.Application.DTOs.NotificacionDTOs;
using FitRank_API.Application.Hubs;
using FitRank_API.Domain.Entities;
using System.Security.Claims;

namespace FitRank_API.tests.ControllersTests;

public class NotificacionControllerTests
{
    private readonly NotificacionController _controller;
    private readonly Mock<AgregarNotificacionCasoDeUso> _mockAgregar;
    private readonly Mock<ObtenerNotificacionPorUsuarioCasoDeUso> _mockObtener;
    private readonly Mock<RetenerSocioCasoDeUso> _mockRetener;
    private readonly Mock<MarcarNotificacionLeidaCasoDeUso> _mockMarcar;
    private readonly Mock<EnviarNotificacionMasivaCasoDeUso> _mockEnviarMasiva;
    private readonly Mock<ObtenerHistorialNotificacionesCasoDeUso> _mockObtenerHistorial;
    private readonly Mock<ObtenerUsuariosParaNotificacionCasoDeUso> _mockObtenerUsuarios;
    private readonly Mock<EnviarNotificacionIndividualCasoDeUso> _mockEnviarIndividual;
    private readonly Mock<IHubContext<NotificacionesHub>> _mockHubContext;

    public NotificacionControllerTests()
    {
        var mockRepo = new Mock<INotificacionRepositorio>();
        var mockMapper = new Mock<IMapper>();
        var mockAdminRepo = new Mock<IAdministradorRepositorio>();
        var mockProfRepo = new Mock<IProfesorRepositorio>();
        var mockSocioRepo = new Mock<ISocioRepositorio>();
        var mockUsuarioRepo = new Mock<IUsuarioRepositorio>();

        _mockAgregar = new Mock<AgregarNotificacionCasoDeUso>(mockRepo.Object, mockMapper.Object);
        _mockObtener = new Mock<ObtenerNotificacionPorUsuarioCasoDeUso>(mockRepo.Object, mockMapper.Object);
        _mockRetener = new Mock<RetenerSocioCasoDeUso>(mockUsuarioRepo.Object, mockRepo.Object);
        _mockMarcar = new Mock<MarcarNotificacionLeidaCasoDeUso>(mockRepo.Object);
        _mockEnviarMasiva = new Mock<EnviarNotificacionMasivaCasoDeUso>(
            mockRepo.Object, mockAdminRepo.Object, mockProfRepo.Object, mockSocioRepo.Object, Mock.Of<IHubContext<NotificacionesHub>>());
        _mockObtenerHistorial = new Mock<ObtenerHistorialNotificacionesCasoDeUso>(
            mockRepo.Object, mockAdminRepo.Object, mockProfRepo.Object, mockSocioRepo.Object);
        _mockObtenerUsuarios = new Mock<ObtenerUsuariosParaNotificacionCasoDeUso>(mockUsuarioRepo.Object, mockMapper.Object);
        _mockEnviarIndividual = new Mock<EnviarNotificacionIndividualCasoDeUso>(mockRepo.Object);
        _mockHubContext = new Mock<IHubContext<NotificacionesHub>>();

        _controller = new NotificacionController(
            _mockAgregar.Object,
            _mockObtener.Object,
            _mockRetener.Object,
            _mockMarcar.Object,
            _mockEnviarMasiva.Object,
            _mockObtenerHistorial.Object,
            _mockObtenerUsuarios.Object,
            _mockEnviarIndividual.Object,
            _mockHubContext.Object
        );
    }

    #region EnviarIndividual Tests

    [Fact]
    public async Task EnviarIndividual_Exitoso_RetornaOk()
    {
        // Arrange
        var dto = new EnviarIndividualDTO
        {
            UsuarioReceptorId = 5,
            Titulo = "Test",
            Mensaje = "Mensaje de prueba"
        };

        var notificacion = new Notificacion
        {
            Id = 1,
            Titulo = "Test",
            Mensaje = "Mensaje de prueba",
            UsuarioReceptorId = 5,
            FechaEnvio = DateTime.UtcNow
        };

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "10")
        };
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        _mockEnviarIndividual.Setup(x => x.Ejecutar(10, 5, "Test", "Mensaje de prueba"))
            .ReturnsAsync(notificacion);

        var mockClients = new Mock<IHubClients>();
        var mockClientProxy = new Mock<IClientProxy>();
        _mockHubContext.Setup(x => x.Clients).Returns(mockClients.Object);
        mockClients.Setup(x => x.Group(It.IsAny<string>())).Returns(mockClientProxy.Object);

        // Act
        var result = await _controller.EnviarIndividual(dto);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task EnviarIndividual_DtoNulo_RetornaBadRequest()
    {
        // Act
        var result = await _controller.EnviarIndividual(null!);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task EnviarIndividual_UsuarioReceptorIdCero_RetornaBadRequest()
    {
        // Arrange
        var dto = new EnviarIndividualDTO { UsuarioReceptorId = 0 };

        // Act
        var result = await _controller.EnviarIndividual(dto);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task EnviarIndividual_UsuarioReceptorIdNegativo_RetornaBadRequest()
    {
        // Arrange
        var dto = new EnviarIndividualDTO { UsuarioReceptorId = -5 };

        // Act
        var result = await _controller.EnviarIndividual(dto);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task EnviarIndividual_ModelStateInvalido_RetornaBadRequest()
    {
        // Arrange
        _controller.ModelState.AddModelError("Titulo", "Requerido");
        var dto = new EnviarIndividualDTO { UsuarioReceptorId = 5 };

        // Act
        var result = await _controller.EnviarIndividual(dto);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task EnviarIndividual_ClaimNulo_RetornaUnauthorized()
    {
        // Arrange
        var dto = new EnviarIndividualDTO { UsuarioReceptorId = 5 };
        var claims = new List<Claim>();
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        // Act
        var result = await _controller.EnviarIndividual(dto);

        // Assert
        var unauthorizedResult = result as UnauthorizedObjectResult;
        unauthorizedResult.Should().NotBeNull();
        unauthorizedResult!.StatusCode.Should().Be(401);
    }

    [Fact]
    public async Task EnviarIndividual_ClaimInvalido_RetornaBadRequest()
    {
        // Arrange
        var dto = new EnviarIndividualDTO { UsuarioReceptorId = 5 };
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "invalid")
        };
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        // Act
        var result = await _controller.EnviarIndividual(dto);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task EnviarIndividual_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        var dto = new EnviarIndividualDTO { UsuarioReceptorId = 5 };
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "10")
        };
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        _mockEnviarIndividual.Setup(x => x.Ejecutar(It.IsAny<long>(), It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception());

        // Act
        var result = await _controller.EnviarIndividual(dto);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region Crear Tests

    [Fact]
    public async Task Crear_Exitoso_RetornaCreatedAtAction()
    {
        // Arrange
        var dto = new AgregarNotificacionDTO
        {
            Titulo = "Test",
            Mensaje = "Mensaje",
            UsuarioEmisorId = 1,
            UsuarioReceptorId = 2
        };

        var notificacion = new ObtenerNotificacionDTO
        {
            Id = 1,
            Titulo = "Test",
            Mensaje = "Mensaje"
        };

        _mockAgregar.Setup(x => x.Ejecutar(dto)).ReturnsAsync(notificacion);

        // Act
        var result = await _controller.Crear(dto);

        // Assert
        var createdResult = result as CreatedAtActionResult;
        createdResult.Should().NotBeNull();
        createdResult!.StatusCode.Should().Be(201);
    }

    [Fact]
    public async Task Crear_DtoNulo_RetornaBadRequest()
    {
        // Act
        var result = await _controller.Crear(null!);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Crear_ModelStateInvalido_RetornaBadRequest()
    {
        // Arrange
        _controller.ModelState.AddModelError("Titulo", "Requerido");
        var dto = new AgregarNotificacionDTO();

        // Act
        var result = await _controller.Crear(dto);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Crear_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        var dto = new AgregarNotificacionDTO();
        _mockAgregar.Setup(x => x.Ejecutar(dto)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.Crear(dto);

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
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "5")
        };
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        var notificaciones = new List<ObtenerNotificacionDTO>
        {
            new ObtenerNotificacionDTO { Id = 1, Titulo = "Test" }
        };

        _mockObtener.Setup(x => x.Ejecutar(5)).ReturnsAsync(notificaciones);

        // Act
        var result = await _controller.ObtenerPorUsuario();

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task ObtenerPorUsuario_ClaimNulo_RetornaUnauthorized()
    {
        // Arrange
        var claims = new List<Claim>();
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        // Act
        var result = await _controller.ObtenerPorUsuario();

        // Assert
        var unauthorizedResult = result as UnauthorizedObjectResult;
        unauthorizedResult.Should().NotBeNull();
        unauthorizedResult!.StatusCode.Should().Be(401);
    }

    [Fact]
    public async Task ObtenerPorUsuario_ClaimInvalido_RetornaBadRequest()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "invalid")
        };
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        // Act
        var result = await _controller.ObtenerPorUsuario();

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerPorUsuario_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "5")
        };
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        _mockObtener.Setup(x => x.Ejecutar(5)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.ObtenerPorUsuario();

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region RetenerSocio Tests

    [Fact]
    public async Task RetenerSocio_Exitoso_RetornaOk()
    {
        // Arrange
        long socioId = 10;
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "5")
        };
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        _mockRetener.Setup(x => x.Ejecutar(5, socioId)).ReturnsAsync(true);

        // Act
        var result = await _controller.RetenerSocio(socioId);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task RetenerSocio_SocioIdCero_RetornaBadRequest()
    {
        // Act
        var result = await _controller.RetenerSocio(0);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task RetenerSocio_SocioIdNegativo_RetornaBadRequest()
    {
        // Act
        var result = await _controller.RetenerSocio(-3);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task RetenerSocio_ClaimNulo_RetornaUnauthorized()
    {
        // Arrange
        var claims = new List<Claim>();
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        // Act
        var result = await _controller.RetenerSocio(10);

        // Assert
        var unauthorizedResult = result as UnauthorizedObjectResult;
        unauthorizedResult.Should().NotBeNull();
        unauthorizedResult!.StatusCode.Should().Be(401);
    }

    [Fact]
    public async Task RetenerSocio_NoSePudoEnviar_RetornaBadRequest()
    {
        // Arrange
        long socioId = 10;
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "5")
        };
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        _mockRetener.Setup(x => x.Ejecutar(5, socioId)).ReturnsAsync(false);

        // Act
        var result = await _controller.RetenerSocio(socioId);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task RetenerSocio_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "5")
        };
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        _mockRetener.Setup(x => x.Ejecutar(5, 10)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.RetenerSocio(10);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region MarcarLeida Tests

    [Fact]
    public async Task MarcarLeida_Exitoso_RetornaOk()
    {
        // Arrange
        long notificacionId = 15;
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "5")
        };
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        _mockMarcar.Setup(x => x.Ejecutar(5, notificacionId)).ReturnsAsync(true);

        // Act
        var result = await _controller.MarcarLeida(notificacionId);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task MarcarLeida_IdCero_RetornaBadRequest()
    {
        // Act
        var result = await _controller.MarcarLeida(0);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task MarcarLeida_IdNegativo_RetornaBadRequest()
    {
        // Act
        var result = await _controller.MarcarLeida(-7);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task MarcarLeida_ClaimNulo_RetornaUnauthorized()
    {
        // Arrange
        var claims = new List<Claim>();
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        // Act
        var result = await _controller.MarcarLeida(15);

        // Assert
        var unauthorizedResult = result as UnauthorizedObjectResult;
        unauthorizedResult.Should().NotBeNull();
        unauthorizedResult!.StatusCode.Should().Be(401);
    }

    [Fact]
    public async Task MarcarLeida_NoSePudoMarcar_RetornaBadRequest()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "5")
        };
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        _mockMarcar.Setup(x => x.Ejecutar(5, 15)).ReturnsAsync(false);

        // Act
        var result = await _controller.MarcarLeida(15);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task MarcarLeida_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "5")
        };
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        _mockMarcar.Setup(x => x.Ejecutar(5, 15)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.MarcarLeida(15);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region EnviarMasiva Tests

    [Fact]
    public async Task EnviarMasiva_Exitoso_RetornaOk()
    {
        // Arrange
        var dto = new EnviarMasivaDTO
        {
            Titulo = "Test Masivo",
            Mensaje = "Mensaje masivo"
        };

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "5")
        };
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        _mockEnviarMasiva.Setup(x => x.Ejecutar(5, "Test Masivo", "Mensaje masivo")).ReturnsAsync(50);

        // Act
        var result = await _controller.EnviarMasiva(dto);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task EnviarMasiva_DtoNulo_RetornaBadRequest()
    {
        // Act
        var result = await _controller.EnviarMasiva(null!);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task EnviarMasiva_ModelStateInvalido_RetornaBadRequest()
    {
        // Arrange
        _controller.ModelState.AddModelError("Titulo", "Requerido");
        var dto = new EnviarMasivaDTO();

        // Act
        var result = await _controller.EnviarMasiva(dto);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task EnviarMasiva_ClaimNulo_RetornaUnauthorized()
    {
        // Arrange
        var dto = new EnviarMasivaDTO { Titulo = "Test", Mensaje = "Test" };
        var claims = new List<Claim>();
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        // Act
        var result = await _controller.EnviarMasiva(dto);

        // Assert
        var unauthorizedResult = result as UnauthorizedObjectResult;
        unauthorizedResult.Should().NotBeNull();
        unauthorizedResult!.StatusCode.Should().Be(401);
    }

    [Fact]
    public async Task EnviarMasiva_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        var dto = new EnviarMasivaDTO { Titulo = "Test", Mensaje = "Test" };
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "5")
        };
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        _mockEnviarMasiva.Setup(x => x.Ejecutar(It.IsAny<long>(), It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception());

        // Act
        var result = await _controller.EnviarMasiva(dto);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ObtenerHistorial Tests

    [Fact]
    public async Task ObtenerHistorial_Exitoso_RetornaOk()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "5")
        };
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        var historial = new List<HistorialNotificacionDTO>
        {
            new HistorialNotificacionDTO { Id = 1, Titulo = "Test", Mensaje = "Test" }
        };
        _mockObtenerHistorial.Setup(x => x.Ejecutar(5)).ReturnsAsync(historial);

        // Act
        var result = await _controller.ObtenerHistorial();

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task ObtenerHistorial_ClaimNulo_RetornaUnauthorized()
    {
        // Arrange
        var claims = new List<Claim>();
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        // Act
        var result = await _controller.ObtenerHistorial();

        // Assert
        var unauthorizedResult = result as UnauthorizedObjectResult;
        unauthorizedResult.Should().NotBeNull();
        unauthorizedResult!.StatusCode.Should().Be(401);
    }

    [Fact]
    public async Task ObtenerHistorial_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "5")
        };
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        _mockObtenerHistorial.Setup(x => x.Ejecutar(5)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.ObtenerHistorial();

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ObtenerUsuarios Tests

    [Fact]
    public async Task ObtenerUsuarios_Exitoso_RetornaOk()
    {
        // Arrange
        var usuarios = new List<UsuarioNotificacionDTO>
        {
            new UsuarioNotificacionDTO { Id = 1, NombreCompleto = "Test", Rol = "Admin" }
        };
        _mockObtenerUsuarios.Setup(x => x.Ejecutar()).ReturnsAsync(usuarios);

        // Act
        var result = await _controller.ObtenerUsuarios();

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task ObtenerUsuarios_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        _mockObtenerUsuarios.Setup(x => x.Ejecutar()).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.ObtenerUsuarios();

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion
}
