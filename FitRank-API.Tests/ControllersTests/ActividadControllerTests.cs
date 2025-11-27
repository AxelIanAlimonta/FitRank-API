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
        _mockRegistrar = new Mock<RegistrarActividadCasoDeUso>(null, null, null, null, null, null);

        _controller = new ActividadController(
            _mockAgregar.Object,
            _mockObtenerTodos.Object,
            _mockObtenerPorId.Object,
            _mockActualizar.Object,
            _mockEliminar.Object,
            _mockRegistrar.Object
        );
    }

    #region Crear Tests

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
    }

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
    }

    [Fact]
    public async Task Agregar_RetornaBadRequest_CuandoModelStateEsInvalido()
    {
        // Arrange
        var nuevaActividad = new AgregarActividadDTO { Peso = 70.5 };
        _controller.ModelState.AddModelError("SerieId", "Requerido");

        // Act
        var resultado = await _controller.Crear(nuevaActividad);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.StatusCode.Should().Be(400);
    }

    #endregion

    #region ObtenerTodas Tests

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
    }

    #endregion

    #region ObtenerPorId Tests

    [Fact]
    public async Task ObtenerPorId_NoExiste_RetornaNotFound()
    {
        // Arrange
        long actividadId = 999;

        _mockObtenerPorId
            .Setup(casoDeUso => casoDeUso.Ejecutar(actividadId))
            .ReturnsAsync((ObtenerActividadDTO)null);

        // Act
        var resultado = await _controller.ObtenerActividadPorId(actividadId);

        // Assert
        var notFoundResult = resultado as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task ObtenerPorId_Existe_RetornaObjetoOkCreado()
    {
        // Arrange
        long actividadId = 1;
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

    [Fact]
    public async Task ObtenerPorId_IdCero_RetornaBadRequest()
    {
        // Act
        var resultado = await _controller.ObtenerActividadPorId(0);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerPorId_IdNegativo_RetornaBadRequest()
    {
        // Act
        var resultado = await _controller.ObtenerActividadPorId(-5);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerPorId_LanzaExcepcion_RetornaStatus500()
    {
        // Arrange
        long actividadId = 1;
        _mockObtenerPorId
            .Setup(casoDeUso => casoDeUso.Ejecutar(actividadId))
            .ThrowsAsync(new System.Exception("Error"));

        // Act
        var resultado = await _controller.ObtenerActividadPorId(actividadId);

        // Assert
        var objectResult = resultado as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult.StatusCode.Should().Be(500);
    }

    #endregion

    #region Actualizar Tests

    [Fact]
    public async Task Actualizar_RetornaOkObjectResult_ConObjetoActualizado()
    {
        // Arrange
        long actividadId = 1;
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

    [Fact]
    public async Task Actualizar_NoEncontrado_RetornaNotFoundResult()
    {
        // Arrange
        long actividadId = 999;
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
    }

    [Fact]
    public async Task Actualizar_LanzaExcepcion_RetornaStatusCode500()
    {
        // Arrange
        long actividadId = 1;
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
    }

    [Fact]
    public async Task Actualizar_IdNoCoincide_RetornaBadRequestResult()
    {
        // Arrange
        long actividadId = 1;
        var actividadActualizada = new ActualizarActividadDTO
        {
            Id = 2,
            Peso = 75.0,
            SerieId = 2,
            EntrenamientoId = 1,
            EjercicioAsignadoId = 2
        };

        // Act
        var resultado = await _controller.Actualizar(actividadId, actividadActualizada);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Actualizar_DTONulo_RetornaBadRequestResult()
    {
        // Arrange
        long actividadId = 1;
        ActualizarActividadDTO actividadActualizada = null;

        // Act
        var resultado = await _controller.Actualizar(actividadId, actividadActualizada);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Actualizar_IdCero_RetornaBadRequest()
    {
        // Arrange
        var dto = new ActualizarActividadDTO { Id = 0 };

        // Act
        var resultado = await _controller.Actualizar(0, dto);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Actualizar_IdNegativo_RetornaBadRequest()
    {
        // Arrange
        var dto = new ActualizarActividadDTO { Id = -1 };

        // Act
        var resultado = await _controller.Actualizar(-1, dto);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Actualizar_ModelStateInvalido_RetornaBadRequest()
    {
        // Arrange
        long actividadId = 1;
        var dto = new ActualizarActividadDTO { Id = actividadId };
        _controller.ModelState.AddModelError("Peso", "Requerido");

        // Act
        var resultado = await _controller.Actualizar(actividadId, dto);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.StatusCode.Should().Be(400);
    }

    #endregion

    #region Eliminar Tests

    [Fact]
    public async Task Eliminar_Existente_DeberiaRetornarNoContent()
    {
        // Arrange
        long actividadId = 1;

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

    [Fact]
    public async Task Eliminar_NoExistente_DeberiaRetornarNotFound()
    {
        // Arrange
        long actividadId = 999;

        _mockEliminar
            .Setup(casoDeUso => casoDeUso.Ejecutar(actividadId))
            .ReturnsAsync(false);

        // Act
        var resultado = await _controller.Eliminar(actividadId);

        // Assert
        var notFoundResult = resultado as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Eliminar_CuandoOcurreErrorEnServidor_DeberiaRetornarStatusCode500()
    {
        // Arrange
        long actividadId = 1;

        _mockEliminar
            .Setup(casoDeUso => casoDeUso.Ejecutar(actividadId))
            .ThrowsAsync(new System.Exception("Error interno del servidor."));

        // Act
        var resultado = await _controller.Eliminar(actividadId);

        // Assert
        var objectResult = resultado as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task Eliminar_IdCero_RetornaBadRequest()
    {
        // Act
        var resultado = await _controller.Eliminar(0);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Eliminar_IdNegativo_RetornaBadRequest()
    {
        // Act
        var resultado = await _controller.Eliminar(-10);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.StatusCode.Should().Be(400);
    }

    #endregion

    #region RegistrarActividad Tests

    [Fact]
    public async Task RegistrarActividad_Exitoso_RetornaOk()
    {
        // Arrange
        var dto = new RegistrarActividadDTO
        {
            SerieId = 1,
            NumeroSerie = 1,
            Repeticiones = 10,
            Peso = 50.0
        };

        var actividad = new Actividad
        {
            Id = 1,
            SerieId = 1,
            EntrenamientoId = 1,
            Repeticiones = 10,
            Peso = 50.0,
            EjercicioAsignadoId = 1,
            Punto = 100.0
        };

        _mockRegistrar
            .Setup(c => c.Ejecutar(dto))
            .ReturnsAsync(actividad);

        // Act
        var resultado = await _controller.RegistrarActividad(dto);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task RegistrarActividad_DTONulo_RetornaBadRequest()
    {
        // Act
        var resultado = await _controller.RegistrarActividad(null);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task RegistrarActividad_ModelStateInvalido_RetornaBadRequest()
    {
        // Arrange
        var dto = new RegistrarActividadDTO();
        _controller.ModelState.AddModelError("SerieId", "Requerido");

        // Act
        var resultado = await _controller.RegistrarActividad(dto);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task RegistrarActividad_LanzaExcepcion_RetornaBadRequest()
    {
        // Arrange
        var dto = new RegistrarActividadDTO
        {
            SerieId = 1,
            NumeroSerie = 1,
            Repeticiones = 10,
            Peso = 50.0
        };

        _mockRegistrar
            .Setup(c => c.Ejecutar(dto))
            .ThrowsAsync(new Exception("Error en el registro"));

        // Act
        var resultado = await _controller.RegistrarActividad(dto);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.StatusCode.Should().Be(400);
    }

    #endregion
}