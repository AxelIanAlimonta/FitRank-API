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
using FitRank_API.Application.CasosDeUso.EjercicioCasosDeUso;
using FitRank_API.Application.DTOs.EjercicioDTOs.AgregarEjercicioDTO;
using FitRank_API.Application.DTOs.EjercicioDTOs.ObtenerEjercicioDTO;
using FitRank_API.Application.DTOs.EjercicioDTOs.ActualizarEjercicioDTO;

namespace FitRank_API.tests.ControllersTests;

public class EjercicioControllerTests
{

    private readonly EjercicioController _controller;
    private readonly Mock<ActualizarEjercicioCasoDeUso> _mockActualizar;
    private readonly Mock<AgregarEjercicioCasoDeUso> _mockAgregar;
    private readonly Mock<EliminarEjercicioCasoDeUso> _mockEliminar;
    private readonly Mock<ObtenerEjercicioPorIdCasoDeUso> _mockObtenerPorId;
    private readonly Mock<ObtenerEjerciciosCasoDeUso> _mockObtenerTodos;

    public EjercicioControllerTests()
    {
        var mockRepositorio = new Mock<IEjercicioRepositorio>();
        var mockMapper = new Mock<IMapper>();

        _mockActualizar = new Mock<ActualizarEjercicioCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockAgregar = new Mock<AgregarEjercicioCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockEliminar = new Mock<EliminarEjercicioCasoDeUso>(mockRepositorio.Object);
        _mockObtenerPorId = new Mock<ObtenerEjercicioPorIdCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockObtenerTodos = new Mock<ObtenerEjerciciosCasoDeUso>(mockRepositorio.Object, mockMapper.Object);

        _controller = new EjercicioController(
            _mockObtenerTodos.Object,
            _mockObtenerPorId.Object,
            _mockAgregar.Object,
            _mockActualizar.Object,
            _mockEliminar.Object
        );

    }

    //Agregar_RetornaCreatedAtActionResult
    [Fact]
    public async Task Agregar_RetornaCreatedAtActionResult()
    {
        // Arrange
        var nuevoEjercicioDTO = new AgregarEjercicioDTO { Nombre = "Ejercicio de prueba", ContraIndicaciones = new List<string>(), Tags = new List<string>() };
        var ejercicioDevuelto = new ObtenerEjercicioDTO { Id = 1, Nombre = "Ejercicio de prueba" };
        _mockAgregar.Setup(caso => caso.Ejecutar(It.IsAny<AgregarEjercicioDTO>()))
            .ReturnsAsync(ejercicioDevuelto);

        // Act
        var resultado = await _controller.AgregarEjercicio(nuevoEjercicioDTO);

        // Assert
        var createdAtActionResult = resultado as CreatedAtActionResult;
        createdAtActionResult.Should().NotBeNull();
        createdAtActionResult.StatusCode.Should().Be(201);
        createdAtActionResult.Value.Should().BeEquivalentTo(nuevoEjercicioDTO);

    }

    //Agregar_LanzaExcepcion_RetornaStatusCode500
    [Fact]
    public async Task Agregar_LanzaExcepcion_RetornaStatusCode500()
    {
        // Arrange
        var nuevoEjercicioDTO = new AgregarEjercicioDTO { Nombre = "Ejercicio de prueba" };
        _mockAgregar.Setup(caso => caso.Ejecutar(It.IsAny<AgregarEjercicioDTO>()))
            .ThrowsAsync(new Exception("Error al agregar ejercicio"));

        // Act
        var resultado = await _controller.AgregarEjercicio(nuevoEjercicioDTO);

        // Assert
        var objectResult = resultado as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult.StatusCode.Should().Be(500);
        objectResult.Value.Should().Be("Error al agregar ejercicio");
    }

    //Agregar_RetornaBadRequest_CuandoDTOEsNulo
    [Fact]
    public async Task Agregar_RetornaBadRequest_CuandoDTOEsNulo()
    {
        // Arrange
        AgregarEjercicioDTO nuevoEjercicioDTO = null;

        // Act
        var resultado = await _controller.AgregarEjercicio(nuevoEjercicioDTO);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.StatusCode.Should().Be(400);
        badRequestResult.Value.Should().Be("El ejercicio no puede ser nulo.");
    }

    //ObtenerTodos_RetornaOkResult_ConListaCompleta
    [Fact]
    public async Task ObtenerTodos_RetornaOkResult_ConListaCompleta()
    {
        // Arrange
        var listaEjerciciosDTO = new List<ObtenerEjercicioDTO>
        {
            new ObtenerEjercicioDTO { Id = 1, Nombre = "Ejercicio 1" },
            new ObtenerEjercicioDTO { Id = 2, Nombre = "Ejercicio 2" }
        };

        _mockObtenerTodos.Setup(caso => caso.EjecutarAsync())
            .ReturnsAsync(listaEjerciciosDTO);

        // Act
        var resultado = await _controller.GetEjercicios();

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(listaEjerciciosDTO);
    }

    //ObtenerTodos_RetornaOkResult_ConListaVacia
    [Fact]
    public async Task ObtenerTodos_RetornaOkResult_ConListaVacia()
    {
        // Arrange
        var listaEjerciciosDTO = new List<ObtenerEjercicioDTO>();

        _mockObtenerTodos.Setup(caso => caso.EjecutarAsync())
            .ReturnsAsync(listaEjerciciosDTO);

        // Act
        var resultado = await _controller.GetEjercicios();

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(listaEjerciciosDTO);
    }

    //ObtenerTodos_LanzaExcepcion_RetornaStatus500
    [Fact]
    public async Task ObtenerTodos_LanzaExcepcion_RetornaStatus500()
    {
        // Arrange
        _mockObtenerTodos.Setup(caso => caso.EjecutarAsync())
            .ThrowsAsync(new Exception("Error al obtener ejercicios"));

        // Act
        var resultado = await _controller.GetEjercicios();

        // Assert
        var objectResult = resultado as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult.StatusCode.Should().Be(500);
        objectResult.Value.Should().Be("Error al obtener ejercicios");
    }

    //ObtenerPorId_NoExiste_RetornaNotFound
    [Fact]
    public async Task ObtenerPorId_NoExiste_RetornaNotFound()
    {
        // Arrange
        int ejercicioId = 99;
        _mockObtenerPorId.Setup(caso => caso.Ejecutar(ejercicioId))
            .ReturnsAsync((ObtenerEjercicioDTO)null);

        // Act
        var resultado = await _controller.GetEjercicioPorId(ejercicioId);

        // Assert
        var notFoundResult = resultado as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult.StatusCode.Should().Be(404);
        notFoundResult.Value.Should().Be($"El ejercicio con ID {ejercicioId} no fue encontrado.");
    }

    //ObtenerPorId_Existe_RetornaObjetoOkCreado
    [Fact]
    public async Task ObtenerPorId_Existe_RetornaObjetoOkCreado()
    {
        // Arrange
        int ejercicioId = 1;
        var ejercicioDTO = new ObtenerEjercicioDTO { Id = ejercicioId, Nombre = "Ejercicio Existente" };
        _mockObtenerPorId.Setup(caso => caso.Ejecutar(ejercicioId))
            .ReturnsAsync(ejercicioDTO);

        // Act
        var resultado = await _controller.GetEjercicioPorId(ejercicioId);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(ejercicioDTO);
    }

    //Actualizar_RetornaOkObjectResult_ConObjetoActualizado
    [Fact]
    public async Task Actualizar_RetornaOkObjectResult_ConObjetoActualizado()
    {
        // Arrange
        var ejercicioActualizarDTO = new ActualizarEjercicioDTO { Id = 1, Nombre = "Ejercicio Actualizado" };
        var ejercicioActualizadoDTO = new ObtenerEjercicioDTO { Id = 1, Nombre = "Ejercicio Actualizado" };

        _mockActualizar.Setup(caso => caso.Ejecutar(It.IsAny<ActualizarEjercicioDTO>()))
            .ReturnsAsync(ejercicioActualizadoDTO);

        // Act
        var resultado = await _controller.ActualizarEjercicio(1, ejercicioActualizarDTO);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(ejercicioActualizadoDTO);
    }

    //Actualizar_NoEncontrado_RetornaNotFoundResult
    [Fact]
    public async Task Actualizar_NoEncontrado_RetornaNotFoundResult()
    {
        // Arrange
        var ejercicioActualizarDTO = new ActualizarEjercicioDTO { Id = 99, Nombre = "Ejercicio No Existente" };

        _mockActualizar.Setup(caso => caso.Ejecutar(It.IsAny<ActualizarEjercicioDTO>()))
            .ReturnsAsync((ObtenerEjercicioDTO)null);

        // Act
        var resultado = await _controller.ActualizarEjercicio(99, ejercicioActualizarDTO);

        // Assert
        var notFoundResult = resultado as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult.StatusCode.Should().Be(404);
        notFoundResult.Value.Should().Be("El ejercicio con ID 99 no fue encontrado para actualizar.");
    }

    //Actualizar_LanzaExcepcion_RetornaStatusCode500
    [Fact]
    public async Task Actualizar_LanzaExcepcion_RetornaStatusCode500()
    {
        // Arrange
        var ejercicioActualizarDTO = new ActualizarEjercicioDTO { Id = 1, Nombre = "Ejercicio Actualizado" };

        _mockActualizar.Setup(caso => caso.Ejecutar(It.IsAny<ActualizarEjercicioDTO>()))
            .ThrowsAsync(new Exception("Error al actualizar ejercicio"));

        // Act
        var resultado = await _controller.ActualizarEjercicio(1, ejercicioActualizarDTO);

        // Assert
        var objectResult = resultado as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult.StatusCode.Should().Be(500);
        objectResult.Value.Should().Be("Error al actualizar ejercicio");
    }

    //Actualizar_IdNoCoincide_RetornaBadRequestResult
    [Fact]
    public async Task Actualizar_IdNoCoincide_RetornaBadRequestResult()
    {
        // Arrange
        var ejercicioActualizarDTO = new ActualizarEjercicioDTO { Id = 2, Nombre = "Ejercicio Actualizado" };

        // Act
        var resultado = await _controller.ActualizarEjercicio(1, ejercicioActualizarDTO);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.StatusCode.Should().Be(400);
        badRequestResult.Value.Should().Be("El ID del ejercicio no coincide con el ID proporcionado en la ruta.");
    }

    //Actualizar_DTONulo_RetornaBadRequestResult
    [Fact]
    public async Task Actualizar_DTONulo_RetornaBadRequestResult()
    {
        // Arrange
        ActualizarEjercicioDTO ejercicioActualizarDTO = null;

        // Act
        var resultado = await _controller.ActualizarEjercicio(1, ejercicioActualizarDTO);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.StatusCode.Should().Be(400);
        badRequestResult.Value.Should().Be("El ejercicio no puede ser nulo.");
    }


    //Eliminar_Existente_DeberiaRetornarNoContent
    [Fact]
    public async Task Eliminar_Existente_DeberiaRetornarNoContent()
    {
        // Arrange
        int ejercicioId = 1;
        _mockEliminar.Setup(caso => caso.Ejecutar(ejercicioId))
            .ReturnsAsync(true);

        // Act
        var resultado = await _controller.EliminarEjercicio(ejercicioId);

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
        int ejercicioId = 99;
        _mockEliminar.Setup(caso => caso.Ejecutar(ejercicioId))
            .ReturnsAsync(false);

        // Act
        var resultado = await _controller.EliminarEjercicio(ejercicioId);

        // Assert
        var notFoundResult = resultado as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult.StatusCode.Should().Be(404);
        notFoundResult.Value.Should().Be($"El ejercicio con ID {ejercicioId} no fue encontrado para eliminar.");
    }

    //Eliminar_CuandoOcurreErrorEnServidor_DeberiaRetornarStatusCode500
    [Fact]
    public async Task Eliminar_CuandoOcurreErrorEnServidor_DeberiaRetornarStatusCode500()
    {
        // Arrange
        int ejercicioId = 1;
        _mockEliminar.Setup(caso => caso.Ejecutar(ejercicioId))
            .ThrowsAsync(new Exception("Error al eliminar ejercicio"));

        // Act
        var resultado = await _controller.EliminarEjercicio(ejercicioId);

        // Assert
        var objectResult = resultado as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult.StatusCode.Should().Be(500);
        objectResult.Value.Should().Be("Error al eliminar ejercicio");
    }

}