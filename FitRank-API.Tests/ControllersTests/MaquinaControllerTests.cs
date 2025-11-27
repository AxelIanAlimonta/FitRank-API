using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using AutoMapper;
using FitRank_API.Domain.Interfaces;
using FitRank_API.Presentacion.Controllers;
using FitRank_API.Application.CasosDeUso.MaquinaCasosDeUso;
using FitRank_API.Application.DTOs.MaquinaDTOs;
using FitRank_API.Application.CasosDeUso.Invitacion;
using System.Security.Claims;

namespace FitRank_API.tests.ControllersTests;

public class MaquinaControllerTests
{
    private readonly MaquinaController _controller;
    private readonly Mock<ObtenerMaquinasCasoDeUso> _mockObtenerMaquinas;
    private readonly Mock<AgregarMaquinaCasoDeUso> _mockAgregarMaquina;
    private readonly Mock<ActualizarMaquinaCasoDeUso> _mockActualizarMaquina;
    private readonly Mock<EliminarMaquinaCasoDeUso> _mockEliminarMaquina;
    private readonly Mock<ObtenerMaquinaPorIdCasoDeUso> _mockObtenerPorId;
    private readonly Mock<ObtenerMaquinaDetalleCasoDeUso> _mockObtenerDetalles;

    public MaquinaControllerTests()
    {
        var mockRepo = new Mock<IMaquinaRepositorio>();
        var mockEjercicioRepo = new Mock<IEjercicioRepositorio>();
        var mockMapper = new Mock<IMapper>();
        var mockQrHelper = new Mock<QrHelper>(Mock.Of<Microsoft.Extensions.Configuration.IConfiguration>());

        _mockObtenerMaquinas = new Mock<ObtenerMaquinasCasoDeUso>(mockRepo.Object, mockMapper.Object);
        _mockAgregarMaquina = new Mock<AgregarMaquinaCasoDeUso>(mockRepo.Object, mockMapper.Object, mockQrHelper.Object);
        _mockActualizarMaquina = new Mock<ActualizarMaquinaCasoDeUso>(mockRepo.Object, mockMapper.Object);
        _mockEliminarMaquina = new Mock<EliminarMaquinaCasoDeUso>(mockRepo.Object);
        _mockObtenerPorId = new Mock<ObtenerMaquinaPorIdCasoDeUso>(mockRepo.Object, mockMapper.Object);
        _mockObtenerDetalles = new Mock<ObtenerMaquinaDetalleCasoDeUso>(mockRepo.Object, mockEjercicioRepo.Object, mockMapper.Object);

        _controller = new MaquinaController(
            _mockObtenerMaquinas.Object,
            _mockAgregarMaquina.Object,
            _mockActualizarMaquina.Object,
            _mockEliminarMaquina.Object,
            _mockObtenerPorId.Object,
            _mockObtenerDetalles.Object
        );
    }

    #region ObtenerTodas Tests

    [Fact]
    public async Task ObtenerTodas_RetornaOkResult_ConListaCompleta()
    {
        // Arrange
        var maquinas = new List<ObtenerMaquinaDTO>
        {
            new ObtenerMaquinaDTO { Id = 1, Nombre = "Maquina 1", GimnasioId = 1 },
            new ObtenerMaquinaDTO { Id = 2, Nombre = "Maquina 2", GimnasioId = 1 }
        };

        _mockObtenerMaquinas.Setup(x => x.Ejecutar()).ReturnsAsync(maquinas);

        // Act
        var result = await _controller.ObtenerTodas();

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        var returnedList = okResult.Value as IEnumerable<ObtenerMaquinaDTO>;
        returnedList.Should().HaveCount(2);
    }

    [Fact]
    public async Task ObtenerTodas_ListaVacia_RetornaOk()
    {
        // Arrange
        _mockObtenerMaquinas.Setup(x => x.Ejecutar()).ReturnsAsync(new List<ObtenerMaquinaDTO>());

        // Act
        var result = await _controller.ObtenerTodas();

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task ObtenerTodas_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        _mockObtenerMaquinas.Setup(x => x.Ejecutar()).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.ObtenerTodas();

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
        long id = 1;
        var maquina = new ObtenerMaquinaDTO { Id = id, Nombre = "Maquina 1", GimnasioId = 1 };
        _mockObtenerPorId.Setup(x => x.Ejecutar(id)).ReturnsAsync(maquina);

        // Act
        var result = await _controller.ObtenerPorId(id);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(maquina);
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
        var result = await _controller.ObtenerPorId(-5);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerPorId_NoEncontrado_RetornaNotFound()
    {
        // Arrange
        long id = 999;
        _mockObtenerPorId.Setup(x => x.Ejecutar(id)).ReturnsAsync((ObtenerMaquinaDTO?)null);

        // Act
        var result = await _controller.ObtenerPorId(id);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task ObtenerPorId_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        _mockObtenerPorId.Setup(x => x.Ejecutar(1)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.ObtenerPorId(1);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region Crear Tests

    [Fact]
    public async Task Crear_Exitoso_RetornaOk()
    {
        // Arrange
        var dto = new AgregarMaquinaDTO { Nombre = "Nueva Maquina", UrlImagen = "http://imagen.com" };
        var maquinaCreada = new ObtenerMaquinaDTO { Id = 1, Nombre = "Nueva Maquina", GimnasioId = 1 };

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.GroupSid, "1")
        };
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        _mockAgregarMaquina.Setup(x => x.Ejecutar(dto, 1)).ReturnsAsync(maquinaCreada);

        // Act
        var result = await _controller.Crear(dto);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
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
        _controller.ModelState.AddModelError("Nombre", "Requerido");
        var dto = new AgregarMaquinaDTO();

        // Act
        var result = await _controller.Crear(dto);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Crear_ClaimNulo_RetornaUnauthorized()
    {
        // Arrange
        var dto = new AgregarMaquinaDTO { Nombre = "Maquina" };
        var claims = new List<Claim>();
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        // Act
        var result = await _controller.Crear(dto);

        // Assert
        var unauthorizedResult = result as UnauthorizedObjectResult;
        unauthorizedResult.Should().NotBeNull();
        unauthorizedResult!.StatusCode.Should().Be(401);
    }

    [Fact]
    public async Task Crear_ClaimInvalido_RetornaBadRequest()
    {
        // Arrange
        var dto = new AgregarMaquinaDTO { Nombre = "Maquina" };
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.GroupSid, "invalid")
        };
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        // Act
        var result = await _controller.Crear(dto);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Crear_ClaimNegativo_RetornaBadRequest()
    {
        // Arrange
        var dto = new AgregarMaquinaDTO { Nombre = "Maquina" };
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.GroupSid, "-5")
        };
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

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
        var dto = new AgregarMaquinaDTO { Nombre = "Maquina" };
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.GroupSid, "1")
        };
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        _mockAgregarMaquina.Setup(x => x.Ejecutar(dto, 1)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.Crear(dto);

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
        long id = 1;
        var dto = new ActualizarMaquinaDTO { Id = id, Nombre = "Maquina Actualizada" };
        var maquinaActualizada = new ObtenerMaquinaDTO { Id = id, Nombre = "Maquina Actualizada" };

        _mockActualizarMaquina.Setup(x => x.Ejecutar(dto)).ReturnsAsync(maquinaActualizada);

        // Act
        var result = await _controller.Actualizar(id, dto);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task Actualizar_IdCero_RetornaBadRequest()
    {
        // Arrange
        var dto = new ActualizarMaquinaDTO { Id = 0 };

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
        var dto = new ActualizarMaquinaDTO { Id = -5 };

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
    public async Task Actualizar_IdNoCoincide_RetornaBadRequest()
    {
        // Arrange
        var dto = new ActualizarMaquinaDTO { Id = 1 };

        // Act
        var result = await _controller.Actualizar(2, dto);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Actualizar_ModelStateInvalido_RetornaBadRequest()
    {
        // Arrange
        _controller.ModelState.AddModelError("Nombre", "Requerido");
        var dto = new ActualizarMaquinaDTO { Id = 1 };

        // Act
        var result = await _controller.Actualizar(1, dto);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Actualizar_NoEncontrado_RetornaNotFound()
    {
        // Arrange
        long id = 999;
        var dto = new ActualizarMaquinaDTO { Id = id };

        _mockActualizarMaquina.Setup(x => x.Ejecutar(dto)).ReturnsAsync((ObtenerMaquinaDTO?)null);

        // Act
        var result = await _controller.Actualizar(id, dto);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Actualizar_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        long id = 1;
        var dto = new ActualizarMaquinaDTO { Id = id };

        _mockActualizarMaquina.Setup(x => x.Ejecutar(dto)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.Actualizar(id, dto);

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
        long id = 1;
        _mockEliminarMaquina.Setup(x => x.Ejecutar(id)).ReturnsAsync(true);

        // Act
        var result = await _controller.Eliminar(id);

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
        var result = await _controller.Eliminar(-3);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Eliminar_NoEncontrado_RetornaNotFound()
    {
        // Arrange
        long id = 999;
        _mockEliminarMaquina.Setup(x => x.Ejecutar(id)).ReturnsAsync(false);

        // Act
        var result = await _controller.Eliminar(id);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Eliminar_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        _mockEliminarMaquina.Setup(x => x.Ejecutar(1)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.Eliminar(1);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ObtenerDetalles Tests

    [Fact]
    public async Task ObtenerDetalles_Exitoso_RetornaOk()
    {
        // Arrange
        long id = 1;
        var detalle = new MaquinaDetalleDTO { Id = id, Nombre = "Maquina 1" };
        _mockObtenerDetalles.Setup(x => x.Ejecutar(id)).ReturnsAsync(detalle);

        // Act
        var result = await _controller.ObtenerDetalles(id);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task ObtenerDetalles_IdCero_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerDetalles(0);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerDetalles_IdNegativo_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerDetalles(-7);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerDetalles_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        _mockObtenerDetalles.Setup(x => x.Ejecutar(1)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.ObtenerDetalles(1);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion
}
