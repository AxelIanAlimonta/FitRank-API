using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using FitRank_API.Presentacion.Controllers;
using FitRank_API.Application.CasosDeUso.ReporteCasosDeUso;
using FitRank_API.Application.DTOs.ReporteDTOs;
using FitRank_API.Domain.Interfaces;
using FitRank_API.Domain.Entities;
using AutoMapper;
using FitRank_API.Application.Mappings;

namespace FitRank_API.tests.ControllersTests;

public class ReporteControllerTests
{
    private readonly ReporteController _controller;
    private readonly Mock<IReporteRepositorio> _mockRepo;
    private readonly IMapper _mapper;

    public ReporteControllerTests()
    {
        _mockRepo = new Mock<IReporteRepositorio>();
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ReporteProfile>();
        });
        _mapper = config.CreateMapper();

        var obtenerTodos = new ObtenerTodosLosReportesDeGimnasioCasoDeUso(_mockRepo.Object, _mapper);
        var agregar = new AgregarReporteCasoDeUso(_mockRepo.Object, _mapper);
        var actualizar = new ActualizarReporteCasoDeUso(_mockRepo.Object, _mapper);
        var eliminar = new EliminarReporteCasoDeUso(_mockRepo.Object);
        var obtenerPorId = new ObtenerReportePorIdCasoDeUso(_mockRepo.Object, _mapper);
        var desactivar = new DesactivarReporteCasoDeUso(_mockRepo.Object);
        var obtenerPorUsuario = new ObtenerReportesPorUsuarioCasoDeUso(_mockRepo.Object, _mapper);
        var obtenerActivos = new ObtenerReportesActivosDeUnGimnasioCasoDeUso(_mockRepo.Object, _mapper);
        var obtenerInactivos = new ObtenerReportesInactivosDeUnGimnasioCasoDeUso(_mockRepo.Object, _mapper);

        _controller = new ReporteController(
            obtenerTodos,
            agregar,
            actualizar,
            eliminar,
            obtenerPorId,
            desactivar,
            obtenerPorUsuario,
            obtenerActivos,
            obtenerInactivos
        );
    }

    #region ObtenerTodosLosReportesDeGimnasio Tests

    [Fact]
    public async Task ObtenerTodosLosReportesDeGimnasio_GimnasioIdCero_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerTodosLosReportesDeGimnasio(0);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerTodosLosReportesDeGimnasio_GimnasioIdNegativo_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerTodosLosReportesDeGimnasio(-3);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerTodosLosReportesDeGimnasio_Exitoso_RetornaOk()
    {
        // Arrange
        var reportes = new List<Reporte>
        {
            new Reporte { Id = 1, Titulo = "Reporte 1", GimnasioId = 1 }
        };
        _mockRepo.Setup(x => x.ObtenerReportesPorGimnasioIdAsync(1)).ReturnsAsync(reportes);

        // Act
        var result = await _controller.ObtenerTodosLosReportesDeGimnasio(1);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task ObtenerTodosLosReportesDeGimnasio_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        _mockRepo.Setup(x => x.ObtenerReportesPorGimnasioIdAsync(1)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.ObtenerTodosLosReportesDeGimnasio(1);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region AgregarReporte Tests

    [Fact]
    public async Task AgregarReporte_DtoNulo_RetornaBadRequest()
    {
        // Act
        var result = await _controller.AgregarReporte(null!);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task AgregarReporte_ModelStateInvalido_RetornaBadRequest()
    {
        // Arrange
        _controller.ModelState.AddModelError("Titulo", "Requerido");
        var dto = new AgregarReporteDTO();

        // Act
        var result = await _controller.AgregarReporte(dto);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task AgregarReporte_Exitoso_RetornaCreatedAtAction()
    {
        // Arrange
        var dto = new AgregarReporteDTO { Titulo = "Nuevo Reporte", Descripcion = "Desc" };
        var reporte = new Reporte { Id = 1, Titulo = "Nuevo Reporte", Descripcion = "Desc" };
        _mockRepo.Setup(x => x.AgregarReporteAsync(It.IsAny<Reporte>())).ReturnsAsync(reporte);

        // Act
        var result = await _controller.AgregarReporte(dto);

        // Assert
        var createdResult = result as CreatedAtActionResult;
        createdResult.Should().NotBeNull();
        createdResult!.StatusCode.Should().Be(201);
    }

    [Fact]
    public async Task AgregarReporte_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        var dto = new AgregarReporteDTO();
        _mockRepo.Setup(x => x.AgregarReporteAsync(It.IsAny<Reporte>())).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.AgregarReporte(dto);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ActualizarReporte Tests

    [Fact]
    public async Task ActualizarReporte_IdCero_RetornaBadRequest()
    {
        // Arrange
        var dto = new ActualizarReporteDTO { Id = 0 };

        // Act
        var result = await _controller.ActualizarReporte(0, dto);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ActualizarReporte_IdNegativo_RetornaBadRequest()
    {
        // Arrange
        var dto = new ActualizarReporteDTO { Id = -5 };

        // Act
        var result = await _controller.ActualizarReporte(-5, dto);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ActualizarReporte_DtoNulo_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ActualizarReporte(1, null!);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ActualizarReporte_ModelStateInvalido_RetornaBadRequest()
    {
        // Arrange
        _controller.ModelState.AddModelError("Titulo", "Requerido");
        var dto = new ActualizarReporteDTO { Id = 1 };

        // Act
        var result = await _controller.ActualizarReporte(1, dto);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ActualizarReporte_IdNoCoincide_RetornaBadRequest()
    {
        // Arrange
        var dto = new ActualizarReporteDTO { Id = 1 };

        // Act
        var result = await _controller.ActualizarReporte(2, dto);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ActualizarReporte_Exitoso_RetornaOk()
    {
        // Arrange
        var dto = new ActualizarReporteDTO { Id = 1, Titulo = "Actualizado", Descripcion = "Desc" };
        var reporte = new Reporte { Id = 1, Titulo = "Original", Descripcion = "Desc" };
        _mockRepo.Setup(x => x.ObtenerReportePorIdAsync(1)).ReturnsAsync(reporte);
        _mockRepo.Setup(x => x.ActualizarReporteAsync(It.IsAny<Reporte>())).ReturnsAsync(reporte);

        // Act
        var result = await _controller.ActualizarReporte(1, dto);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task ActualizarReporte_NoEncontrado_RetornaNotFound()
    {
        // Arrange
        var dto = new ActualizarReporteDTO { Id = 999 };
        _mockRepo.Setup(x => x.ObtenerReportePorIdAsync(999)).ReturnsAsync((Reporte?)null);

        // Act
        var result = await _controller.ActualizarReporte(999, dto);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task ActualizarReporte_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        var dto = new ActualizarReporteDTO { Id = 1 };
        _mockRepo.Setup(x => x.ObtenerReportePorIdAsync(1)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.ActualizarReporte(1, dto);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region EliminarReporte Tests

    [Fact]
    public async Task EliminarReporte_IdCero_RetornaBadRequest()
    {
        // Act
        var result = await _controller.EliminarReporte(0);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task EliminarReporte_IdNegativo_RetornaBadRequest()
    {
        // Act
        var result = await _controller.EliminarReporte(-7);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task EliminarReporte_Exitoso_RetornaNoContent()
    {
        // Arrange
        _mockRepo.Setup(x => x.EliminarReporteAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _controller.EliminarReporte(1);

        // Assert
        var noContentResult = result as NoContentResult;
        noContentResult.Should().NotBeNull();
        noContentResult!.StatusCode.Should().Be(204);
    }

    [Fact]
    public async Task EliminarReporte_NoEncontrado_RetornaNotFound()
    {
        // Arrange
        _mockRepo.Setup(x => x.EliminarReporteAsync(999)).ReturnsAsync(false);

        // Act
        var result = await _controller.EliminarReporte(999);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task EliminarReporte_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        _mockRepo.Setup(x => x.EliminarReporteAsync(1)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.EliminarReporte(1);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ObtenerReportePorId Tests

    [Fact]
    public async Task ObtenerReportePorId_IdCero_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerReportePorId(0);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerReportePorId_IdNegativo_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerReportePorId(-4);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerReportePorId_Exitoso_RetornaOk()
    {
        // Arrange
        var reporte = new Reporte { Id = 1, Titulo = "Reporte Test" };
        _mockRepo.Setup(x => x.ObtenerReportePorIdAsync(1)).ReturnsAsync(reporte);

        // Act
        var result = await _controller.ObtenerReportePorId(1);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task ObtenerReportePorId_NoEncontrado_RetornaNotFound()
    {
        // Arrange
        _mockRepo.Setup(x => x.ObtenerReportePorIdAsync(999)).ReturnsAsync((Reporte?)null);

        // Act
        var result = await _controller.ObtenerReportePorId(999);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task ObtenerReportePorId_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        _mockRepo.Setup(x => x.ObtenerReportePorIdAsync(1)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.ObtenerReportePorId(1);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region DesactivarReporte Tests

    [Fact]
    public async Task DesactivarReporte_IdCero_RetornaBadRequest()
    {
        // Act
        var result = await _controller.DesactivarReporte(0);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task DesactivarReporte_IdNegativo_RetornaBadRequest()
    {
        // Act
        var result = await _controller.DesactivarReporte(-6);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task DesactivarReporte_Exitoso_RetornaOk()
    {
        // Arrange
        var reporte = new Reporte { Id = 1, Titulo = "Test", Activo = true };
        _mockRepo.Setup(x => x.ObtenerReportePorIdAsync(1)).ReturnsAsync(reporte);
        _mockRepo.Setup(x => x.ActualizarReporteAsync(It.IsAny<Reporte>())).ReturnsAsync(reporte);

        // Act
        var result = await _controller.DesactivarReporte(1);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task DesactivarReporte_NoEncontrado_RetornaNotFound()
    {
        // Arrange
        _mockRepo.Setup(x => x.ObtenerReportePorIdAsync(999)).ReturnsAsync((Reporte?)null);

        // Act
        var result = await _controller.DesactivarReporte(999);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task DesactivarReporte_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        _mockRepo.Setup(x => x.ObtenerReportePorIdAsync(1)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.DesactivarReporte(1);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ObtenerReportesPorUsuario Tests

    [Fact]
    public async Task ObtenerReportesPorUsuario_UsuarioIdCero_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerReportesPorUsuario(0);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerReportesPorUsuario_UsuarioIdNegativo_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerReportesPorUsuario(-2);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerReportesPorUsuario_Exitoso_RetornaOk()
    {
        // Arrange
        var reportes = new List<Reporte>
        {
            new Reporte { Id = 1, Titulo = "Reporte 1", UsuarioId = 1 }
        };
        _mockRepo.Setup(x => x.ObtenerReportesPorUsuarioIdAsync(1)).ReturnsAsync(reportes);

        // Act
        var result = await _controller.ObtenerReportesPorUsuario(1);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task ObtenerReportesPorUsuario_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        _mockRepo.Setup(x => x.ObtenerReportesPorUsuarioIdAsync(1)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.ObtenerReportesPorUsuario(1);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ObtenerReportesActivos Tests

    [Fact]
    public async Task ObtenerReportesActivos_GimnasioIdCero_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerReportesActivos(0);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerReportesActivos_GimnasioIdNegativo_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerReportesActivos(-8);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerReportesActivos_Exitoso_RetornaOk()
    {
        // Arrange
        var reportes = new List<Reporte>
        {
            new Reporte { Id = 1, Titulo = "Activo", Activo = true, GimnasioId = 1 }
        };
        _mockRepo.Setup(x => x.ObtenerReportesActivosPorGimnasioAsync(1)).ReturnsAsync(reportes);

        // Act
        var result = await _controller.ObtenerReportesActivos(1);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task ObtenerReportesActivos_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        _mockRepo.Setup(x => x.ObtenerReportesActivosPorGimnasioAsync(1)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.ObtenerReportesActivos(1);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ObtenerReportesInactivos Tests

    [Fact]
    public async Task ObtenerReportesInactivos_GimnasioIdCero_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerReportesInactivos(0);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerReportesInactivos_GimnasioIdNegativo_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerReportesInactivos(-9);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerReportesInactivos_Exitoso_RetornaOk()
    {
        // Arrange
        var reportes = new List<Reporte>
        {
            new Reporte { Id = 1, Titulo = "Inactivo", Activo = false, GimnasioId = 1 }
        };
        _mockRepo.Setup(x => x.ObtenerReportesInactivosPorGimnasioAsync(1)).ReturnsAsync(reportes);

        // Act
        var result = await _controller.ObtenerReportesInactivos(1);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task ObtenerReportesInactivos_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        _mockRepo.Setup(x => x.ObtenerReportesInactivosPorGimnasioAsync(1)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.ObtenerReportesInactivos(1);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion
}
