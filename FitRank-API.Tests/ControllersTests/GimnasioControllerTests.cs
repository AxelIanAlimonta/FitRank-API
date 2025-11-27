using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using FitRank_API.Domain.Interfaces;
using FitRank_API.Presentacion.Controllers;
using FitRank_API.Application.CasosDeUso.GimnasioCasosDeUso;
using FitRank_API.Application.DTOs.GimnasioDTOs;
using FitRank_API.Application.Hubs;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace FitRank_API.tests.ControllersTests;

public class GimnasioControllerTests
{
    private readonly GimnasioController _controller;
    private readonly Mock<ActualizarGimnasioCasoDeUso> _mockActualizar;
    private readonly Mock<AgregarGimnasioCasoDeUso> _mockAgregar;
    private readonly Mock<EliminarGimnasioCasoDeUso> _mockEliminar;
    private readonly Mock<ObtenerGimnasioPorIdCasoDeUso> _mockObtenerPorId;
    private readonly Mock<ObtenerGimnasiosCasoDeUso> _mockObtenerTodos;
    private readonly Mock<IAdministradorRepositorio> _mockAdminRepositorio;
    private readonly Mock<IHubContext<NotificacionesHub>> _hubMock;
    private readonly Mock<IHubClients> _hubClientsMock;
    private readonly Mock<IClientProxy> _clientProxyMock;
    private readonly Mock<ActualizarPersonalizacionGimnasioCasoDeUso> _mockActualizarPersonalizacion;

    public GimnasioControllerTests()
    {
        _hubMock = new Mock<IHubContext<NotificacionesHub>>();
        _hubClientsMock = new Mock<IHubClients>();
        _clientProxyMock = new Mock<IClientProxy>();

        _hubClientsMock.Setup(c => c.Group(It.IsAny<string>())).Returns(_clientProxyMock.Object);
        _hubMock.Setup(h => h.Clients).Returns(_hubClientsMock.Object);

        var mockRepo = new Mock<IGimnasioRepositorio>();
        var mapper = new Mock<IMapper>();
        var mockAdminRepo = new Mock<IAdministradorRepositorio>();

        _mockObtenerTodos = new Mock<ObtenerGimnasiosCasoDeUso>(mockRepo.Object, mapper.Object);
        _mockAgregar = new Mock<AgregarGimnasioCasoDeUso>(mockRepo.Object, mapper.Object, mockAdminRepo.Object);
        _mockActualizar = new Mock<ActualizarGimnasioCasoDeUso>(mockRepo.Object, mapper.Object);
        _mockEliminar = new Mock<EliminarGimnasioCasoDeUso>(mockRepo.Object);
        _mockObtenerPorId = new Mock<ObtenerGimnasioPorIdCasoDeUso>(mockRepo.Object, mapper.Object);
        _mockActualizarPersonalizacion = new Mock<ActualizarPersonalizacionGimnasioCasoDeUso>(mockRepo.Object, mapper.Object);

        _controller = new GimnasioController(
            _hubMock.Object,
            _mockObtenerTodos.Object,
            _mockAgregar.Object,
            _mockActualizar.Object,
            _mockEliminar.Object,
            _mockObtenerPorId.Object,
            _mockActualizarPersonalizacion.Object
        );
    }

    #region ObtenerTodos Tests

    [Fact]
    public async Task ObtenerTodos_RetornaOkResult_ConListaCompleta()
    {
        // Arrange
        var listaGimnasios = new List<ObtenerGimnasioDTO>
        {
            new ObtenerGimnasioDTO { Id = 1, Nombre = "Gimnasio 1", Direccion = "Direccion 1", LogoUrl = "http://logo1.url" },
            new ObtenerGimnasioDTO { Id = 2, Nombre = "Gimnasio 2", Direccion = "Direccion 2", LogoUrl = "http://logo2.url" }
        };

        _mockObtenerTodos
            .Setup(casoDeUso => casoDeUso.Ejecutar())
            .ReturnsAsync(listaGimnasios);

        // Act
        var resultado = await _controller.ObtenerTodos();

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(listaGimnasios);
    }

    [Fact]
    public async Task ObtenerTodos_RetornaOkResult_ConListaVacia()
    {
        // Arrange
        var listaGimnasios = new List<ObtenerGimnasioDTO>();

        _mockObtenerTodos
            .Setup(casoDeUso => casoDeUso.Ejecutar())
            .ReturnsAsync(listaGimnasios);

        // Act
        var resultado = await _controller.ObtenerTodos();

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(listaGimnasios);
    }

    [Fact]
    public async Task ObtenerTodos_LanzaExcepcion_RetornaStatus500()
    {
        // Arrange
        _mockObtenerTodos
            .Setup(casoDeUso => casoDeUso.Ejecutar())
            .ThrowsAsync(new Exception("Error interno"));

        // Act
        var resultado = await _controller.ObtenerTodos();

        // Assert
        var statusCodeResult = resultado as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ObtenerPorId Tests

    [Fact]
    public async Task ObtenerPorId_Existe_RetornaObjetoOkCreado()
    {
        // Arrange
        long gimnasioId = 1;
        var obtenerGimnasioDto = new ObtenerGimnasioDTO
        {
            Id = gimnasioId,
            Nombre = "Gimnasio Existente",
            Direccion = "Direccion Existente",
            LogoUrl = "http://logoexistente.url",
        };

        _mockObtenerPorId
            .Setup(casoDeUso => casoDeUso.Ejecutar(gimnasioId))
            .ReturnsAsync(obtenerGimnasioDto);

        // Act
        var resultado = await _controller.ObtenerPorId(gimnasioId);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(obtenerGimnasioDto);
    }

    [Fact]
    public async Task ObtenerPorId_IdCero_RetornaBadRequest()
    {
        // Act
        var resultado = await _controller.ObtenerPorId(0);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerPorId_IdNegativo_RetornaBadRequest()
    {
        // Act
        var resultado = await _controller.ObtenerPorId(-5);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerPorId_NoExiste_RetornaNotFound()
    {
        // Arrange
        long gimnasioId = 999;

        _mockObtenerPorId
            .Setup(casoDeUso => casoDeUso.Ejecutar(gimnasioId))
            .ReturnsAsync((ObtenerGimnasioDTO?)null);

        // Act
        var resultado = await _controller.ObtenerPorId(gimnasioId);

        // Assert
        var notFoundResult = resultado as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task ObtenerPorId_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        _mockObtenerPorId.Setup(x => x.Ejecutar(1)).ThrowsAsync(new Exception());

        // Act
        var resultado = await _controller.ObtenerPorId(1);

        // Assert
        var statusCodeResult = resultado as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region Agregar Tests

    [Fact]
    public async Task Agregar_RetornaCreatedAtActionResult()
    {
        // Arrange
        var gimnasioCrearDto = new AgregarGimnasioDTO
        {
            Nombre = "Gimnasio Test",
            Direccion = "Calle Falsa 123",
            LogoUrl = "http://logo.url",
        };

        var obtenerGimnasioDto = new ObtenerGimnasioDTO
        {
            Id = 1,
            Nombre = gimnasioCrearDto.Nombre,
            Direccion = gimnasioCrearDto.Direccion,
            LogoUrl = gimnasioCrearDto.LogoUrl,
        };

        _mockAgregar
            .Setup(casoDeUso => casoDeUso.Ejecutar(gimnasioCrearDto))
            .ReturnsAsync(obtenerGimnasioDto);

        // Act
        var resultado = await _controller.Agregar(gimnasioCrearDto);

        // Assert
        var createdAtActionResult = resultado as CreatedAtActionResult;
        createdAtActionResult.Should().NotBeNull();
        createdAtActionResult!.StatusCode.Should().Be(201);
        createdAtActionResult.Value.Should().BeEquivalentTo(obtenerGimnasioDto);
    }

    [Fact]
    public async Task Agregar_RetornaBadRequest_CuandoDTOEsNulo()
    {
        // Act
        var resultado = await _controller.Agregar(null);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Agregar_ModelStateInvalido_RetornaBadRequest()
    {
        // Arrange
        _controller.ModelState.AddModelError("Nombre", "Requerido");
        var dto = new AgregarGimnasioDTO();

        // Act
        var resultado = await _controller.Agregar(dto);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Agregar_LanzaExcepcion_RetornaStatusCode500()
    {
        // Arrange
        var gimnasioCrearDto = new AgregarGimnasioDTO
        {
            Nombre = "Gimnasio Test",
            Direccion = "Calle Falsa 123",
            LogoUrl = "http://logo.url",
        };

        _mockAgregar
            .Setup(casoDeUso => casoDeUso.Ejecutar(gimnasioCrearDto))
            .ThrowsAsync(new Exception("Error en el servidor."));

        // Act
        var resultado = await _controller.Agregar(gimnasioCrearDto);

        // Assert
        var statusCodeResult = resultado as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region Actualizar Tests

    [Fact]
    public async Task Actualizar_RetornaOkObjectResult_ConObjetoActualizado()
    {
        // Arrange
        var gimnasioActualizarDto = new ActualizarGimnasioDTO
        {
            Id = 1,
            Nombre = "Gimnasio Actualizado",
            Direccion = "Direccion Actualizada",
            LogoUrl = "http://logoactualizado.url",
        };

        var obtenerGimnasioDto = new ObtenerGimnasioDTO
        {
            Id = gimnasioActualizarDto.Id,
            Nombre = gimnasioActualizarDto.Nombre,
            Direccion = gimnasioActualizarDto.Direccion,
            LogoUrl = gimnasioActualizarDto.LogoUrl,
        };

        _mockActualizar
            .Setup(casoDeUso => casoDeUso.Ejecutar(gimnasioActualizarDto))
            .ReturnsAsync(obtenerGimnasioDto);

        // Act
        var resultado = await _controller.Actualizar(gimnasioActualizarDto.Id, gimnasioActualizarDto);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(obtenerGimnasioDto);
    }

    [Fact]
    public async Task Actualizar_IdCero_RetornaBadRequest()
    {
        // Arrange
        var dto = new ActualizarGimnasioDTO { Id = 0 };

        // Act
        var resultado = await _controller.Actualizar(0, dto);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Actualizar_IdNegativo_RetornaBadRequest()
    {
        // Arrange
        var dto = new ActualizarGimnasioDTO { Id = -5 };

        // Act
        var resultado = await _controller.Actualizar(-5, dto);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Actualizar_DTONulo_RetornaBadRequestResult()
    {
        // Act
        var resultado = await _controller.Actualizar(1, null);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Actualizar_ModelStateInvalido_RetornaBadRequest()
    {
        // Arrange
        _controller.ModelState.AddModelError("Nombre", "Requerido");
        var dto = new ActualizarGimnasioDTO { Id = 1 };

        // Act
        var resultado = await _controller.Actualizar(1, dto);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Actualizar_IdNoCoincide_RetornaBadRequestResult()
    {
        // Arrange
        var gimnasioActualizarDto = new ActualizarGimnasioDTO
        {
            Id = 1,
            Nombre = "Gimnasio Actualizado",
            Direccion = "Direccion Actualizada",
            LogoUrl = "http://logoactualizado.url",
        };

        // Act
        var resultado = await _controller.Actualizar(2, gimnasioActualizarDto);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Actualizar_NoEncontrado_RetornaNotFoundResult()
    {
        // Arrange
        var gimnasioActualizarDto = new ActualizarGimnasioDTO
        {
            Id = 999,
            Nombre = "Gimnasio No Existente",
            Direccion = "Direccion No Existente",
            LogoUrl = "http://logonoexistente.url",
        };

        _mockActualizar
            .Setup(casoDeUso => casoDeUso.Ejecutar(gimnasioActualizarDto))
            .ReturnsAsync((ObtenerGimnasioDTO?)null);

        // Act
        var resultado = await _controller.Actualizar(gimnasioActualizarDto.Id, gimnasioActualizarDto);

        // Assert
        var notFoundResult = resultado as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Actualizar_LanzaExcepcion_RetornaStatusCode500()
    {
        // Arrange
        var gimnasioActualizarDto = new ActualizarGimnasioDTO
        {
            Id = 1,
            Nombre = "Gimnasio Actualizado",
            Direccion = "Direccion Actualizada",
            LogoUrl = "http://logoactualizado.url",
        };

        _mockActualizar
            .Setup(casoDeUso => casoDeUso.Ejecutar(gimnasioActualizarDto))
            .ThrowsAsync(new Exception("Error interno"));

        // Act
        var resultado = await _controller.Actualizar(gimnasioActualizarDto.Id, gimnasioActualizarDto);

        // Assert
        var statusCodeResult = resultado as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region Eliminar Tests

    [Fact]
    public async Task Eliminar_Existente_DeberiaRetornarNoContent()
    {
        // Arrange
        long gimnasioId = 1;

        _mockEliminar
            .Setup(casoDeUso => casoDeUso.Ejecutar(gimnasioId))
            .ReturnsAsync(true);

        // Act
        var resultado = await _controller.Eliminar(gimnasioId);

        // Assert
        var noContentResult = resultado as NoContentResult;
        noContentResult.Should().NotBeNull();
        noContentResult!.StatusCode.Should().Be(204);
    }

    [Fact]
    public async Task Eliminar_IdCero_RetornaBadRequest()
    {
        // Act
        var resultado = await _controller.Eliminar(0);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Eliminar_IdNegativo_RetornaBadRequest()
    {
        // Act
        var resultado = await _controller.Eliminar(-3);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Eliminar_NoExistente_DeberiaRetornarNotFound()
    {
        // Arrange
        long gimnasioId = 999;

        _mockEliminar
            .Setup(casoDeUso => casoDeUso.Ejecutar(gimnasioId))
            .ReturnsAsync(false);

        // Act
        var resultado = await _controller.Eliminar(gimnasioId);

        // Assert
        var notFoundResult = resultado as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Eliminar_CuandoOcurreErrorEnServidor_DeberiaRetornarStatusCode500()
    {
        // Arrange
        long gimnasioId = 1;

        _mockEliminar
            .Setup(casoDeUso => casoDeUso.Ejecutar(gimnasioId))
            .ThrowsAsync(new Exception("Error interno"));

        // Act
        var resultado = await _controller.Eliminar(gimnasioId);

        // Assert
        var statusCodeResult = resultado as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ActualizarPersonalizacion Tests

    [Fact]
    public async Task ActualizarPersonalizacion_Exitoso_RetornaOk()
    {
        // Arrange
        var dto = new ActualizarPersonalizacionDTO
        {
            Id = 1,
            ColorPrincipal = "#FF0000",
            ColorSecundario = "#00FF00",
            LogoUrl = "http://logo.url"
        };

        var resultado = new ObtenerGimnasioDTO
        {
            Id = 1,
            ColorPrincipal = dto.ColorPrincipal,
            ColorSecundario = dto.ColorSecundario,
            LogoUrl = dto.LogoUrl
        };

        _mockActualizarPersonalizacion.Setup(x => x.Ejecutar(dto)).ReturnsAsync(resultado);

        // Act
        var response = await _controller.ActualizarPersonalizacion(dto);

        // Assert
        var okResult = response as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task ActualizarPersonalizacion_DtoNulo_RetornaBadRequest()
    {
        // Act
        var response = await _controller.ActualizarPersonalizacion(null);

        // Assert
        var badRequestResult = response as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ActualizarPersonalizacion_ModelStateInvalido_RetornaBadRequest()
    {
        // Arrange
        _controller.ModelState.AddModelError("ColorPrincipal", "Requerido");
        var dto = new ActualizarPersonalizacionDTO();

        // Act
        var response = await _controller.ActualizarPersonalizacion(dto);

        // Assert
        var badRequestResult = response as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ActualizarPersonalizacion_NoEncontrado_RetornaNotFound()
    {
        // Arrange
        var dto = new ActualizarPersonalizacionDTO { Id = 999 };
        _mockActualizarPersonalizacion.Setup(x => x.Ejecutar(dto)).ReturnsAsync((ObtenerGimnasioDTO)null);

        // Act
        var response = await _controller.ActualizarPersonalizacion(dto);

        // Assert
        var notFoundResult = response as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task ActualizarPersonalizacion_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        var dto = new ActualizarPersonalizacionDTO { Id = 1 };
        _mockActualizarPersonalizacion.Setup(x => x.Ejecutar(dto)).ThrowsAsync(new Exception());

        // Act
        var response = await _controller.ActualizarPersonalizacion(dto);

        // Assert
        var statusCodeResult = response as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion
}