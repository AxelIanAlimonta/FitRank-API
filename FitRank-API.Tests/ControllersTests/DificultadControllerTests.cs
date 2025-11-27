using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using FitRank_API.Presentacion.Controllers;
using FitRank_API.Application.DTOs.DificultadDTOs;
using AutoMapper;
using FitRank_API.Domain.Interfaces;
using FitRank_API.Application.CasosDeUso.DificultadCasosDeUso;

namespace FitRank_API.tests.ControllersTests;

public class DificultadControllerTests
{
    private readonly DificultadController _controller;
    private readonly Mock<ActualizarDificultadCasoDeUso> _mockActualizar;
    private readonly Mock<AgregarDificultadCasoDeUso> _mockAgregar;
    private readonly Mock<EliminarDificultadCasoDeUso> _mockEliminar;
    private readonly Mock<ObtenerDificultadPorIdCasoDeUso> _mockObtenerPorId;
    private readonly Mock<ObtenerTodasLasDificultadesCasoDeUso> _mockObtenerTodos;

    public DificultadControllerTests()
    {
        var mockRepositorio = new Mock<IDificultadRepositorio>();
        var mockMapper = new Mock<IMapper>();

        _mockActualizar = new Mock<ActualizarDificultadCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockAgregar = new Mock<AgregarDificultadCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockEliminar = new Mock<EliminarDificultadCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockObtenerPorId = new Mock<ObtenerDificultadPorIdCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockObtenerTodos = new Mock<ObtenerTodasLasDificultadesCasoDeUso>(mockRepositorio.Object, mockMapper.Object);

        _controller = new DificultadController(
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
        var nuevaDificultadDTO = new AgregarDificultadDTO { Descripcion = "Principiante" };
        var dificultadCreada = new DificultadDTO { Id = 1, Descripcion = "Principiante" };

        _mockAgregar
            .Setup(caso => caso.Ejecutar(nuevaDificultadDTO))
            .ReturnsAsync(dificultadCreada);

        // Act
        var resultado = await _controller.Agregar(nuevaDificultadDTO);

        // Assert
        var createdAtActionResult = resultado as CreatedAtActionResult;
        createdAtActionResult.Should().NotBeNull();
        createdAtActionResult!.StatusCode.Should().Be(201);
        createdAtActionResult.Value.Should().BeEquivalentTo(dificultadCreada);
    }

    //ObtenerTodos_RetornaOkResult_ConListaCompleta
    [Fact]
    public async Task ObtenerTodos_RetornaOkResult_ConListaCompleta()
    {
        // Arrange
        var listaDificultades = new List<DificultadDTO>
        {
            new DificultadDTO { Id = 1, Descripcion = "Principiante" },
            new DificultadDTO { Id = 2, Descripcion = "Intermedio" },
            new DificultadDTO { Id = 3, Descripcion = "Avanzado" }
        };

        _mockObtenerTodos
            .Setup(caso => caso.Ejecutar())
            .ReturnsAsync(listaDificultades);

        // Act
        var resultado = await _controller.ObtenerTodos();

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(listaDificultades);
    }

    //ObtenerTodos_RetornaOkResult_ConListaVacia
    [Fact]
    public async Task ObtenerTodos_RetornaOkResult_ConListaVacia()
    {
        // Arrange
        var listaDificultades = new List<DificultadDTO>();

        _mockObtenerTodos
            .Setup(caso => caso.Ejecutar())
            .ReturnsAsync(listaDificultades);

        // Act
        var resultado = await _controller.ObtenerTodos();

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(listaDificultades);
    }

    //ObtenerPorId_NoExiste_RetornaNotFound
    [Fact]
    public async Task ObtenerPorId_NoExiste_RetornaNotFound()
    {
        // Arrange
        int dificultadId = 999;

        _mockObtenerPorId
            .Setup(caso => caso.Ejecutar(dificultadId))
            .ReturnsAsync((DificultadDTO?)null);

        // Act
        var resultado = await _controller.ObtenerPorId(dificultadId);

        // Assert
        var notFoundResult = resultado as NotFoundResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    //ObtenerPorId_Existe_RetornaObjetoOkCreado
    [Fact]
    public async Task ObtenerPorId_Existe_RetornaObjetoOkCreado()
    {
        // Arrange
        int dificultadId = 1;
        var dificultadDTO = new DificultadDTO { Id = dificultadId, Descripcion = "Principiante" };

        _mockObtenerPorId
            .Setup(caso => caso.Ejecutar(dificultadId))
            .ReturnsAsync(dificultadDTO);

        // Act
        var resultado = await _controller.ObtenerPorId(dificultadId);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(dificultadDTO);
    }

    //Actualizar_RetornaOkObjectResult_ConObjetoActualizado
    [Fact]
    public async Task Actualizar_RetornaOkObjectResult_ConObjetoActualizado()
    {
        // Arrange
        int dificultadId = 1;
        var dificultadActualizarDTO = new DificultadDTO { Id = dificultadId, Descripcion = "Experto" };
        var dificultadActualizada = new DificultadDTO { Id = dificultadId, Descripcion = "Experto" };

        _mockActualizar
            .Setup(caso => caso.Ejecutar(dificultadActualizarDTO))
            .ReturnsAsync(dificultadActualizada);

        // Act
        var resultado = await _controller.Actualizar(dificultadId, dificultadActualizarDTO);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(dificultadActualizada);
    }

    //Actualizar_NoEncontrado_RetornaNotFoundResult
    [Fact]
    public async Task Actualizar_NoEncontrado_RetornaNotFoundResult()
    {
        // Arrange
        int dificultadId = 999;
        var dificultadActualizarDTO = new DificultadDTO { Id = dificultadId, Descripcion = "No Existe" };

        _mockActualizar
            .Setup(caso => caso.Ejecutar(dificultadActualizarDTO))
            .ReturnsAsync((DificultadDTO?)null);

        // Act
        var resultado = await _controller.Actualizar(dificultadId, dificultadActualizarDTO);

        // Assert
        var notFoundResult = resultado as NotFoundResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    //Actualizar_IdNoCoincide_RetornaBadRequestResult
    [Fact]
    public async Task Actualizar_IdNoCoincide_RetornaBadRequestResult()
    {
        // Arrange
        int dificultadIdRuta = 1;
        var dificultadActualizarDTO = new DificultadDTO { Id = 2, Descripcion = "Avanzado" };

        // Act
        var resultado = await _controller.Actualizar(dificultadIdRuta, dificultadActualizarDTO);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
        badRequestResult.Value.Should().Be("El ID del grupo muscular no coincide.");
    }

    //Eliminar_Existente_DeberiaRetornarNoContent
    [Fact]
    public async Task Eliminar_Existente_DeberiaRetornarNoContent()
    {
        // Arrange
        int dificultadId = 1;

        _mockEliminar
            .Setup(caso => caso.Ejecutar(dificultadId))
            .Returns(Task.CompletedTask);

        // Act
        var resultado = await _controller.Eliminar(dificultadId);

        // Assert
        var noContentResult = resultado as NoContentResult;
        noContentResult.Should().NotBeNull();
        noContentResult!.StatusCode.Should().Be(204);
    }
}
