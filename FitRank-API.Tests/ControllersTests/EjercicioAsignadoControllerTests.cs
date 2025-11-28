using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using FitRank_API.Presentacion.Controllers;
using AutoMapper;
using FitRank_API.Domain.Interfaces;
using FitRank_API.Application.CasosDeUso.EjercicioAsignadoCasoDeUso;
using FitRank_API.Application.DTOs.EjercicioAsignadoDTOs;

namespace FitRank_API.tests.ControllersTests;

public class EjercicioAsignadoControllerTests
{
    private readonly EjercicioAsignadoController _controller;
    private readonly Mock<ActualizarEjercicioAsignadoCasoDeUso> _mockActualizar;
    private readonly Mock<AgregarEjercicioAsignadoCasoDeUso> _mockAgregar;
    private readonly Mock<EliminarEjercicioAsignadoCasoDeUso> _mockEliminar;
    private readonly Mock<ObtenerEjercicioAsignadoPorIdCasoDeUso> _mockObtenerPorId;
    private readonly Mock<ObtenerEjerciciosAsignadosCasoDeUso> _mockObtenerTodos;

    public EjercicioAsignadoControllerTests()
    {
        var mockRepositorio = new Mock<IEjercicioAsignadoRepositorio>();
        var mockMapper = new Mock<IMapper>();

        _mockActualizar = new Mock<ActualizarEjercicioAsignadoCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockAgregar = new Mock<AgregarEjercicioAsignadoCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockEliminar = new Mock<EliminarEjercicioAsignadoCasoDeUso>(mockRepositorio.Object);
        _mockObtenerPorId = new Mock<ObtenerEjercicioAsignadoPorIdCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockObtenerTodos = new Mock<ObtenerEjerciciosAsignadosCasoDeUso>(mockRepositorio.Object, mockMapper.Object);

        _controller = new EjercicioAsignadoController(
            _mockActualizar.Object,
            _mockObtenerPorId.Object,
            _mockAgregar.Object,
            _mockEliminar.Object,
            _mockObtenerTodos.Object
        );
    }

    #region ObtenerTodos Tests

    [Fact]
    public async Task ObtenerTodos_RetornaOkResult_ConListaCompleta()
    {
        // Arrange
        var listaEjerciciosDTO = new List<ObtenerEjercicioAsignadoDTO>
        {
            new ObtenerEjercicioAsignadoDTO { Id = 1, NumeroEjercicio = 1, EjercicioId = 2, SesionId = 3 },
            new ObtenerEjercicioAsignadoDTO { Id = 2, NumeroEjercicio = 2, EjercicioId = 3, SesionId = 4 }
        };

        _mockObtenerTodos
            .Setup(casoDeUso => casoDeUso.Ejecutar())
            .ReturnsAsync(listaEjerciciosDTO);

        // Act
        var resultado = await _controller.ObtenerTodo();

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(listaEjerciciosDTO);
    }

    [Fact]
    public async Task ObtenerTodos_RetornaOkResult_ConListaVacia()
    {
        // Arrange
        var listaEjerciciosDTO = new List<ObtenerEjercicioAsignadoDTO>();

        _mockObtenerTodos
            .Setup(casoDeUso => casoDeUso.Ejecutar())
            .ReturnsAsync(listaEjerciciosDTO);

        // Act
        var resultado = await _controller.ObtenerTodo();

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(listaEjerciciosDTO);
    }

    [Fact]
    public async Task ObtenerTodos_LanzaExcepcion_RetornaStatus500()
    {
        // Arrange
        _mockObtenerTodos
            .Setup(casoDeUso => casoDeUso.Ejecutar())
            .ThrowsAsync(new Exception("Error al obtener los ejercicios asignados"));

        // Act
        var resultado = await _controller.ObtenerTodo();

        // Assert
        var objectResult = resultado as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ObtenerPorId Tests

    [Fact]
    public async Task ObtenerPorId_Existe_RetornaObjetoOkCreado()
    {
        // Arrange
        long ejercicioId = 1;
        var ejercicioDTO = new ObtenerEjercicioAsignadoDTO
        {
            Id = ejercicioId,
            NumeroEjercicio = 1,
            EjercicioId = 2,
            SesionId = 3
        };

        _mockObtenerPorId
            .Setup(casoDeUso => casoDeUso.Ejecutar(ejercicioId))
            .ReturnsAsync(ejercicioDTO);

        // Act
        var resultado = await _controller.ObtenerPorId(ejercicioId);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(ejercicioDTO);
    }

    [Fact]
    public async Task ObtenerPorId_IdCero_RetornaBadRequest()
    {
        // Act
        var resultado = await _controller.ObtenerPorId(0);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerPorId_IdNegativo_RetornaBadRequest()
    {
        // Act
        var resultado = await _controller.ObtenerPorId(-5);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerPorId_NoExiste_RetornaNotFound()
    {
        // Arrange
        long ejercicioId = 999;

        _mockObtenerPorId
            .Setup(casoDeUso => casoDeUso.Ejecutar(ejercicioId))
            .ReturnsAsync((ObtenerEjercicioAsignadoDTO?)null);

        // Act
        var resultado = await _controller.ObtenerPorId(ejercicioId);

        // Assert
        var notFoundResult = resultado as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task ObtenerPorId_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        _mockObtenerPorId.Setup(x => x.Ejecutar(1)).ThrowsAsync(new Exception());

        // Act
        var resultado = await _controller.ObtenerPorId(1);

        // Assert
        var statusCodeResult = resultado as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region Agregar Tests

    [Fact]
    public async Task Agregar_RetornaCreatedAtActionResult()
    {
        // Arrange
        var nuevoEjercicioDTO = new AgregarEjercicioAsignadoDTO
        {
            NumeroEjercicio = 1,
            EjercicioId = 2,
            SesionId = 3
        };

        var ejercicioCreadoDTO = new ObtenerEjercicioAsignadoDTO
        {
            Id = 1,
            NumeroEjercicio = 1,
            EjercicioId = 2,
            SesionId = 3
        };

        _mockAgregar
            .Setup(casoDeUso => casoDeUso.Ejecutar(nuevoEjercicioDTO))
            .ReturnsAsync(ejercicioCreadoDTO);

        // Act
        var resultado = await _controller.Agregar(nuevoEjercicioDTO);

        // Assert
        var createdAtActionResult = resultado as CreatedAtActionResult;
        createdAtActionResult.Should().NotBeNull();
        createdAtActionResult!.StatusCode.Should().Be(201);
        createdAtActionResult.Value.Should().BeEquivalentTo(ejercicioCreadoDTO);
    }

    [Fact]
    public async Task Agregar_RetornaBadRequest_CuandoDTOEsNulo()
    {
        // Act
        var resultado = await _controller.Agregar(null);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Agregar_ModelStateInvalido_RetornaBadRequest()
    {
        // Arrange
        _controller.ModelState.AddModelError("NumeroEjercicio", "Requerido");
        var dto = new AgregarEjercicioAsignadoDTO();

        // Act
        var resultado = await _controller.Agregar(dto);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Agregar_LanzaExcepcion_RetornaStatusCode500()
    {
        // Arrange
        var nuevoEjercicioDTO = new AgregarEjercicioAsignadoDTO
        {
            NumeroEjercicio = 1,
            EjercicioId = 2,
            SesionId = 3
        };

        _mockAgregar
            .Setup(casoDeUso => casoDeUso.Ejecutar(nuevoEjercicioDTO))
            .ThrowsAsync(new Exception("Error interno del servidor"));

        // Act
        var resultado = await _controller.Agregar(nuevoEjercicioDTO);

        // Assert
        var objectResult = resultado as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region Actualizar Tests

    [Fact]
    public async Task Actualizar_RetornaOkObjectResult_ConObjetoActualizado()
    {
        // Arrange
        long ejercicioId = 1;
        var actualizarEjercicioDTO = new ActualizarEjercicioAsignadoDTO
        {
            Id = ejercicioId,
            NumeroEjercicio = 2,
            EjercicioId = 3,
            SesionId = 4
        };

        var ejercicioActualizadoDTO = new ObtenerEjercicioAsignadoDTO
        {
            Id = ejercicioId,
            NumeroEjercicio = 2,
            EjercicioId = 3,
            SesionId = 4
        };

        _mockActualizar
            .Setup(casoDeUso => casoDeUso.Ejecutar(actualizarEjercicioDTO))
            .ReturnsAsync(ejercicioActualizadoDTO);

        // Act
        var resultado = await _controller.Actualizar(ejercicioId, actualizarEjercicioDTO);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(ejercicioActualizadoDTO);
    }

    [Fact]
    public async Task Actualizar_IdCero_RetornaBadRequest()
    {
        // Arrange
        var dto = new ActualizarEjercicioAsignadoDTO { Id = 0 };

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
        var dto = new ActualizarEjercicioAsignadoDTO { Id = -5 };

        // Act
        var resultado = await _controller.Actualizar(-5, dto);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Actualizar_DTONulo_RetornaBadRequestResult()
    {
        // Arrange
        long ejercicioId = 1;

        // Act
        var resultado = await _controller.Actualizar(ejercicioId, null);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Actualizar_ModelStateInvalido_RetornaBadRequest()
    {
        // Arrange
        _controller.ModelState.AddModelError("NumeroEjercicio", "Requerido");
        var dto = new ActualizarEjercicioAsignadoDTO { Id = 1 };

        // Act
        var resultado = await _controller.Actualizar(1, dto);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Actualizar_IdNoCoincide_RetornaBadRequestResult()
    {
        // Arrange
        long ejercicioId = 1;
        var actualizarEjercicioDTO = new ActualizarEjercicioAsignadoDTO
        {
            Id = ejercicioId,
            NumeroEjercicio = 2,
            EjercicioId = 3,
            SesionId = 4
        };

        // Act
        var resultado = await _controller.Actualizar(ejercicioId + 1, actualizarEjercicioDTO);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Actualizar_NoEncontrado_RetornaNotFoundResult()
    {
        // Arrange
        long ejercicioId = 999;
        var actualizarEjercicioDTO = new ActualizarEjercicioAsignadoDTO
        {
            Id = ejercicioId,
            NumeroEjercicio = 2,
            EjercicioId = 3,
            SesionId = 4
        };

        _mockActualizar
            .Setup(casoDeUso => casoDeUso.Ejecutar(actualizarEjercicioDTO))
            .ReturnsAsync((ObtenerEjercicioAsignadoDTO?)null);

        // Act
        var resultado = await _controller.Actualizar(ejercicioId, actualizarEjercicioDTO);

        // Assert
        var notFoundResult = resultado as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Actualizar_LanzaExcepcion_RetornaStatusCode500()
    {
        // Arrange
        long ejercicioId = 1;
        var actualizarEjercicioDTO = new ActualizarEjercicioAsignadoDTO
        {
            Id = ejercicioId,
            NumeroEjercicio = 2,
            EjercicioId = 3,
            SesionId = 4
        };

        _mockActualizar
            .Setup(casoDeUso => casoDeUso.Ejecutar(actualizarEjercicioDTO))
            .ThrowsAsync(new Exception("Error al actualizar el ejercicio asignado"));

        // Act
        var resultado = await _controller.Actualizar(ejercicioId, actualizarEjercicioDTO);

        // Assert
        var objectResult = resultado as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region Eliminar Tests

    [Fact]
    public async Task Eliminar_Existente_DeberiaRetornarNoContent()
    {
        // Arrange
        long ejercicioId = 1;

        _mockEliminar
            .Setup(casoDeUso => casoDeUso.Ejecutar(ejercicioId))
            .ReturnsAsync(true);

        // Act
        var resultado = await _controller.Eliminar(ejercicioId);

        // Assert
        var noContentResult = resultado as NoContentResult;
        noContentResult.Should().NotBeNull();
        noContentResult!.StatusCode.Should().Be(204);
    }

    [Fact]
    public async Task Eliminar_IdCero_RetornaBadRequest()
    {
        // Act
        var resultado = await _controller.Eliminar(0);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Eliminar_IdNegativo_RetornaBadRequest()
    {
        // Act
        var resultado = await _controller.Eliminar(-3);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Eliminar_NoExistente_DeberiaRetornarNotFound()
    {
        // Arrange
        long ejercicioId = 999;

        _mockEliminar
            .Setup(casoDeUso => casoDeUso.Ejecutar(ejercicioId))
            .ReturnsAsync(false);

        // Act
        var resultado = await _controller.Eliminar(ejercicioId);

        // Assert
        var notFoundResult = resultado as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Eliminar_CuandoOcurreErrorEnServidor_DeberiaRetornarStatusCode500()
    {
        // Arrange
        long ejercicioId = 1;

        _mockEliminar
            .Setup(casoDeUso => casoDeUso.Ejecutar(ejercicioId))
            .ThrowsAsync(new Exception("Error al eliminar el ejercicio asignado"));

        // Act
        var resultado = await _controller.Eliminar(ejercicioId);

        // Assert
        var objectResult = resultado as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(500);
    }

    #endregion
}