using FitRank_API.Application.CasosDeUso.BatallasCasosDeUso;
using FitRank_API.Application.DTOs.BatallaDTOs;
using FitRank_API.Application.UseCases.Batallas;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Enums;
using FitRank_API.Infrastructure.Persistence;
using FitRank_API.Presentacion.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace FitRank_API.tests.ControllersTests;

public class BatallaControllerTests
{
    private readonly BatallaController _controller;
    private readonly Mock<CrearBatallaCasoDeUso> _mockCrear;
    private readonly Mock<AceptarBatallaCasoDeUso> _mockAceptar;
    private readonly Mock<RechazarBatallaCasoDeUso> _mockRechazar;
    private readonly Mock<ObtenerBatallasActivasCasoDeUso> _mockObtenerActivas;
    private readonly Mock<FinalizarBatallaCasoDeUso> _mockFinalizar;
    private readonly Mock<ObtenerProgresoBatallaCasoDeUso> _mockObtenerProgreso;
    private readonly Mock<ObtenerHistorialBatallasCasoDeUso> _mockObtenerHistorial;
    private readonly Mock<ObtenerBatallasPendientesCasoDeUso> _mockObtenerPendientes;

    public BatallaControllerTests()
    {
        _mockCrear = new Mock<CrearBatallaCasoDeUso>(null);
        _mockAceptar = new Mock<AceptarBatallaCasoDeUso>(null);
        _mockRechazar = new Mock<RechazarBatallaCasoDeUso>(null);
        _mockObtenerActivas = new Mock<ObtenerBatallasActivasCasoDeUso>(null);
        _mockFinalizar = new Mock<FinalizarBatallaCasoDeUso>(null);
        _mockObtenerProgreso = new Mock<ObtenerProgresoBatallaCasoDeUso>(null);
        _mockObtenerHistorial = new Mock<ObtenerHistorialBatallasCasoDeUso>(null);
        _mockObtenerPendientes = new Mock<ObtenerBatallasPendientesCasoDeUso>(null);

        _controller = new BatallaController(
            _mockCrear.Object,
            _mockAceptar.Object,
            _mockRechazar.Object,
            _mockObtenerActivas.Object,
            _mockFinalizar.Object,
            _mockObtenerProgreso.Object,
            _mockObtenerHistorial.Object,
            _mockObtenerPendientes.Object
        );
    }

    #region Crear Tests

    [Fact]
    public async Task Crear_Exitoso_RetornaOk()
    {
        // Arrange
        var dto = new CrearBatallaDTO
        {
            SocioAId = 1,
            SocioBId = 2,
            Tipo = BatallaTipo.Puntos,
            DiasDuracion = 7
        };

        var batallaPunto = new BatallaPunto
        {
            Id = 1,
            SocioAId = 1,
            SocioBId = 2,
            Estado = BatallaEstado.Pendiente
        };

        _mockCrear.Setup(x => x.Ejecutar(dto)).ReturnsAsync(batallaPunto);

        // Act
        var result = await _controller.Crear(dto);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(batallaPunto);
    }

    [Fact]
    public async Task Crear_DtoNulo_RetornaBadRequest()
    {
        // Act
        var result = await _controller.Crear(null);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Crear_ModelStateInvalido_RetornaBadRequest()
    {
        // Arrange
        _controller.ModelState.AddModelError("SocioAId", "Requerido");
        var dto = new CrearBatallaDTO();

        // Act
        var result = await _controller.Crear(dto);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Crear_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        var dto = new CrearBatallaDTO { SocioAId = 1, SocioBId = 2 };
        _mockCrear.Setup(x => x.Ejecutar(dto)).ThrowsAsync(new Exception("Error"));

        // Act
        var result = await _controller.Crear(dto);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region Aceptar Tests

    [Fact]
    public async Task Aceptar_Exitoso_RetornaOk()
    {
        // Arrange
        _mockAceptar.Setup(x => x.Ejecutar(1)).ReturnsAsync(true);

        // Act
        var result = await _controller.Aceptar(1);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task Aceptar_IdCero_RetornaBadRequest()
    {
        // Act
        var result = await _controller.Aceptar(0);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Aceptar_IdNegativo_RetornaBadRequest()
    {
        // Act
        var result = await _controller.Aceptar(-5);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Aceptar_BatallaNoEncontrada_RetornaNotFound()
    {
        // Arrange
        _mockAceptar.Setup(x => x.Ejecutar(999)).ReturnsAsync(false);

        // Act
        var result = await _controller.Aceptar(999);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Aceptar_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        _mockAceptar.Setup(x => x.Ejecutar(1)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.Aceptar(1);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region Rechazar Tests

    [Fact]
    public async Task Rechazar_Exitoso_RetornaOk()
    {
        // Arrange
        _mockRechazar.Setup(x => x.Ejecutar(1)).ReturnsAsync(true);

        // Act
        var result = await _controller.Rechazar(1);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task Rechazar_IdCero_RetornaBadRequest()
    {
        // Act
        var result = await _controller.Rechazar(0);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Rechazar_IdNegativo_RetornaBadRequest()
    {
        // Act
        var result = await _controller.Rechazar(-10);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Rechazar_BatallaNoEncontrada_RetornaNotFound()
    {
        // Arrange
        _mockRechazar.Setup(x => x.Ejecutar(999)).ReturnsAsync(false);

        // Act
        var result = await _controller.Rechazar(999);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Rechazar_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        _mockRechazar.Setup(x => x.Ejecutar(1)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.Rechazar(1);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ObtenerActivas Tests

    [Fact]
    public async Task ObtenerActivas_Exitoso_RetornaOk()
    {
        // Arrange
        var lista = new List<HistorialBatallaDTO>
        {
            new HistorialBatallaDTO { BatallaId = 1, Estado = BatallaEstado.Activa }
        };
        _mockObtenerActivas.Setup(x => x.Ejecutar(1)).ReturnsAsync(lista);

        // Act
        var result = await _controller.ObtenerActivas(1);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(lista);
    }

    [Fact]
    public async Task ObtenerActivas_SocioIdCero_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerActivas(0);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerActivas_SocioIdNegativo_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerActivas(-3);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerActivas_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        _mockObtenerActivas.Setup(x => x.Ejecutar(1)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.ObtenerActivas(1);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region Progreso Tests

    [Fact]
    public async Task Progreso_Exitoso_RetornaOk()
    {
        // Arrange
        var progreso = new ProgresoBatallaDTO { BatallaId = 1, PuntosJugadorA = 100, PuntosJugadorB = 80 };
        _mockObtenerProgreso.Setup(x => x.Ejecutar(1)).ReturnsAsync(progreso);

        // Act
        var result = await _controller.Progreso(1);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(progreso);
    }

    [Fact]
    public async Task Progreso_IdCero_RetornaBadRequest()
    {
        // Act
        var result = await _controller.Progreso(0);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Progreso_IdNegativo_RetornaBadRequest()
    {
        // Act
        var result = await _controller.Progreso(-7);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Progreso_BatallaNoEncontrada_RetornaNotFound()
    {
        // Arrange
        _mockObtenerProgreso.Setup(x => x.Ejecutar(999)).ReturnsAsync((ProgresoBatallaDTO)null);

        // Act
        var result = await _controller.Progreso(999);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Progreso_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        _mockObtenerProgreso.Setup(x => x.Ejecutar(1)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.Progreso(1);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region FinalizarBatalla Tests

    [Fact]
    public async Task FinalizarBatalla_Exitoso_RetornaOk()
    {
        // Arrange
        var resultado = new ResultadoBatallaDTO { BatallaId = 1, GanadorId = 1 };
        _mockFinalizar.Setup(x => x.Ejecutar(1)).ReturnsAsync(resultado);

        // Act
        var result = await _controller.FinalizarBatalla(1);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(resultado);
    }

    [Fact]
    public async Task FinalizarBatalla_IdCero_RetornaBadRequest()
    {
        // Act
        var result = await _controller.FinalizarBatalla(0);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task FinalizarBatalla_IdNegativo_RetornaBadRequest()
    {
        // Act
        var result = await _controller.FinalizarBatalla(-2);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task FinalizarBatalla_ExcepcionGenerica_RetornaBadRequest()
    {
        // Arrange
        _mockFinalizar.Setup(x => x.Ejecutar(1)).ThrowsAsync(new Exception("No se puede finalizar"));

        // Act
        var result = await _controller.FinalizarBatalla(1);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    #endregion

    #region ObtenerHistorial Tests

    [Fact]
    public async Task ObtenerHistorial_Exitoso_RetornaOk()
    {
        // Arrange
        var historial = new List<HistorialBatallaDTO>
        {
            new HistorialBatallaDTO { BatallaId = 1, Estado = BatallaEstado.Finalizada }
        };
        _mockObtenerHistorial.Setup(x => x.Ejecutar(1)).ReturnsAsync(historial);

        // Act
        var result = await _controller.ObtenerHistorial(1);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(historial);
    }

    [Fact]
    public async Task ObtenerHistorial_SocioIdCero_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerHistorial(0);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerHistorial_SocioIdNegativo_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerHistorial(-8);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerHistorial_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        _mockObtenerHistorial.Setup(x => x.Ejecutar(1)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.ObtenerHistorial(1);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ObtenerPendientes Tests

    [Fact]
    public async Task ObtenerPendientes_Exitoso_RetornaOk()
    {
        // Arrange
        var pendientes = new List<HistorialBatallaDTO>
        {
            new HistorialBatallaDTO { BatallaId = 1, Estado = BatallaEstado.Pendiente }
        };
        _mockObtenerPendientes.Setup(x => x.Ejecutar(1)).ReturnsAsync(pendientes);

        // Act
        var result = await _controller.ObtenerPendientes(1);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(pendientes);
    }

    [Fact]
    public async Task ObtenerPendientes_SocioIdCero_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerPendientes(0);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerPendientes_SocioIdNegativo_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerPendientes(-4);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerPendientes_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        _mockObtenerPendientes.Setup(x => x.Ejecutar(1)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.ObtenerPendientes(1);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion
}
