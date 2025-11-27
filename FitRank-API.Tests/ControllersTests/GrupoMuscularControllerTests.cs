using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using FitRank_API.Application.CasosDeUso.GrupoMuscularCasosDeUso;
using FitRank_API.Presentacion.Controllers;
using FitRank_API.Application.DTOs.GrupoMuscularDTOs;
using AutoMapper;
using FitRank_API.Domain.Interfaces;
using FitRank_API.Application.DTOs;

namespace FitRank_API.tests.ControllersTests;

public class GrupoMuscularControllerActualizarTests
{
    private readonly GrupoMuscularController _controller;
    private readonly Mock<ActualizarGrupoMuscularCasoDeUso> _mockActualizar;
    private readonly Mock<AgregarGrupoMuscularCasoDeUso> _mockAgregar;
    private readonly Mock<EliminarGrupoMuscularCasoDeUso> _mockEliminar;
    private readonly Mock<ObtenerGrupoMuscularPorIdCasoDeUso> _mockObtenerPorId;
    private readonly Mock<ObtenerTodosLosGruposMuscularesCasoDeUso> _mockObtenerTodos;

    public GrupoMuscularControllerActualizarTests()
    {
        var mockRepositorio = new Mock<IGrupoMuscularRepositorio>();
        var mockMapper = new Mock<IMapper>();

        _mockActualizar = new Mock<ActualizarGrupoMuscularCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockAgregar = new Mock<AgregarGrupoMuscularCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockEliminar = new Mock<EliminarGrupoMuscularCasoDeUso>(mockRepositorio.Object);
        _mockObtenerPorId = new Mock<ObtenerGrupoMuscularPorIdCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockObtenerTodos = new Mock<ObtenerTodosLosGruposMuscularesCasoDeUso>(mockRepositorio.Object, mockMapper.Object);

        _controller = new GrupoMuscularController(
            _mockObtenerTodos.Object,
            _mockObtenerPorId.Object,
            _mockEliminar.Object,
            _mockActualizar.Object,
            _mockAgregar.Object
        );
    }

    [Fact]
    public async Task Agregar_RetornaCreatedAtActionResult_ConGrupoMuscularCreado()
    {
        // Arrange
        var agregarDTO = new AgregarGrupoMuscularDTO { Nombre = "Piernas" };
        var obtenerDTO = new ObtenerGrupoMuscularDTO { Id = 1, Nombre = "Piernas" };
        _mockAgregar.Setup(x => x.Ejecutar(agregarDTO)).ReturnsAsync(obtenerDTO);
        // Act
        var result = await _controller.Agregar(agregarDTO);
        // Assert
        var createdAtActionResult = result as CreatedAtActionResult;
        createdAtActionResult.Should().NotBeNull();
        createdAtActionResult!.StatusCode.Should().Be(201);
        var returnedGrupoMuscular = createdAtActionResult.Value as ObtenerGrupoMuscularDTO;
        returnedGrupoMuscular.Should().NotBeNull();
        returnedGrupoMuscular!.Id.Should().Be(1);
        returnedGrupoMuscular.Nombre.Should().Be("Piernas");
    }


    [Fact]
    public async Task Agregar_LanzaExcepcion_RetornaStatusCode500()
    {
        // Arrange
        var agregarDTO = new AgregarGrupoMuscularDTO { Nombre = "Piernas" };
        _mockAgregar.Setup(x => x.Ejecutar(agregarDTO)).ThrowsAsync(new Exception("Hubo un error en el servidor."));
        //Act
        var result = await _controller.Agregar(agregarDTO);
        //Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
        statusCodeResult.Value.Should().Be("Hubo un error en el servidor.");
    }

    [Fact]
    public async Task Agregar_RetornaBadRequest_CuandoDTOEsNulo()
    {
        // Arrange
        AgregarGrupoMuscularDTO agregarDTO = null!;
        // Act
        var result = await _controller.Agregar(agregarDTO);
        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
        badRequestResult.Value.Should().Be("El grupo muscular no puede ser nulo.");
    }

    [Fact]
    public async Task ObtenerTodos_RetornaOkResult_ConListaDeGruposMusculares()
    {
        // Arrange
        var gruposMusculares = new List<ObtenerGrupoMuscularDTO>
        {
            new ObtenerGrupoMuscularDTO { Id = 1, Nombre = "Pecho" },
            new ObtenerGrupoMuscularDTO { Id = 2, Nombre = "Espalda" }
        };
        _mockObtenerTodos.Setup(x => x.Ejecutar()).ReturnsAsync(gruposMusculares);
        // Act
        var result = await _controller.ObtenerTodos();
        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        var returnedGruposMusculares = okResult.Value as List<ObtenerGrupoMuscularDTO>;
        returnedGruposMusculares.Should().NotBeNull();
        returnedGruposMusculares!.Count.Should().Be(2);
        returnedGruposMusculares[0].Nombre.Should().Be("Pecho");
        returnedGruposMusculares[1].Nombre.Should().Be("Espalda");
    }

    [Fact]
    public async Task ObtenerTodos_RetornaOkResult_ConListaVacia()
    {
        // Arrange
        var gruposMusculares = new List<ObtenerGrupoMuscularDTO>();
        _mockObtenerTodos.Setup(x => x.Ejecutar()).ReturnsAsync(gruposMusculares);
        // Act
        var result = await _controller.ObtenerTodos();
        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        var returnedGruposMusculares = okResult.Value as List<ObtenerGrupoMuscularDTO>;
        returnedGruposMusculares.Should().NotBeNull();
        returnedGruposMusculares!.Count.Should().Be(0);
    }

    [Fact]
    public async Task ObtenerTodos_LanzaExcepcion_RetornaStatus500()
    {
        // Arrange
        _mockObtenerTodos.Setup(x => x.Ejecutar()).ThrowsAsync(new Exception("Error inesperado"));
        // Act
        var result = await _controller.ObtenerTodos();
        // Assert
        var objectResult = result as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult.StatusCode.Should().Be(500);
        objectResult.Value.Should().Be("Error inesperado");
    }

    [Fact]
    public async Task ObtenerPorId_GrupoMuscularNoExiste_RetornaNotFound()
    {
        // Arrange
        long grupoMuscularId = 1;
        _mockObtenerPorId.Setup(x => x.Ejecutar(grupoMuscularId)).ReturnsAsync((ObtenerGrupoMuscularDTO?)null);
        // Act
        var result = await _controller.ObtenerPorId(grupoMuscularId);

        // Assert
        var notFoundResult = result as NotFoundResult;

        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task ObtenerPorId_GrupoMuscularExiste_RetornaOkConGrupoMuscular()
    {
        // Arrange
        long grupoMuscularId = 1;
        var grupoMuscularDTO = new ObtenerGrupoMuscularDTO { Id = grupoMuscularId, Nombre = "Pecho" };
        _mockObtenerPorId.Setup(x => x.Ejecutar(grupoMuscularId)).ReturnsAsync(grupoMuscularDTO);
        // Act
        var result = await _controller.ObtenerPorId(grupoMuscularId);
        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        var returnedGrupoMuscular = okResult.Value as ObtenerGrupoMuscularDTO;
        returnedGrupoMuscular.Should().NotBeNull();
        returnedGrupoMuscular!.Id.Should().Be(grupoMuscularId);
        returnedGrupoMuscular.Nombre.Should().Be("Pecho");
    }

    [Fact]
    public async Task Actualizar_RetornaOkObjectResult_ConGrupoMuscularActualizado()
    {
        // Arrange
        var actualizarDTO = new ActualizarGrupoMuscularDTO { Id = 1, Nombre = "Espalda" };
        var obtenerDTO = new ObtenerGrupoMuscularDTO { Id = 1, Nombre = "Espalda" };

        _mockActualizar.Setup(x => x.Ejecutar(actualizarDTO)).ReturnsAsync(obtenerDTO);

        // Act
        var result = await _controller.Actualizar(1, actualizarDTO);

        // Assert
        var okObjectResult = result as OkObjectResult;
        okObjectResult.Should().NotBeNull();
        okObjectResult!.StatusCode.Should().Be(200);
        var returnedGrupoMuscular = okObjectResult.Value as ObtenerGrupoMuscularDTO;
        returnedGrupoMuscular.Should().NotBeNull();
        returnedGrupoMuscular!.Id.Should().Be(1);
        returnedGrupoMuscular.Nombre.Should().Be("Espalda");
    }

    [Fact]
    public async Task Actualizar_GrupoMuscularNoEncontrado_RetornaNotFoundResult()
    {
        // Arrange
        var actualizarDTO = new ActualizarGrupoMuscularDTO { Id = 99, Nombre = "NoExiste" };
        _mockActualizar.Setup(x => x.Ejecutar(actualizarDTO)).ReturnsAsync((ObtenerGrupoMuscularDTO?)null);
        // Act
        var result = await _controller.Actualizar(99, actualizarDTO);
        // Assert
        var notFoundResult = result as NotFoundResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Actualizar_LanzaExcepcion_RetornaStatusCode500()
    {
        // Arrange
        var actualizarDTO = new ActualizarGrupoMuscularDTO { Id = 1, Nombre = "Espalda" };
        _mockActualizar.Setup(x => x.Ejecutar(actualizarDTO)).ThrowsAsync(new Exception("Hubo un error en el servidor."));
        // Act
        var result = await _controller.Actualizar(1, actualizarDTO);
        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task Actualizar_IdNoCoincide_RetornaBadRequestResult()
    {
        // Arrange
        var actualizarDTO = new ActualizarGrupoMuscularDTO { Id = 1, Nombre = "Espalda" };
        // Act
        var result = await _controller.Actualizar(2, actualizarDTO);
        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Actualizar_DTONulo_RetornaBadRequestResult()
    {
        // Arrange
        ActualizarGrupoMuscularDTO actualizarDTO = null!;
        // Act
        var result = await _controller.Actualizar(1, actualizarDTO);
        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Eliminar_GrupoMuscular_Existente_Deberia_Retornar_NoContent()
    {
        // Arrange
        long grupoMuscularId = 1;
        _mockEliminar
            .Setup(x => x.Ejecutar(grupoMuscularId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.Eliminar(grupoMuscularId);

        // Assert
        result.Should().BeOfType<NoContentResult>();
    }

    [Fact]
    public async Task Eliminar_GrupoMuscular_NoExistente_Deberia_Retornar_NotFound()
    {
        // Arrange
        long grupoMuscularId = 1;
        _mockEliminar
            .Setup(x => x.Ejecutar(grupoMuscularId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Eliminar(grupoMuscularId);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Eliminar_Cuando_Ocurre_Error_Deberia_Retornar_StatusCode500()
    {
        // Arrange
        long grupoMuscularId = 1;
        _mockEliminar
            .Setup(x => x.Ejecutar(grupoMuscularId))
            .ThrowsAsync(new Exception("Error inesperado"));

        // Act
        var result = await _controller.Eliminar(grupoMuscularId);

        // Assert
        var objectResult = result as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(500);
        objectResult.Value.Should().Be("Error inesperado");
    }
}