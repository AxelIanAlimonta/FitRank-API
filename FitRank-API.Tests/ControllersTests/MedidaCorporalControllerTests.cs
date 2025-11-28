using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using FitRank_API.Presentacion.Controllers;
using AutoMapper;
using FitRank_API.Domain.Interfaces;
using FitRank_API.Application.CasosDeUso.MedidaCorporalCasosDeUso;
using FitRank_API.Application.DTOs.MedidaCorporalDTOs;
using System.Security.Claims;

namespace FitRank_API.tests.ControllersTests;

public class MedidaCorporalControllerTests
{
    private readonly MedidaCorporalController _controller;
    private readonly Mock<ActualizarMedidaCorporalCasoDeUso> _mockActualizar;
    private readonly Mock<AgregarMedidaCorporalCasoDeUso> _mockAgregar;
    private readonly Mock<EliminarMedidaCorporalCasoDeUso> _mockEliminar;
    private readonly Mock<ObtenerMedidaCorporalPorIdCasoDeUso> _mockObtenerPorId;
    private readonly Mock<ObtenerMedidasPorSocioCasoDeUso> _mockObtenerPorSocio;

    public MedidaCorporalControllerTests()
    {
        var mockRepositorio = new Mock<IMedidaCorporalRepositorio>();
        var mockMapper = new Mock<IMapper>();

        _mockActualizar = new Mock<ActualizarMedidaCorporalCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockAgregar = new Mock<AgregarMedidaCorporalCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockEliminar = new Mock<EliminarMedidaCorporalCasoDeUso>(mockRepositorio.Object);
        _mockObtenerPorId = new Mock<ObtenerMedidaCorporalPorIdCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockObtenerPorSocio = new Mock<ObtenerMedidasPorSocioCasoDeUso>(mockRepositorio.Object, mockMapper.Object);

        _controller = new MedidaCorporalController(
            _mockAgregar.Object,
            _mockActualizar.Object,
            _mockObtenerPorId.Object,
            _mockObtenerPorSocio.Object,
            _mockEliminar.Object
        );
    }

    #region Agregar Tests

    [Fact]
    public async Task Agregar_Exitoso_RetornaOk()
    {
        // Arrange
        var nuevaMedidaDto = new AgregarMedidaCorporalDTO
        {
            SocioId = 1,
            BrazoIzquierdoCm = 30.0,
            BrazoDerechoCm = 30.5
        };

        var medidaCreadaDto = new ObtenerMedidaCorporalDTO
        {
            Id = 1,
            SocioId = 1,
            BrazoIzquierdoCm = 30.0,
            BrazoDerechoCm = 30.5
        };

        _mockAgregar.Setup(m => m.Ejecutar(nuevaMedidaDto)).ReturnsAsync(medidaCreadaDto);

        // Act
        var result = await _controller.Agregar(nuevaMedidaDto);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(medidaCreadaDto);
    }

    [Fact]
    public async Task Agregar_DtoNulo_RetornaBadRequest()
    {
        // Act
        var result = await _controller.Agregar(null!);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Agregar_ModelStateInvalido_RetornaBadRequest()
    {
        // Arrange
        _controller.ModelState.AddModelError("SocioId", "Requerido");
        var dto = new AgregarMedidaCorporalDTO();

        // Act
        var result = await _controller.Agregar(dto);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Agregar_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        var dto = new AgregarMedidaCorporalDTO { SocioId = 1 };
        _mockAgregar.Setup(m => m.Ejecutar(dto)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.Agregar(dto);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region Actualizar Tests

    [Fact]
    public async Task Actualizar_Exitoso_RetornaOk()
    {
        // Arrange
        var medidaActualizarDto = new ActualizarMedidaCorporalDTO
        {
            Id = 1,
            SocioId = 1,
            BrazoIzquierdoCm = 32.0,
            BrazoDerechoCm = 31.5
        };

        var medidaActualizadaDto = new ObtenerMedidaCorporalDTO
        {
            Id = 1,
            SocioId = 1,
            BrazoIzquierdoCm = 32.0,
            BrazoDerechoCm = 31.5
        };

        _mockActualizar.Setup(m => m.Ejecutar(medidaActualizarDto)).ReturnsAsync(medidaActualizadaDto);

        // Act
        var result = await _controller.Actualizar(medidaActualizarDto.Id, medidaActualizarDto);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(medidaActualizadaDto);
    }

    [Fact]
    public async Task Actualizar_IdCero_RetornaBadRequest()
    {
        // Arrange
        var dto = new ActualizarMedidaCorporalDTO { Id = 0 };

        // Act
        var result = await _controller.Actualizar(0, dto);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Actualizar_IdNegativo_RetornaBadRequest()
    {
        // Arrange
        var dto = new ActualizarMedidaCorporalDTO { Id = -5 };

        // Act
        var result = await _controller.Actualizar(-5, dto);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Actualizar_DtoNulo_RetornaBadRequest()
    {
        // Act
        var result = await _controller.Actualizar(1, null!);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Actualizar_ModelStateInvalido_RetornaBadRequest()
    {
        // Arrange
        _controller.ModelState.AddModelError("SocioId", "Requerido");
        var dto = new ActualizarMedidaCorporalDTO { Id = 1 };

        // Act
        var result = await _controller.Actualizar(1, dto);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Actualizar_IdNoCoincide_RetornaBadRequest()
    {
        // Arrange
        var dto = new ActualizarMedidaCorporalDTO { Id = 1 };

        // Act
        var result = await _controller.Actualizar(2, dto);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Actualizar_NoEncontrado_RetornaNotFound()
    {
        // Arrange
        var dto = new ActualizarMedidaCorporalDTO { Id = 999 };
        _mockActualizar.Setup(m => m.Ejecutar(dto)).ReturnsAsync((ObtenerMedidaCorporalDTO?)null);

        // Act
        var result = await _controller.Actualizar(dto.Id, dto);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Actualizar_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        var dto = new ActualizarMedidaCorporalDTO { Id = 1 };
        _mockActualizar.Setup(m => m.Ejecutar(dto)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.Actualizar(dto.Id, dto);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ObtenerPorId Tests

    [Fact]
    public async Task ObtenerPorId_Exitoso_RetornaOk()
    {
        // Arrange
        long medidaId = 1;
        var medidaDto = new ObtenerMedidaCorporalDTO
        {
            Id = medidaId,
            SocioId = 1,
            BrazoIzquierdoCm = 30.0,
            BrazoDerechoCm = 30.5
        };

        _mockObtenerPorId.Setup(m => m.Ejecutar(medidaId)).ReturnsAsync(medidaDto);

        // Act
        var result = await _controller.ObtenerPorId(medidaId);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(medidaDto);
    }

    [Fact]
    public async Task ObtenerPorId_IdCero_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerPorId(0);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerPorId_IdNegativo_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerPorId(-3);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerPorId_NoEncontrado_RetornaNotFound()
    {
        // Arrange
        long medidaId = 999;
        _mockObtenerPorId.Setup(m => m.Ejecutar(medidaId)).ReturnsAsync((ObtenerMedidaCorporalDTO?)null);

        // Act
        var result = await _controller.ObtenerPorId(medidaId);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task ObtenerPorId_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        _mockObtenerPorId.Setup(m => m.Ejecutar(1)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.ObtenerPorId(1);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ObtenerPorSocio Tests

    [Fact]
    public async Task ObtenerPorSocio_RolSocio_UsaUsuarioId()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Role, "Socio"),
            new Claim(ClaimTypes.NameIdentifier, "5")
        };
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        var medidas = new List<ObtenerMedidaCorporalDTO>
        {
            new ObtenerMedidaCorporalDTO { Id = 1, SocioId = 5 }
        };

        _mockObtenerPorSocio.Setup(m => m.Ejecutar(5)).ReturnsAsync(medidas);

        // Act
        var result = await _controller.ObtenerPorSocio();

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task ObtenerPorSocio_RolAdminConSocioId_RetornaOk()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim(ClaimTypes.NameIdentifier, "1")
        };
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        var medidas = new List<ObtenerMedidaCorporalDTO>
        {
            new ObtenerMedidaCorporalDTO { Id = 1, SocioId = 10 }
        };

        _mockObtenerPorSocio.Setup(m => m.Ejecutar(10)).ReturnsAsync(medidas);

        // Act
        var result = await _controller.ObtenerPorSocio(10);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task ObtenerPorSocio_SocioIdNulo_RetornaBadRequest()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim(ClaimTypes.NameIdentifier, "1")
        };
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        // Act
        var result = await _controller.ObtenerPorSocio();

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerPorSocio_SocioIdCero_RetornaBadRequest()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim(ClaimTypes.NameIdentifier, "1")
        };
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        // Act
        var result = await _controller.ObtenerPorSocio(0);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerPorSocio_SocioIdNegativo_RetornaBadRequest()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim(ClaimTypes.NameIdentifier, "1")
        };
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        // Act
        var result = await _controller.ObtenerPorSocio(-5);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerPorSocio_ClaimNameIdentifierNulo_RetornaUnauthorized()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Role, "Socio")
        };
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        // Act
        var result = await _controller.ObtenerPorSocio();

        // Assert
        var unauthorizedResult = result as UnauthorizedObjectResult;
        unauthorizedResult.Should().NotBeNull();
        unauthorizedResult!.StatusCode.Should().Be(401);
    }

    [Fact]
    public async Task ObtenerPorSocio_ClaimNameIdentifierInvalido_RetornaBadRequest()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Role, "Socio"),
            new Claim(ClaimTypes.NameIdentifier, "invalid")
        };
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        // Act
        var result = await _controller.ObtenerPorSocio();

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerPorSocio_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Role, "Admin"),
            new Claim(ClaimTypes.NameIdentifier, "1")
        };
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        _mockObtenerPorSocio.Setup(m => m.Ejecutar(1)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.ObtenerPorSocio(1);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region Eliminar Tests

    [Fact]
    public async Task Eliminar_Exitoso_RetornaNoContent()
    {
        // Arrange
        long medidaId = 1;
        _mockEliminar.Setup(m => m.Ejecutar(medidaId)).ReturnsAsync(true);

        // Act
        var result = await _controller.Eliminar(medidaId);

        // Assert
        var noContentResult = result as NoContentResult;
        noContentResult.Should().NotBeNull();
        noContentResult!.StatusCode.Should().Be(204);
    }

    [Fact]
    public async Task Eliminar_IdCero_RetornaBadRequest()
    {
        // Act
        var result = await _controller.Eliminar(0);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Eliminar_IdNegativo_RetornaBadRequest()
    {
        // Act
        var result = await _controller.Eliminar(-2);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Eliminar_NoEncontrado_RetornaNotFound()
    {
        // Arrange
        long medidaId = 999;
        _mockEliminar.Setup(m => m.Ejecutar(medidaId)).ReturnsAsync(false);

        // Act
        var result = await _controller.Eliminar(medidaId);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Eliminar_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        _mockEliminar.Setup(m => m.Ejecutar(1)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.Eliminar(1);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion
}