using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using FitRank_API.Presentacion.Controllers;
using AutoMapper;
using FitRank_API.Domain.Interfaces;
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
    private readonly Mock<ObtenerEjerciciosPorGrupoMuscularCasoDeUso> _mockObtenerEjerciciosPorGrupoMuscularCasoDeUso;

    public EjercicioControllerTests()
    {
        var mockRepositorio = new Mock<IEjercicioRepositorio>();
        var mockMapper = new Mock<IMapper>();

        _mockActualizar = new Mock<ActualizarEjercicioCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockAgregar = new Mock<AgregarEjercicioCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockEliminar = new Mock<EliminarEjercicioCasoDeUso>(mockRepositorio.Object);
        _mockObtenerPorId = new Mock<ObtenerEjercicioPorIdCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockObtenerTodos = new Mock<ObtenerEjerciciosCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockObtenerEjerciciosPorGrupoMuscularCasoDeUso = new Mock<ObtenerEjerciciosPorGrupoMuscularCasoDeUso>(mockRepositorio.Object, mockMapper.Object);

        _controller = new EjercicioController(
            _mockObtenerTodos.Object,
            _mockObtenerPorId.Object,
            _mockAgregar.Object,
            _mockActualizar.Object,
            _mockEliminar.Object,
            _mockObtenerEjerciciosPorGrupoMuscularCasoDeUso.Object
        );
    }

    #region GetEjercicios Tests

    [Fact]
    public async Task ObtenerTodos_RetornaOkResult_ConListaCompleta()
    {
        // Arrange
        var listaEjerciciosDTO = new List<ObtenerEjercicioDTO>
        {
            new ObtenerEjercicioDTO { Id = 1, Nombre = "Ejercicio 1" },
            new ObtenerEjercicioDTO { Id = 2, Nombre = "Ejercicio 2" }
        };

        _mockObtenerTodos.Setup(caso => caso.Ejecutar())
            .ReturnsAsync(listaEjerciciosDTO);

        // Act
        var resultado = await _controller.GetEjercicios();

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(listaEjerciciosDTO);
    }

    [Fact]
    public async Task ObtenerTodos_RetornaOkResult_ConListaVacia()
    {
        // Arrange
        var listaEjerciciosDTO = new List<ObtenerEjercicioDTO>();

        _mockObtenerTodos.Setup(caso => caso.Ejecutar())
            .ReturnsAsync(listaEjerciciosDTO);

        // Act
        var resultado = await _controller.GetEjercicios();

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(listaEjerciciosDTO);
    }

    [Fact]
    public async Task ObtenerTodos_LanzaExcepcion_RetornaStatus500()
    {
        // Arrange
        _mockObtenerTodos.Setup(caso => caso.Ejecutar())
            .ThrowsAsync(new Exception("Error al obtener ejercicios"));

        // Act
        var resultado = await _controller.GetEjercicios();

        // Assert
        var objectResult = resultado as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult.StatusCode.Should().Be(500);
    }

    #endregion

    #region GetEjercicioPorId Tests

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

    [Fact]
    public async Task ObtenerPorId_IdCero_RetornaBadRequest()
    {
        // Act
        var resultado = await _controller.GetEjercicioPorId(0);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerPorId_IdNegativo_RetornaBadRequest()
    {
        // Act
        var resultado = await _controller.GetEjercicioPorId(-5);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.StatusCode.Should().Be(400);
    }

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
    }

    [Fact]
    public async Task ObtenerPorId_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        _mockObtenerPorId.Setup(x => x.Ejecutar(1)).ThrowsAsync(new Exception());

        // Act
        var resultado = await _controller.GetEjercicioPorId(1);

        // Assert
        var statusCodeResult = resultado as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult.StatusCode.Should().Be(500);
    }

    #endregion

    #region AgregarEjercicio Tests

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
        createdAtActionResult.Value.Should().BeEquivalentTo(ejercicioDevuelto);
    }

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
    }

    [Fact]
    public async Task Agregar_ModelStateInvalido_RetornaBadRequest()
    {
        // Arrange
        _controller.ModelState.AddModelError("Nombre", "Requerido");
        var dto = new AgregarEjercicioDTO();

        // Act
        var resultado = await _controller.AgregarEjercicio(dto);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.StatusCode.Should().Be(400);
    }

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
    }

    #endregion

    #region ActualizarEjercicio Tests

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

    [Fact]
    public async Task Actualizar_IdCero_RetornaBadRequest()
    {
        // Arrange
        var dto = new ActualizarEjercicioDTO { Id = 0 };

        // Act
        var resultado = await _controller.ActualizarEjercicio(0, dto);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Actualizar_IdNegativo_RetornaBadRequest()
    {
        // Arrange
        var dto = new ActualizarEjercicioDTO { Id = -5 };

        // Act
        var resultado = await _controller.ActualizarEjercicio(-5, dto);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.StatusCode.Should().Be(400);
    }

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
    }

    [Fact]
    public async Task Actualizar_ModelStateInvalido_RetornaBadRequest()
    {
        // Arrange
        _controller.ModelState.AddModelError("Nombre", "Requerido");
        var dto = new ActualizarEjercicioDTO { Id = 1 };

        // Act
        var resultado = await _controller.ActualizarEjercicio(1, dto);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.StatusCode.Should().Be(400);
    }

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
    }

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
    }

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
    }

    #endregion

    #region EliminarEjercicio Tests

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

    [Fact]
    public async Task Eliminar_IdCero_RetornaBadRequest()
    {
        // Act
        var resultado = await _controller.EliminarEjercicio(0);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Eliminar_IdNegativo_RetornaBadRequest()
    {
        // Act
        var resultado = await _controller.EliminarEjercicio(-3);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult.StatusCode.Should().Be(400);
    }

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
    }

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
    }

    #endregion

    #region GetEjerciciosPorGrupoMuscular Tests

    [Fact]
    public async Task GetEjerciciosPorGrupoMuscular_Exitoso_RetornaOk()
    {
        // Arrange
        var ejercicios = new List<ObtenerEjercicioDTO>
        {
            new ObtenerEjercicioDTO { Id = 1, Nombre = "Ejercicio 1", GrupoMuscularId = 1 },
            new ObtenerEjercicioDTO { Id = 2, Nombre = "Ejercicio 2", GrupoMuscularId = 1 }
        };
        _mockObtenerEjerciciosPorGrupoMuscularCasoDeUso.Setup(x => x.Ejecutar(1)).ReturnsAsync(ejercicios);

        // Act
        var resultado = await _controller.GetEjerciciosPorGrupoMuscular(1);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(ejercicios);
    }

    [Fact]
    public async Task GetEjerciciosPorGrupoMuscular_IdCero_RetornaBadRequest()
    {
        // Act
        var resultado = await _controller.GetEjerciciosPorGrupoMuscular(0);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task GetEjerciciosPorGrupoMuscular_IdNegativo_RetornaBadRequest()
    {
        // Act
        var resultado = await _controller.GetEjerciciosPorGrupoMuscular(-5);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task GetEjerciciosPorGrupoMuscular_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        _mockObtenerEjerciciosPorGrupoMuscularCasoDeUso.Setup(x => x.Ejecutar(1)).ThrowsAsync(new Exception());

        // Act
        var resultado = await _controller.GetEjerciciosPorGrupoMuscular(1);

        // Assert
        var statusCodeResult = resultado as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion
}