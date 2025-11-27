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

namespace FitRank_API.tests.ControllersTests;

public class JornadaControllerTests
{

    private readonly JornadaController _controller;
    private readonly Mock<ActualizarJornadaCasoDeUso> _mockActualizar;
    private readonly Mock<AgregarJornadaCasoDeUso> _mockAgregar;
    private readonly Mock<EliminarJornadaCasoDeUso> _mockEliminar;
    private readonly Mock<ObtenerJornadaPorIdCasoDeUso> _mockObtenerPorId;
    private readonly Mock<ObtenerTodasLasJornadasCasoDeUso> _mockObtenerTodos;

    public JornadaControllerTests()
    {
        var mockRepositorio = new Mock<IJornadaRepositorio>();
        var mockMapper = new Mock<IMapper>();

        _mockActualizar = new Mock<ActualizarJornadaCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockAgregar = new Mock<AgregarJornadaCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockEliminar = new Mock<EliminarJornadaCasoDeUso>(mockRepositorio.Object);
        _mockObtenerPorId = new Mock<ObtenerJornadaPorIdCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockObtenerTodos = new Mock<ObtenerTodasLasJornadasCasoDeUso>(mockRepositorio.Object, mockMapper.Object);

        _controller = new JornadaController(
            _mockObtenerTodos.Object,
            _mockObtenerPorId.Object,
            _mockAgregar.Object,
            _mockActualizar.Object,
            _mockEliminar.Object
        );

    }

    [Fact]
    public async Task Agregar_RetornaCreatedAtActionResult()
    {
        // Arrange
        var nuevaJornadaDTO = new AgregarJornadaDTO
        {
            HoraInicio = new TimeSpan(8, 0, 0),
            HoraFin = new TimeSpan(14, 0, 0),
            ProfesorId = 1,
            DiaDeLaSemanaId = 2
        };

        var jornadaCreadaDTO = new ObtenerJornadaDTO
        {
            Id = 1,
            HoraInicio = new TimeSpan(8, 0, 0),
            HoraFin = new TimeSpan(14, 0, 0),
            ProfesorId = 1,
            DiaDeLaSemanaId = 2
        };

        _mockAgregar
            .Setup(m => m.Ejecutar(It.IsAny<AgregarJornadaDTO>()))
            .ReturnsAsync(jornadaCreadaDTO);

        // Act
        var resultado = await _controller.Agregar(nuevaJornadaDTO);

        // Assert

        var createdAtActionResult = resultado as CreatedAtActionResult;
        createdAtActionResult.Should().NotBeNull();
        createdAtActionResult!.StatusCode.Should().Be(201);
        createdAtActionResult.ActionName.Should().Be(nameof(_controller.ObtenerPorId));
        createdAtActionResult.RouteValues!["id"].Should().Be(jornadaCreadaDTO.Id);
        var valorRetornado = createdAtActionResult.Value as ObtenerJornadaDTO;
        valorRetornado!.Should().NotBeNull();
        valorRetornado!.Id.Should().Be(jornadaCreadaDTO.Id);
        valorRetornado!.ProfesorId.Should().Be(jornadaCreadaDTO.ProfesorId);
        valorRetornado!.DiaDeLaSemanaId.Should().Be(jornadaCreadaDTO.DiaDeLaSemanaId);
        valorRetornado!.HoraInicio.Should().Be(jornadaCreadaDTO.HoraInicio);
    }

    //Agregar_LanzaExcepcion_RetornaStatusCode500
    [Fact]
    public async Task Agregar_LanzaExcepcion_RetornaStatusCode500()
    {
        // Arrange
        var nuevaJornadaDTO = new AgregarJornadaDTO
        {
            HoraInicio = new TimeSpan(8, 0, 0),
            HoraFin = new TimeSpan(14, 0, 0),
            ProfesorId = 1,
            DiaDeLaSemanaId = 2
        };

        _mockAgregar
            .Setup(m => m.Ejecutar(It.IsAny<AgregarJornadaDTO>()))
            .ThrowsAsync(new Exception("Error al agregar la jornada"));

        // Act
        var resultado = await _controller.Agregar(nuevaJornadaDTO);

        // Assert
        var objectResult = resultado as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(500);
        objectResult.Value.Should().Be("Error al agregar la jornada");
    }

    //agregar devuelve badrequest cuando el modelo no es válido
    [Fact]
    public async Task Agregar_RetornaBadRequest_CuandoModeloNoEsValido()
    {
        // Arrange
        var nuevaJornadaDTO = new AgregarJornadaDTO
        {
            HoraInicio = new TimeSpan(8, 0, 0),
            HoraFin = new TimeSpan(14, 0, 0),
            ProfesorId = -1, // ID inválido para simular error de validación
            DiaDeLaSemanaId = 2
        };
        _controller.ModelState.AddModelError("ProfesorId", "El ID del profesor es inválido.");
        // Act
        var resultado = await _controller.Agregar(nuevaJornadaDTO);
        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
        var errors = badRequestResult.Value as SerializableError;
        errors!.Should().ContainKey("ProfesorId");
    }

    [Fact]
    public async Task Agregar_RetornaBadRequest_CuandoDTOEsNulo()
    {
        // Arrange
        AgregarJornadaDTO nuevaJornadaDTO = null!;

        // Act
        var resultado = await _controller.Agregar(nuevaJornadaDTO);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
        badRequestResult.Value.Should().Be("El objeto JornadaDTO no puede ser nulo.");
    }

    [Fact]
    public async Task ObtenerTodos_RetornaOkResult_ConListaCompleta()
    {
        // Arrange
        var listaJornadasDTO = new List<ObtenerJornadaDTO>
        {
            new ObtenerJornadaDTO
            {
                Id = 1,
                HoraInicio = new TimeSpan(8, 0, 0),
                HoraFin = new TimeSpan(14, 0, 0),
                ProfesorId = 1,
                DiaDeLaSemanaId = 2
            },
            new ObtenerJornadaDTO
            {
                Id = 2,
                HoraInicio = new TimeSpan(9, 0, 0),
                HoraFin = new TimeSpan(15, 0, 0),
                ProfesorId = 2,
                DiaDeLaSemanaId = 3
            }
        };

        _mockObtenerTodos
            .Setup(m => m.Ejecutar())
            .ReturnsAsync(listaJornadasDTO);

        // Act
        var resultado = await _controller.ObtenerTodasAsync();

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        var valorRetornado = okResult.Value as List<ObtenerJornadaDTO>;
        valorRetornado!.Should().NotBeNull();
        valorRetornado!.Count.Should().Be(2);
    }

    [Fact]
    public async Task ObtenerTodos_RetornaOkResult_ConListaVacia()
    {
        // Arrange
        var listaJornadasDTO = new List<ObtenerJornadaDTO>();

        _mockObtenerTodos
            .Setup(m => m.Ejecutar())
            .ReturnsAsync(listaJornadasDTO);

        // Act
        var resultado = await _controller.ObtenerTodasAsync();

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        var valorRetornado = okResult.Value as List<ObtenerJornadaDTO>;
        valorRetornado!.Should().NotBeNull();
        valorRetornado!.Count.Should().Be(0);
    }

    [Fact]
    public async Task ObtenerTodos_LanzaExcepcion_RetornaStatus500()
    {
        // Arrange
        _mockObtenerTodos
            .Setup(m => m.Ejecutar())
            .ThrowsAsync(new Exception("Error al obtener las jornadas"));

        // Act
        var resultado = await _controller.ObtenerTodasAsync();

        // Assert
        var objectResult = resultado as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(500);
        objectResult.Value.Should().Be("Error al obtener las jornadas");
    }

    [Fact]
    public async Task ObtenerPorId_NoExiste_RetornaNotFound()
    {
        // Arrange
        long jornadaId = 1;

        _mockObtenerPorId
            .Setup(m => m.Ejecutar(jornadaId))
            .ReturnsAsync((ObtenerJornadaDTO?)null);

        // Act
        var resultado = await _controller.ObtenerPorId(jornadaId);

        // Assert
        var notFoundResult = resultado as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
        notFoundResult.Value.Should().Be($"La jornada con ID {jornadaId} no fue encontrada.");
    }

    [Fact]
    public async Task ObtenerPorId_Existe_RetornaObjetoOkCreado()
    {
        // Arrange
        long jornadaId = 1;

        var jornadaDTO = new ObtenerJornadaDTO
        {
            Id = jornadaId,
            HoraInicio = new TimeSpan(8, 0, 0),
            HoraFin = new TimeSpan(14, 0, 0),
            ProfesorId = 1,
            DiaDeLaSemanaId = 2
        };

        _mockObtenerPorId
            .Setup(m => m.Ejecutar(jornadaId))
            .ReturnsAsync(jornadaDTO);

        // Act
        var resultado = await _controller.ObtenerPorId(jornadaId);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        var valorRetornado = okResult.Value as ObtenerJornadaDTO;
        valorRetornado!.Should().NotBeNull();
        valorRetornado!.Id.Should().Be(jornadaDTO.Id);
        valorRetornado!.ProfesorId.Should().Be(jornadaDTO.ProfesorId);
        valorRetornado!.DiaDeLaSemanaId.Should().Be(jornadaDTO.DiaDeLaSemanaId);
        valorRetornado!.HoraInicio.Should().Be(jornadaDTO.HoraInicio);
    }

    [Fact]
    public async Task Actualizar_RetornaOkObjectResult_ConObjetoActualizado()
    {
        // Arrange
        var jornadaId = 1;
        var actualizarJornadaDTO = new ActualizarJornadaDTO
        {
            Id = jornadaId,
            HoraInicio = new TimeSpan(10, 0, 0),
            HoraFin = new TimeSpan(16, 0, 0),
            ProfesorId = 1,
            DiaDeLaSemanaId = 2
        };
        var jornadaActualizadaDTO = new ObtenerJornadaDTO
        {
            Id = jornadaId,
            HoraInicio = new TimeSpan(10, 0, 0),
            HoraFin = new TimeSpan(16, 0, 0),
            ProfesorId = 1,
            DiaDeLaSemanaId = 2
        };

        _mockActualizar
            .Setup(m => m.Ejecutar(It.IsAny<ActualizarJornadaDTO>()))
            .ReturnsAsync(jornadaActualizadaDTO);

        // Act
        var resultado = await _controller.Actualizar(jornadaId, actualizarJornadaDTO);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        var valorRetornado = okResult.Value as ObtenerJornadaDTO;
        valorRetornado!.Should().NotBeNull();
        valorRetornado!.Id.Should().Be(actualizarJornadaDTO.Id);
        valorRetornado!.ProfesorId.Should().Be(actualizarJornadaDTO.ProfesorId);
        valorRetornado!.DiaDeLaSemanaId.Should().Be(actualizarJornadaDTO.DiaDeLaSemanaId);
        valorRetornado!.HoraInicio.Should().Be(actualizarJornadaDTO.HoraInicio);
    }

    [Fact]
    public async Task Actualizar_NoEncontrado_RetornaNotFoundResult()
    {
        // Arrange
        var jornadaId = 1;
        var actualizarJornadaDTO = new ActualizarJornadaDTO
        {
            Id = jornadaId,
            HoraInicio = new TimeSpan(10, 0, 0),
            HoraFin = new TimeSpan(16, 0, 0),
            ProfesorId = 1,
            DiaDeLaSemanaId = 2
        };

        _mockActualizar
            .Setup(m => m.Ejecutar(It.IsAny<ActualizarJornadaDTO>()))
            .ReturnsAsync((ObtenerJornadaDTO?)null);

        // Act
        var resultado = await _controller.Actualizar(jornadaId, actualizarJornadaDTO);

        // Assert
        var notFoundResult = resultado as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
        notFoundResult.Value.Should().Be($"La jornada con ID {jornadaId} no fue encontrada para actualizar.");
    }

    [Fact]
    public async Task Actualizar_LanzaExcepcion_RetornaStatusCode500()
    {
        // Arrange
        var jornadaId = 1;
        var actualizarJornadaDTO = new ActualizarJornadaDTO
        {
            Id = jornadaId,
            HoraInicio = new TimeSpan(10, 0, 0),
            HoraFin = new TimeSpan(16, 0, 0),
            ProfesorId = 1,
            DiaDeLaSemanaId = 2
        };

        _mockActualizar
            .Setup(m => m.Ejecutar(It.IsAny<ActualizarJornadaDTO>()))
            .ThrowsAsync(new Exception("Error al actualizar la jornada"));

        // Act
        var resultado = await _controller.Actualizar(jornadaId, actualizarJornadaDTO);

        // Assert
        var objectResult = resultado as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(500);
        objectResult.Value.Should().Be("Error al actualizar la jornada");
    }

    [Fact]
    public async Task Actualizar_IdNoCoincide_RetornaBadRequestResult()
    {
        // Arrange
        var jornadaId = 1;
        var actualizarJornadaDTO = new ActualizarJornadaDTO
        {
            Id = 2, // ID diferente al proporcionado en la ruta
            HoraInicio = new TimeSpan(10, 0, 0),
            HoraFin = new TimeSpan(16, 0, 0),
            ProfesorId = 1,
            DiaDeLaSemanaId = 2
        };

        // Act
        var resultado = await _controller.Actualizar(jornadaId, actualizarJornadaDTO);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
        badRequestResult.Value.Should().Be("El ID de la jornada no coincide con el ID proporcionado en la ruta.");
    }

    //Actualizar_DTONulo_RetornaBadRequestResult
    [Fact]
    public async Task Actualizar_DTONulo_RetornaBadRequestResult()
    {
        // Arrange
        var jornadaId = 1;
        ActualizarJornadaDTO actualizarJornadaDTO = null!;

        // Act
        var resultado = await _controller.Actualizar(jornadaId, actualizarJornadaDTO);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
        badRequestResult.Value.Should().Be("El objeto ActualizarJornadaDTO no puede ser nulo.");
    }

    //Eliminar_Existente_DeberiaRetornarNoContent
    [Fact]
    public async Task Eliminar_Existente_DeberiaRetornarNoContent()
    {
        // Arrange
        long jornadaId = 1;

        _mockEliminar
            .Setup(m => m.Ejecutar(jornadaId))
            .ReturnsAsync(true);

        // Act
        var resultado = await _controller.Eliminar(jornadaId);

        // Assert
        var noContentResult = resultado as NoContentResult;
        noContentResult.Should().NotBeNull();
        noContentResult!.StatusCode.Should().Be(204);
    }

    //Eliminar_NoExistente_DeberiaRetornarNotFound
    [Fact]
    public async Task Eliminar_NoExistente_DeberiaRetornarNotFound()
    {
        // Arrange
        long jornadaId = 1;

        _mockEliminar
            .Setup(m => m.Ejecutar(jornadaId))
            .ReturnsAsync(false);

        // Act
        var resultado = await _controller.Eliminar(jornadaId);

        // Assert
        var notFoundResult = resultado as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
        notFoundResult.Value.Should().Be($"La jornada con ID {jornadaId} no fue encontrada para eliminar.");
    }

    //Eliminar_CuandoOcurreErrorEnServidor_DeberiaRetornarStatusCode500
    [Fact]
    public async Task Eliminar_CuandoOcurreErrorEnServidor_DeberiaRetornarStatusCode500()
    {
        // Arrange
        long jornadaId = 1;

        _mockEliminar
            .Setup(m => m.Ejecutar(jornadaId))
            .ThrowsAsync(new Exception("Error al eliminar la jornada"));

        // Act
        var resultado = await _controller.Eliminar(jornadaId);

        // Assert
        var objectResult = resultado as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(500);
        objectResult.Value.Should().Be("Error al eliminar la jornada");
    }


}