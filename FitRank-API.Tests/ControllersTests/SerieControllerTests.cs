using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using FitRank_API.Presentacion.Controllers;
using FitRank_API.Application.DTOs;
using AutoMapper;
using FitRank_API.Domain.Interfaces;
using FitRank_API.Application.DTOs;
using FitRank_API.Controllers;
using FitRank_API.Application.CasosDeUso.SerieCasosDeUso;
using FitRank_API.Application.DTOs.SerieDTOs;
using FitRank_API.Domain.Entities;

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

    //Agregar_RetornaCreatedAtActionResult
    [Fact]
    public async Task Agregar_RetornaCreatedAtActionResult()
    {
        // Arrange
        var nuevaSerieDTO = new AgregarSerieDTO
        {
            NumeroDeSerie = 1,
            Repeticiones = 10,
            Peso = 50.0,
            EjercicioAsignadoId = 1
        };
        var serieDTO = new ObtenerSerieDTO
        {
            Id = 1,
            NumeroDeSerie = 1,
            Repeticiones = 10,
            Peso = 50.0,
            EjercicioAsignadoId = 1
        };

        _mockAgregar
            .Setup(s => s.Ejecutar(nuevaSerieDTO))
            .ReturnsAsync(serieDTO);

        // Act
        var resultado = await _controller.Agregar(nuevaSerieDTO);

        // Assert
        var createdAtActionResult = resultado as CreatedAtActionResult;
        createdAtActionResult.Should().NotBeNull();
        createdAtActionResult!.StatusCode.Should().Be(201);
        createdAtActionResult.Value.Should().BeEquivalentTo(serieDTO);
    }

    //Agregar_LanzaExcepcion_RetornaStatusCode500
    [Fact]
    public async Task Agregar_LanzaExcepcion_RetornaStatusCode500()
    {
        // Arrange
        var nuevaSerieDTO = new AgregarSerieDTO
        {
            NumeroDeSerie = 1,
            Repeticiones = 10,
            Peso = 50.0,
            EjercicioAsignadoId = 1
        };

        _mockAgregar
            .Setup(s => s.Ejecutar(nuevaSerieDTO))
            .ThrowsAsync(new Exception("Error en el servidor."));

        // Act
        var resultado = await _controller.Agregar(nuevaSerieDTO);

        // Assert
        var objectResult = resultado as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(500);
        objectResult.Value.Should().Be("Error en el servidor.");
    }

    //Agregar_RetornaBadRequest_CuandoDTOEsNulo
    [Fact]
    public async Task Agregar_RetornaBadRequest_CuandoDTOEsNulo()
    {
        // Act
        var resultado = await _controller.Agregar(null);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
        badRequestResult.Value.Should().Be("El objeto no puede ser nulo.");
    }

    //ObtenerTodos_RetornaOkResult_ConListaCompleta
    [Fact]
    public async Task ObtenerTodos_RetornaOkResult_ConListaCompleta()
    {
        // Arrange
        var listaSeriesDTO = new List<ObtenerSerieDTO>
        {
            new ObtenerSerieDTO { Id = 1, NumeroDeSerie = 1, Repeticiones = 10, Peso = 50.0, EjercicioAsignadoId = 1 },
            new ObtenerSerieDTO { Id = 2, NumeroDeSerie = 2, Repeticiones = 8, Peso = 60.0, EjercicioAsignadoId = 1 }
        };

        _mockObtenerTodos
            .Setup(s => s.Ejecutar())
            .ReturnsAsync(listaSeriesDTO);

        // Act
        var resultado = await _controller.ObtenerTodaslasSeries();

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(listaSeriesDTO);
    }

    //ObtenerTodos_RetornaOkResult_ConListaVacia
    [Fact]
    public async Task ObtenerTodos_RetornaOkResult_ConListaVacia()
    {
        // Arrange
        var listaSeriesDTO = new List<ObtenerSerieDTO>();

        _mockObtenerTodos
            .Setup(s => s.Ejecutar())
            .ReturnsAsync(listaSeriesDTO);

        // Act
        var resultado = await _controller.ObtenerTodaslasSeries();

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(listaSeriesDTO);
    }

    //ObtenerTodos_LanzaExcepcion_RetornaStatus500
    [Fact]
    public async Task ObtenerTodos_LanzaExcepcion_RetornaStatus500()
    {
        // Arrange
        _mockObtenerTodos
            .Setup(s => s.Ejecutar())
            .ThrowsAsync(new Exception("Error en el servidor."));

        // Act
        var resultado = await _controller.ObtenerTodaslasSeries();

        // Assert
        var objectResult = resultado as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(500);
        objectResult.Value.Should().Be("Error en el servidor.");
    }

    //ObtenerPorId_NoExiste_RetornaNotFound
    [Fact]
    public async Task ObtenerPorId_NoExiste_RetornaNotFound()
    {
        // Arrange
        int serieId = 1;

        _mockObtenerPorId
            .Setup(s => s.Ejecutar(serieId))
            .ReturnsAsync((ObtenerSerieDTO?)null);

        // Act
        var resultado = await _controller.ObtenerSeriePorId(serieId);

        // Assert
        var notFoundResult = resultado as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
        notFoundResult.Value.Should().Be($"La serie con ID {serieId} no existe.");
    }

    //ObtenerPorId_Existe_RetornaObjetoOkCreado
    [Fact]
    public async Task ObtenerPorId_Existe_RetornaObjetoOkCreado()
    {
        // Arrange
        int serieId = 1;
        var serieDTO = new ObtenerSerieDTO
        {
            Id = serieId,
            NumeroDeSerie = 1,
            Repeticiones = 10,
            Peso = 50.0,
            EjercicioAsignadoId = 1
        };

        _mockObtenerPorId
            .Setup(s => s.Ejecutar(serieId))
            .ReturnsAsync(serieDTO);

        // Act
        var resultado = await _controller.ObtenerSeriePorId(serieId);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(serieDTO);
    }

    //Actualizar_RetornaOkObjectResult_ConObjetoActualizado
    [Fact]
    public async Task Actualizar_RetornaOkObjectResult_ConObjetoActualizado()
    {
        // Arrange
        int serieId = 1;
        var actualizarSerieDTO = new ActualizarSerieDTO
        {
            Id = serieId,
            NumeroDeSerie = 2,
            Repeticiones = 12,
            Peso = 55.0,
            EjercicioAsignadoId = 1
        };
        var serieDTO = new ObtenerSerieDTO
        {
            Id = serieId,
            NumeroDeSerie = 2,
            Repeticiones = 12,
            Peso = 55.0,
            EjercicioAsignadoId = 1
        };

        _mockActualizar
            .Setup(s => s.Ejecutar(actualizarSerieDTO))
            .ReturnsAsync(serieDTO);

        // Act
        var resultado = await _controller.Actualizar(serieId, actualizarSerieDTO);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(serieDTO);
    }

    //Actualizar_NoEncontrado_RetornaNotFoundResult
    [Fact]
    public async Task Actualizar_NoEncontrado_RetornaNotFoundResult()
    {
        // Arrange
        int serieId = 1;
        var actualizarSerieDTO = new ActualizarSerieDTO
        {
            Id = serieId,
            NumeroDeSerie = 2,
            Repeticiones = 12,
            Peso = 55.0,
            EjercicioAsignadoId = 1
        };

        _mockActualizar
            .Setup(s => s.Ejecutar(actualizarSerieDTO))
            .ReturnsAsync((ObtenerSerieDTO?)null);

        // Act
        var resultado = await _controller.Actualizar(serieId, actualizarSerieDTO);

        // Assert
        var notFoundResult = resultado as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
        notFoundResult.Value.Should().Be($"La serie con ID {serieId} no existe.");
    }

    //Actualizar_LanzaExcepcion_RetornaStatusCode500
    [Fact]
    public async Task Actualizar_LanzaExcepcion_RetornaStatusCode500()
    {
        // Arrange
        int serieId = 1;
        var actualizarSerieDTO = new ActualizarSerieDTO
        {
            Id = serieId,
            NumeroDeSerie = 2,
            Repeticiones = 12,
            Peso = 55.0,
            EjercicioAsignadoId = 1
        };

        _mockActualizar
            .Setup(s => s.Ejecutar(actualizarSerieDTO))
            .ThrowsAsync(new Exception("Error al actualizar la serie"));

        // Act
        var resultado = await _controller.Actualizar(serieId, actualizarSerieDTO);

        // Assert
        var objectResult = resultado as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(500);
        objectResult.Value.Should().Be("Error en el servidor.");
    }

    //Actualizar_IdNoCoincide_RetornaBadRequestResult
    [Fact]
    public async Task Actualizar_IdNoCoincide_RetornaBadRequestResult()
    {
        // Arrange
        int serieId = 1;
        var actualizarSerieDTO = new ActualizarSerieDTO
        {
            Id = serieId,
            NumeroDeSerie = 2,
            Repeticiones = 12,
            Peso = 55.0,
            EjercicioAsignadoId = 1
        };

        // Act
        var resultado = await _controller.Actualizar(serieId + 1, actualizarSerieDTO);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
        badRequestResult.Value.Should().Be("El ID en la URL no coincide con el ID en el cuerpo.");
    }

    //Actualizar_DTONulo_RetornaBadRequestResult
    [Fact]
    public async Task Actualizar_DTONulo_RetornaBadRequestResult()
    {
        // Arrange
        int serieId = 1;

        // Act
        var resultado = await _controller.Actualizar(serieId, null);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
        badRequestResult.Value.Should().Be("El objeto no puede ser nulo.");
    }

    //Eliminar_Existente_DeberiaRetornarNoContent
    [Fact]
    public async Task Eliminar_Existente_DeberiaRetornarNoContent()
    {
        // Arrange
        int serieId = 1;

        _mockEliminar
            .Setup(s => s.Ejecutar(serieId))
            .ReturnsAsync(true);

        // Act
        var resultado = await _controller.Eliminar(serieId);

        // Assert
        var noContentResult = resultado as NoContentResult;
        noContentResult.Should().NotBeNull();
        noContentResult!.StatusCode.Should().Be(204);
    }

    //Eliminar_NoExistente_DeberiaRetornarNotFound
    [Fact]
    public async Task Eliminar_NoExistente_DeberiaRetornarNotFound()
    {
        // Arrange
        int serieId = 1;

        _mockEliminar
            .Setup(s => s.Ejecutar(serieId))
            .ReturnsAsync(false);

        // Act
        var resultado = await _controller.Eliminar(serieId);

        // Assert
        var notFoundResult = resultado as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
        notFoundResult.Value.Should().Be($"La serie con ID {serieId} no existe.");
    }

    //Eliminar_CuandoOcurreErrorEnServidor_DeberiaRetornarStatusCode500
    [Fact]
    public async Task Eliminar_CuandoOcurreErrorEnServidor_DeberiaRetornarStatusCode500()
    {
        // Arrange
        int serieId = 1;

        _mockEliminar
            .Setup(s => s.Ejecutar(serieId))
            .ThrowsAsync(new Exception("Error al eliminar la serie"));

        // Act
        var resultado = await _controller.Eliminar(serieId);

        // Assert
        var objectResult = resultado as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(500);
        objectResult.Value.Should().Be("Error en el servidor.");
    }
}