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
using FitRank_API.Domain.Entities;
using FitRank_API.Application.CasosDeUso.MedidaCorporalCasosDeUso;
using CasosDeUsoTests.MedidaCorporalCasosDeUsoTests;
using FitRank_API.Application.DTOs.MedidaCorporalDTOs;

namespace FitRank_API.tests.ControllersTests;

public class MedidaCorporalControllerTests
{

    private readonly MedidaCorporalController _controller;
    private readonly Mock<ActualizarMedidaCorporalCasoDeUso> _mockActualizar;
    private readonly Mock<AgregarMedidaCorporalCasoDeUso> _mockAgregar;
    private readonly Mock<EliminarMedidaCorporalCasoDeUso> _mockEliminar;
    private readonly Mock<ObtenerMedidaCorporalPorIdCasoDeUso> _mockObtenerPorId;
    private readonly Mock<ObtenerMedidasPorSocioCasoDeUso> _mockObtenerPorSocio;

    public MedidaCorporalControllerTests()
    {
        var mockRepositorio = new Mock<IMedidaCorporalRepositorio>();
        var mockMapper = new Mock<IMapper>();

        _mockActualizar = new Mock<ActualizarMedidaCorporalCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockAgregar = new Mock<AgregarMedidaCorporalCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockEliminar = new Mock<EliminarMedidaCorporalCasoDeUso>(mockRepositorio.Object);
        _mockObtenerPorId = new Mock<ObtenerMedidaCorporalPorIdCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockObtenerPorSocio = new Mock<ObtenerMedidasPorSocioCasoDeUso>(mockRepositorio.Object, mockMapper.Object);

        _controller = new MedidaCorporalController(
            _mockAgregar.Object,
            _mockActualizar.Object,
            _mockObtenerPorId.Object,
            _mockObtenerPorSocio.Object,
            _mockEliminar.Object
        );

    }

    //Agregar_RetornaCreatedAtActionResult
    [Fact]
    public async Task Agregar_RetornaCreatedAtActionResult()
    {
        // Arrange
        var nuevaMedidaDto = new AgregarMedidaCorporalDTO
        {
            SocioId = 1,
            BrazoIzquierdoCm = 30.0,
            BrazoDerechoCm = 30.5
        };

        var medidaCreadaDto = new ObtenerMedidaCorporalDTO
        {
            Id = 1,
            SocioId = 1,
            BrazoIzquierdoCm = 30.0,
            BrazoDerechoCm = 30.5
        };

        _mockAgregar
            .Setup(m => m.Ejecutar(nuevaMedidaDto))
            .ReturnsAsync(medidaCreadaDto);

        // Act
        var result = await _controller.Agregar(nuevaMedidaDto);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(medidaCreadaDto);
    }

    //Agregar_LanzaExcepcion_RetornaStatusCode500
    [Fact]
    public async Task Agregar_LanzaExcepcion_RetornaStatusCode500()
    {
        // Arrange
        var nuevaMedidaDto = new AgregarMedidaCorporalDTO
        {
            SocioId = 1,
            BrazoIzquierdoCm = 30.0,
            BrazoDerechoCm = 30.5
        };

        _mockAgregar
            .Setup(m => m.Ejecutar(nuevaMedidaDto))
            .ThrowsAsync(new Exception("Error interno del servidor."));

        // Act
        var result = await _controller.Agregar(nuevaMedidaDto);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
        statusCodeResult.Value.Should().Be("Error interno del servidor.");
    }

    //Agregar_RetornaBadRequest_CuandoDTOEsNulo
    [Fact]
    public async Task Agregar_RetornaBadRequest_CuandoDTOEsNulo()
    {
        // Act
        var result = await _controller.Agregar(null);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
        badRequestResult.Value.Should().Be("El cuerpo de la solicitud no puede ser nulo.");
    }

    //ObtenerPorId_NoExiste_RetornaNotFound
    [Fact]
    public async Task ObtenerPorId_NoExiste_RetornaNotFound()
    {
        // Arrange
        long medidaId = 999;

        _mockObtenerPorId
            .Setup(m => m.Ejecutar(medidaId))
            .ReturnsAsync((ObtenerMedidaCorporalDTO?)null);

        // Act
        var result = await _controller.ObtenerPorId(medidaId);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
        notFoundResult.Value.Should().Be($"No se encontró ninguna medida corporal con ID {medidaId}.");
    }

    //ObtenerPorId_Existe_RetornaObjetoOkCreado
    [Fact]
    public async Task ObtenerPorId_Existe_RetornaObjetoOkCreado()
    {
        // Arrange
        long medidaId = 1;
        var medidaDto = new ObtenerMedidaCorporalDTO
        {
            Id = medidaId,
            SocioId = 1,
            BrazoIzquierdoCm = 30.0,
            BrazoDerechoCm = 30.5
        };

        _mockObtenerPorId
            .Setup(m => m.Ejecutar(medidaId))
            .ReturnsAsync(medidaDto);

        // Act
        var result = await _controller.ObtenerPorId(medidaId);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(medidaDto);
    }

    //Actualizar_RetornaOkObjectResult_ConObjetoActualizado
    [Fact]
    public async Task Actualizar_RetornaOkObjectResult_ConObjetoActualizado()
    {
        // Arrange
        var medidaActualizarDto = new ActualizarMedidaCorporalDTO
        {
            Id = 1,
            SocioId = 1,
            BrazoIzquierdoCm = 32.0,
            BrazoDerechoCm = 31.5
        };

        var medidaActualizadaDto = new ObtenerMedidaCorporalDTO
        {
            Id = 1,
            SocioId = 1,
            BrazoIzquierdoCm = 32.0,
            BrazoDerechoCm = 31.5
        };

        _mockActualizar
            .Setup(m => m.Ejecutar(medidaActualizarDto))
            .ReturnsAsync(medidaActualizadaDto);

        // Act
        var result = await _controller.Actualizar(medidaActualizarDto.Id, medidaActualizarDto);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(medidaActualizadaDto);
    }

    //Actualizar_NoEncontrado_RetornaNotFoundResult
    [Fact]
    public async Task Actualizar_NoEncontrado_RetornaNotFoundResult()
    {
        // Arrange
        var medidaActualizarDto = new ActualizarMedidaCorporalDTO
        {
            Id = 999,
            SocioId = 1,
            BrazoIzquierdoCm = 32.0,
            BrazoDerechoCm = 31.5
        };

        _mockActualizar
            .Setup(m => m.Ejecutar(medidaActualizarDto))
            .ReturnsAsync((ObtenerMedidaCorporalDTO?)null);

        // Act
        var result = await _controller.Actualizar(medidaActualizarDto.Id, medidaActualizarDto);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
        notFoundResult.Value.Should().Be($"No se encontró ninguna medida corporal con ID {medidaActualizarDto.Id}.");
    }

    //Actualizar_LanzaExcepcion_RetornaStatusCode500
    [Fact]
    public async Task Actualizar_LanzaExcepcion_RetornaStatusCode500()
    {
        // Arrange
        var medidaActualizarDto = new ActualizarMedidaCorporalDTO
        {
            Id = 1,
            SocioId = 1,
            BrazoIzquierdoCm = 32.0,
            BrazoDerechoCm = 31.5
        };

        _mockActualizar
            .Setup(m => m.Ejecutar(medidaActualizarDto))
            .ThrowsAsync(new Exception("Error al actualizar medida corporal"));

        // Act
        var result = await _controller.Actualizar(medidaActualizarDto.Id, medidaActualizarDto);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
        statusCodeResult.Value.Should().Be("Error interno del servidor.");
    }

    //Actualizar_IdNoCoincide_RetornaBadRequestResult
    [Fact]
    public async Task Actualizar_IdNoCoincide_RetornaBadRequestResult()
    {
        // Arrange
        var medidaActualizarDto = new ActualizarMedidaCorporalDTO
        {
            Id = 1,
            SocioId = 1,
            BrazoIzquierdoCm = 32.0,
            BrazoDerechoCm = 31.5
        };

        // Act
        var result = await _controller.Actualizar(medidaActualizarDto.Id + 1, medidaActualizarDto);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
        badRequestResult.Value.Should().Be("El ID en la ruta no coincide con el ID en el cuerpo de la solicitud.");
    }

    //Actualizar_DTONulo_RetornaBadRequestResult
    [Fact]
    public async Task Actualizar_DTONulo_RetornaBadRequestResult()
    {
        // Act
        var result = await _controller.Actualizar(0, null);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
        badRequestResult.Value.Should().Be("El cuerpo de la solicitud no puede ser nulo.");
    }

    //Eliminar_Existente_DeberiaRetornarNoContent
    [Fact]
    public async Task Eliminar_Existente_DeberiaRetornarNoContent()
    {
        // Arrange
        long medidaId = 1;

        _mockEliminar
            .Setup(m => m.Ejecutar(medidaId))
            .ReturnsAsync(true);

        // Act
        var result = await _controller.Eliminar(medidaId);

        // Assert
        var noContentResult = result as NoContentResult;
        noContentResult.Should().NotBeNull();
        noContentResult!.StatusCode.Should().Be(204);
    }

    //Eliminar_NoExistente_DeberiaRetornarNotFound
    [Fact]
    public async Task Eliminar_NoExistente_DeberiaRetornarNotFound()
    {
        // Arrange
        long medidaId = 999;

        _mockEliminar
            .Setup(m => m.Ejecutar(medidaId))
            .ReturnsAsync(false);

        // Act
        var result = await _controller.Eliminar(medidaId);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
        notFoundResult.Value.Should().Be($"Medición no encontrada o no autorizada");
    }

    //Eliminar_CuandoOcurreErrorEnServidor_DeberiaRetornarStatusCode500
    [Fact]
    public async Task Eliminar_CuandoOcurreErrorEnServidor_DeberiaRetornarStatusCode500()
    {
        // Arrange
        long medidaId = 1;

        _mockEliminar
            .Setup(m => m.Ejecutar(medidaId))
            .ThrowsAsync(new Exception("Error interno del servidor."));

        // Act
        var result = await _controller.Eliminar(medidaId);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
        statusCodeResult.Value.Should().Be("Error interno del servidor.");
    }



}