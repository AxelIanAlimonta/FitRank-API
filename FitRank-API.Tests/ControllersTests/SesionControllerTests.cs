using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using FitRank_API.Presentacion.Controllers;
using AutoMapper;
using FitRank_API.Domain.Interfaces;
using FitRank_API.Application.CasosDeUso.SesionCasosDeUso;
using FitRank_API.Application.DTOs.SesionDTOs;

namespace FitRank_API.tests.ControllersTests;

public class SesionControllerTests
{
    private readonly SesionController _controller;
    private readonly Mock<ActualizarSesionCasoDeUso> _mockActualizar;
    private readonly Mock<AgregarSesionCasoDeUso> _mockAgregar;
    private readonly Mock<EliminarSesionCasoDeUso> _mockEliminar;
    private readonly Mock<ObtenerSesionPorIdCasoDeUso> _mockObtenerPorId;
    private readonly Mock<ObtenerTodasLasSesionesCasoDeUso> _mockObtenerTodos;

    public SesionControllerTests()
    {
        var mockRepositorio = new Mock<ISesionRepositorio>();
        var mockMapper = new Mock<IMapper>();

        _mockActualizar = new Mock<ActualizarSesionCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockAgregar = new Mock<AgregarSesionCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockEliminar = new Mock<EliminarSesionCasoDeUso>(mockRepositorio.Object);
        _mockObtenerPorId = new Mock<ObtenerSesionPorIdCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockObtenerTodos = new Mock<ObtenerTodasLasSesionesCasoDeUso>(mockRepositorio.Object, mockMapper.Object);

        _controller = new SesionController(
            _mockAgregar.Object,
            _mockActualizar.Object,
            _mockEliminar.Object,
            _mockObtenerPorId.Object,
            _mockObtenerTodos.Object
        );
    }

    #region Agregar Tests

    [Fact]
    public async Task Agregar_DtoNulo_RetornaBadRequest()
    {
        // Arrange
        var resultado = await _controller.Agregar(null!);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Agregar_Exitoso_RetornaCreatedAtAction()
    {
        // Arrange
        var nuevaSesionDTO = new AgregarSesionDTO { NumeroDeSesion = 1, Nombre = "Sesion de prueba" };
        var sesionCreadaDTO = new ObtenerSesionDTO { Id = 1, NumeroDeSesion = 1, Nombre = "Sesion de prueba" };

        _mockAgregar.Setup(caso => caso.Ejecutar(nuevaSesionDTO)).ReturnsAsync(sesionCreadaDTO);

        // Act
        var resultado = await _controller.Agregar(nuevaSesionDTO);

        // Assert
        var createdAtActionResult = resultado as CreatedAtActionResult;
        createdAtActionResult.Should().NotBeNull();
        createdAtActionResult!.StatusCode.Should().Be(201);
        createdAtActionResult.Value.Should().BeEquivalentTo(sesionCreadaDTO);
    }

    [Fact]
    public async Task Agregar_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        var nuevaSesionDTO = new AgregarSesionDTO { NumeroDeSesion = 1 };
        _mockAgregar.Setup(caso => caso.Ejecutar(nuevaSesionDTO)).ThrowsAsync(new Exception());

        // Act
        var resultado = await _controller.Agregar(nuevaSesionDTO);

        // Assert
        var objectResult = resultado as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ObtenerTodas Tests

    [Fact]
    public async Task ObtenerTodas_Exitoso_RetornaOk()
    {
        // Arrange
        var sesiones = new List<ObtenerSesionDTO>
        {
            new ObtenerSesionDTO { Id = 1, NumeroDeSesion = 1, Nombre = "Sesion 1" },
            new ObtenerSesionDTO { Id = 2, NumeroDeSesion = 2, Nombre = "Sesion 2" }
        };

        _mockObtenerTodos.Setup(caso => caso.Ejecutar()).ReturnsAsync(sesiones);

        // Act
        var resultado = await _controller.ObtenerTodas();

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(sesiones);
    }

    [Fact]
    public async Task ObtenerTodas_ListaVacia_RetornaOk()
    {
        // Arrange
        var sesiones = new List<ObtenerSesionDTO>();
        _mockObtenerTodos.Setup(caso => caso.Ejecutar()).ReturnsAsync(sesiones);

        // Act
        var resultado = await _controller.ObtenerTodas();

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(sesiones);
    }

    [Fact]
    public async Task ObtenerTodas_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        _mockObtenerTodos.Setup(caso => caso.Ejecutar()).ThrowsAsync(new Exception());

        // Act
        var resultado = await _controller.ObtenerTodas();

        // Assert
        var objectResult = resultado as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ObtenerPorId Tests

    [Fact]
    public async Task ObtenerPorId_IdCero_RetornaBadRequest()
    {
        // Arrange
        var resultado = await _controller.ObtenerPorId(0);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerPorId_IdNegativo_RetornaBadRequest()
    {
        // Arrange
        var resultado = await _controller.ObtenerPorId(-5);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerPorId_Exitoso_RetornaOk()
    {
        // Arrange
        var idExistente = 1;
        var sesionDTO = new ObtenerSesionDTO { Id = idExistente, NumeroDeSesion = 1, Nombre = "Sesion de prueba" };

        _mockObtenerPorId.Setup(caso => caso.Ejecutar(idExistente)).ReturnsAsync(sesionDTO);

        // Act
        var resultado = await _controller.ObtenerPorId(idExistente);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(sesionDTO);
    }

    [Fact]
    public async Task ObtenerPorId_NoExiste_RetornaNotFound()
    {
        // Arrange
        var idInexistente = 999;
        _mockObtenerPorId.Setup(caso => caso.Ejecutar(idInexistente)).ReturnsAsync((ObtenerSesionDTO?)null);

        // Act
        var resultado = await _controller.ObtenerPorId(idInexistente);

        // Assert
        var notFoundResult = resultado as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task ObtenerPorId_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        _mockObtenerPorId.Setup(caso => caso.Ejecutar(1)).ThrowsAsync(new Exception());

        // Act
        var resultado = await _controller.ObtenerPorId(1);

        // Assert
        var objectResult = resultado as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region Actualizar Tests

    [Fact]
    public async Task Actualizar_IdCero_RetornaBadRequest()
    {
        // Arrange
        var dto = new ActualizarSesionDTO { Id = 0 };

        // Act
        var resultado = await _controller.Actualizar(0, dto);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Actualizar_IdNegativo_RetornaBadRequest()
    {
        // Arrange
        var dto = new ActualizarSesionDTO { Id = -3 };

        // Act
        var resultado = await _controller.Actualizar(-3, dto);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Actualizar_DtoNulo_RetornaBadRequest()
    {
        // Arrange
        var resultado = await _controller.Actualizar(1, null!);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Actualizar_IdNoCoincide_RetornaBadRequest()
    {
        // Arrange
        var actualizarSesionDTO = new ActualizarSesionDTO { Id = 1 };

        // Act
        var resultado = await _controller.Actualizar(2, actualizarSesionDTO);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Actualizar_Exitoso_RetornaOk()
    {
        // Arrange
        var idExistente = 1;
        var actualizarSesionDTO = new ActualizarSesionDTO { Id = idExistente, NumeroDeSesion = 2, Nombre = "Sesion actualizada" };
        var sesionActualizadaDTO = new ObtenerSesionDTO { Id = idExistente, NumeroDeSesion = 2, Nombre = "Sesion actualizada" };

        _mockActualizar.Setup(caso => caso.Ejecutar(actualizarSesionDTO)).ReturnsAsync(sesionActualizadaDTO);

        // Act
        var resultado = await _controller.Actualizar(idExistente, actualizarSesionDTO);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(sesionActualizadaDTO);
    }

    [Fact]
    public async Task Actualizar_NoEncontrado_RetornaNotFound()
    {
        // Arrange
        var idInexistente = 999;
        var actualizarSesionDTO = new ActualizarSesionDTO { Id = idInexistente };

        _mockActualizar.Setup(caso => caso.Ejecutar(actualizarSesionDTO)).ReturnsAsync((ObtenerSesionDTO?)null);

        // Act
        var resultado = await _controller.Actualizar(idInexistente, actualizarSesionDTO);

        // Assert
        var notFoundResult = resultado as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Actualizar_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        var idExistente = 1;
        var actualizarSesionDTO = new ActualizarSesionDTO { Id = idExistente };

        _mockActualizar.Setup(caso => caso.Ejecutar(actualizarSesionDTO)).ThrowsAsync(new Exception());

        // Act
        var resultado = await _controller.Actualizar(idExistente, actualizarSesionDTO);

        // Assert
        var objectResult = resultado as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region Eliminar Tests

    [Fact]
    public async Task Eliminar_IdCero_RetornaBadRequest()
    {
        // Arrange
        var resultado = await _controller.Eliminar(0);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Eliminar_IdNegativo_RetornaBadRequest()
    {
        // Arrange
        var resultado = await _controller.Eliminar(-7);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Eliminar_Exitoso_RetornaNoContent()
    {
        // Arrange
        var idExistente = 1;
        _mockEliminar.Setup(caso => caso.Ejecutar(idExistente)).ReturnsAsync(true);

        // Act
        var resultado = await _controller.Eliminar(idExistente);

        // Assert
        var noContentResult = resultado as NoContentResult;
        noContentResult.Should().NotBeNull();
        noContentResult!.StatusCode.Should().Be(204);
    }

    [Fact]
    public async Task Eliminar_NoExistente_RetornaNotFound()
    {
        // Arrange
        var idInexistente = 999;
        _mockEliminar.Setup(caso => caso.Ejecutar(idInexistente)).ReturnsAsync(false);

        // Act
        var resultado = await _controller.Eliminar(idInexistente);

        // Assert
        var notFoundResult = resultado as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Eliminar_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        var idExistente = 1;
        _mockEliminar.Setup(caso => caso.Ejecutar(idExistente)).ThrowsAsync(new Exception());

        // Act
        var resultado = await _controller.Eliminar(idExistente);

        // Assert
        var objectResult = resultado as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(500);
    }

    #endregion
}
