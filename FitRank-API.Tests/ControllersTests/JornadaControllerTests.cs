using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using FitRank_API.Domain.Interfaces;
using FitRank_API.Presentacion.Controllers;
using FitRank_API.Application.CasosDeUso.JornadaCasosDeUso;
using FitRank_API.Application.DTOs.JornadaDTOs;

namespace FitRank_API.tests.ControllersTests;

public class JornadaControllerTests
{
    private readonly JornadaController _controller;
    private readonly Mock<ActualizarJornadaCasoDeUso> _mockActualizar;
    private readonly Mock<AgregarJornadaCasoDeUso> _mockAgregar;
    private readonly Mock<EliminarJornadaCasoDeUso> _mockEliminar;
    private readonly Mock<ObtenerJornadaPorIdCasoDeUso> _mockObtenerPorId;
    private readonly Mock<ObtenerTodasLasJornadasCasoDeUso> _mockObtenerTodos;

    public JornadaControllerTests()
    {
        var mockRepositorio = new Mock<IJornadaRepositorio>();
        var mockMapper = new Mock<IMapper>();

        _mockActualizar = new Mock<ActualizarJornadaCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockAgregar = new Mock<AgregarJornadaCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockEliminar = new Mock<EliminarJornadaCasoDeUso>(mockRepositorio.Object);
        _mockObtenerPorId = new Mock<ObtenerJornadaPorIdCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockObtenerTodos = new Mock<ObtenerTodasLasJornadasCasoDeUso>(mockRepositorio.Object, mockMapper.Object);

        _controller = new JornadaController(
            _mockObtenerTodos.Object,
            _mockObtenerPorId.Object,
            _mockAgregar.Object,
            _mockActualizar.Object,
            _mockEliminar.Object
        );
    }

    #region ObtenerTodasAsync Tests

    [Fact]
    public async Task ObtenerTodos_RetornaOkResult_ConListaCompleta()
    {
        // Arrange
        var listaJornadasDTO = new List<ObtenerJornadaDTO>
        {
            new ObtenerJornadaDTO { Id = 1, HoraInicio = new TimeSpan(8, 0, 0), HoraFin = new TimeSpan(14, 0, 0), ProfesorId = 1, DiaDeLaSemanaId = 2 },
            new ObtenerJornadaDTO { Id = 2, HoraInicio = new TimeSpan(9, 0, 0), HoraFin = new TimeSpan(15, 0, 0), ProfesorId = 2, DiaDeLaSemanaId = 3 }
        };

        _mockObtenerTodos.Setup(m => m.Ejecutar()).ReturnsAsync(listaJornadasDTO);

        // Act
        var resultado = await _controller.ObtenerTodasAsync();

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        var valorRetornado = okResult.Value as List<ObtenerJornadaDTO>;
        valorRetornado!.Count.Should().Be(2);
    }

    [Fact]
    public async Task ObtenerTodos_RetornaOkResult_ConListaVacia()
    {
        // Arrange
        _mockObtenerTodos.Setup(m => m.Ejecutar()).ReturnsAsync(new List<ObtenerJornadaDTO>());

        // Act
        var resultado = await _controller.ObtenerTodasAsync();

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        var valorRetornado = okResult.Value as List<ObtenerJornadaDTO>;
        valorRetornado!.Count.Should().Be(0);
    }

    [Fact]
    public async Task ObtenerTodos_LanzaExcepcion_RetornaStatus500()
    {
        // Arrange
        _mockObtenerTodos.Setup(m => m.Ejecutar()).ThrowsAsync(new Exception());

        // Act
        var resultado = await _controller.ObtenerTodasAsync();

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
        long jornadaId = 1;
        var jornadaDTO = new ObtenerJornadaDTO
        {
            Id = jornadaId,
            HoraInicio = new TimeSpan(8, 0, 0),
            HoraFin = new TimeSpan(14, 0, 0),
            ProfesorId = 1,
            DiaDeLaSemanaId = 2
        };

        _mockObtenerPorId.Setup(m => m.Ejecutar(jornadaId)).ReturnsAsync(jornadaDTO);

        // Act
        var resultado = await _controller.ObtenerPorId(jornadaId);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        var valorRetornado = okResult.Value as ObtenerJornadaDTO;
        valorRetornado!.Id.Should().Be(jornadaDTO.Id);
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
        long jornadaId = 1;
        _mockObtenerPorId.Setup(m => m.Ejecutar(jornadaId)).ReturnsAsync((ObtenerJornadaDTO?)null);

        // Act
        var resultado = await _controller.ObtenerPorId(jornadaId);

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
        var nuevaJornadaDTO = new AgregarJornadaDTO
        {
            HoraInicio = new TimeSpan(8, 0, 0),
            HoraFin = new TimeSpan(14, 0, 0),
            ProfesorId = 1,
            DiaDeLaSemanaId = 2
        };

        var jornadaCreadaDTO = new ObtenerJornadaDTO
        {
            Id = 1,
            HoraInicio = new TimeSpan(8, 0, 0),
            HoraFin = new TimeSpan(14, 0, 0),
            ProfesorId = 1,
            DiaDeLaSemanaId = 2
        };

        _mockAgregar.Setup(m => m.Ejecutar(It.IsAny<AgregarJornadaDTO>())).ReturnsAsync(jornadaCreadaDTO);

        // Act
        var resultado = await _controller.Agregar(nuevaJornadaDTO);

        // Assert
        var createdAtActionResult = resultado as CreatedAtActionResult;
        createdAtActionResult.Should().NotBeNull();
        createdAtActionResult!.StatusCode.Should().Be(201);
        createdAtActionResult.ActionName.Should().Be(nameof(_controller.ObtenerPorId));
        createdAtActionResult.RouteValues!["id"].Should().Be(jornadaCreadaDTO.Id);
    }

    [Fact]
    public async Task Agregar_RetornaBadRequest_CuandoDTOEsNulo()
    {
        // Act
        var resultado = await _controller.Agregar(null!);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Agregar_RetornaBadRequest_CuandoModeloNoEsValido()
    {
        // Arrange
        var nuevaJornadaDTO = new AgregarJornadaDTO();
        _controller.ModelState.AddModelError("ProfesorId", "El ID del profesor es inválido.");

        // Act
        var resultado = await _controller.Agregar(nuevaJornadaDTO);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Agregar_LanzaExcepcion_RetornaStatusCode500()
    {
        // Arrange
        var nuevaJornadaDTO = new AgregarJornadaDTO
        {
            HoraInicio = new TimeSpan(8, 0, 0),
            HoraFin = new TimeSpan(14, 0, 0),
            ProfesorId = 1,
            DiaDeLaSemanaId = 2
        };

        _mockAgregar.Setup(m => m.Ejecutar(It.IsAny<AgregarJornadaDTO>())).ThrowsAsync(new Exception());

        // Act
        var resultado = await _controller.Agregar(nuevaJornadaDTO);

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
        var jornadaId = 1;
        var actualizarJornadaDTO = new ActualizarJornadaDTO
        {
            Id = jornadaId,
            HoraInicio = new TimeSpan(10, 0, 0),
            HoraFin = new TimeSpan(16, 0, 0),
            ProfesorId = 1,
            DiaDeLaSemanaId = 2
        };
        var jornadaActualizadaDTO = new ObtenerJornadaDTO
        {
            Id = jornadaId,
            HoraInicio = new TimeSpan(10, 0, 0),
            HoraFin = new TimeSpan(16, 0, 0),
            ProfesorId = 1,
            DiaDeLaSemanaId = 2
        };

        _mockActualizar.Setup(m => m.Ejecutar(It.IsAny<ActualizarJornadaDTO>())).ReturnsAsync(jornadaActualizadaDTO);

        // Act
        var resultado = await _controller.Actualizar(jornadaId, actualizarJornadaDTO);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task Actualizar_IdCero_RetornaBadRequest()
    {
        // Arrange
        var dto = new ActualizarJornadaDTO { Id = 0 };

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
        var dto = new ActualizarJornadaDTO { Id = -5 };

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
        // Act
        var resultado = await _controller.Actualizar(1, null!);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Actualizar_ModelStateInvalido_RetornaBadRequest()
    {
        // Arrange
        _controller.ModelState.AddModelError("ProfesorId", "Requerido");
        var dto = new ActualizarJornadaDTO { Id = 1 };

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
        var actualizarJornadaDTO = new ActualizarJornadaDTO { Id = 1 };

        // Act
        var resultados = await _controller.Actualizar(2, actualizarJornadaDTO);

        // Assert
        var badRequestResult = resultados as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Actualizar_NoEncontrado_RetornaNotFoundResult()
    {
        // Arrange
        var jornadaId = 1;
        var actualizarJornadaDTO = new ActualizarJornadaDTO { Id = jornadaId };

        _mockActualizar.Setup(m => m.Ejecutar(It.IsAny<ActualizarJornadaDTO>())).ReturnsAsync((ObtenerJornadaDTO?)null);

        // Act
        var resultado = await _controller.Actualizar(jornadaId, actualizarJornadaDTO);

        // Assert
        var notFoundResult = resultado as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Actualizar_LanzaExcepcion_RetornaStatusCode500()
    {
        // Arrange
        var jornadaId = 1;
        var actualizarJornadaDTO = new ActualizarJornadaDTO { Id = jornadaId };

        _mockActualizar.Setup(m => m.Ejecutar(It.IsAny<ActualizarJornadaDTO>())).ThrowsAsync(new Exception());

        // Act
        var resultado = await _controller.Actualizar(jornadaId, actualizarJornadaDTO);

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
        long jornadaId = 1;
        _mockEliminar.Setup(m => m.Ejecutar(jornadaId)).ReturnsAsync(true);

        // Act
        var resultado = await _controller.Eliminar(jornadaId);

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
        long jornadaId = 1;
        _mockEliminar.Setup(m => m.Ejecutar(jornadaId)).ReturnsAsync(false);

        // Act
        var resultado = await _controller.Eliminar(jornadaId);

        // Assert
        var notFoundResult = resultado as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Eliminar_CuandoOcurreErrorEnServidor_DeberiaRetornarStatusCode500()
    {
        // Arrange
        long jornadaId = 1;
        _mockEliminar.Setup(m => m.Ejecutar(jornadaId)).ThrowsAsync(new Exception());

        // Act
        var resultado = await _controller.Eliminar(jornadaId);

        // Assert
        var objectResult = resultado as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(500);
    }

    #endregion
}