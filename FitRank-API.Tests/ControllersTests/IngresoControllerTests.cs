using FitRank_API.Application.CasosDeUso.Ingreso;
using FitRank_API.Application.DTOs.IngresoDTOs;
using FitRank_API.Presentacion.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;
using AutoMapper;
using FitRank_API.Domain.Interfaces;

namespace FitRank_API.tests.ControllersTests;

public class IngresoControllerTests
{
    private readonly IngresoController _controller;
    private readonly Mock<AgregarIngresoCasoDeUso> _mockAgregar;
    private readonly Mock<ObtenerIngresosCasoDeUso> _mockObtenerTodos;
    private readonly Mock<ObtenerIngresoPorIdCasoDeUso> _mockObtenerPorId;
    private readonly Mock<ObtenerIngresosPorGimnasioCasoDeUso> _mockObtenerPorGimnasio;
    private readonly Mock<EliminarIngresoCasoDeUso> _mockEliminar;

    public IngresoControllerTests()
    {
        var mockRepo = new Mock<IIngresoRepositorio>();
        var mockMapper = new Mock<IMapper>();

        _mockAgregar = new Mock<AgregarIngresoCasoDeUso>(mockRepo.Object, mockMapper.Object);
        _mockObtenerTodos = new Mock<ObtenerIngresosCasoDeUso>(mockRepo.Object, mockMapper.Object);
        _mockObtenerPorId = new Mock<ObtenerIngresoPorIdCasoDeUso>(mockRepo.Object, mockMapper.Object);
        _mockObtenerPorGimnasio = new Mock<ObtenerIngresosPorGimnasioCasoDeUso>(mockRepo.Object, mockMapper.Object);
        _mockEliminar = new Mock<EliminarIngresoCasoDeUso>(mockRepo.Object);

        _controller = new IngresoController(
            _mockAgregar.Object,
            _mockObtenerTodos.Object,
            _mockObtenerPorId.Object,
            _mockObtenerPorGimnasio.Object,
            _mockEliminar.Object
        );
    }

    #region ObtenerTodosLosIngresos Tests

    [Fact]
    public async Task ObtenerTodosLosIngresos_Exitoso_RetornaOk()
    {
        // Arrange
        var ingresos = new List<ObtenerIngresoDTO>
        {
            new ObtenerIngresoDTO { Id = 1, GimnasioId = 1, Monto = 1000 },
            new ObtenerIngresoDTO { Id = 2, GimnasioId = 1, Monto = 2000 }
        };

        _mockObtenerTodos.Setup(x => x.Ejecutar()).ReturnsAsync(ingresos);

        // Act
        var result = await _controller.ObtenerTodosLosIngresos();

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        var returnedList = okResult.Value as IEnumerable<ObtenerIngresoDTO>;
        returnedList.Should().HaveCount(2);
    }

    [Fact]
    public async Task ObtenerTodosLosIngresos_ListaVacia_RetornaOk()
    {
        // Arrange
        _mockObtenerTodos.Setup(x => x.Ejecutar()).ReturnsAsync(new List<ObtenerIngresoDTO>());

        // Act
        var result = await _controller.ObtenerTodosLosIngresos();

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        var returnedList = okResult.Value as IEnumerable<ObtenerIngresoDTO>;
        returnedList.Should().BeEmpty();
    }

    [Fact]
    public async Task ObtenerTodosLosIngresos_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        _mockObtenerTodos.Setup(x => x.Ejecutar()).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.ObtenerTodosLosIngresos();

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ObtenerIngresoPorId Tests

    [Fact]
    public async Task ObtenerIngresoPorId_Exitoso_RetornaOk()
    {
        // Arrange
        long id = 1;
        var ingreso = new ObtenerIngresoDTO { Id = id, GimnasioId = 1, Monto = 1500 };
        _mockObtenerPorId.Setup(x => x.Ejecutar(id)).ReturnsAsync(ingreso);

        // Act
        var result = await _controller.ObtenerIngresoPorId(id);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(ingreso);
    }

    [Fact]
    public async Task ObtenerIngresoPorId_IdCero_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerIngresoPorId(0);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerIngresoPorId_IdNegativo_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerIngresoPorId(-5);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerIngresoPorId_NoEncontrado_RetornaNotFound()
    {
        // Arrange
        long id = 999;
        _mockObtenerPorId.Setup(x => x.Ejecutar(id)).ReturnsAsync((ObtenerIngresoDTO?)null);

        // Act
        var result = await _controller.ObtenerIngresoPorId(id);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task ObtenerIngresoPorId_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        _mockObtenerPorId.Setup(x => x.Ejecutar(1)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.ObtenerIngresoPorId(1);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ObtenerIngresoPorGimnasio Tests

    [Fact]
    public async Task ObtenerIngresoPorGimnasio_Exitoso_RetornaOk()
    {
        // Arrange
        var adminId = 1L;
        var ingresos = new List<ObtenerIngresoDTO>
        {
            new ObtenerIngresoDTO { Id = 1, GimnasioId = 1, Monto = 1000 }
        };

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, adminId.ToString())
        };
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        _mockObtenerPorGimnasio.Setup(x => x.Ejecutar(adminId)).ReturnsAsync(ingresos);

        // Act
        var result = await _controller.ObtenerIngresoPorGimnasio();

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task ObtenerIngresoPorGimnasio_ClaimNulo_RetornaBadRequest()
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
        var result = await _controller.ObtenerIngresoPorGimnasio();

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerIngresoPorGimnasio_ClaimInvalido_RetornaBadRequest()
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
        var result = await _controller.ObtenerIngresoPorGimnasio();

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerIngresoPorGimnasio_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        var adminId = 1L;
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, adminId.ToString())
        };
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        _mockObtenerPorGimnasio.Setup(x => x.Ejecutar(adminId)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.ObtenerIngresoPorGimnasio();

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
        var dto = new AgregarIngresoDTO
        {
            GimnasioId = 1,
            Monto = 1500,
            MetodoPago = "Efectivo"
        };

        var ingresoCreado = new ObtenerIngresoDTO
        {
            Id = 1,
            GimnasioId = 1,
            Monto = 1500,
            MetodoPago = "Efectivo"
        };

        _mockAgregar.Setup(x => x.Ejecutar(dto)).ReturnsAsync(ingresoCreado);

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
        _controller.ModelState.AddModelError("Monto", "Requerido");
        var dto = new AgregarIngresoDTO();

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
        var dto = new AgregarIngresoDTO { GimnasioId = 1, Monto = 1000 };
        _mockAgregar.Setup(x => x.Ejecutar(dto)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.Crear(dto);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region Eliminar Tests

    [Fact]
    public async Task Eliminar_Exitoso_RetornaOk()
    {
        // Arrange
        long id = 1;
        _mockEliminar.Setup(x => x.Ejecutar(id)).ReturnsAsync(true);

        // Act
        var result = await _controller.Eliminar(id);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
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
        _mockEliminar.Setup(x => x.Ejecutar(id)).ReturnsAsync(false);

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
        _mockEliminar.Setup(x => x.Ejecutar(1)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.Eliminar(1);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion
}
