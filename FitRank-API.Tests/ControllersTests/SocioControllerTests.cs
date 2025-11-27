using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using FitRank_API.Presentacion.Controllers;
using AutoMapper;
using FitRank_API.Domain.Interfaces;
using FitRank_API.Application.CasosDeUso.SocioCasoDeUso;
using FitRank_API.Application.DTOs.SocioDTOs;
using FitRank_API.Application.Interfaces;
using FitRank_API.Application.CasosDeUso.SocioCasosDeUso;

namespace FitRank_API.tests.ControllersTests;

public class SocioControllerTests
{
    private readonly SocioController _controller;
    private readonly Mock<ObtenerSociosCasoDeUso> _mockObtenerTodos;
    private readonly Mock<ObtenerSocioPorIdCasoDeUso> _mockObtenerPorId;
    private readonly Mock<AgregarSocioCasoDeUso> _mockAgregar;
    private readonly Mock<ActualizarSocioCasoDeUso> _mockActualizar;
    private readonly Mock<EliminarSocioCasoDeUso> _mockEliminar;
    private readonly Mock<CambiarParticipacionRankingCasoDeUso> _mockCambiarParticipacion;
    private readonly Mock<ObtenerSocioConMedidasCasoDeUso> _mockObtenerCompleto;
    private readonly Mock<EditarPerfilSocioCasoDeUso> _mockEditarPerfil;

    public SocioControllerTests()
    {
        var mockRepo = new Mock<ISocioRepositorio>();
        var mockMapper = new Mock<IMapper>();
        var mockPasswordService = new Mock<IPasswordService>();
        var mockMedidaRepo = new Mock<IMedidaCorporalRepositorio>();

        _mockObtenerTodos = new Mock<ObtenerSociosCasoDeUso>(mockRepo.Object, mockMapper.Object);
        _mockObtenerPorId = new Mock<ObtenerSocioPorIdCasoDeUso>(mockRepo.Object, mockMapper.Object);
        _mockAgregar = new Mock<AgregarSocioCasoDeUso>(mockRepo.Object, mockMapper.Object, mockPasswordService.Object);
        _mockActualizar = new Mock<ActualizarSocioCasoDeUso>(mockRepo.Object, mockMapper.Object);
        _mockEliminar = new Mock<EliminarSocioCasoDeUso>(mockRepo.Object);
        _mockCambiarParticipacion = new Mock<CambiarParticipacionRankingCasoDeUso>(mockRepo.Object);
        _mockObtenerCompleto = new Mock<ObtenerSocioConMedidasCasoDeUso>(mockRepo.Object, mockMedidaRepo.Object);
        _mockEditarPerfil = new Mock<EditarPerfilSocioCasoDeUso>(mockRepo.Object);

        _controller = new SocioController(
            _mockObtenerTodos.Object,
            _mockObtenerPorId.Object,
            _mockAgregar.Object,
            _mockActualizar.Object,
            _mockEliminar.Object,
            _mockCambiarParticipacion.Object,
            _mockObtenerCompleto.Object,
            _mockEditarPerfil.Object
        );
    }

    #region ObtenerTodos Tests

    [Fact]
    public async Task ObtenerTodos_Exitoso_RetornaOk()
    {
        var socios = new List<SocioDTO>
        {
            new SocioDTO { Id = 1, Nombre = "Juan", Apellido = "Pérez" },
            new SocioDTO { Id = 2, Nombre = "María", Apellido = "González" }
        };
        _mockObtenerTodos.Setup(x => x.Ejecutar()).ReturnsAsync(socios);

        var result = await _controller.ObtenerTodos();

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(socios);
    }

    [Fact]
    public async Task ObtenerTodos_ExcepcionGenerica_RetornaInternalServerError()
    {
        _mockObtenerTodos.Setup(x => x.Ejecutar()).ThrowsAsync(new Exception());

        var result = await _controller.ObtenerTodos();

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
        var socio = new SocioDTO { Id = 1, Nombre = "Juan", Apellido = "Pérez" };
        _mockObtenerPorId.Setup(x => x.Ejecutar(1)).ReturnsAsync(socio);

        var result = await _controller.ObtenerPorId(1);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(socio);
    }

    [Fact]
    public async Task ObtenerPorId_NoEncontrado_RetornaNotFound()
    {
        _mockObtenerPorId.Setup(x => x.Ejecutar(999)).ReturnsAsync((SocioDTO?)null);

        var result = await _controller.ObtenerPorId(999);

        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task ObtenerPorId_ExcepcionGenerica_RetornaInternalServerError()
    {
        _mockObtenerPorId.Setup(x => x.Ejecutar(1)).ThrowsAsync(new Exception());

        var result = await _controller.ObtenerPorId(1);

        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

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
        var dto = new AgregarSocioDTO { NombreUsuario = "juanp", Email = "juan@test.com", Password = "Pass123!" };
        var socioCreado = new SocioDTO { Id = 1, NombreUsuario = "juanp" };
        _mockAgregar.Setup(x => x.Ejecutar(dto)).ReturnsAsync(socioCreado);

        var result = await _controller.Agregar(dto);

        var createdResult = result as CreatedAtActionResult;
        createdResult.Should().NotBeNull();
        createdResult!.StatusCode.Should().Be(201);
        createdResult.Value.Should().BeEquivalentTo(socioCreado);
    }

    [Fact]
    public async Task Agregar_ExcepcionGenerica_RetornaInternalServerError()
    {
        var dto = new AgregarSocioDTO { NombreUsuario = "juanp", Email = "juan@test.com" };
        _mockAgregar.Setup(x => x.Ejecutar(dto)).ThrowsAsync(new Exception());

        var result = await _controller.Agregar(dto);

        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region Actualizar Tests

    [Fact]
    public async Task Actualizar_IdCero_RetornaBadRequest()
    {
        var dto = new SocioDTO { Id = 0 };

        var result = await _controller.Actualizar(0, dto);

        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Actualizar_IdNegativo_RetornaBadRequest()
    {
        var dto = new SocioDTO { Id = -3 };

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
        var dto = new SocioDTO { Id = 1 };

        var result = await _controller.Actualizar(2, dto);

        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Actualizar_Exitoso_RetornaOk()
    {
        var dto = new SocioDTO { Id = 1, Nombre = "Juan Actualizado" };
        var socioActualizado = new SocioDTO { Id = 1, Nombre = "Juan Actualizado" };
        _mockActualizar.Setup(x => x.Ejecutar(dto)).ReturnsAsync(socioActualizado);

        var result = await _controller.Actualizar(1, dto);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(socioActualizado);
    }

    [Fact]
    public async Task Actualizar_NoEncontrado_RetornaNotFound()
    {
        var dto = new SocioDTO { Id = 999 };
        _mockActualizar.Setup(x => x.Ejecutar(dto)).ReturnsAsync((SocioDTO?)null);

        var result = await _controller.Actualizar(999, dto);

        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Actualizar_ExcepcionGenerica_RetornaInternalServerError()
    {
        var dto = new SocioDTO { Id = 1 };
        _mockActualizar.Setup(x => x.Ejecutar(dto)).ThrowsAsync(new Exception());

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
        _mockEliminar.Setup(x => x.Ejecutar(1)).ReturnsAsync(true);

        var result = await _controller.Eliminar(1);

        var noContentResult = result as NoContentResult;
        noContentResult.Should().NotBeNull();
        noContentResult!.StatusCode.Should().Be(204);
    }

    [Fact]
    public async Task Eliminar_NoEncontrado_RetornaNotFound()
    {
        _mockEliminar.Setup(x => x.Ejecutar(999)).ReturnsAsync(false);

        var result = await _controller.Eliminar(999);

        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Eliminar_ExcepcionGenerica_RetornaInternalServerError()
    {
        _mockEliminar.Setup(x => x.Ejecutar(1)).ThrowsAsync(new Exception());

        var result = await _controller.Eliminar(1);

        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region CambiarParticipacionRanking Tests

    [Fact]
    public async Task CambiarParticipacionRanking_SocioIdCero_RetornaBadRequest()
    {
        var dto = new CambiarParticipacionRankingDTO { ParticipaEnRanking = true };

        var result = await _controller.CambiarParticipacionRanking(0, dto);

        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task CambiarParticipacionRanking_SocioIdNegativo_RetornaBadRequest()
    {
        var dto = new CambiarParticipacionRankingDTO { ParticipaEnRanking = true };

        var result = await _controller.CambiarParticipacionRanking(-3, dto);

        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task CambiarParticipacionRanking_DtoNulo_RetornaBadRequest()
    {
        var result = await _controller.CambiarParticipacionRanking(1, null!);

        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task CambiarParticipacionRanking_Exitoso_RetornaOk()
    {
        var dto = new CambiarParticipacionRankingDTO { ParticipaEnRanking = true };
        _mockCambiarParticipacion.Setup(x => x.Ejecutar(1, true)).ReturnsAsync(true);

        var result = await _controller.CambiarParticipacionRanking(1, dto);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task CambiarParticipacionRanking_NoEncontrado_RetornaNotFound()
    {
        var dto = new CambiarParticipacionRankingDTO { ParticipaEnRanking = true };
        _mockCambiarParticipacion.Setup(x => x.Ejecutar(999, true)).ReturnsAsync(false);

        var result = await _controller.CambiarParticipacionRanking(999, dto);

        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task CambiarParticipacionRanking_ExcepcionGenerica_RetornaInternalServerError()
    {
        var dto = new CambiarParticipacionRankingDTO { ParticipaEnRanking = true };
        _mockCambiarParticipacion.Setup(x => x.Ejecutar(1, true)).ThrowsAsync(new Exception());

        var result = await _controller.CambiarParticipacionRanking(1, dto);

        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ObtenerSocioCompleto Tests

    [Fact]
    public async Task ObtenerSocioCompleto_IdCero_RetornaBadRequest()
    {
        var result = await _controller.ObtenerSocioCompleto(0);

        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerSocioCompleto_IdNegativo_RetornaBadRequest()
    {
        var result = await _controller.ObtenerSocioCompleto(-4);

        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerSocioCompleto_Exitoso_RetornaOk()
    {
        var socioCompleto = new SocioConMedidasDTO 
        { 
            Socio = new SocioDTO { Id = 1, Nombre = "Juan" }
        };
        _mockObtenerCompleto.Setup(x => x.Ejecutar(1)).ReturnsAsync(socioCompleto);

        var result = await _controller.ObtenerSocioCompleto(1);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(socioCompleto);
    }

    [Fact]
    public async Task ObtenerSocioCompleto_NoEncontrado_RetornaNotFound()
    {
        _mockObtenerCompleto.Setup(x => x.Ejecutar(999)).ReturnsAsync((SocioConMedidasDTO?)null);

        var result = await _controller.ObtenerSocioCompleto(999);

        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task ObtenerSocioCompleto_ExcepcionGenerica_RetornaInternalServerError()
    {
        _mockObtenerCompleto.Setup(x => x.Ejecutar(1)).ThrowsAsync(new Exception());

        var result = await _controller.ObtenerSocioCompleto(1);

        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region EditarPerfil Tests

    [Fact]
    public async Task EditarPerfil_SocioIdCero_RetornaBadRequest()
    {
        var dto = new EditarPerfilSocioDTO();

        var result = await _controller.EditarPerfil(0, dto);

        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task EditarPerfil_SocioIdNegativo_RetornaBadRequest()
    {
        var dto = new EditarPerfilSocioDTO();

        var result = await _controller.EditarPerfil(-6, dto);

        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task EditarPerfil_DtoNulo_RetornaBadRequest()
    {
        var result = await _controller.EditarPerfil(1, null!);

        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task EditarPerfil_Exitoso_RetornaOk()
    {
        var dto = new EditarPerfilSocioDTO();
        _mockEditarPerfil.Setup(x => x.Ejecutar(1, dto)).ReturnsAsync(true);

        var result = await _controller.EditarPerfil(1, dto);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task EditarPerfil_NoEncontrado_RetornaNotFound()
    {
        var dto = new EditarPerfilSocioDTO();
        _mockEditarPerfil.Setup(x => x.Ejecutar(999, dto)).ReturnsAsync(false);

        var result = await _controller.EditarPerfil(999, dto);

        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task EditarPerfil_ExcepcionGenerica_RetornaInternalServerError()
    {
        var dto = new EditarPerfilSocioDTO();
        _mockEditarPerfil.Setup(x => x.Ejecutar(1, dto)).ThrowsAsync(new Exception());

        var result = await _controller.EditarPerfil(1, dto);

        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion
}
