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
using FitRank_API.Application.CasosDeUso.JornadaCasosDeUso;
using FitRank_API.Application.DTOs.JornadaDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Application.CasosDeUso.MaquinaCasosDeUso;
using FitRank_API.Application.DTOs.MaquinaDTOs;

namespace FitRank_API.tests.ControllersTests;

public class MaquinaControllerTests
{

    private readonly MaquinaController _controller;
    private readonly Mock<ActualizarMaquinaCasoDeUso> _mockActualizar;
    private readonly Mock<AgregarMaquinaCasoDeUso> _mockAgregar;
    private readonly Mock<EliminarMaquinaCasoDeUso> _mockEliminar;
    private readonly Mock<ObtenerMaquinaPorIdCasoDeUso> _mockObtenerPorId;
    private readonly Mock<ObtenerMaquinasCasoDeUso> _mockObtenerTodos;

    public MaquinaControllerTests()
    {
        var mockRepositorio = new Mock<IMaquinaRepositorio>();
        var mockMapper = new Mock<IMapper>();

        _mockActualizar = new Mock<ActualizarMaquinaCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockAgregar = new Mock<AgregarMaquinaCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockEliminar = new Mock<EliminarMaquinaCasoDeUso>(mockRepositorio.Object);
        _mockObtenerPorId = new Mock<ObtenerMaquinaPorIdCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockObtenerTodos = new Mock<ObtenerMaquinasCasoDeUso>(mockRepositorio.Object, mockMapper.Object);

        _controller = new MaquinaController(
            _mockObtenerTodos.Object,
            _mockAgregar.Object,
            _mockActualizar.Object,
            _mockEliminar.Object,
            _mockObtenerPorId.Object
        );

    }

    //Agregar_RetornaCreatedAtActionResult
    [Fact]
    public async Task Agregar_RetornaCreatedAtActionResult()
    {
        // Arrange
        var nuevaMaquinaDto = new AgregarMaquinaDTO
        {
            GimnasioId = 1,
            Nombre = "Maquina de Prueba",
            UrlImagen = "http://imagen.com/maquina.jpg",
            Qr = "QR12345"
        };

        var maquinaDtoCreada = new ObtenerMaquinaDTO
        {
            Id = 1,
            GimnasioId = 1,
            Nombre = "Maquina de Prueba",
            UrlImagen = "http://imagen.com/maquina.jpg",
            Qr = "QR12345"
        };

        _mockAgregar
            .Setup(m => m.Ejecutar(nuevaMaquinaDto))
            .ReturnsAsync(maquinaDtoCreada);

        // Act
        var resultado = await _controller.Agregar(nuevaMaquinaDto);

        // Assert
        var createdAtActionResult = resultado as CreatedAtActionResult;
        createdAtActionResult.Should().NotBeNull();
        createdAtActionResult!.StatusCode.Should().Be(201);
        createdAtActionResult.Value.Should().BeEquivalentTo(maquinaDtoCreada);
    }

    //Agregar_LanzaExcepcion_RetornaStatusCode500
    [Fact]
    public async Task Agregar_LanzaExcepcion_RetornaStatusCode500()
    {
        // Arrange
        var nuevaMaquinaDto = new AgregarMaquinaDTO
        {
            GimnasioId = 1,
            Nombre = "Maquina de Prueba",
            UrlImagen = "http://imagen.com/maquina.jpg",
            Qr = "QR12345"
        };

        _mockAgregar
            .Setup(m => m.Ejecutar(nuevaMaquinaDto))
            .ThrowsAsync(new Exception("Error de servidor."));

        // Act
        var resultado = await _controller.Agregar(nuevaMaquinaDto);

        // Assert
        var objectResult = resultado as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(500);
        objectResult.Value.Should().Be("Error de servidor.");
    }

    //Agregar_RetornaBadRequest_CuandoDTOEsNulo
    [Fact]
    public async Task Agregar_RetornaBadRequest_CuandoDTOEsNulo()
    {
        // Act
        var resultado = await _controller.Agregar(null);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
        badRequestResult.Value.Should().Be("El objeto no puede ser nulo.");
    }

    //ObtenerTodos_RetornaOkResult_ConListaCompleta
    [Fact]
    public async Task ObtenerTodos_RetornaOkResult_ConListaCompleta()
    {
        // Arrange
        var maquinasDto = new List<ObtenerMaquinaDTO>
        {
            new ObtenerMaquinaDTO { Id = 1, GimnasioId = 1, Nombre = "Maquina 1", UrlImagen = "http://imagen.com/maquina1.jpg", Qr = "QR1" },
            new ObtenerMaquinaDTO { Id = 2, GimnasioId = 1, Nombre = "Maquina 2", UrlImagen = "http://imagen.com/maquina2.jpg", Qr = "QR2" }
        };

        _mockObtenerTodos
            .Setup(m => m.Ejecutar())
            .ReturnsAsync(maquinasDto);

        // Act
        var resultado = await _controller.ObtenerTodas();

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(maquinasDto);
    }

    //ObtenerTodos_RetornaOkResult_ConListaVacia
    [Fact]
    public async Task ObtenerTodos_RetornaOkResult_ConListaVacia()
    {
        // Arrange
        var maquinasDto = new List<ObtenerMaquinaDTO>();

        _mockObtenerTodos
            .Setup(m => m.Ejecutar())
            .ReturnsAsync(maquinasDto);

        // Act
        var resultado = await _controller.ObtenerTodas();

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(maquinasDto);
    }

    //ObtenerTodos_LanzaExcepcion_RetornaStatus500
    [Fact]
    public async Task ObtenerTodos_LanzaExcepcion_RetornaStatus500()
    {
        // Arrange
        _mockObtenerTodos
            .Setup(m => m.Ejecutar())
            .ThrowsAsync(new Exception("Error de servidor."));

        // Act
        var resultado = await _controller.ObtenerTodas();

        // Assert
        var objectResult = resultado as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(500);
        objectResult.Value.Should().Be("Error de servidor.");
    }

    //ObtenerPorId_NoExiste_RetornaNotFound
    [Fact]
    public async Task ObtenerPorId_NoExiste_RetornaNotFound()
    {
        // Arrange
        long maquinaId = 999;

        _mockObtenerPorId
            .Setup(m => m.Ejecutar(maquinaId))
            .ReturnsAsync((ObtenerMaquinaDTO?)null);

        // Act
        var resultado = await _controller.ObtenerPorId(maquinaId);

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
        long maquinaId = 1;
        var maquinaDto = new ObtenerMaquinaDTO
        {
            Id = maquinaId,
            GimnasioId = 1,
            Nombre = "Maquina de Prueba",
            UrlImagen = "http://imagen.com/maquina.jpg",
            Qr = "QR12345"
        };

        _mockObtenerPorId
            .Setup(m => m.Ejecutar(maquinaId))
            .ReturnsAsync(maquinaDto);

        // Act
        var resultado = await _controller.ObtenerPorId(maquinaId);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(maquinaDto);
    }

    //Actualizar_RetornaOkObjectResult_ConObjetoActualizado
    [Fact]
    public async Task Actualizar_RetornaOkObjectResult_ConObjetoActualizado()
    {
        // Arrange
        long maquinaId = 1;
        var actualizarMaquinaDto = new ActualizarMaquinaDTO
        {
            Id = maquinaId,
            GimnasioId = 1,
            Nombre = "Maquina Actualizada",
            UrlImagen = "http://imagen.com/maquina_actualizada.jpg",
            Qr = "QR12345_UPDATED"
        };

        var maquinaDtoActualizada = new ObtenerMaquinaDTO
        {
            Id = maquinaId,
            GimnasioId = 1,
            Nombre = "Maquina Actualizada",
            UrlImagen = "http://imagen.com/maquina_actualizada.jpg",
            Qr = "QR12345_UPDATED"
        };

        _mockActualizar
            .Setup(m => m.Ejecutar(actualizarMaquinaDto))
            .ReturnsAsync(maquinaDtoActualizada);

        // Act
        var resultado = await _controller.Actualizar(maquinaId, actualizarMaquinaDto);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(maquinaDtoActualizada);
    }

    //Actualizar_NoEncontrado_RetornaNotFoundResult
    [Fact]
    public async Task Actualizar_NoEncontrado_RetornaNotFoundResult()
    {
        // Arrange
        long maquinaId = 999;
        var actualizarMaquinaDto = new ActualizarMaquinaDTO
        {
            Id = maquinaId,
            GimnasioId = 1,
            Nombre = "Maquina Actualizada",
            UrlImagen = "http://imagen.com/maquina_actualizada.jpg",
            Qr = "QR12345_UPDATED"
        };

        _mockActualizar
            .Setup(m => m.Ejecutar(actualizarMaquinaDto))
            .ReturnsAsync((ObtenerMaquinaDTO?)null);

        // Act
        var resultado = await _controller.Actualizar(maquinaId, actualizarMaquinaDto);

        // Assert
        var notFoundResult = resultado as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    //Actualizar_LanzaExcepcion_RetornaStatusCode500
    [Fact]
    public async Task Actualizar_LanzaExcepcion_RetornaStatusCode500()
    {
        // Arrange
        long maquinaId = 1;
        var actualizarMaquinaDto = new ActualizarMaquinaDTO
        {
            Id = maquinaId,
            GimnasioId = 1,
            Nombre = "Maquina Actualizada",
            UrlImagen = "http://imagen.com/maquina_actualizada.jpg",
            Qr = "QR12345_UPDATED"
        };

        _mockActualizar
            .Setup(m => m.Ejecutar(actualizarMaquinaDto))
            .ThrowsAsync(new Exception("Error de servidor."));

        // Act
        var resultado = await _controller.Actualizar(maquinaId, actualizarMaquinaDto);

        // Assert
        var objectResult = resultado as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(500);
        objectResult.Value.Should().Be("Error de servidor.");
    }

    //Actualizar_IdNoCoincide_RetornaBadRequestResult
    [Fact]
    public async Task Actualizar_IdNoCoincide_RetornaBadRequestResult()
    {
        // Arrange
        long maquinaId = 1;
        var actualizarMaquinaDto = new ActualizarMaquinaDTO
        {
            GimnasioId = 1,
            Nombre = "Maquina Actualizada",
            UrlImagen = "http://imagen.com/maquina_actualizada.jpg",
            Qr = "QR12345_UPDATED"
        };

        // Act
        var resultado = await _controller.Actualizar(maquinaId + 1, actualizarMaquinaDto);

        // Assertr
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
        badRequestResult.Value.Should().Be("El ID de la ruta no coincide con el ID del cuerpo de la solicitud.");
    }

    //Actualizar_DTONulo_RetornaBadRequestResult
    [Fact]
    public async Task Actualizar_DTONulo_RetornaBadRequestResult()
    {
        // Arrange
        long maquinaId = 1;

        // Act
        var resultado = await _controller.Actualizar(maquinaId, null);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
        badRequestResult.Value.Should().Be("El objeto no puede ser nulo.");
    }

    //Eliminar_Existente_DeberiaRetornarNoContent
    [Fact]
    public async Task Eliminar_Existente_DeberiaRetornarNoContent()
    {
        // Arrange
        long maquinaId = 1;

        _mockEliminar
            .Setup(m => m.Ejecutar(maquinaId))
            .ReturnsAsync(true);

        // Act
        var resultado = await _controller.Eliminar(maquinaId);

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
        long maquinaId = 999;

        _mockEliminar
            .Setup(m => m.Ejecutar(maquinaId))
            .ReturnsAsync(false);

        // Act
        var resultado = await _controller.Eliminar(maquinaId);

        // Assert
        var notFoundResult = resultado as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
        notFoundResult.Value.Should().Be("Maquina no encontrada.");
    }

    //Eliminar_CuandoOcurreErrorEnServidor_DeberiaRetornarStatusCode500
    [Fact]
    public async Task Eliminar_CuandoOcurreErrorEnServidor_DeberiaRetornarStatusCode500()
    {
        // Arrange
        long maquinaId = 1;

        _mockEliminar
            .Setup(m => m.Ejecutar(maquinaId))
            .ThrowsAsync(new Exception("Error de servidor."));

        // Act
        var resultado = await _controller.Eliminar(maquinaId);

        // Assert
        var objectResult = resultado as ObjectResult;
        objectResult.Should().NotBeNull();
        objectResult!.StatusCode.Should().Be(500);
        objectResult.Value.Should().Be("Error de servidor.");
    }
}