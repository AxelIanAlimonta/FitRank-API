using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using FitRank_API.Presentacion.Controllers;
using FitRank_API.Application.DTOs.ConfiguracionGrupoMuscularDTOs;
using AutoMapper;
using FitRank_API.Domain.Interfaces;
using FitRank_API.Application.CasosDeUso.ConfiguracionGrupoMuscular;

namespace FitRank_API.tests.ControllersTests;

public class ConfiguracionGrupoMuscularControllerTests
{
    private readonly ConfiguracionGrupoMuscularController _controller;
    private readonly Mock<ActualizarConfiguracionGrupoMuscularCasoDeUso> _mockActualizar;
    private readonly Mock<AgregarConfiguracionGrupoMuscularCasoDeUso> _mockAgregar;
    private readonly Mock<EliminarConfiguracionGrupoMuscularCasoDeUso> _mockEliminar;
    private readonly Mock<ObtenerConfiguracionGrupoMuscularPorIdCasoDeUso> _mockObtenerPorId;
    private readonly Mock<ObtenerTodasLasConfiguracionGrupoMuscularCasoDeUso> _mockObtenerTodos;

    public ConfiguracionGrupoMuscularControllerTests()
    {
        var mockRepositorio = new Mock<IConfiguracionGrupoMuscularRepositorio>();
        var mockMapper = new Mock<IMapper>();

        _mockActualizar = new Mock<ActualizarConfiguracionGrupoMuscularCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockAgregar = new Mock<AgregarConfiguracionGrupoMuscularCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockEliminar = new Mock<EliminarConfiguracionGrupoMuscularCasoDeUso>(mockRepositorio.Object);
        _mockObtenerPorId = new Mock<ObtenerConfiguracionGrupoMuscularPorIdCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockObtenerTodos = new Mock<ObtenerTodasLasConfiguracionGrupoMuscularCasoDeUso>(mockRepositorio.Object, mockMapper.Object);

        _controller = new ConfiguracionGrupoMuscularController(
            _mockObtenerTodos.Object,
            _mockObtenerPorId.Object,
            _mockEliminar.Object,
            _mockActualizar.Object,
            _mockAgregar.Object
        );
    }

    //Agregar_RetornaCreatedAtActionResult
    [Fact]
    public async Task Agregar_RetornaCreatedAtActionResult()
    {
        // Arrange
        var nuevaConfiguracionDTO = new AgregarConfiguracionGrupoMuscularDTO { GrupoMuscularId = 1, MultiplicadorPeso = 1.5, MultiplicadorRepeticiones = 1.2, FactorProgresion = 0.05 };
        var configuracionCreada = new ConfiguracionGrupoMuscularDTO { Id = 1, GrupoMuscularId = 1, MultiplicadorPeso = 1.5, MultiplicadorRepeticiones = 1.2, FactorProgresion = 0.05 };

        _mockAgregar
            .Setup(caso => caso.Ejecutar(nuevaConfiguracionDTO))
            .ReturnsAsync(configuracionCreada);

        // Act
        var resultado = await _controller.Agregar(nuevaConfiguracionDTO);

        // Assert
        var createdAtActionResult = resultado as CreatedAtActionResult;
        createdAtActionResult.Should().NotBeNull();
        createdAtActionResult!.StatusCode.Should().Be(201);
        createdAtActionResult.Value.Should().BeEquivalentTo(configuracionCreada);
    }

    //ObtenerTodos_RetornaOkResult_ConListaCompleta
    [Fact]
    public async Task ObtenerTodos_RetornaOkResult_ConListaCompleta()
    {
        // Arrange
        var listaConfiguraciones = new List<ConfiguracionGrupoMuscularDTO>
        {
            new ConfiguracionGrupoMuscularDTO { Id = 1, GrupoMuscularId = 1, MultiplicadorPeso = 1.5, MultiplicadorRepeticiones = 1.2, FactorProgresion = 0.05 },
            new ConfiguracionGrupoMuscularDTO { Id = 2, GrupoMuscularId = 2, MultiplicadorPeso = 2.0, MultiplicadorRepeticiones = 1.0, FactorProgresion = 0.03 }
        };

        _mockObtenerTodos
            .Setup(caso => caso.Ejecutar())
            .ReturnsAsync(listaConfiguraciones);

        // Act
        var resultado = await _controller.ObtenerTodos();

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(listaConfiguraciones);
    }

    //ObtenerTodos_RetornaOkResult_ConListaVacia
    [Fact]
    public async Task ObtenerTodos_RetornaOkResult_ConListaVacia()
    {
        // Arrange
        var listaConfiguraciones = new List<ConfiguracionGrupoMuscularDTO>();

        _mockObtenerTodos
            .Setup(caso => caso.Ejecutar())
            .ReturnsAsync(listaConfiguraciones);

        // Act
        var resultado = await _controller.ObtenerTodos();

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(listaConfiguraciones);
    }

    //ObtenerPorId_NoExiste_RetornaNotFound
    [Fact]
    public async Task ObtenerPorId_NoExiste_RetornaNotFound()
    {
        // Arrange
        long configuracionId = 999;

        _mockObtenerPorId
            .Setup(caso => caso.Ejecutar(configuracionId))
            .ReturnsAsync((ConfiguracionGrupoMuscularDTO?)null);

        // Act
        var resultado = await _controller.ObtenerPorId(configuracionId);

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
        long configuracionId = 1;
        var configuracionDTO = new ConfiguracionGrupoMuscularDTO { Id = configuracionId, GrupoMuscularId = 1, MultiplicadorPeso = 1.5, MultiplicadorRepeticiones = 1.2, FactorProgresion = 0.05 };

        _mockObtenerPorId
            .Setup(caso => caso.Ejecutar(configuracionId))
            .ReturnsAsync(configuracionDTO);

        // Act
        var resultado = await _controller.ObtenerPorId(configuracionId);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(configuracionDTO);
    }

    //Actualizar_RetornaOkObjectResult_ConObjetoActualizado
    [Fact]
    public async Task Actualizar_RetornaOkObjectResult_ConObjetoActualizado()
    {
        // Arrange
        long configuracionId = 1;
        var configuracionActualizarDTO = new ConfiguracionGrupoMuscularDTO { Id = configuracionId, GrupoMuscularId = 1, MultiplicadorPeso = 2.0, MultiplicadorRepeticiones = 1.5, FactorProgresion = 0.08 };
        var configuracionActualizada = new ConfiguracionGrupoMuscularDTO { Id = configuracionId, GrupoMuscularId = 1, MultiplicadorPeso = 2.0, MultiplicadorRepeticiones = 1.5, FactorProgresion = 0.08 };

        _mockActualizar
            .Setup(caso => caso.Ejecutar(configuracionActualizarDTO))
            .ReturnsAsync(configuracionActualizada);

        // Act
        var resultado = await _controller.Actualizar(configuracionId, configuracionActualizarDTO);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(configuracionActualizada);
    }

    //Actualizar_NoEncontrado_RetornaNotFoundResult
    [Fact]
    public async Task Actualizar_NoEncontrado_RetornaNotFoundResult()
    {
        // Arrange
        long configuracionId = 999;
        var configuracionActualizarDTO = new ConfiguracionGrupoMuscularDTO { Id = configuracionId, GrupoMuscularId = 1, MultiplicadorPeso = 1.5, MultiplicadorRepeticiones = 1.2, FactorProgresion = 0.05 };

        _mockActualizar
            .Setup(caso => caso.Ejecutar(configuracionActualizarDTO))
            .ReturnsAsync((ConfiguracionGrupoMuscularDTO?)null);

        // Act
        var resultado = await _controller.Actualizar(configuracionId, configuracionActualizarDTO);

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
        long configuracionIdRuta = 1;
        var configuracionActualizarDTO = new ConfiguracionGrupoMuscularDTO { Id = 2, GrupoMuscularId = 1, MultiplicadorPeso = 1.5, MultiplicadorRepeticiones = 1.2, FactorProgresion = 0.05 };

        // Act
        var resultado = await _controller.Actualizar(configuracionIdRuta, configuracionActualizarDTO);

        // Assert
        var badRequestResult = resultado as BadRequestResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    //Eliminar_Existente_DeberiaRetornarNoContent
    [Fact]
    public async Task Eliminar_Existente_DeberiaRetornarNoContent()
    {
        // Arrange
        long configuracionId = 1;

        _mockEliminar
            .Setup(caso => caso.Ejecutar(configuracionId))
            .Returns(Task.CompletedTask);

        // Act
        var resultado = await _controller.Eliminar(configuracionId);

        // Assert
        var noContentResult = resultado as NoContentResult;
        noContentResult.Should().NotBeNull();
        noContentResult!.StatusCode.Should().Be(204);
    }
}
