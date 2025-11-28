using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using FitRank_API.Presentacion.Controllers;
using FitRank_API.Application.DTOs.DificultadDTOs;
using AutoMapper;
using FitRank_API.Domain.Interfaces;
using FitRank_API.Application.CasosDeUso.DificultadCasosDeUso;

namespace FitRank_API.tests.ControllersTests;

public class DificultadControllerTests
{
    private readonly DificultadController _controller;
    private readonly Mock<ActualizarDificultadCasoDeUso> _mockActualizar;
    private readonly Mock<AgregarDificultadCasoDeUso> _mockAgregar;
    private readonly Mock<EliminarDificultadCasoDeUso> _mockEliminar;
    private readonly Mock<ObtenerDificultadPorIdCasoDeUso> _mockObtenerPorId;
    private readonly Mock<ObtenerTodasLasDificultadesCasoDeUso> _mockObtenerTodos;

    public DificultadControllerTests()
    {
        var mockRepositorio = new Mock<IDificultadRepositorio>();
        var mockMapper = new Mock<IMapper>();

        _mockActualizar = new Mock<ActualizarDificultadCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockAgregar = new Mock<AgregarDificultadCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockEliminar = new Mock<EliminarDificultadCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockObtenerPorId = new Mock<ObtenerDificultadPorIdCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockObtenerTodos = new Mock<ObtenerTodasLasDificultadesCasoDeUso>(mockRepositorio.Object, mockMapper.Object);

        _controller = new DificultadController(
            _mockObtenerTodos.Object,
            _mockObtenerPorId.Object,
            _mockAgregar.Object,
            _mockActualizar.Object,
            _mockEliminar.Object
        );
    }

    #region ObtenerTodos Tests

    //ObtenerTodos_RetornaOkResult_ConListaCompleta
    [Fact]
    public async Task ObtenerTodos_RetornaOkResult_ConListaCompleta()
    {
        // Arrange
        var listaDificultades = new List<DificultadDTO>
        {
            new DificultadDTO { Id = 1, Descripcion = "Principiante" },
            new DificultadDTO { Id = 2, Descripcion = "Intermedio" },
            new DificultadDTO { Id = 3, Descripcion = "Avanzado" }
        };

        _mockObtenerTodos
            .Setup(caso => caso.Ejecutar())
            .ReturnsAsync(listaDificultades);

        // Act
        var resultado = await _controller.ObtenerTodos();

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(listaDificultades);
    }

    //ObtenerTodos_RetornaOkResult_ConListaVacia
    [Fact]
    public async Task ObtenerTodos_RetornaOkResult_ConListaVacia()
    {
        // Arrange
        var listaDificultades = new List<DificultadDTO>();

        _mockObtenerTodos
            .Setup(caso => caso.Ejecutar())
            .ReturnsAsync(listaDificultades);

        // Act
        var resultado = await _controller.ObtenerTodos();

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(listaDificultades);
    }

    //ObtenerTodos_ExcepcionGenerica_RetornaInternalServerError
    [Fact]
    public async Task ObtenerTodos_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        _mockObtenerTodos.Setup(x => x.Ejecutar()).ThrowsAsync(new Exception());

        // Act
        var resultado = await _controller.ObtenerTodos();

        // Assert
        var statusCodeResult = resultado as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ObtenerPorId Tests

    //ObtenerPorId_Existe_RetornaObjetoOkCreado
    [Fact]
    public async Task ObtenerPorId_Existe_RetornaObjetoOkCreado()
    {
        // Arrange
        int dificultadId = 1;
        var dificultadDTO = new DificultadDTO { Id = dificultadId, Descripcion = "Principiante" };

        _mockObtenerPorId
            .Setup(caso => caso.Ejecutar(dificultadId))
            .ReturnsAsync(dificultadDTO);

        // Act
        var resultado = await _controller.ObtenerPorId(dificultadId);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(dificultadDTO);
    }

    //ObtenerPorId_IdCero_RetornaBadRequest
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

    //ObtenerPorId_IdNegativo_RetornaBadRequest
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

    //ObtenerPorId_NoExiste_RetornaNotFound
    [Fact]
    public async Task ObtenerPorId_NoExiste_RetornaNotFound()
    {
        // Arrange
        int dificultadId = 999;

        _mockObtenerPorId
            .Setup(caso => caso.Ejecutar(dificultadId))
            .ReturnsAsync((DificultadDTO?)null);

        // Act
        var resultado = await _controller.ObtenerPorId(dificultadId);

        // Assert
        var notFoundResult = resultado as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    //ObtenerPorId_ExcepcionGenerica_RetornaInternalServerError
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

    //Agregar_RetornaCreatedAtActionResult
    [Fact]
    public async Task Agregar_RetornaCreatedAtActionResult()
    {
        // Arrange
        var nuevaDificultadDTO = new AgregarDificultadDTO { Descripcion = "Principiante" };
        var dificultadCreada = new DificultadDTO { Id = 1, Descripcion = "Principiante" };

        _mockAgregar
            .Setup(caso => caso.Ejecutar(nuevaDificultadDTO))
            .ReturnsAsync(dificultadCreada);

        // Act
        var resultado = await _controller.Agregar(nuevaDificultadDTO);

        // Assert
        var createdAtActionResult = resultado as CreatedAtActionResult;
        createdAtActionResult.Should().NotBeNull();
        createdAtActionResult!.StatusCode.Should().Be(201);
        createdAtActionResult.Value.Should().BeEquivalentTo(dificultadCreada);
    }

    //Agregar_DtoNulo_RetornaBadRequest
    [Fact]
    public async Task Agregar_DtoNulo_RetornaBadRequest()
    {
        // Act
        var resultado = await _controller.Agregar(null);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    //Agregar_ModelStateInvalido_RetornaBadRequest
    [Fact]
    public async Task Agregar_ModelStateInvalido_RetornaBadRequest()
    {
        // Arrange
        _controller.ModelState.AddModelError("Descripcion", "Requerido");
        var dto = new AgregarDificultadDTO();

        // Act
        var resultado = await _controller.Agregar(dto);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    //Agregar_ExcepcionGenerica_RetornaInternalServerError
    [Fact]
    public async Task Agregar_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        var dto = new AgregarDificultadDTO { Descripcion = "Difícil" };
        _mockAgregar.Setup(x => x.Ejecutar(dto)).ThrowsAsync(new Exception());

        // Act
        var resultado = await _controller.Agregar(dto);

        // Assert
        var statusCodeResult = resultado as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region Actualizar Tests

    //Actualizar_RetornaOkObjectResult_ConObjetoActualizado
    [Fact]
    public async Task Actualizar_RetornaOkObjectResult_ConObjetoActualizado()
    {
        // Arrange
        int dificultadId = 1;
        var dificultadActualizarDTO = new DificultadDTO { Id = dificultadId, Descripcion = "Experto" };
        var dificultadActualizada = new DificultadDTO { Id = dificultadId, Descripcion = "Experto" };

        _mockActualizar
            .Setup(caso => caso.Ejecutar(dificultadActualizarDTO))
            .ReturnsAsync(dificultadActualizada);

        // Act
        var resultado = await _controller.Actualizar(dificultadId, dificultadActualizarDTO);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(dificultadActualizada);
    }

    //Actualizar_IdCero_RetornaBadRequest
    [Fact]
    public async Task Actualizar_IdCero_RetornaBadRequest()
    {
        // Arrange
        var dto = new DificultadDTO { Id = 0 };

        // Act
        var resultado = await _controller.Actualizar(0, dto);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    //Actualizar_IdNegativo_RetornaBadRequest
    [Fact]
    public async Task Actualizar_IdNegativo_RetornaBadRequest()
    {
        // Arrange
        var dto = new DificultadDTO { Id = -5 };

        // Act
        var resultado = await _controller.Actualizar(-5, dto);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    //Actualizar_DtoNulo_RetornaBadRequest
    [Fact]
    public async Task Actualizar_DtoNulo_RetornaBadRequest()
    {
        // Act
        var resultado = await _controller.Actualizar(1, null);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    //Actualizar_ModelStateInvalido_RetornaBadRequest
    [Fact]
    public async Task Actualizar_ModelStateInvalido_RetornaBadRequest()
    {
        // Arrange
        _controller.ModelState.AddModelError("Descripcion", "Requerido");
        var dto = new DificultadDTO { Id = 1 };

        // Act
        var resultado = await _controller.Actualizar(1, dto);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    //Actualizar_IdNoCoincide_RetornaBadRequestResult
    [Fact]
    public async Task Actualizar_IdNoCoincide_RetornaBadRequestResult()
    {
        // Arrange
        int dificultadIdRuta = 1;
        var dificultadActualizarDTO = new DificultadDTO { Id = 2, Descripcion = "Avanzado" };

        // Act
        var resultado = await _controller.Actualizar(dificultadIdRuta, dificultadActualizarDTO);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    //Actualizar_NoEncontrado_RetornaNotFoundResult
    [Fact]
    public async Task Actualizar_NoEncontrado_RetornaNotFoundResult()
    {
        // Arrange
        int dificultadId = 999;
        var dificultadActualizarDTO = new DificultadDTO { Id = dificultadId, Descripcion = "No Existe" };

        _mockActualizar
            .Setup(caso => caso.Ejecutar(dificultadActualizarDTO))
            .ReturnsAsync((DificultadDTO?)null);

        // Act
        var resultado = await _controller.Actualizar(dificultadId, dificultadActualizarDTO);

        // Assert
        var notFoundResult = resultado as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    //Actualizar_ExcepcionGenerica_RetornaInternalServerError
    [Fact]
    public async Task Actualizar_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        var dto = new DificultadDTO { Id = 1, Descripcion = "Difícil" };
        _mockActualizar.Setup(x => x.Ejecutar(dto)).ThrowsAsync(new Exception());

        // Act
        var resultado = await _controller.Actualizar(1, dto);

        // Assert
        var statusCodeResult = resultado as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region Eliminar Tests

    //Eliminar_Existente_DeberiaRetornarNoContent
    [Fact]
    public async Task Eliminar_Existente_DeberiaRetornarNoContent()
    {
        // Arrange
        int dificultadId = 1;

        _mockEliminar
            .Setup(caso => caso.Ejecutar(dificultadId))
            .Returns(Task.CompletedTask);

        // Act
        var resultado = await _controller.Eliminar(dificultadId);

        // Assert
        var noContentResult = resultado as NoContentResult;
        noContentResult.Should().NotBeNull();
        noContentResult!.StatusCode.Should().Be(204);
    }

    //Eliminar_IdCero_RetornaBadRequest
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

    //Eliminar_IdNegativo_RetornaBadRequest
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

    //Eliminar_ExcepcionGenerica_RetornaInternalServerError
    [Fact]
    public async Task Eliminar_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        _mockEliminar.Setup(x => x.Ejecutar(1)).ThrowsAsync(new Exception());

        // Act
        var resultado = await _controller.Eliminar(1);

        // Assert
        var statusCodeResult = resultado as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion
}
