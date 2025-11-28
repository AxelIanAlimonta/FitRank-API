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
using FitRank_API.Application.CasosDeUso.JornadaCasosDeUso;
using FitRank_API.Application.DTOs.JornadaDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Application.CasosDeUso.DiaDeLaSemanaCasoDeUso;
using FitRank_API.Application.DTOs.DiaDeLaSemanaDTOs;

namespace FitRank_API.tests.ControllersTests;

public class DiaDeLaSemanaControllerTests
{
    private readonly DiaDeLaSemanaController _controller;
    private readonly Mock<ActualizarDiaDeLaSemanaCasoDeUso> _mockActualizar;
    private readonly Mock<AgregarDiaDeLaSemanaCasoDeUso> _mockAgregar;
    private readonly Mock<EliminarDiaDeLaSemanaCasoDeUso> _mockEliminar;
    private readonly Mock<ObtenerDiaDeLaSemanaPorIdCasoDeUso> _mockObtenerPorId;
    private readonly Mock<ObtenerTodosLosDiasDeLaSemanaCasoDeUso> _mockObtenerTodos;

    public DiaDeLaSemanaControllerTests()
    {
        var mockRepositorio = new Mock<IDiaDeLaSemanaRepositorio>();
        var mockMapper = new Mock<IMapper>();

        _mockActualizar = new Mock<ActualizarDiaDeLaSemanaCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockAgregar = new Mock<AgregarDiaDeLaSemanaCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockEliminar = new Mock<EliminarDiaDeLaSemanaCasoDeUso>(mockRepositorio.Object);
        _mockObtenerPorId = new Mock<ObtenerDiaDeLaSemanaPorIdCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockObtenerTodos = new Mock<ObtenerTodosLosDiasDeLaSemanaCasoDeUso>(mockRepositorio.Object, mockMapper.Object);

        _controller = new DiaDeLaSemanaController(
            _mockObtenerTodos.Object,
            _mockAgregar.Object,
            _mockActualizar.Object,
            _mockEliminar.Object,
            _mockObtenerPorId.Object
        );
    }

    #region ObtenerTodosAsync Tests

    [Fact]
    public async Task ObtenerTodos_RetornaOkResult_ConListaCompleta()
    {
        var listaDiasDTO = new List<ObtenerDiaDeLaSemanaDTO>
        {
            new ObtenerDiaDeLaSemanaDTO { Id = 1, Nombre = "Lunes" },
            new ObtenerDiaDeLaSemanaDTO { Id = 2, Nombre = "Martes" }
        };

        _mockObtenerTodos
            .Setup(m => m.Ejecutar())
            .ReturnsAsync(listaDiasDTO);

        var resultado = await _controller.ObtenerTodosAsync();

        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(listaDiasDTO);
    }

    [Fact]
    public async Task ObtenerTodos_RetornaOkResult_ConListaVacia()
    {
        var listaDiasDTO = new List<ObtenerDiaDeLaSemanaDTO>();

        _mockObtenerTodos
            .Setup(m => m.Ejecutar())
            .ReturnsAsync(listaDiasDTO);

        var resultado = await _controller.ObtenerTodosAsync();

        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(listaDiasDTO);
    }

    [Fact]
    public async Task ObtenerTodos_LanzaExcepcion_RetornaStatus500()
    {
        _mockObtenerTodos
            .Setup(m => m.Ejecutar())
            .ThrowsAsync(new Exception("Error inesperado"));

        var resultado = await _controller.ObtenerTodosAsync();

        var objectResult = resultado as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ObtenerPorId Tests

    [Fact]
    public async Task ObtenerPorId_Existe_RetornaObjetoOkCreado()
    {
        int diaId = 1;
        var diaDTO = new ObtenerDiaDeLaSemanaDTO
        {
            Id = diaId,
            Nombre = "Lunes"
        };

        _mockObtenerPorId
            .Setup(m => m.Ejecutar(diaId))
            .ReturnsAsync(diaDTO);

        var resultado = await _controller.ObtenerPorId(diaId);

        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(diaDTO);
    }

    [Fact]
    public async Task ObtenerPorId_IdCero_RetornaBadRequest()
    {
        var resultado = await _controller.ObtenerPorId(0);

        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerPorId_IdNegativo_RetornaBadRequest()
    {
        var resultado = await _controller.ObtenerPorId(-5);

        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerPorId_NoExiste_RetornaNotFound()
    {
        int diaId = 99;

        _mockObtenerPorId
            .Setup(m => m.Ejecutar(diaId))
            .ReturnsAsync((ObtenerDiaDeLaSemanaDTO?)null);

        var resultado = await _controller.ObtenerPorId(diaId);

        var notFoundResult = resultado as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task ObtenerPorId_ExcepcionGenerica_RetornaInternalServerError()
    {
        _mockObtenerPorId.Setup(x => x.Ejecutar(1)).ThrowsAsync(new Exception());

        var resultado = await _controller.ObtenerPorId(1);

        var statusCodeResult = resultado as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region AgregarAsync Tests

    [Fact]
    public async Task Agregar_RetornaCreatedAtActionResult()
    {
        var nuevoDiaDTO = new AgregarDiaDeLaSemanaDTO
        {
            Nombre = "Lunes"
        };

        var diaCreadoDTO = new ObtenerDiaDeLaSemanaDTO
        {
            Id = 1,
            Nombre = "Lunes"
        };

        _mockAgregar
            .Setup(m => m.Ejecutar(It.IsAny<AgregarDiaDeLaSemanaDTO>()))
            .ReturnsAsync(diaCreadoDTO);

        var resultado = await _controller.AgregarAsync(nuevoDiaDTO);

        var createdAtActionResult = resultado as CreatedAtActionResult;
        createdAtActionResult.Should().NotBeNull();
        createdAtActionResult!.StatusCode.Should().Be(201);
        createdAtActionResult.ActionName.Should().Be(nameof(_controller.ObtenerPorId));
        createdAtActionResult.RouteValues!["id"].Should().Be(diaCreadoDTO.Id);
        createdAtActionResult.Value.Should().BeEquivalentTo(diaCreadoDTO);
    }

    [Fact]
    public async Task Agregar_RetornaBadRequest_CuandoDTOEsNulo()
    {
        AgregarDiaDeLaSemanaDTO nuevoDiaDTO = null!;

        var resultado = await _controller.AgregarAsync(nuevoDiaDTO);

        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Agregar_ModelStateInvalido_RetornaBadRequest()
    {
        _controller.ModelState.AddModelError("Nombre", "Requerido");
        var dto = new AgregarDiaDeLaSemanaDTO();

        var resultado = await _controller.AgregarAsync(dto);

        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Agregar_LanzaExcepcion_RetornaStatusCode500()
    {
        var nuevoDiaDTO = new AgregarDiaDeLaSemanaDTO
        {
            Nombre = "Lunes"
        };

        _mockAgregar
            .Setup(m => m.Ejecutar(It.IsAny<AgregarDiaDeLaSemanaDTO>()))
            .ThrowsAsync(new Exception("Error inesperado"));

        var resultado = await _controller.AgregarAsync(nuevoDiaDTO);

        var objectResult = resultado as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ActualizarAsync Tests

    [Fact]
    public async Task Actualizar_RetornaOkObjectResult_ConObjetoActualizado()
    {
        var actualizarDiaDTO = new ActualizarDiaDeLaSemanaDTO
        {
            Id = 1,
            Nombre = "Lunes Modificado"
        };

        var diaActualizadoDTO = new ObtenerDiaDeLaSemanaDTO
        {
            Id = 1,
            Nombre = "Lunes Modificado"
        };

        _mockActualizar
            .Setup(m => m.Ejecutar(It.IsAny<ActualizarDiaDeLaSemanaDTO>()))
            .ReturnsAsync(diaActualizadoDTO);

        var resultado = await _controller.ActualizarAsync(1, actualizarDiaDTO);

        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(diaActualizadoDTO);
    }

    [Fact]
    public async Task Actualizar_IdCero_RetornaBadRequest()
    {
        var dto = new ActualizarDiaDeLaSemanaDTO { Id = 0 };

        var resultado = await _controller.ActualizarAsync(0, dto);

        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Actualizar_IdNegativo_RetornaBadRequest()
    {
        var dto = new ActualizarDiaDeLaSemanaDTO { Id = -5 };

        var resultado = await _controller.ActualizarAsync(-5, dto);

        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Actualizar_DTONulo_RetornaBadRequestResult()
    {
        ActualizarDiaDeLaSemanaDTO actualizarDiaDTO = null!;

        var resultado = await _controller.ActualizarAsync(1, actualizarDiaDTO);

        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Actualizar_ModelStateInvalido_RetornaBadRequest()
    {
        _controller.ModelState.AddModelError("Nombre", "Requerido");
        var dto = new ActualizarDiaDeLaSemanaDTO { Id = 1 };

        var resultado = await _controller.ActualizarAsync(1, dto);

        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Actualizar_IdNoCoincide_RetornaBadRequestResult()
    {
        var actualizarDiaDTO = new ActualizarDiaDeLaSemanaDTO
        {
            Id = 1,
            Nombre = "Lunes Modificado"
        };

        var resultado = await _controller.ActualizarAsync(2, actualizarDiaDTO);

        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Actualizar_NoEncontrado_RetornaNotFoundResult()
    {
        var actualizarDiaDTO = new ActualizarDiaDeLaSemanaDTO
        {
            Id = 99,
            Nombre = "Dia Inexistente"
        };

        _mockActualizar
            .Setup(m => m.Ejecutar(It.IsAny<ActualizarDiaDeLaSemanaDTO>()))
            .ReturnsAsync((ObtenerDiaDeLaSemanaDTO?)null);

        var resultado = await _controller.ActualizarAsync(99, actualizarDiaDTO);

        var notFoundResult = resultado as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Actualizar_LanzaExcepcion_RetornaStatusCode500()
    {
        var actualizarDiaDTO = new ActualizarDiaDeLaSemanaDTO
        {
            Id = 1,
            Nombre = "Lunes Modificado"
        };

        _mockActualizar
            .Setup(m => m.Ejecutar(It.IsAny<ActualizarDiaDeLaSemanaDTO>()))
            .ThrowsAsync(new Exception("Error inesperado"));

        var resultado = await _controller.ActualizarAsync(1, actualizarDiaDTO);

        var objectResult = resultado as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region EliminarAsync Tests

    [Fact]
    public async Task Eliminar_Existente_DeberiaRetornarNoContent()
    {
        int diaId = 1;

        _mockEliminar
            .Setup(m => m.Ejecutar(diaId))
            .ReturnsAsync(true);

        var resultado = await _controller.EliminarAsync(diaId);

        var noContentResult = resultado as NoContentResult;
        noContentResult.Should().NotBeNull();
        noContentResult!.StatusCode.Should().Be(204);
    }

    [Fact]
    public async Task Eliminar_IdCero_RetornaBadRequest()
    {
        var resultado = await _controller.EliminarAsync(0);

        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Eliminar_IdNegativo_RetornaBadRequest()
    {
        var resultado = await _controller.EliminarAsync(-3);

        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Eliminar_NoExistente_DeberiaRetornarNotFound()
    {
        int diaId = 99;

        _mockEliminar
            .Setup(m => m.Ejecutar(diaId))
            .ReturnsAsync(false);

        var resultado = await _controller.EliminarAsync(diaId);

        var notFoundResult = resultado as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Eliminar_CuandoOcurreErrorEnServidor_DeberiaRetornarStatusCode500()
    {
        int diaId = 1;

        _mockEliminar
            .Setup(m => m.Ejecutar(diaId))
            .ThrowsAsync(new Exception("Error inesperado"));

        var resultado = await _controller.EliminarAsync(diaId);

        var objectResult = resultado as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(500);
    }

    #endregion
}