using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using FitRank_API.Presentacion.Controllers;
using FitRank_API.Application.CasosDeUso.RutinaCasosDeUso;
using FitRank_API.Application.DTOs.RutinaDTOs;
using FitRank_API.Domain.Interfaces;
using FitRank_API.Domain.Entities;
using AutoMapper;
using FitRank_API.Application.Mappings;
using FitRank.API.Application.Rutinas.Abstractions;

namespace FitRank_API.tests.ControllersTests;

public class RutinaControllerTests
{
    private readonly RutinaController _controller;
    private readonly Mock<IRutinaRepositorio> _mockRepo;
    private readonly IMapper _mapper;
    private readonly Mock<IRoutineRulesRunner> _mockRulesRunner;
    private readonly Mock<IRoutineBuilder> _mockRoutineBuilder;

    public RutinaControllerTests()
    {
        _mockRepo = new Mock<IRutinaRepositorio>();
        _mockRulesRunner = new Mock<IRoutineRulesRunner>();
        _mockRoutineBuilder = new Mock<IRoutineBuilder>();
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<RutinaProfile>();
        });
        _mapper = config.CreateMapper();

        var agregar = new AgregarRutinaCasoDeUso(_mockRepo.Object, _mapper);
        var obtenerPorId = new ObtenerRutinaPorIdCasoDeUso(_mockRepo.Object, _mapper);
        var actualizar = new ActualizarRutinaCasoDeUso(_mockRepo.Object, _mapper);
        var eliminar = new EliminarRutinaCasoDeUso(_mockRepo.Object);
        var obtenerTodos = new ObtenerTodasLasRutinasCasoDeUso(_mockRepo.Object, _mapper);
        var generarIA = new GenerarRutinaIACasoDeUso(_mockRulesRunner.Object, _mockRoutineBuilder.Object);
        var confirmarIA = new ConfirmarRutinaIACasoDeUso(_mockRepo.Object);
        var obtenerCompleta = new ObtenerRutinaCompletaCasoDeUso(_mockRepo.Object, _mapper);
        var obtenerFavoritas = new ObtenerRutinasFavoritasCasoDeUso(_mockRepo.Object);
        var marcarFavorita = new MarcarDesmarcarRutinaFavoritaCasoDeUso(_mockRepo.Object);
        var cambiarEstado = new CambiarEstadoRutinaCasoDeUso(_mockRepo.Object);

        _controller = new RutinaController(
            agregar,
            obtenerPorId,
            actualizar,
            eliminar,
            obtenerTodos,
            generarIA,
            confirmarIA,
            obtenerCompleta,
            obtenerFavoritas,
            marcarFavorita,
            cambiarEstado
        );
    }

    #region Agregar Tests

    [Fact]
    public async Task Agregar_DtoNulo_RetornaBadRequest()
    {
        var result = await _controller.Agregar(null!);
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Agregar_Exitoso_RetornaCreatedAtAction()
    {
        var dto = new AgregarRutinaDTO { Nombre = "Nueva Rutina" };
        var rutina = new Rutina { Id = 1, Nombre = "Nueva Rutina" };
        _mockRepo.Setup(x => x.AgregarAsync(It.IsAny<Rutina>())).ReturnsAsync(rutina);

        var result = await _controller.Agregar(dto);

        var createdResult = result as CreatedAtActionResult;
        createdResult.Should().NotBeNull();
        createdResult!.StatusCode.Should().Be(201);
    }

    [Fact]
    public async Task Agregar_ExcepcionGenerica_RetornaInternalServerError()
    {
        var dto = new AgregarRutinaDTO();
        _mockRepo.Setup(x => x.AgregarAsync(It.IsAny<Rutina>())).ThrowsAsync(new Exception());

        var result = await _controller.Agregar(dto);

        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ObtenerPorId Tests

    [Fact]
    public async Task ObtenerPorId_IdCero_RetornaBadRequest()
    {
        var result = await _controller.ObtenerPorId(0);
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerPorId_IdNegativo_RetornaBadRequest()
    {
        var result = await _controller.ObtenerPorId(-5);
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerPorId_Exitoso_RetornaOk()
    {
        var rutina = new Rutina { Id = 1, Nombre = "Test" };
        _mockRepo.Setup(x => x.ObtenerPorIdAsync(1)).ReturnsAsync(rutina);

        var result = await _controller.ObtenerPorId(1);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task ObtenerPorId_NoEncontrado_RetornaNotFound()
    {
        _mockRepo.Setup(x => x.ObtenerPorIdAsync(999)).ReturnsAsync((Rutina?)null);

        var result = await _controller.ObtenerPorId(999);

        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task ObtenerPorId_ExcepcionGenerica_RetornaInternalServerError()
    {
        _mockRepo.Setup(x => x.ObtenerPorIdAsync(1)).ThrowsAsync(new Exception());

        var result = await _controller.ObtenerPorId(1);

        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region Actualizar Tests

    [Fact]
    public async Task Actualizar_IdCero_RetornaBadRequest()
    {
        var dto = new ActualizarRutinaDTO { Id = 0 };

        var result = await _controller.Actualizar(0, dto);

        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Actualizar_IdNegativo_RetornaBadRequest()
    {
        var dto = new ActualizarRutinaDTO { Id = -3 };

        var result = await _controller.Actualizar(-3, dto);

        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Actualizar_DtoNulo_RetornaBadRequest()
    {
        var result = await _controller.Actualizar(1, null!);

        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Actualizar_IdNoCoincide_RetornaBadRequest()
    {
        var dto = new ActualizarRutinaDTO { Id = 1 };

        var result = await _controller.Actualizar(2, dto);

        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Actualizar_Exitoso_RetornaOk()
    {
        var dto = new ActualizarRutinaDTO { Id = 1, Nombre = "Actualizada" };
        var rutina = new Rutina { Id = 1, Nombre = "Original" };
        _mockRepo.Setup(x => x.ObtenerPorIdAsync(1)).ReturnsAsync(rutina);
        _mockRepo.Setup(x => x.ActualizarAsync(It.IsAny<Rutina>())).ReturnsAsync(rutina);

        var result = await _controller.Actualizar(1, dto);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task Actualizar_NoEncontrado_RetornaNotFound()
    {
        var dto = new ActualizarRutinaDTO { Id = 999 };
        _mockRepo.Setup(x => x.ObtenerPorIdAsync(999)).ReturnsAsync((Rutina?)null);

        var result = await _controller.Actualizar(999, dto);

        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Actualizar_ExcepcionGenerica_RetornaInternalServerError()
    {
        var dto = new ActualizarRutinaDTO { Id = 1 };
        var rutina = new Rutina { Id = 1, Nombre = "Test" };
        _mockRepo.Setup(x => x.ObtenerPorIdAsync(1)).ReturnsAsync(rutina);
        _mockRepo.Setup(x => x.ActualizarAsync(It.IsAny<Rutina>())).ThrowsAsync(new Exception());

        var result = await _controller.Actualizar(1, dto);

        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region Eliminar Tests

    [Fact]
    public async Task Eliminar_IdCero_RetornaBadRequest()
    {
        var result = await _controller.Eliminar(0);
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Eliminar_IdNegativo_RetornaBadRequest()
    {
        var result = await _controller.Eliminar(-7);
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Eliminar_Exitoso_RetornaNoContent()
    {
        _mockRepo.Setup(x => x.EliminarAsync(1)).ReturnsAsync(true);

        var result = await _controller.Eliminar(1);

        var noContentResult = result as NoContentResult;
        noContentResult.Should().NotBeNull();
        noContentResult!.StatusCode.Should().Be(204);
    }

    [Fact]
    public async Task Eliminar_NoEncontrado_RetornaNotFound()
    {
        _mockRepo.Setup(x => x.EliminarAsync(999)).ReturnsAsync(false);

        var result = await _controller.Eliminar(999);

        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Eliminar_ExcepcionGenerica_RetornaInternalServerError()
    {
        _mockRepo.Setup(x => x.EliminarAsync(1)).ThrowsAsync(new Exception());

        var result = await _controller.Eliminar(1);

        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ObtenerRutinaCompletaPorSocio Tests

    [Fact]
    public async Task ObtenerRutinaCompletaPorSocio_SocioIdCero_RetornaBadRequest()
    {
        var result = await _controller.ObtenerRutinaCompletaPorSocio(0);
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerRutinaCompletaPorSocio_SocioIdNegativo_RetornaBadRequest()
    {
        var result = await _controller.ObtenerRutinaCompletaPorSocio(-3);
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerRutinaCompletaPorSocio_Exitoso_RetornaOk()
    {
        var rutinas = new List<Rutina> { new Rutina { Id = 1, Nombre = "Test", Sesiones = new List<Sesion>() } };
        _mockRepo.Setup(x => x.ObtenerRutinasPorSocioAsync(1)).ReturnsAsync(rutinas);

        var result = await _controller.ObtenerRutinaCompletaPorSocio(1);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task ObtenerRutinaCompletaPorSocio_SinRutinas_RetornaNotFound()
    {
        _mockRepo.Setup(x => x.ObtenerRutinasPorSocioAsync(1)).ReturnsAsync(new List<Rutina>());

        var result = await _controller.ObtenerRutinaCompletaPorSocio(1);

        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task ObtenerRutinaCompletaPorSocio_ExcepcionGenerica_RetornaInternalServerError()
    {
        _mockRepo.Setup(x => x.ObtenerRutinasPorSocioAsync(1)).ThrowsAsync(new Exception());

        var result = await _controller.ObtenerRutinaCompletaPorSocio(1);

        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region CambiarFavorita Tests

    [Fact]
    public async Task CambiarFavorita_RutinaIdCero_RetornaBadRequest()
    {
        var result = await _controller.CambiarFavorita(0, true);
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task CambiarFavorita_RutinaIdNegativo_RetornaBadRequest()
    {
        var result = await _controller.CambiarFavorita(-4, true);
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task CambiarFavorita_Exitoso_RetornaOk()
    {
        _mockRepo.Setup(x => x.MarcarFavoritaAsync(1, true)).ReturnsAsync(true);

        var result = await _controller.CambiarFavorita(1, true);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task CambiarFavorita_NoEncontrada_RetornaNotFound()
    {
        _mockRepo.Setup(x => x.MarcarFavoritaAsync(999, true)).ReturnsAsync(false);

        var result = await _controller.CambiarFavorita(999, true);

        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task CambiarFavorita_ExcepcionGenerica_RetornaInternalServerError()
    {
        _mockRepo.Setup(x => x.MarcarFavoritaAsync(1, true)).ThrowsAsync(new Exception());

        var result = await _controller.CambiarFavorita(1, true);

        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region CambiarEstado Tests

    [Fact]
    public async Task CambiarEstado_RutinaIdCero_RetornaBadRequest()
    {
        var result = await _controller.CambiarEstado(0, true);
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task CambiarEstado_RutinaIdNegativo_RetornaBadRequest()
    {
        var result = await _controller.CambiarEstado(-6, true);
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task CambiarEstado_Exitoso_RetornaOk()
    {
        _mockRepo.Setup(x => x.CambiarEstadoRutinaAsync(1, true)).ReturnsAsync(true);

        var result = await _controller.CambiarEstado(1, true);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task CambiarEstado_NoEncontrada_RetornaNotFound()
    {
        _mockRepo.Setup(x => x.CambiarEstadoRutinaAsync(999, true)).ReturnsAsync(false);

        var result = await _controller.CambiarEstado(999, true);

        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task CambiarEstado_ExcepcionGenerica_RetornaInternalServerError()
    {
        _mockRepo.Setup(x => x.CambiarEstadoRutinaAsync(1, true)).ThrowsAsync(new Exception());

        var result = await _controller.CambiarEstado(1, true);

        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region GetFavoritas Tests

    [Fact]
    public async Task GetFavoritas_SocioIdCero_RetornaBadRequest()
    {
        var result = await _controller.GetFavoritas(0);
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task GetFavoritas_SocioIdNegativo_RetornaBadRequest()
    {
        var result = await _controller.GetFavoritas(-2);
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task GetFavoritas_Exitoso_RetornaOk()
    {
        var rutinas = new List<Rutina> { new Rutina { Id = 1, Favorita = true } };
        _mockRepo.Setup(x => x.ObtenerFavoritasPorSocioAsync(1)).ReturnsAsync(rutinas);

        var result = await _controller.GetFavoritas(1);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task GetFavoritas_ExcepcionGenerica_RetornaInternalServerError()
    {
        _mockRepo.Setup(x => x.ObtenerFavoritasPorSocioAsync(1)).ThrowsAsync(new Exception());

        var result = await _controller.GetFavoritas(1);

        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region GetFavoritasGimnasio Tests

    [Fact]
    public async Task GetFavoritasGimnasio_GimnasioIdCero_RetornaBadRequest()
    {
        var result = await _controller.GetFavoritasGimnasio(0);
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task GetFavoritasGimnasio_GimnasioIdNegativo_RetornaBadRequest()
    {
        var result = await _controller.GetFavoritasGimnasio(-8);
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task GetFavoritasGimnasio_Exitoso_RetornaOk()
    {
        var rutinas = new List<Rutina> { new Rutina { Id = 1, Favorita = true } };
        _mockRepo.Setup(x => x.ObtenerFavoritasPorSocioAsync(1)).ReturnsAsync(rutinas);

        var result = await _controller.GetFavoritasGimnasio(1);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task GetFavoritasGimnasio_ExcepcionGenerica_RetornaInternalServerError()
    {
        _mockRepo.Setup(x => x.ObtenerFavoritasPorSocioAsync(1)).ThrowsAsync(new Exception());

        var result = await _controller.GetFavoritasGimnasio(1);

        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion
}
