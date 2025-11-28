using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using FitRank_API.Domain.Interfaces;
using FitRank_API.Presentacion.Controllers;
using FitRank_API.Application.CasosDeUso.LogroCasosDeUso;
using FitRank_API.Application.DTOs.LogroDTOs;
using System.Threading.Tasks;
using System.Collections.Generic;
using FitRank_API.Application.DTOs;
using FitRank_API.Domain.Entities;

namespace FitRank_API.tests.ControllersTests;

public class LogroControllerTests
{
    private readonly LogroController _controller;
    private readonly Mock<ActualizarLogroCasoDeUso> _mockActualizar;
    private readonly Mock<AgregarLogroCasoDeUso> _mockAgregar;
    private readonly Mock<EliminarLogroCasoDeUso> _mockEliminar;
    private readonly Mock<ObtenerLogroPorIdCasoDeUso> _mockObtenerPorId;
    private readonly Mock<ObtenerLogrosCasoDeUso> _mockObtenerTodos;
    private readonly Mock<OtorgarLogroPorNombreClaveCasoDeUso> _mockOtorgarLogroPorNombreClave;

    public LogroControllerTests()
    {
        var mockRepositorio = new Mock<ILogroRepositorio>();
        var mockLogroGimnasioRepo = new Mock<ILogroGimnasioRepositorio>();
        var mockLogroSocioRepo = new Mock<ILogroSocioRepositorio>();
        var mockMapper = new Mock<IMapper>();

        _mockActualizar = new Mock<ActualizarLogroCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockAgregar = new Mock<AgregarLogroCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockEliminar = new Mock<EliminarLogroCasoDeUso>(mockRepositorio.Object);
        _mockObtenerPorId = new Mock<ObtenerLogroPorIdCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockObtenerTodos = new Mock<ObtenerLogrosCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
        _mockOtorgarLogroPorNombreClave = new Mock<OtorgarLogroPorNombreClaveCasoDeUso>(
            mockRepositorio.Object, mockLogroGimnasioRepo.Object, mockLogroSocioRepo.Object, mockMapper.Object);

        _controller = new LogroController(
            _mockObtenerTodos.Object,
            _mockAgregar.Object,
            _mockActualizar.Object,
            _mockEliminar.Object,
            _mockObtenerPorId.Object,
            _mockOtorgarLogroPorNombreClave.Object
        );
    }

    #region ObtenerTodos Tests

    //ObtenerTodos_RetornaOkResult_ConListaCompleta
    [Fact]
    public async Task ObtenerTodos_RetornaOkResult_ConListaCompleta()
    {
        // Arrange
        var listaLogros = new List<ObtenerLogroDTO>
        {
            new ObtenerLogroDTO { Id = 1, NombreClave = "logro1", Nombre = "Logro 1", Puntos = 100 },
            new ObtenerLogroDTO { Id = 2, NombreClave = "logro2", Nombre = "Logro 2", Puntos = 200 }
        };

        _mockObtenerTodos.Setup(casoDeUso => casoDeUso.Ejecutar()).ReturnsAsync(listaLogros);

        // Act
        var resultado = await _controller.ObtenerTodos();

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(listaLogros);
    }

    //ObtenerTodos_RetornaOkResult_ConListaVacia
    [Fact]
    public async Task ObtenerTodos_RetornaOkResult_ConListaVacia()
    {
        // Arrange
        _mockObtenerTodos.Setup(casoDeUso => casoDeUso.Ejecutar()).ReturnsAsync(new List<ObtenerLogroDTO>());

        // Act
        var resultado = await _controller.ObtenerTodos();

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    //ObtenerTodos_LanzaExcepcion_RetornaStatus500
    [Fact]
    public async Task ObtenerTodos_LanzaExcepcion_RetornaStatus500()
    {
        // Arrange
        _mockObtenerTodos.Setup(casoDeUso => casoDeUso.Ejecutar()).ThrowsAsync(new Exception());

        // Act
        var resultado = await _controller.ObtenerTodos();

        // Assert
        var statusCodeResult = resultado as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ObtenerPorId Tests

    //ObtenerPorId_Existe_RetornaObjetoOkCreado
    [Fact]
    public async Task ObtenerPorId_Existe_RetornaObjetoOkCreado()
    {
        // Arrange
        int logroId = 1;
        var logroExistente = new ObtenerLogroDTO { Id = logroId, NombreClave = "logro1", Nombre = "Logro 1" };

        _mockObtenerPorId.Setup(casoDeUso => casoDeUso.Ejecutar(logroId)).ReturnsAsync(logroExistente);

        // Act
        var resultado = await _controller.ObtenerPorId(logroId);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(logroExistente);
    }

    //ObtenerPorId_IdCero_RetornaBadRequest
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

    //ObtenerPorId_IdNegativo_RetornaBadRequest
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

    //ObtenerPorId_NoExiste_RetornaNotFound
    [Fact]
    public async Task ObtenerPorId_NoExiste_RetornaNotFound()
    {
        // Arrange
        int logroId = 999;
        _mockObtenerPorId.Setup(casoDeUso => casoDeUso.Ejecutar(logroId)).ReturnsAsync((ObtenerLogroDTO?)null);

        // Act
        var resultado = await _controller.ObtenerPorId(logroId);

        // Assert
        var notFoundResult = resultado as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    //ObtenerPorId_ExcepcionGenerica_RetornaInternalServerError
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

    //Agregar_RetornaCreatedAtActionResult
    [Fact]
    public async Task Agregar_RetornaCreatedAtActionResult()
    {
        // Arrange
        var nuevoLogro = new AgregarLogroDTO
        {
            NombreClave = "logro1",
            Nombre = "Logro 1",
            Descripcion = "Descripcion",
            Puntos = 100
        };

        var nuevoLogroCreado = new ObtenerLogroDTO { Id = 1, NombreClave = "logro1", Nombre = "Logro 1", Puntos = 100 };

        _mockAgregar.Setup(x => x.Ejecutar(nuevoLogro)).ReturnsAsync(nuevoLogroCreado);

        // Act
        var resultado = await _controller.Agregar(nuevoLogro);


        // Assert
        var createdAtActionResult = resultado as CreatedAtActionResult;
        createdAtActionResult.Should().NotBeNull();
        createdAtActionResult!.StatusCode.Should().Be(201);
        createdAtActionResult.Value.Should().BeEquivalentTo(nuevoLogroCreado);
    }

    //Agregar_RetornaBadRequest_CuandoDTOEsNulo
    [Fact]
    public async Task Agregar_RetornaBadRequest_CuandoDTOEsNulo()
    {
        // Act
        var resultado = await _controller.Agregar(null!);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    //Agregar_ModelStateInvalido_RetornaBadRequest
    [Fact]
    public async Task Agregar_ModelStateInvalido_RetornaBadRequest()
    {
        // Arrange
        _controller.ModelState.AddModelError("Nombre", "Requerido");
        var dto = new AgregarLogroDTO();

        // Act
        var resultado = await _controller.Agregar(dto);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    //Agregar_LanzaExcepcion_RetornaStatusCode500
    [Fact]
    public async Task Agregar_LanzaExcepcion_RetornaStatusCode500()
    {
        // Arrange
        var nuevaActividad = new AgregarLogroDTO { NombreClave = "logro1" };
        _mockAgregar.Setup(casoDeUso => casoDeUso.Ejecutar(nuevaActividad)).ThrowsAsync(new Exception());

        // Act
        var resultado = await _controller.Agregar(nuevaActividad);

        // Assert
        var statusCodeResult = resultado as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region Actualizar Tests

    //Actualizar_RetornaOkObjectResult_ConObjetoActualizado
    [Fact]
    public async Task Actualizar_RetornaOkObjectResult_ConObjetoActualizado()
    {
        // Arrange
        int logroId = 1;
        var logroActualizar = new ActualizarLogroDTO { Id = logroId, Nombre = "Logro Actualizado" };
        var logroActualizado = new ObtenerLogroDTO { Id = logroId, Nombre = "Logro Actualizado" };

        _mockActualizar.Setup(casoDeUso => casoDeUso.Ejecutar(logroActualizar)).ReturnsAsync(logroActualizado);

        // Act
        var resultado = await _controller.Actualizar(logroId, logroActualizar);

        // Assert
        var okResult = resultado as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    //Actualizar_IdCero_RetornaBadRequest
    [Fact]
    public async Task Actualizar_IdCero_RetornaBadRequest()
    {
        // Arrange
        var dto = new ActualizarLogroDTO { Id = 0 };

        // Act
        var resultado = await _controller.Actualizar(0, dto);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    //Actualizar_IdNegativo_RetornaBadRequest
    [Fact]
    public async Task Actualizar_IdNegativo_RetornaBadRequest()
    {
        // Arrange
        var dto = new ActualizarLogroDTO { Id = -5 };

        // Act
        var resultado = await _controller.Actualizar(-5, dto);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    //Actualizar_DTONulo_RetornaBadRequestResult
    [Fact]
    public async Task Actualizar_DTONulo_RetornaBadRequestResult()
    {
        // Act
        var resultado = await _controller.Actualizar(1, null!);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    //Actualizar_ModelStateInvalido_RetornaBadRequest
    [Fact]
    public async Task Actualizar_ModelStateInvalido_RetornaBadRequest()
    {
        // Arrange
        _controller.ModelState.AddModelError("Nombre", "Requerido");
        var dto = new ActualizarLogroDTO { Id = 1 };

        // Act
        var resultado = await _controller.Actualizar(1, dto);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    //Actualizar_IdNoCoincide_RetornaBadRequestResult
    [Fact]
    public async Task Actualizar_IdNoCoincide_RetornaBadRequestResult()
    {
        // Arrange
        var logroActualizar = new ActualizarLogroDTO { Id = 1 };

        // Act
        var resultado = await _controller.Actualizar(2, logroActualizar);

        // Assert
        var badRequestResult = resultado as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    //Actualizar_NoEncontrado_RetornaNotFoundResult
    [Fact]
    public async Task Actualizar_NoEncontrado_RetornaNotFoundResult()
    {
        // Arrange
        int logroId = 999;
        var logroActualizar = new ActualizarLogroDTO { Id = logroId };

        _mockActualizar.Setup(casoDeUso => casoDeUso.Ejecutar(logroActualizar)).ReturnsAsync((ObtenerLogroDTO?)null);

        // Act
        var resultado = await _controller.Actualizar(logroId, logroActualizar);

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
        int logroId = 1;
        var logroActualizar = new ActualizarLogroDTO { Id = logroId };

        _mockActualizar.Setup(casoDeUso => casoDeUso.Ejecutar(logroActualizar)).ThrowsAsync(new Exception());

        // Act
        var resultado = await _controller.Actualizar(logroId, logroActualizar);

        // Assert
        var statusCodeResult = resultado as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region Eliminar Tests

    //Eliminar_Existente_DeberiaRetornarNoContent
    [Fact]
    public async Task Eliminar_Existente_DeberiaRetornarNoContent()
    {
        // Arrange
        int logroId = 1;
        _mockEliminar.Setup(casoDeUso => casoDeUso.Ejecutar(logroId)).ReturnsAsync(true);

        // Act
        var resultado = await _controller.Eliminar(logroId);

        // Assert
        var noContentResult = resultado as NoContentResult;
        noContentResult.Should().NotBeNull();
        noContentResult!.StatusCode.Should().Be(204);
    }

    //Eliminar_IdCero_RetornaBadRequest
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

    //Eliminar_IdNegativo_RetornaBadRequest
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

    //Eliminar_NoExistente_DeberiaRetornarNotFound
    [Fact]
    public async Task Eliminar_NoExistente_DeberiaRetornarNotFound()
    {
        // Arrange
        int logroId = 999;
        _mockEliminar.Setup(casoDeUso => casoDeUso.Ejecutar(logroId)).ReturnsAsync(false);

        // Act
        var resultado = await _controller.Eliminar(logroId);

        // Assert
        var notFoundResult = resultado as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    //Eliminar_CuandoOcurreErrorEnServidor_DeberiaRetornarStatusCode500
    [Fact]
    public async Task Eliminar_CuandoOcurreErrorEnServidor_DeberiaRetornarStatusCode500()
    {
        // Arrange
        int logroId = 1;
        _mockEliminar.Setup(casoDeUso => casoDeUso.Ejecutar(logroId)).ThrowsAsync(new Exception());

        // Act
        var resultado = await _controller.Eliminar(logroId);

        // Assert
        var statusCodeResult = resultado as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region Otorgar Tests

    [Fact]
    public async Task Otorgar_Exitoso_RetornaOk()
    {
        // Arrange
        var dto = new OtorgarLogroPorNombreClaveDTO
        {
            NombreClave = "logro1",
            SocioId = 1,
            GimnasioId = 1
        };

        var resultado = new LogroOtorgadoDTO
        {
            Otorgado = true,
            LogroId = 1,
            Nombre = "Logro 1",
            SocioId = 1,
            GimnasioId = 1
        };

        _mockOtorgarLogroPorNombreClave.Setup(x => x.Ejecutar(dto)).ReturnsAsync(resultado);

        // Act
        var response = await _controller.Otorgar(dto);

        // Assert
        var okResult = response.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task Otorgar_NoOtorgado_RetornaBadRequest()
    {
        // Arrange
        var dto = new OtorgarLogroPorNombreClaveDTO
        {
            NombreClave = "logro1",
            SocioId = 1,
            GimnasioId = 1
        };

        var resultado = new LogroOtorgadoDTO
        {
            Otorgado = false,
            Motivo = "El socio ya tiene este logro."
        };

        _mockOtorgarLogroPorNombreClave.Setup(x => x.Ejecutar(dto)).ReturnsAsync(resultado);

        // Act
        var response = await _controller.Otorgar(dto);

        // Assert
        var badRequestResult = response.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Otorgar_DtoNulo_RetornaBadRequest()
    {
        // Act
        var response = await _controller.Otorgar(null!);

        // Assert
        var badRequestResult = response.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Otorgar_ModelStateInvalido_RetornaBadRequest()
    {
        // Arrange
        _controller.ModelState.AddModelError("NombreClave", "Requerido");
        var dto = new OtorgarLogroPorNombreClaveDTO();

        // Act
        var response = await _controller.Otorgar(dto);

        // Assert
        var badRequestResult = response.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Otorgar_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        var dto = new OtorgarLogroPorNombreClaveDTO { NombreClave = "logro1", SocioId = 1, GimnasioId = 1 };
        _mockOtorgarLogroPorNombreClave.Setup(x => x.Ejecutar(dto)).ThrowsAsync(new Exception());

        // Act
        var response = await _controller.Otorgar(dto);

        // Assert
        var statusCodeResult = response.Result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion
}
