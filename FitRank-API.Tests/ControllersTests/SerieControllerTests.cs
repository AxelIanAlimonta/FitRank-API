using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using FitRank_API.Presentacion.Controllers;
using AutoMapper;
using FitRank_API.Domain.Interfaces;
using FitRank_API.Application.CasosDeUso.SerieCasosDeUso;
using FitRank_API.Application.DTOs.SerieDTOs;

namespace FitRank_API.tests.ControllersTests;

public class SerieControllerTests
{
    private readonly SerieController _controller;
    private readonly Mock<ActualizarSerieCasoDeUso> _mockActualizar;
    private readonly Mock<AgregarSerieCasoDeUso> _mockAgregar;
    private readonly Mock<EliminarSerieCasoDeUso> _mockEliminar;
    private readonly Mock<ObtenerSeriePorIdCasoDeUso> _mockObtenerPorId;
    private readonly Mock<ObtenerSeriesCasoDeUso> _mockObtenerTodos;

    public SerieControllerTests()
    {
        var mockRepositorio = new Mock<ISerieRepositorio>();
        var mockMapper = new Mock<IMapper>();

        _mockActualizar = new Mock<ActualizarSerieCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockAgregar = new Mock<AgregarSerieCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockEliminar = new Mock<EliminarSerieCasoDeUso>(mockRepositorio.Object);
        _mockObtenerPorId = new Mock<ObtenerSeriePorIdCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockObtenerTodos = new Mock<ObtenerSeriesCasoDeUso>(mockRepositorio.Object, mockMapper.Object);

        _controller = new SerieController(
            _mockAgregar.Object,
            _mockObtenerTodos.Object,
            _mockObtenerPorId.Object,
            _mockActualizar.Object,
            _mockEliminar.Object
        );
    }

    #region Agregar Tests

    [Fact]
    public async Task Agregar_DtoNulo_RetornaBadRequest()
    {
        // Act
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
        var nuevaSerieDTO = new AgregarSerieDTO { NumeroDeSerie = 1, Repeticiones = 10, Peso = 50.0 };
        var serieDTO = new ObtenerSerieDTO { Id = 1, NumeroDeSerie = 1, Repeticiones = 10, Peso = 50.0 };

        _mockAgregar.Setup(s => s.Ejecutar(nuevaSerieDTO)).ReturnsAsync(serieDTO);

        // Act
        var resultado = await _controller.Agregar(nuevaSerieDTO);

        // Assert
        var createdAtActionResult = resultado as CreatedAtActionResult;
        createdAtActionResult.Should().NotBeNull();
        createdAtActionResult!.StatusCode.Should().Be(201);
        createdAtActionResult.Value.Should().BeEquivalentTo(serieDTO);
    }

    [Fact]
    public async Task Agregar_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        var nuevaSerieDTO = new AgregarSerieDTO { NumeroDeSerie = 1 };
        _mockAgregar.Setup(s => s.Ejecutar(nuevaSerieDTO)).ThrowsAsync(new Exception());

        // Act
        var resultado = await _controller.Agregar(nuevaSerieDTO);

        // Assert
        var objectResult = resultado as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ObtenerTodaslasSeries Tests

    [Fact]
    public async Task ObtenerTodos_Exitoso_RetornaOk()
    {
        // Arrange
        var listaSeriesDTO = new List<ObtenerSerieDTO>
        {
            new ObtenerSerieDTO { Id = 1, NumeroDeSerie = 1 },
            new ObtenerSerieDTO { Id = 2, NumeroDeSerie = 2 }
        };

        _mockObtenerTodos.Setup(s => s.Ejecutar()).ReturnsAsync(listaSeriesDTO);

        // Act
        var resultado = await _controller.ObtenerTodaslasSeries();

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(listaSeriesDTO);
    }

    [Fact]
    public async Task ObtenerTodos_ListaVacia_RetornaOk()
    {
        // Arrange
        var listaSeriesDTO = new List<ObtenerSerieDTO>();
        _mockObtenerTodos.Setup(s => s.Ejecutar()).ReturnsAsync(listaSeriesDTO);

        // Act
        var resultado = await _controller.ObtenerTodaslasSeries();

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(listaSeriesDTO);
    }

    [Fact]
    public async Task ObtenerTodos_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        _mockObtenerTodos.Setup(s => s.Ejecutar()).ThrowsAsync(new Exception());

        // Act
        var resultado = await _controller.ObtenerTodaslasSeries();

        // Assert
        var objectResult = resultado as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ObtenerSeriePorId Tests

    [Fact]
    public async Task ObtenerSeriePorId_IdCero_RetornaBadRequest()
    {
        // Act
        var resultado = await _controller.ObtenerSeriePorId(0);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerSeriePorId_IdNegativo_RetornaBadRequest()
    {
        // Act
        var resultado = await _controller.ObtenerSeriePorId(-5);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerSeriePorId_Existoso_RetornaOk()
    {
        // Arrange
        int serieId = 1;
        var serieDTO = new ObtenerSerieDTO { Id = serieId, NumeroDeSerie = 1 };

        _mockObtenerPorId.Setup(s => s.Ejecutar(serieId)).ReturnsAsync(serieDTO);

        // Act
        var resultado = await _controller.ObtenerSeriePorId(serieId);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(serieDTO);
    }

    [Fact]
    public async Task ObtenerSeriePorId_NoExiste_RetornaNotFound()
    {
        // Arrange
        int serieId = 999;
        _mockObtenerPorId.Setup(s => s.Ejecutar(serieId)).ReturnsAsync((ObtenerSerieDTO?)null);

        // Act
        var resultado = await _controller.ObtenerSeriePorId(serieId);

        // Assert
        var notFoundResult = resultado as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task ObtenerSeriePorId_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        _mockObtenerPorId.Setup(s => s.Ejecutar(1)).ThrowsAsync(new Exception());

        // Act
        var resultado = await _controller.ObtenerSeriePorId(1);

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
        var dto = new ActualizarSerieDTO { Id = 0 };

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
        var dto = new ActualizarSerieDTO { Id = -3 };

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
        // Act
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
        int serieId = 1;
        var actualizarSerieDTO = new ActualizarSerieDTO { Id = serieId };

        // Act
        var resultado = await _controller.Actualizar(serieId + 1, actualizarSerieDTO);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Actualizar_Exitoso_RetornaOk()
    {
        // Arrange
        int serieId = 1;
        var actualizarSerieDTO = new ActualizarSerieDTO { Id = serieId, NumeroDeSerie = 2 };
        var serieDTO = new ObtenerSerieDTO { Id = serieId, NumeroDeSerie = 2 };

        _mockActualizar.Setup(s => s.Ejecutar(actualizarSerieDTO)).ReturnsAsync(serieDTO);

        // Act
        var resultado = await _controller.Actualizar(serieId, actualizarSerieDTO);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(serieDTO);
    }

    [Fact]
    public async Task Actualizar_NoEncontrado_RetornaNotFound()
    {
        // Arrange
        int serieId = 999;
        var actualizarSerieDTO = new ActualizarSerieDTO { Id = serieId };

        _mockActualizar.Setup(s => s.Ejecutar(actualizarSerieDTO)).ReturnsAsync((ObtenerSerieDTO?)null);

        // Act
        var resultado = await _controller.Actualizar(serieId, actualizarSerieDTO);

        // Assert
        var notFoundResult = resultado as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Actualizar_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        int serieId = 1;
        var actualizarSerieDTO = new ActualizarSerieDTO { Id = serieId };

        _mockActualizar.Setup(s => s.Ejecutar(actualizarSerieDTO)).ThrowsAsync(new Exception());

        // Act
        var resultado = await _controller.Actualizar(serieId, actualizarSerieDTO);

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
        int serieId = 1;
        _mockEliminar.Setup(s => s.Ejecutar(serieId)).ReturnsAsync(true);

        // Act
        var resultado = await _controller.Eliminar(serieId);

        // Assert
        var noContentResult = resultado as NoContentResult;
        noContentResult.Should().NotBeNull();
        noContentResult!.StatusCode.Should().Be(204);
    }

    [Fact]
    public async Task Eliminar_NoExistente_RetornaNotFound()
    {
        // Arrange
        int serieId = 999;
        _mockEliminar.Setup(s => s.Ejecutar(serieId)).ReturnsAsync(false);

        // Act
        var resultado = await _controller.Eliminar(serieId);

        // Assert
        var notFoundResult = resultado as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Eliminar_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        int serieId = 1;
        _mockEliminar.Setup(s => s.Ejecutar(serieId)).ThrowsAsync(new Exception());

        // Act
        var resultado = await _controller.Eliminar(serieId);

        // Assert
        var objectResult = resultado as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(500);
    }

    #endregion
}
