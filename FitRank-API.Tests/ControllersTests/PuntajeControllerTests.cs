using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using FitRank_API.Presentacion.Controllers;
using FitRank_API.Application.CasosDeUso.CalculoPuntajeCasosDeUso;
using FitRank_API.Application.UseCases;
using FitRank_API.Application.DTOs.CalcularPuntajeDTOs;
using FitRank_API.Application.DTOs.PuntajeDTOs;
using FitRank_API.Domain.Interfaces;
using AutoMapper;

namespace FitRank_API.tests.ControllersTests;

public class PuntajeControllerTests
{
    private readonly PuntajeController _controller;
    private readonly Mock<ISocioRepositorio> _mockSocioRepo;
    private readonly Mock<IActividadRepositorio> _mockActividadRepo;
    private readonly Mock<IGrupoMuscularRepositorio> _mockGrupoMuscularRepo;
    private readonly Mock<IMapper> _mockMapper;

    public PuntajeControllerTests()
    {
        _mockSocioRepo = new Mock<ISocioRepositorio>();
        _mockActividadRepo = new Mock<IActividadRepositorio>();
        _mockGrupoMuscularRepo = new Mock<IGrupoMuscularRepositorio>();
        _mockMapper = new Mock<IMapper>();

        var calcularEstadistica = new CalcularEstadisticaCorporalSocioCasoDeUso(_mockSocioRepo.Object);
        var calcularCombinada = new CalcularEstadisticaCombinadaPuntajeSocioCasoDeUso(_mockSocioRepo.Object, _mockActividadRepo.Object);
        var obtenerPorGrupo = new ObtenerPuntajePorGrupoMuscularSocioCasoDeUso(_mockSocioRepo.Object);
        var obtenerRanking = new ObtenerRankingSociosCasoDeUso(_mockSocioRepo.Object);
        var obtenerPuntajeTotal = new ObtenerPuntajeTotalSocioCasoDeUso(_mockSocioRepo.Object);
        var obtenerRankingPorFecha = new ObtenerRankingPorFechaCasoDeUso(_mockSocioRepo.Object, _mockMapper.Object);
        var obtenerRankingPorGrupo = new ObtenerRankingPorGrupoMuscularCasoDeUso(_mockSocioRepo.Object, _mockMapper.Object, _mockGrupoMuscularRepo.Object);
        var obtenerRankingFiltrado = new ObtenerRankingFiltradoCasoDeUso(_mockSocioRepo.Object, _mockMapper.Object, _mockGrupoMuscularRepo.Object);

        _controller = new PuntajeController(
            calcularEstadistica,
            calcularCombinada,
            obtenerPorGrupo,
            obtenerRanking,
            obtenerPuntajeTotal,
            obtenerRankingPorFecha,
            obtenerRankingPorGrupo,
            obtenerRankingFiltrado
        );
    }

    #region ObtenerEstadisticaCorporal Tests

    [Fact]
    public async Task ObtenerEstadisticaCorporal_SocioIdCero_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerEstadisticaCorporal(0);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerEstadisticaCorporal_SocioIdNegativo_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerEstadisticaCorporal(-5);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerEstadisticaCorporal_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        _mockSocioRepo.Setup(x => x.ObtenerSocioConMedidasAsync(1)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.ObtenerEstadisticaCorporal(1);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ObtenerPuntajeCombinado Tests

    [Fact]
    public async Task ObtenerPuntajeCombinado_SocioIdCero_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerPuntajeCombinado(0);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerPuntajeCombinado_SocioIdNegativo_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerPuntajeCombinado(-3);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerPuntajeCombinado_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        _mockSocioRepo.Setup(x => x.ObtenerSocioConEntrenamientosAsync(1)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.ObtenerPuntajeCombinado(1);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ObtenerPuntajePorGrupoMuscular Tests

    [Fact]
    public async Task ObtenerPuntajePorGrupoMuscular_SocioIdCero_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerPuntajePorGrupoMuscular(0);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerPuntajePorGrupoMuscular_SocioIdNegativo_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerPuntajePorGrupoMuscular(-7);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerPuntajePorGrupoMuscular_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        _mockSocioRepo.Setup(x => x.ObtenerSocioConEntrenamientosAsync(1)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.ObtenerPuntajePorGrupoMuscular(1);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ObtenerRanking Tests

    [Fact]
    public async Task ObtenerRanking_GimnasioIdCero_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerRanking(0, 20);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerRanking_GimnasioIdNegativo_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerRanking(-2, 20);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerRanking_CantidadCero_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerRanking(1, 0);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerRanking_CantidadNegativa_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerRanking(1, -5);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerRanking_SinDatos_RetornaNotFound()
    {
        // Arrange
        _mockSocioRepo.Setup(x => x.ObtenerRankingGeneralAsync(1, 20)).ReturnsAsync(new List<SocioRankingDto>());

        // Act
        var result = await _controller.ObtenerRanking(1, 20);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task ObtenerRanking_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        _mockSocioRepo.Setup(x => x.ObtenerRankingGeneralAsync(1, 20)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.ObtenerRanking(1, 20);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ObtenerRankingPorGrupoMuscular Tests

    [Fact]
    public async Task ObtenerRankingPorGrupoMuscular_GrupoIdCero_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerRankingPorGrupoMuscular(0, 1, 20);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerRankingPorGrupoMuscular_GrupoIdNegativo_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerRankingPorGrupoMuscular(-3, 1, 20);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerRankingPorGrupoMuscular_GimnasioIdCero_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerRankingPorGrupoMuscular(1, 0, 20);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerRankingPorGrupoMuscular_GimnasioIdNegativo_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerRankingPorGrupoMuscular(1, -5, 20);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerRankingPorGrupoMuscular_CantidadCero_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerRankingPorGrupoMuscular(1, 1, 0);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerRankingPorGrupoMuscular_CantidadNegativa_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerRankingPorGrupoMuscular(1, 1, -10);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerRankingPorGrupoMuscular_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        _mockGrupoMuscularRepo.Setup(x => x.ObtenerPorIdAsync(1)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.ObtenerRankingPorGrupoMuscular(1, 1, 20);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ObtenerRankingPorFecha Tests

    [Fact]
    public async Task ObtenerRankingPorFecha_GimnasioIdCero_RetornaBadRequest()
    {
        // Arrange
        var desde = DateTime.Now;
        var hasta = DateTime.Now.AddDays(7);

        // Act
        var result = await _controller.ObtenerRankingPorFecha(0, desde, hasta, 20);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerRankingPorFecha_GimnasioIdNegativo_RetornaBadRequest()
    {
        // Arrange
        var desde = DateTime.Now;
        var hasta = DateTime.Now.AddDays(7);

        // Act
        var result = await _controller.ObtenerRankingPorFecha(-1, desde, hasta, 20);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerRankingPorFecha_CantidadCero_RetornaBadRequest()
    {
        // Arrange
        var desde = DateTime.Now;
        var hasta = DateTime.Now.AddDays(7);

        // Act
        var result = await _controller.ObtenerRankingPorFecha(1, desde, hasta, 0);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerRankingPorFecha_CantidadNegativa_RetornaBadRequest()
    {
        // Arrange
        var desde = DateTime.Now;
        var hasta = DateTime.Now.AddDays(7);

        // Act
        var result = await _controller.ObtenerRankingPorFecha(1, desde, hasta, -3);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerRankingPorFecha_DesdeMayorQueHasta_RetornaBadRequest()
    {
        // Arrange
        var desde = DateTime.Now.AddDays(10);
        var hasta = DateTime.Now;

        // Act
        var result = await _controller.ObtenerRankingPorFecha(1, desde, hasta, 20);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerRankingPorFecha_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        var desde = DateTime.Now;
        var hasta = DateTime.Now.AddDays(7);
        _mockSocioRepo.Setup(x => x.ObtenerSociosParaRankingAsync(It.IsAny<long>()))
            .ThrowsAsync(new Exception());

        // Act
        var result = await _controller.ObtenerRankingPorFecha(1, desde, hasta, 20);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ObtenerPuntajeTotal Tests

    [Fact]
    public async Task ObtenerPuntajeTotal_SocioIdCero_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerPuntajeTotal(0);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerPuntajeTotal_SocioIdNegativo_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerPuntajeTotal(-8);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerPuntajeTotal_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        _mockSocioRepo.Setup(x => x.ObtenerSocioConEntrenamientosAsync(1)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.ObtenerPuntajeTotal(1);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ObtenerRankingFiltrado Tests

    [Fact]
    public async Task ObtenerRankingFiltrado_GimnasioIdCero_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerRankingFiltrado(0, null, null, null, 20);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerRankingFiltrado_GimnasioIdNegativo_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerRankingFiltrado(-4, null, null, null, 20);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerRankingFiltrado_GrupoIdCero_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerRankingFiltrado(1, 0, null, null, 20);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerRankingFiltrado_GrupoIdNegativo_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerRankingFiltrado(1, -2, null, null, 20);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerRankingFiltrado_CantidadCero_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerRankingFiltrado(1, null, null, null, 0);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerRankingFiltrado_CantidadNegativa_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerRankingFiltrado(1, null, null, null, -6);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerRankingFiltrado_DesdeMayorQueHasta_RetornaBadRequest()
    {
        // Arrange
        var desde = DateOnly.FromDateTime(DateTime.Now.AddDays(10));
        var hasta = DateOnly.FromDateTime(DateTime.Now);

        // Act
        var result = await _controller.ObtenerRankingFiltrado(1, null, desde, hasta, 20);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerRankingFiltrado_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        _mockSocioRepo.Setup(x => x.ObtenerSociosParaRankingAsync(1)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.ObtenerRankingFiltrado(1, null, null, null, 20);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion
}
