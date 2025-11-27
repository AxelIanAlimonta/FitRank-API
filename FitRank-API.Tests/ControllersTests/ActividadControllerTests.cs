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
using FitRank_API.Application.UseCases.Actividad;
using FitRank_API.Application.DTOs.ActividadDTOs;
using FitRank_API.Domain.Entities;
using System.Threading.Tasks;
using FitRank_API.Application.UseCases;

namespace FitRank_API.tests.ControllersTests;

public class ActividadControllerTests
{

    private readonly ActividadController _controller;
    private readonly Mock<ActualizarActividadCasoDeUso> _mockActualizar;
    private readonly Mock<AgregarActividadCasoDeUso> _mockAgregar;
    private readonly Mock<EliminarActividadCasoDeUso> _mockEliminar;
    private readonly Mock<ObtenerActividadPorIdCasoDeUso> _mockObtenerPorId;
    private readonly Mock<ObtenerActividadesCasoDeUso> _mockObtenerTodos;
    private readonly Mock<RegistrarActividadCasoDeUso> _mockRegistrar;

    public ActividadControllerTests()
    {
        var mockRepositorio = new Mock<IActividadRepositorio>();
        var mockMapper = new Mock<IMapper>();

        _mockActualizar = new Mock<ActualizarActividadCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockAgregar = new Mock<AgregarActividadCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockEliminar = new Mock<EliminarActividadCasoDeUso>(mockRepositorio.Object);
        _mockObtenerPorId = new Mock<ObtenerActividadPorIdCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockObtenerTodos = new Mock<ObtenerActividadesCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        // _mockRegistrar = new Mock<RegistrarActividadCasoDeUso>(mockRepositorio.Object, mockMapper.Object);

        _controller = new ActividadController(
            _mockAgregar.Object,
            _mockObtenerTodos.Object,
            _mockObtenerPorId.Object,
            _mockActualizar.Object,
            _mockEliminar.Object,
            null
        );

    }

    //Agregar_RetornaCreatedAtActionResult
    [Fact]
    public async Task Agregar_RetornaCreatedAtActionResult()
    {
        // Arrange
        var nuevaActividad = new AgregarActividadDTO
        {
            Peso = 70.5,
            SerieId = 1,
            EntrenamientoId = 1,
            EjercicioAsignadoId = 1
        };

        var actividadCreada = new ObtenerActividadDTO
        {
            Id = 1,
            Peso = 70.5,
            SerieId = 1,
            EntrenamientoId = 1,
            EjercicioAsignadoId = 1
        };

        _mockAgregar
            .Setup(casoDeUso => casoDeUso.Ejecutar(nuevaActividad))
            .ReturnsAsync(actividadCreada);

        // Act
        var resultado = await _controller.Crear(nuevaActividad);

        // Assert
        var createdAtActionResult = resultado as CreatedAtActionResult;
        createdAtActionResult.Should().NotBeNull();
        createdAtActionResult.StatusCode.Should().Be(201);
        createdAtActionResult.Value.Should().BeEquivalentTo(actividadCreada);
    }

    //Agregar_LanzaExcepcion_RetornaStatusCode500
    [Fact]
    public async Task Agregar_LanzaExcepcion_RetornaStatusCode500()
    {
        // Arrange
        var nuevaActividad = new AgregarActividadDTO
        {
            Peso = 70.5,
            SerieId = 1,
            EntrenamientoId = 1,
            EjercicioAsignadoId = 1
        };

        _mockAgregar
            .Setup(casoDeUso => casoDeUso.Ejecutar(nuevaActividad))
            .ThrowsAsync(new System.Exception("Error interno del servidor."));

        // Act
        var resultado = await _controller.Crear(nuevaActividad);

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
        AgregarActividadDTO nuevaActividad = null;

        // Act
        var resultado = await _controller.Crear(nuevaActividad);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.StatusCode.Should().Be(400);
        badRequestResult.Value.Should().Be("El objeto Actividad es nulo.");
    }

    //ObtenerTodos_RetornaOkResult_ConListaCompleta
    [Fact]
    public async Task ObtenerTodos_RetornaOkResult_ConListaCompleta()
    {
        // Arrange
        var listaActividades = new List<ObtenerActividadDTO>
        {
            new ObtenerActividadDTO { Id = 1, Peso = 70.5, SerieId = 1, EntrenamientoId = 1, EjercicioAsignadoId = 1 },
            new ObtenerActividadDTO { Id = 2, Peso = 75.0, SerieId = 2, EntrenamientoId = 1, EjercicioAsignadoId = 2 }
        };

        _mockObtenerTodos
            .Setup(casoDeUso => casoDeUso.Ejecutar())
            .ReturnsAsync(listaActividades);

        // Act
        var resultado = await _controller.ObtenerTodasLasActividades();

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(listaActividades);
    }

    //ObtenerTodos_RetornaOkResult_ConListaVacia
    [Fact]
    public async Task ObtenerTodos_RetornaOkResult_ConListaVacia()
    {
        // Arrange
        var listaActividades = new List<ObtenerActividadDTO>();

        _mockObtenerTodos
            .Setup(casoDeUso => casoDeUso.Ejecutar())
            .ReturnsAsync(listaActividades);

        // Act
        var resultado = await _controller.ObtenerTodasLasActividades();

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(listaActividades);
    }

    //ObtenerTodos_LanzaExcepcion_RetornaStatus500
    [Fact]
    public async Task ObtenerTodos_LanzaExcepcion_RetornaStatus500()
    {
        // Arrange
        _mockObtenerTodos
            .Setup(casoDeUso => casoDeUso.Ejecutar())
            .ThrowsAsync(new System.Exception("Error interno del servidor."));

        // Act
        var resultado = await _controller.ObtenerTodasLasActividades();

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
        int actividadId = 999;

        _mockObtenerPorId
            .Setup(casoDeUso => casoDeUso.Ejecutar(actividadId))
            .ReturnsAsync((ObtenerActividadDTO)null);

        // Act
        var resultado = await _controller.ObtenerActividadPorId(actividadId);

        // Assert
        var notFoundResult = resultado as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult.StatusCode.Should().Be(404);
        notFoundResult.Value.Should().Be($"La actividad con ID {actividadId} no existe.");
    }

    //ObtenerPorId_Existe_RetornaObjetoOkCreado
    [Fact]
    public async Task ObtenerPorId_Existe_RetornaObjetoOkCreado()
    {
        // Arrange
        int actividadId = 1;
        var actividadExistente = new ObtenerActividadDTO
        {
            Id = actividadId,
            Peso = 70.5,
            SerieId = 1,
            EntrenamientoId = 1,
            EjercicioAsignadoId = 1
        };

        _mockObtenerPorId
            .Setup(casoDeUso => casoDeUso.Ejecutar(actividadId))
            .ReturnsAsync(actividadExistente);

        // Act
        var resultado = await _controller.ObtenerActividadPorId(actividadId);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(actividadExistente);
    }

    //Actualizar_RetornaOkObjectResult_ConObjetoActualizado
    [Fact]
    public async Task Actualizar_RetornaOkObjectResult_ConObjetoActualizado()
    {
        // Arrange
        int actividadId = 1;
        var actividadActualizada = new ActualizarActividadDTO
        {
            Id = actividadId,
            Peso = 75.0,
            SerieId = 2,
            EntrenamientoId = 1,
            EjercicioAsignadoId = 2
        };

        var actividadResultado = new ObtenerActividadDTO
        {
            Id = actividadId,
            Peso = 75.0,
            SerieId = 2,
            EntrenamientoId = 1,
            EjercicioAsignadoId = 2
        };

        _mockActualizar
            .Setup(casoDeUso => casoDeUso.Ejecutar(actividadActualizada))
            .ReturnsAsync(actividadResultado);

        // Act
        var resultado = await _controller.Actualizar(actividadId, actividadActualizada);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(actividadResultado);
    }

    //Actualizar_NoEncontrado_RetornaNotFoundResult
    [Fact]
    public async Task Actualizar_NoEncontrado_RetornaNotFoundResult()
    {
        // Arrange
        int actividadId = 999;
        var actividadActualizada = new ActualizarActividadDTO
        {
            Id = actividadId,
            Peso = 75.0,
            SerieId = 2,
            EntrenamientoId = 1,
            EjercicioAsignadoId = 2
        };

        _mockActualizar
            .Setup(casoDeUso => casoDeUso.Ejecutar(actividadActualizada))
            .ReturnsAsync((ObtenerActividadDTO)null);

        // Act
        var resultado = await _controller.Actualizar(actividadId, actividadActualizada);

        // Assert
        var notFoundResult = resultado as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult.StatusCode.Should().Be(404);
        notFoundResult.Value.Should().Be($"La actividad con ID {actividadId} no existe.");
    }

    //Actualizar_LanzaExcepcion_RetornaStatusCode500
    [Fact]
    public async Task Actualizar_LanzaExcepcion_RetornaStatusCode500()
    {
        // Arrange
        int actividadId = 1;
        var actividadActualizada = new ActualizarActividadDTO
        {
            Id = actividadId,
            Peso = 75.0,
            SerieId = 2,
            EntrenamientoId = 1,
            EjercicioAsignadoId = 2
        };

        _mockActualizar
            .Setup(casoDeUso => casoDeUso.Ejecutar(actividadActualizada))
            .ThrowsAsync(new System.Exception("Error interno del servidor."));

        // Act
        var resultado = await _controller.Actualizar(actividadId, actividadActualizada);

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
        int actividadId = 1;
        var actividadActualizada = new ActualizarActividadDTO
        {
            Peso = 75.0,
            SerieId = 2,
            EntrenamientoId = 1,
            EjercicioAsignadoId = 2
        };

        // Act
        var resultado = await _controller.Actualizar(actividadId + 1, actividadActualizada);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.StatusCode.Should().Be(400);
        badRequestResult.Value.Should().Be("El ID de la ruta no coincide con el ID del objeto.");
    }

    //Actualizar_DTONulo_RetornaBadRequestResult
    [Fact]
    public async Task Actualizar_DTONulo_RetornaBadRequestResult()
    {
        // Arrange
        int actividadId = 1;
        ActualizarActividadDTO actividadActualizada = null;

        // Act
        var resultado = await _controller.Actualizar(actividadId, actividadActualizada);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.StatusCode.Should().Be(400);
        badRequestResult.Value.Should().Be("El objeto Actividad es nulo.");
    }

    //Eliminar_Existente_DeberiaRetornarNoContent
    [Fact]
    public async Task Eliminar_Existente_DeberiaRetornarNoContent()
    {
        // Arrange
        int actividadId = 1;

        _mockEliminar
            .Setup(casoDeUso => casoDeUso.Ejecutar(actividadId))
            .ReturnsAsync(true);

        // Act
        var resultado = await _controller.Eliminar(actividadId);

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
        int actividadId = 999;

        _mockEliminar
            .Setup(casoDeUso => casoDeUso.Ejecutar(actividadId))
            .ReturnsAsync(false);

        // Act
        var resultado = await _controller.Eliminar(actividadId);

        // Assert
        var notFoundResult = resultado as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult.StatusCode.Should().Be(404);
        notFoundResult.Value.Should().Be($"La actividad con ID {actividadId} no existe.");
    }

    //Eliminar_CuandoOcurreErrorEnServidor_DeberiaRetornarStatusCode500
    [Fact]
    public async Task Eliminar_CuandoOcurreErrorEnServidor_DeberiaRetornarStatusCode500()
    {
        // Arrange
        int actividadId = 1;

        _mockEliminar
            .Setup(casoDeUso => casoDeUso.Ejecutar(actividadId))
            .ThrowsAsync(new System.Exception("Error interno del servidor."));

        // Act
        var resultado = await _controller.Eliminar(actividadId);

        // Assert
        var objectResult = resultado as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult.StatusCode.Should().Be(500);
        objectResult.Value.Should().Be("Error interno del servidor.");
    }
}