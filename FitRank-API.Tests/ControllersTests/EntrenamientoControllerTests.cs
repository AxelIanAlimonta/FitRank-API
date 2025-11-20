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
using FitRank_API.Domain.Entities;
using FitRank_API.Application.UseCases.Entrenamiento;
using FitRank_API.Application.DTOs.EntrenamientoDTOs;
using FitRank_API.Application.CasosDeUso.EntrenamientoCasosDeUso;

namespace FitRank_API.tests.ControllersTests;

public class EntrenamientoControllerTests
{

    private readonly EntrenamientoController _controller;
    private readonly Mock<ActualizarEntrenamientoCasoDeUso> _mockActualizar;
    private readonly Mock<AgregarEntrenamientoCasoDeUso> _mockAgregar;
    private readonly Mock<EliminarEntrenamientoCasoDeUso> _mockEliminar;
    private readonly Mock<ObtenerEntrenamientoPorIdCasoDeUso> _mockObtenerPorId;
    private readonly Mock<ObtenerEntrenamientosCasoDeUso> _mockObtenerTodos;
    private readonly Mock<ObtenerHistorialEntrenamientosDeUnUsuarioCasoDeUso> _mockObtenerHistorial;

    public EntrenamientoControllerTests()
    {
        var mockRepositorio = new Mock<IEntrenamientoRepositorio>();
        var mockMapper = new Mock<IMapper>();

        _mockActualizar = new Mock<ActualizarEntrenamientoCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockAgregar = new Mock<AgregarEntrenamientoCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockEliminar = new Mock<EliminarEntrenamientoCasoDeUso>(mockRepositorio.Object);
        _mockObtenerPorId = new Mock<ObtenerEntrenamientoPorIdCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockObtenerTodos = new Mock<ObtenerEntrenamientosCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockObtenerHistorial = new Mock<ObtenerHistorialEntrenamientosDeUnUsuarioCasoDeUso>(mockRepositorio.Object, mockMapper.Object);

        _controller = new EntrenamientoController(
            _mockAgregar.Object,
            _mockObtenerTodos.Object,
            _mockObtenerPorId.Object,
            _mockActualizar.Object,
            _mockEliminar.Object,
            _mockObtenerHistorial.Object
        );
    }

    //Agregar_RetornaCreatedAtActionResult
    [Fact]
    public async Task Agregar_RetornaCreatedAtActionResult()
    {
        // Arrange
        var nuevoEntrenamiento = new AgregarEntrenamientoDTO
        {
            SocioId = 1,
            Fecha = new DateTime(2024, 6, 1)

        };

        var entrenamientoCreado = new ObtenerEntrenamientoDTO
        {
            Id = 1,
            SocioId = 1,
            Fecha = new DateTime(2024, 6, 1),
        };

        _mockAgregar
            .Setup(casoDeUso => casoDeUso.Ejecutar(nuevoEntrenamiento))
            .ReturnsAsync(entrenamientoCreado);

        // Act
        var resultado = await _controller.Crear(nuevoEntrenamiento);

        // Assert
        var createdAtActionResult = resultado as CreatedAtActionResult;
        createdAtActionResult.Should().NotBeNull();
        createdAtActionResult.StatusCode.Should().Be(201);
        createdAtActionResult.Value.Should().BeEquivalentTo(entrenamientoCreado);
    }

    //Agregar_LanzaExcepcion_RetornaStatusCode500
    [Fact]
    public async Task Agregar_LanzaExcepcion_RetornaStatusCode500()
    {
        // Arrange
        var nuevoEntrenamiento = new AgregarEntrenamientoDTO
        {
            SocioId = 1,
            Fecha = new DateTime(2024, 6, 1)
        };

        _mockAgregar
            .Setup(casoDeUso => casoDeUso.Ejecutar(nuevoEntrenamiento))
            .ThrowsAsync(new Exception("Error interno del servidor."));

        // Act
        var resultado = await _controller.Crear(nuevoEntrenamiento);
        // Assert
        var objectResult = resultado as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult.StatusCode.Should().Be(500);
        objectResult.Value.Should().Be("Error interno del servidor.");
    }

    //Agregar_RetornaBadRequest_CuandoDTOEsNulo
    [Fact]
    public async Task Agregar_RetornaBadRequest_CuandoDTOEsNulo()
    {
        // Arrange
        AgregarEntrenamientoDTO nuevoEntrenamiento = null;

        // Act
        var resultado = await _controller.Crear(nuevoEntrenamiento);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.StatusCode.Should().Be(400);
        badRequestResult.Value.Should().Be("El cuerpo de la solicitud no puede ser nulo.");
    }

    //ObtenerTodos_RetornaOkResult_ConListaCompleta
    [Fact]
    public async Task ObtenerTodos_RetornaOkResult_ConListaCompleta()
    {
        // Arrange
        var listaEntrenamientos = new List<ObtenerEntrenamientoDTO>
        {
            new ObtenerEntrenamientoDTO { Id = 1, SocioId = 1, Fecha = new DateTime(2024, 6, 1) },
            new ObtenerEntrenamientoDTO { Id = 2, SocioId = 2, Fecha = new DateTime(2024, 6, 2) }
        };

        _mockObtenerTodos
            .Setup(casoDeUso => casoDeUso.Ejecutar())
            .ReturnsAsync(listaEntrenamientos);

        // Act
        var resultado = await _controller.GetAll();

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(listaEntrenamientos);
    }

    //ObtenerTodos_RetornaOkResult_ConListaVacia
    [Fact]
    public async Task ObtenerTodos_RetornaOkResult_ConListaVacia()
    {
        // Arrange
        var listaVacia = new List<ObtenerEntrenamientoDTO>();

        _mockObtenerTodos
            .Setup(casoDeUso => casoDeUso.Ejecutar())
            .ReturnsAsync(listaVacia);

        // Act
        var resultado = await _controller.GetAll();

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(listaVacia);
    }

    //ObtenerTodos_LanzaExcepcion_RetornaStatus500
    [Fact]
    public async Task ObtenerTodos_LanzaExcepcion_RetornaStatus500()
    {
        // Arrange
        _mockObtenerTodos
            .Setup(casoDeUso => casoDeUso.Ejecutar())
            .ThrowsAsync(new Exception("Error interno del servidor."));

        // Act
        var resultado = await _controller.GetAll();

        // Assert
        var objectResult = resultado as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult.StatusCode.Should().Be(500);
        objectResult.Value.Should().Be("Error interno del servidor.");
    }

    //ObtenerPorId_NoExiste_RetornaNotFound
    [Fact]
    public async Task ObtenerPorId_NoExiste_RetornaNotFound()
    {
        // Arrange
        int entrenamientoId = 99;

        _mockObtenerPorId
            .Setup(casoDeUso => casoDeUso.Ejecutar(entrenamientoId))
            .ReturnsAsync((ObtenerEntrenamientoDTO)null);

        // Act
        var resultado = await _controller.GetById(entrenamientoId);

        // Assert
        var notFoundResult = resultado as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult.StatusCode.Should().Be(404);
        notFoundResult.Value.Should().Be($"No se encontró ningún entrenamiento con ID {entrenamientoId}.");
    }

    //ObtenerPorId_Existe_RetornaObjetoOkCreado
    [Fact]
    public async Task ObtenerPorId_Existe_RetornaObjetoOkCreado()
    {
        // Arrange
        int entrenamientoId = 1;
        var entrenamientoExistente = new ObtenerEntrenamientoDTO
        {
            Id = entrenamientoId,
            SocioId = 1,
            Fecha = new DateTime(2024, 6, 1)
        };

        _mockObtenerPorId
            .Setup(casoDeUso => casoDeUso.Ejecutar(entrenamientoId))
            .ReturnsAsync(entrenamientoExistente);

        // Act
        var resultado = await _controller.GetById(entrenamientoId);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(entrenamientoExistente);
    }

    //Actualizar_RetornaOkObjectResult_ConObjetoActualizado
    [Fact]
    public async Task Actualizar_RetornaOkObjectResult_ConObjetoActualizado()
    {
        // Arrange
        int entrenamientoId = 1;
        var entrenamientoActualizado = new ActualizarEntrenamientoDTO
        {
            Id = entrenamientoId,
            SocioId = 1,
            Fecha = new DateTime(2024, 6, 2)
        };

        var entrenamientoEsperado = new ObtenerEntrenamientoDTO
        {
            Id = entrenamientoId,
            SocioId = 1,
            Fecha = new DateTime(2024, 6, 2)
        };

        _mockActualizar
            .Setup(casoDeUso => casoDeUso.Ejecutar(entrenamientoActualizado))
            .ReturnsAsync(entrenamientoEsperado);

        // Act
        var resultado = await _controller.Actualizar(entrenamientoId, entrenamientoActualizado);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(entrenamientoEsperado);
    }

    //Actualizar_NoEncontrado_RetornaNotFoundResult
    [Fact]
    public async Task Actualizar_NoEncontrado_RetornaNotFoundResult()
    {
        // Arrange
        int entrenamientoId = 99;
        var entrenamientoActualizado = new ActualizarEntrenamientoDTO
        {
            Id = entrenamientoId,
            SocioId = 1,
            Fecha = new DateTime(2024, 6, 2)
        };

        _mockActualizar
            .Setup(casoDeUso => casoDeUso.Ejecutar(entrenamientoActualizado))
            .ReturnsAsync((ObtenerEntrenamientoDTO)null);

        // Act
        var resultado = await _controller.Actualizar(entrenamientoId, entrenamientoActualizado);

        // Assert
        var notFoundResult = resultado as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult.StatusCode.Should().Be(404);
        notFoundResult.Value.Should().Be($"No se encontró ningún entrenamiento con ID {entrenamientoId}.");
    }

    //Actualizar_LanzaExcepcion_RetornaStatusCode500
    [Fact]
    public async Task Actualizar_LanzaExcepcion_RetornaStatusCode500()
    {
        // Arrange
        int entrenamientoId = 1;
        var entrenamientoActualizado = new ActualizarEntrenamientoDTO
        {
            Id = entrenamientoId,
            SocioId = 1,
            Fecha = new DateTime(2024, 6, 2)
        };

        _mockActualizar
            .Setup(casoDeUso => casoDeUso.Ejecutar(entrenamientoActualizado))
            .ThrowsAsync(new Exception("Error interno del servidor."));

        // Act
        var resultado = await _controller.Actualizar(entrenamientoId, entrenamientoActualizado);

        // Assert
        var objectResult = resultado as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult.StatusCode.Should().Be(500);
        objectResult.Value.Should().Be("Error interno del servidor.");
    }

    //Actualizar_IdNoCoincide_RetornaBadRequestResult
    [Fact]
    public async Task Actualizar_IdNoCoincide_RetornaBadRequestResult()
    {
        // Arrange
        int entrenamientoId = 1;
        var entrenamientoActualizado = new ActualizarEntrenamientoDTO
        {
            Id = entrenamientoId,
            SocioId = 1,
            Fecha = new DateTime(2024, 6, 2)
        };

        // Act
        var resultado = await _controller.Actualizar(entrenamientoId + 1, entrenamientoActualizado);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.StatusCode.Should().Be(400);
        badRequestResult.Value.Should().Be("El ID en la URL no coincide con el ID en el cuerpo de la solicitud.");
    }

    //Actualizar_DTONulo_RetornaBadRequestResult
    [Fact]
    public async Task Actualizar_DTONulo_RetornaBadRequestResult()
    {
        // Arrange
        int entrenamientoId = 1;
        ActualizarEntrenamientoDTO entrenamientoActualizado = null;

        // Act
        var resultado = await _controller.Actualizar(entrenamientoId, entrenamientoActualizado);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.StatusCode.Should().Be(400);
        badRequestResult.Value.Should().Be("El cuerpo de la solicitud no puede ser nulo.");
    }

    //Eliminar_Existente_DeberiaRetornarNoContent
    [Fact]
    public async Task Eliminar_Existente_DeberiaRetornarNoContent()
    {
        // Arrange
        int entrenamientoId = 1;

        _mockEliminar
            .Setup(casoDeUso => casoDeUso.Ejecutar(entrenamientoId))
            .ReturnsAsync(true);

        // Act
        var resultado = await _controller.Eliminar(entrenamientoId);

        // Assert
        var noContentResult = resultado as NoContentResult;
        noContentResult.Should().NotBeNull();
        noContentResult.StatusCode.Should().Be(204);
    }

    //Eliminar_NoExistente_DeberiaRetornarNotFound
    [Fact]
    public async Task Eliminar_NoExistente_DeberiaRetornarNotFound()
    {
        // Arrange
        int entrenamientoId = 99;

        _mockEliminar
            .Setup(casoDeUso => casoDeUso.Ejecutar(entrenamientoId))
            .ReturnsAsync(false);

        // Act
        var resultado = await _controller.Eliminar(entrenamientoId);

        // Assert
        var notFoundResult = resultado as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult.StatusCode.Should().Be(404);
        notFoundResult.Value.Should().Be($"No se encontró ningún entrenamiento con ID {entrenamientoId}.");
    }

    //Eliminar_CuandoOcurreErrorEnServidor_DeberiaRetornarStatusCode500
    [Fact]
    public async Task Eliminar_CuandoOcurreErrorEnServidor_DeberiaRetornarStatusCode500()
    {
        // Arrange
        int entrenamientoId = 1;

        _mockEliminar
            .Setup(casoDeUso => casoDeUso.Ejecutar(entrenamientoId))
            .ThrowsAsync(new Exception("Error interno del servidor."));

        // Act
        var resultado = await _controller.Eliminar(entrenamientoId);

        // Assert
        var objectResult = resultado as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult.StatusCode.Should().Be(500);
        objectResult.Value.Should().Be("Error interno del servidor.");
    }
}