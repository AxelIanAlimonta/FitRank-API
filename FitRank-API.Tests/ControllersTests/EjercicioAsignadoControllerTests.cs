using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using FitRank_API.Presentacion.Controllers;
using FitRank_API.Application.DTOs;
using AutoMapper;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Application.DTOs;
using FitRank_API.Controllers;
using FitRank_API.Application.CasosDeUso.EjercicioAsignadoCasoDeUso;
using FitRank_API.Application.DTOs.EjercicioAsignadoDTOs;
using FitRank_API.Domain.Entities;

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

    //Agregar_RetornaCreatedAtActionResult
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


    //Agregar_LanzaExcepcion_RetornaStatusCode500
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
        objectResult.Value.Should().Be("Error interno del servidor");
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
        badRequestResult.Value.Should().Be("El cuerpo de la solicitud no puede ser nulo.");
    }


    //ObtenerTodos_RetornaOkResult_ConListaCompleta
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

    //ObtenerTodos_RetornaOkResult_ConListaVacia
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

    //ObtenerTodos_LanzaExcepcion_RetornaStatus500
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
        objectResult.Value.Should().Be("Error interno del servidor.");
    }

    //ObtenerPorId_NoExiste_RetornaNotFound
    [Fact]
    public async Task ObtenerPorId_NoExiste_RetornaNotFound()
    {
        // Arrange
        long ejercicioId = 1;

        _mockObtenerPorId
            .Setup(casoDeUso => casoDeUso.Ejecutar(ejercicioId))
            .ReturnsAsync((ObtenerEjercicioAsignadoDTO?)null);

        // Act
        var resultado = await _controller.ObtenerPorId(ejercicioId);

        // Assert
        var notFoundResult = resultado as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
        notFoundResult.Value.Should().Be($"No se encontró ningún ejercicio asignado con ID {ejercicioId}.");
    }

    //ObtenerPorId_Existe_RetornaObjetoOkCreado
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

    //Actualizar_RetornaOkObjectResult_ConObjetoActualizado
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

    //Actualizar_NoEncontrado_RetornaNotFoundResult
    [Fact]
    public async Task Actualizar_NoEncontrado_RetornaNotFoundResult()
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
            .ReturnsAsync((ObtenerEjercicioAsignadoDTO?)null);

        // Act
        var resultado = await _controller.Actualizar(ejercicioId, actualizarEjercicioDTO);

        // Assert
        var notFoundResult = resultado as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
        notFoundResult.Value.Should().Be($"No se encontró ningún ejercicio asignado con ID {ejercicioId} para actualizar.");
    }

    //Actualizar_LanzaExcepcion_RetornaStatusCode500
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
        objectResult.Value.Should().Be("Error interno del servidor.");
    }

    //Actualizar_IdNoCoincide_RetornaBadRequestResult
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
        badRequestResult.Value.Should().Be("El ID del ejercicio asignado no coincide con el ID proporcionado.");
    }

    //Actualizar_DTONulo_RetornaBadRequestResult
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
        badRequestResult.Value.Should().Be("El cuerpo de la solicitud no puede ser nulo.");
    }

    //Eliminar_Existente_DeberiaRetornarNoContent
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

    //Eliminar_NoExistente_DeberiaRetornarNotFound
    [Fact]
    public async Task Eliminar_NoExistente_DeberiaRetornarNotFound()
    {
        // Arrange
        long ejercicioId = 1;

        _mockEliminar
            .Setup(casoDeUso => casoDeUso.Ejecutar(ejercicioId))
            .ReturnsAsync(false);

        // Act
        var resultado = await _controller.Eliminar(ejercicioId);

        // Assert
        var notFoundResult = resultado as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
        notFoundResult.Value.Should().Be($"No se encontró ningún ejercicio asignado con ID {ejercicioId} para eliminar.");
    }

    //Eliminar_CuandoOcurreErrorEnServidor_DeberiaRetornarStatusCode500
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
        objectResult.Value.Should().Be("Error interno del servidor.");
    }
}