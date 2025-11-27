//using Xunit;
//using Moq;
//using FluentAssertions;
//using Microsoft.AspNetCore.Mvc;
//using FitRank_API.Presentacion.Controllers;
//using FitRank_API.Application.DTOs.GrupoMuscularDTOs;
//using AutoMapper;
//using FitRank_API.Domain.Interfaces;
//using FitRank_API.Application.DTOs;
//using FitRank_API.Application.CasosDeUso.RutinaCasosDeUso;
//using FitRank_API.Application.DTOs.RutinaDTOs;
//using FitRank.API.Application.Rutinas.Abstractions;

//namespace FitRank_API.tests.ControllersTests;

//public class RutinaControllerTests
//{
//    private readonly RutinaController _controller;
//    private readonly Mock<ActualizarRutinaCasoDeUso> _mockActualizar;
//    private readonly Mock<AgregarRutinaCasoDeUso> _mockAgregar;
//    private readonly Mock<EliminarRutinaCasoDeUso> _mockEliminar;
//    private readonly Mock<ObtenerRutinaPorIdCasoDeUso> _mockObtenerPorId;
//    private readonly Mock<ObtenerTodasLasRutinasCasoDeUso> _mockObtenerTodos;
//    private readonly Mock<ObtenerRutinaCompletaCasoDeUso> _mockObtenerCompleta;
//    private readonly Mock<GenerarRutinaIACasoDeUso> _mockGenerarRutinaIA;
//    private readonly Mock<ConfirmarRutinaIACasoDeUso> _mockConfirmarRutinaIA;
//    private readonly Mock<CambiarEstadoRutinaCasoDeUso> _mockCambiarEstadorutina;
//    private readonly Mock<MarcarDesmarcarRutinaFavoritaCasoDeUso> _mockMarcarDesmarcarRutinaFavoritaCasoDeUso;
//    private readonly Mock<ObtenerRutinasFavoritasCasoDeUso> _mockObtenerRutinasFavoritasCasoDeUso;


//    public RutinaControllerTests()
//    {
//        var mockRepositorio = new Mock<IRutinaRepositorio>();
//        var mockMapper = new Mock<IMapper>();
//        var mockRulesRunner = new Mock<IRoutineRulesRunner>();
//        var mockRoutineBuilder = new Mock<IRoutineBuilder>();

//        _mockActualizar = new Mock<ActualizarRutinaCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
//        _mockAgregar = new Mock<AgregarRutinaCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
//        _mockEliminar = new Mock<EliminarRutinaCasoDeUso>(mockRepositorio.Object);
//        _mockObtenerPorId = new Mock<ObtenerRutinaPorIdCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
//        _mockObtenerTodos = new Mock<ObtenerTodasLasRutinasCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
//        _mockGenerarRutinaIA = new Mock<GenerarRutinaIACasoDeUso>(mockRulesRunner.Object, mockRoutineBuilder.Object);
//        _mockConfirmarRutinaIA = new Mock<ConfirmarRutinaIACasoDeUso>(mockRepositorio.Object);

//        _mockObtenerCompleta = new Mock<ObtenerRutinaCompletaCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
//        _mockCambiarEstadorutina = new Mock<CambiarEstadoRutinaCasoDeUso>(mockRepositorio.Object);
//        _mockMarcarDesmarcarRutinaFavoritaCasoDeUso = new Mock<MarcarDesmarcarRutinaFavoritaCasoDeUso>(mockRepositorio.Object);
//        _mockObtenerRutinasFavoritasCasoDeUso = new Mock<ObtenerRutinasFavoritasCasoDeUso>(mockRepositorio.Object, mockMapper.Object);


//        _controller = new RutinaController(
//            _mockAgregar.Object,
//            _mockObtenerPorId.Object,
//            _mockActualizar.Object,
//            _mockObtenerTodos.Object,
//            _mockEliminar.Object,
//            _mockGenerarRutinaIA.Object,
//            _mockConfirmarRutinaIA.Object,
//            _mockObtenerCompleta.Object,
//            _mockMarcarDesmarcarRutinaFavoritaCasoDeUso.Object,
//            _mockCambiarEstadorutina.Object,
//            _mockObtenerRutinasFavoritasCasoDeUso.Object
//        );
//    }

//    [Fact]
//    public async Task AgregarRutina_Retorna_CreatedAtActionResult()
//    {
//        // Arrange
//        var nuevaRutinaDTO = new AgregarRutinaDTO { Nombre = "Rutina A" };
//        var obtenerRutina = new ObtenerRutinaDTO { Id = 1, Nombre = "Rutina A" };

//        _mockAgregar
//            .Setup(c => c.Ejecutar(nuevaRutinaDTO))
//            .ReturnsAsync(obtenerRutina);

//        // Act
//        var resultado = await _controller.Agregar(nuevaRutinaDTO);

//        // Assert
//        var createdAtActionResult = resultado as CreatedAtActionResult;
//        createdAtActionResult.Should().NotBeNull();
//        createdAtActionResult!.StatusCode.Should().Be(201);
//        createdAtActionResult.Value.Should().BeEquivalentTo(obtenerRutina);

//    }

//    [Fact]
//    public async Task Agregar_LanzaExcepcion_RetornaStatusCode500()
//    {
//        // Arrange
//        var nuevaRutinaDTO = new AgregarRutinaDTO { Nombre = "Rutina A" };

//        _mockAgregar
//            .Setup(c => c.Ejecutar(nuevaRutinaDTO))
//            .ThrowsAsync(new Exception("Error al agregar rutina"));

//        // Act
//        var resultado = await _controller.Agregar(nuevaRutinaDTO);

//        // Assert
//        var objectResult = resultado as ObjectResult;
//        objectResult.Should().NotBeNull();
//        objectResult!.StatusCode.Should().Be(500);
//        objectResult.Value.Should().Be("Error al agregar rutina");
//    }

//    [Fact]
//    public async Task Agregar_RetornaBadRequest_CuandoDTOEsNulo()
//    {
//        // Act
//        var resultado = await _controller.Agregar(null);

//        // Assert
//        var badRequestResult = resultado as BadRequestObjectResult;
//        badRequestResult.Should().NotBeNull();
//        badRequestResult!.StatusCode.Should().Be(400);
//        badRequestResult.Value.Should().Be("El objeto rutina no puede ser nulo.");
//    }

//    [Fact]
//    public async Task ObtenerTodos_RetornaOkResult_ConListaDeRutinas()
//    {
//        // Arrange
//        var listaRutinas = new List<ObtenerRutinaDTO>
//        {
//            new ObtenerRutinaDTO { Id = 1, Nombre = "Rutina A" },
//            new ObtenerRutinaDTO { Id = 2, Nombre = "Rutina B" }
//        };

//        _mockObtenerTodos
//            .Setup(c => c.Ejecutar())
//            .ReturnsAsync(listaRutinas);

//        // Act
//        var resultado = await _controller.ObtenerTodo();

//        // Assert
//        var okResult = resultado as OkObjectResult;
//        okResult.Should().NotBeNull();
//        okResult!.StatusCode.Should().Be(200);
//        okResult.Value.Should().BeEquivalentTo(listaRutinas);
//    }

//    [Fact]
//    public async Task ObtenerTodos_RetornaOkResult_ConListaVacia()
//    {
//        // Arrange
//        var listaRutinas = new List<ObtenerRutinaDTO>();

//        _mockObtenerTodos
//            .Setup(c => c.Ejecutar())
//            .ReturnsAsync(listaRutinas);

//        // Act
//        var resultado = await _controller.ObtenerTodo();

//        // Assert
//        var okResult = resultado as OkObjectResult;
//        okResult.Should().NotBeNull();
//        okResult!.StatusCode.Should().Be(200);
//        okResult.Value.Should().BeEquivalentTo(listaRutinas);
//    }

//    //ObtenerTodos_LanzaExcepcion_RetornaStatus500
//    [Fact]
//    public async Task ObtenerTodos_LanzaExcepcion_RetornaStatus500()
//    {
//        // Arrange
//        _mockObtenerTodos
//            .Setup(c => c.Ejecutar())
//            .ThrowsAsync(new Exception("Error al obtener rutinas"));

//        // Act
//        var resultado = await _controller.ObtenerTodo();

//        // Assert
//        var objectResult = resultado as ObjectResult;
//        objectResult.Should().NotBeNull();
//        objectResult!.StatusCode.Should().Be(500);
//        objectResult.Value.Should().Be("Error al obtener rutinas");
//    }

//    [Fact]
//    public async Task ObtenerPorId_LanzaExcepcion_RetornaStatusCode500()
//    {
//        // Arrange
//        int rutinaId = 1;

//        _mockObtenerPorId
//            .Setup(c => c.Ejecutar(rutinaId))
//            .ThrowsAsync(new Exception("Error al obtener rutina"));

//        // Act
//        var resultado = await _controller.ObtenerPorId(rutinaId);

//        // Assert
//        var objectResult = resultado as ObjectResult;
//        objectResult.Should().NotBeNull();
//        objectResult!.StatusCode.Should().Be(500);
//        objectResult.Value.Should().Be("Error al obtener rutina");
//    }


//    [Fact]
//    public async Task ObtenerPorId_RutinaNoExiste_RetornaNotFound()
//    {
//        // Arrange
//        int rutinaId = 1;

//        _mockObtenerPorId
//            .Setup(c => c.Ejecutar(rutinaId))
//            .ReturnsAsync((ObtenerRutinaDTO?)null);

//        // Act
//        var resultado = await _controller.ObtenerPorId(rutinaId);

//        // Assert
//        var notFoundResult = resultado as NotFoundObjectResult;
//        notFoundResult.Should().NotBeNull();
//        notFoundResult!.StatusCode.Should().Be(404);
//        notFoundResult.Value.Should().Be($"La rutina con ID {rutinaId} no fue encontrada.");
//    }



//    [Fact]
//    public async Task ObtenerPorId_RutinaExiste_RetornaOkConRutina()
//    {
//        // Arrange
//        int rutinaId = 1;
//        var rutinaDTO = new ObtenerRutinaDTO { Id = rutinaId, Nombre = "Rutina A" };

//        _mockObtenerPorId
//            .Setup(c => c.Ejecutar(rutinaId))
//            .ReturnsAsync(rutinaDTO);

//        // Act
//        var resultado = await _controller.ObtenerPorId(rutinaId);

//        // Assert
//        var okResult = resultado as OkObjectResult;
//        okResult.Should().NotBeNull();
//        okResult!.StatusCode.Should().Be(200);
//        okResult.Value.Should().BeEquivalentTo(rutinaDTO);
//    }

//    [Fact]
//    public async Task Actualizar_RetornaOkObjectResult_ConRutinaActualizada()
//    {
//        // Arrange
//        var rutinaId = 1;
//        var actualizarRutinaDTO = new ActualizarRutinaDTO { Id = rutinaId, Nombre = "Rutina Actualizada" };
//        var rutinaActualizadaDTO = new ObtenerRutinaDTO { Id = rutinaId, Nombre = "Rutina Actualizada" };

//        _mockActualizar
//            .Setup(c => c.Ejecutar(actualizarRutinaDTO))
//            .ReturnsAsync(rutinaActualizadaDTO);

//        // Act
//        var resultado = await _controller.Actualizar(rutinaId, actualizarRutinaDTO);

//        // Assert
//        var okResult = resultado as OkObjectResult;
//        okResult.Should().NotBeNull();
//        okResult!.StatusCode.Should().Be(200);
//        okResult.Value.Should().BeEquivalentTo(rutinaActualizadaDTO);
//    }

//    //Actualizar_GrupoMuscularNoEncontrado_RetornaNotFoundResult
//    [Fact]
//    public async Task Actualizar_RutinaNoEncontrada_RetornaNotFoundResult()
//    {
//        // Arrange
//        var rutinaId = 1;
//        var actualizarRutinaDTO = new ActualizarRutinaDTO { Id = rutinaId, Nombre = "Rutina Actualizada" };

//        _mockActualizar
//            .Setup(c => c.Ejecutar(actualizarRutinaDTO))
//            .ReturnsAsync((ObtenerRutinaDTO?)null);

//        // Act
//        var resultado = await _controller.Actualizar(rutinaId, actualizarRutinaDTO);

//        // Assert
//        var notFoundResult = resultado as NotFoundObjectResult;
//        notFoundResult.Should().NotBeNull();
//        notFoundResult!.StatusCode.Should().Be(404);
//        notFoundResult.Value.Should().Be($"La rutina con ID {rutinaId} no fue encontrada.");
//    }


//    [Fact]
//    public async Task Actualizar_LanzaExcepcion_RetornaStatusCode500()
//    {
//        // Arrange
//        var rutinaId = 1;
//        var actualizarRutinaDTO = new ActualizarRutinaDTO { Id = rutinaId, Nombre = "Rutina Actualizada" };

//        _mockActualizar
//            .Setup(c => c.Ejecutar(actualizarRutinaDTO))
//            .ThrowsAsync(new Exception("Error al actualizar rutina"));

//        // Act
//        var resultado = await _controller.Actualizar(rutinaId, actualizarRutinaDTO);

//        // Assert
//        var objectResult = resultado as ObjectResult;
//        objectResult.Should().NotBeNull();
//        objectResult!.StatusCode.Should().Be(500);
//        objectResult.Value.Should().Be("Error al actualizar rutina");
//    }

//    //Actualizar_IdNoCoincide_RetornaBadRequestResult
//    [Fact]
//    public async Task Actualizar_IdNoCoincide_RetornaBadRequestResult()
//    {
//        // Arrange
//        var rutinaId = 1;
//        var actualizarRutinaDTO = new ActualizarRutinaDTO { Id = rutinaId, Nombre = "Rutina Actualizada" };

//        // Act
//        var resultado = await _controller.Actualizar(999, actualizarRutinaDTO);

//        // Assert
//        var badRequestResult = resultado as BadRequestObjectResult;
//        badRequestResult.Should().NotBeNull();
//        badRequestResult!.StatusCode.Should().Be(400);
//        badRequestResult.Value.Should().Be("El ID de la ruta no coincide con el ID del objeto rutina.");
//    }

//    //Actualizar_DTONulo_RetornaBadRequestResult
//    [Fact]
//    public async Task Actualizar_DTONulo_RetornaBadRequestResult()
//    {
//        // Arrange
//        var rutinaId = 1;

//        // Act
//        var resultado = await _controller.Actualizar(rutinaId, null);

//        // Assert
//        var badRequestResult = resultado as BadRequestObjectResult;
//        badRequestResult.Should().NotBeNull();
//        badRequestResult!.StatusCode.Should().Be(400);
//        badRequestResult.Value.Should().Be("El objeto rutina no puede ser nulo.");
//    }


//    [Fact]
//    public async Task Eliminar_Rutina_Existente_Deberia_Retornar_NoContent()
//    {
//        // Arrange
//        var rutinaId = 1;

//        _mockEliminar
//            .Setup(c => c.Ejecutar(rutinaId))
//            .ReturnsAsync(true);

//        // Act
//        var resultado = await _controller.Eliminar(rutinaId);

//        // Assert
//        var noContentResult = resultado as NoContentResult;
//        noContentResult.Should().NotBeNull();
//        noContentResult!.StatusCode.Should().Be(204);
//    }

//    //Eliminar_GrupoMuscular_NoExistente_Deberia_Retornar_NotFound
//    [Fact]
//    public async Task Eliminar_Rutina_NoExistente_Deberia_Retornar_NotFound()
//    {
//        // Arrange
//        var rutinaId = 1;

//        _mockEliminar
//            .Setup(c => c.Ejecutar(rutinaId))
//            .ReturnsAsync(false);

//        // Act
//        var resultado = await _controller.Eliminar(rutinaId);

//        // Assert
//        var notFoundResult = resultado as NotFoundObjectResult;
//        notFoundResult.Should().NotBeNull();
//        notFoundResult!.StatusCode.Should().Be(404);
//        notFoundResult.Value.Should().Be($"La rutina con ID {rutinaId} no fue encontrada.");
//    }

//    [Fact]
//    public async Task Eliminar_Cuando_Ocurre_Error_Deberia_Retornar_StatusCode500()
//    {
//        // Arrange
//        var rutinaId = 1;

//        _mockEliminar
//            .Setup(c => c.Ejecutar(rutinaId))
//            .ThrowsAsync(new Exception("Error al eliminar rutina"));

//        // Act
//        var resultado = await _controller.Eliminar(rutinaId);

//        // Assert
//        var objectResult = resultado as ObjectResult;
//        objectResult.Should().NotBeNull();
//        objectResult!.StatusCode.Should().Be(500);
//        objectResult.Value.Should().Be("Error al eliminar rutina");
//    }
//}