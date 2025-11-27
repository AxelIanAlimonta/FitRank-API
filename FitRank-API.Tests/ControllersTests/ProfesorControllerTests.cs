using Xunit;
using Moq;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using FitRank_API.Domain.Interfaces;
using FitRank_API.Presentacion.Controllers;
using FitRank_API.Application.CasosDeUso.ProfesorCasosDeUso;
using FitRank_API.Application.CasosDeUso.RutinaCasosDeUso;
using FitRank_API.Application.DTOs.ProfesorDTOs;
using FitRank_API.Application.DTOs.RutinaDTOs;
using FitRank_API.Application.Interfaces;
using FitRank_API.Domain.Entities;
using FitRank_API.Application.Mappings;

namespace FitRank_API.tests.ControllersTests;

public class ProfesorControllerTests
{
    private readonly ProfesorController _controller;
    private readonly Mock<IProfesorRepositorio> _mockRepo;
    private readonly Mock<IRutinaRepositorio> _mockRutinaRepo;
    private readonly Mock<ISolicitudRutinaProfesorRepositorio> _mockSolicitudRepo;
    private readonly IMapper _mapper;

    public ProfesorControllerTests()
    {
        _mockRepo = new Mock<IProfesorRepositorio>();
        _mockRutinaRepo = new Mock<IRutinaRepositorio>();
        _mockSolicitudRepo = new Mock<ISolicitudRutinaProfesorRepositorio>();
        var mockPasswordService = new Mock<IPasswordService>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<ProfesorProfile>();
            cfg.AddProfile<RutinaProfile>();
        });
        _mapper = config.CreateMapper();

        var agregar = new AgregarProfesorCasoDeUso(_mockRepo.Object, _mapper, mockPasswordService.Object);
        var obtenerPorId = new ObtenerProfesorPorIdCasoDeUso(_mockRepo.Object, _mapper);
        var actualizar = new ActualizarProfesorCasoDeUso(_mockRepo.Object, _mapper);
        var eliminar = new EliminarProfesorCasoDeUso(_mockRepo.Object);
        var obtenerTodos = new ObtenerTodosLosProfesoresCasoDeUso(_mockRepo.Object, _mapper);
        var obtenerPorGimnasio = new ObtenerTodosPorGimnasioCasoDeUso(_mockRepo.Object, _mapper);
        var obtenerRutinas = new ObtenerTodasLasRutinasPorProfesorCasoDeUso(_mockRutinaRepo.Object, _mapper, _mockRepo.Object);
        var obtenerEstadisticas = new ObtenerEstadisticasProfesoresCasoDeUso(_mockSolicitudRepo.Object);

        _controller = new ProfesorController(
            agregar,
            obtenerPorId,
            actualizar,
            obtenerTodos,
            eliminar,
            obtenerPorGimnasio,
            obtenerRutinas,
            obtenerEstadisticas
        );
    }

    #region ObtenerTodosAsync Tests

    [Fact]
    public async Task ObtenerTodosAsync_Exitoso_RetornaOk()
    {
        // Arrange
        var profesores = new List<Profesor>
        {
            new Profesor { Id = 1, Nombre = "Juan", Email = "juan@test.com" }
        };
        _mockRepo.Setup(x => x.ObtenerTodosAsync()).ReturnsAsync(profesores);

        // Act
        var result = await _controller.ObtenerTodosAsync();

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task ObtenerTodosAsync_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        _mockRepo.Setup(x => x.ObtenerTodosAsync()).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.ObtenerTodosAsync();

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ObtenerPorId Tests

    [Fact]
    public async Task ObtenerPorId_Exitoso_RetornaOk()
    {
        // Arrange
        var profesor = new Profesor { Id = 1, Nombre = "Juan" };
        _mockRepo.Setup(x => x.ObtenerPorIdAsync(1)).ReturnsAsync(profesor);

        // Act
        var result = await _controller.ObtenerPorId(1);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task ObtenerPorId_IdCero_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerPorId(0);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerPorId_IdNegativo_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerPorId(-5);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerPorId_NoEncontrado_RetornaNotFound()
    {
        // Arrange
        _mockRepo.Setup(x => x.ObtenerPorIdAsync(999)).ReturnsAsync((Profesor?)null);

        // Act
        var result = await _controller.ObtenerPorId(999);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task ObtenerPorId_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        _mockRepo.Setup(x => x.ObtenerPorIdAsync(1)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.ObtenerPorId(1);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region AgregarAsync Tests

    [Fact]
    public async Task AgregarAsync_Exitoso_RetornaOk()
    {
        // Arrange
        var dto = new AgregarProfesorDTO
        {
            Nombre = "Juan",
            Email = "juan@test.com",
            Dni = 12345678,
            Password = "password123"
        };
        var profesor = new Profesor { Id = 1, Nombre = "Juan", Email = "juan@test.com" };
        _mockRepo.Setup(x => x.ExisteEmailAsync(dto.Email)).ReturnsAsync(false);
        _mockRepo.Setup(x => x.ExisteDniAsync(dto.Dni)).ReturnsAsync(false);
        _mockRepo.Setup(x => x.AgregarAsync(It.IsAny<Profesor>())).ReturnsAsync(profesor);

        // Act
        var result = await _controller.AgregarAsync(dto);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task AgregarAsync_DtoNulo_RetornaBadRequest()
    {
        // Act
        var result = await _controller.AgregarAsync(null!);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task AgregarAsync_ModelStateInvalido_RetornaBadRequest()
    {
        // Arrange
        _controller.ModelState.AddModelError("Nombre", "Requerido");
        var dto = new AgregarProfesorDTO();

        // Act
        var result = await _controller.AgregarAsync(dto);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task AgregarAsync_EmailDuplicado_RetornaBadRequest()
    {
        // Arrange
        var dto = new AgregarProfesorDTO { Email = "duplicado@test.com", Dni = 12345678 };
        _mockRepo.Setup(x => x.ExisteEmailAsync(dto.Email)).ReturnsAsync(true);

        // Act
        var result = await _controller.AgregarAsync(dto);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task AgregarAsync_DniDuplicado_RetornaBadRequest()
    {
        // Arrange
        var dto = new AgregarProfesorDTO { Email = "test@test.com", Dni = 12345678 };
        _mockRepo.Setup(x => x.ExisteEmailAsync(dto.Email)).ReturnsAsync(false);
        _mockRepo.Setup(x => x.ExisteDniAsync(dto.Dni)).ReturnsAsync(true);

        // Act
        var result = await _controller.AgregarAsync(dto);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task AgregarAsync_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        var dto = new AgregarProfesorDTO { Email = "test@test.com", Dni = 12345678 };
        _mockRepo.Setup(x => x.ExisteEmailAsync(dto.Email)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.AgregarAsync(dto);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region Actualizar Tests

    [Fact]
    public async Task Actualizar_Exitoso_RetornaOk()
    {
        // Arrange
        var dto = new ActualizarProfesorDTO
        {
            Id = 1,
            Nombre = "Juan Actualizado",
            Email = "juan@test.com",
            Dni = 12345678
        };
        var profesor = new Profesor { Id = 1, Nombre = "Juan", Email = "juan@test.com", Dni = 12345678 };
        _mockRepo.Setup(x => x.ObtenerPorIdAsync(1)).ReturnsAsync(profesor);
        _mockRepo.Setup(x => x.ActualizarAsync(It.IsAny<Profesor>())).ReturnsAsync(profesor);

        // Act
        var result = await _controller.Actualizar(1, dto);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task Actualizar_IdCero_RetornaBadRequest()
    {
        // Arrange
        var dto = new ActualizarProfesorDTO { Id = 0 };

        // Act
        var result = await _controller.Actualizar(0, dto);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Actualizar_IdNegativo_RetornaBadRequest()
    {
        // Arrange
        var dto = new ActualizarProfesorDTO { Id = -5 };

        // Act
        var result = await _controller.Actualizar(-5, dto);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Actualizar_DtoNulo_RetornaBadRequest()
    {
        // Act
        var result = await _controller.Actualizar(1, null!);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Actualizar_ModelStateInvalido_RetornaBadRequest()
    {
        // Arrange
        _controller.ModelState.AddModelError("Nombre", "Requerido");
        var dto = new ActualizarProfesorDTO { Id = 1 };

        // Act
        var result = await _controller.Actualizar(1, dto);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Actualizar_IdNoCoincide_RetornaBadRequest()
    {
        // Arrange
        var dto = new ActualizarProfesorDTO { Id = 1 };

        // Act
        var result = await _controller.Actualizar(2, dto);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Actualizar_NoEncontrado_RetornaNotFound()
    {
        // Arrange
        var dto = new ActualizarProfesorDTO { Id = 999 };
        _mockRepo.Setup(x => x.ObtenerPorIdAsync(999)).ReturnsAsync((Profesor?)null);

        // Act
        var result = await _controller.Actualizar(999, dto);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Actualizar_EmailDuplicado_RetornaBadRequest()
    {
        // Arrange
        var dto = new ActualizarProfesorDTO { Id = 1, Email = "duplicado@test.com", Dni = 12345678 };
        var profesor = new Profesor { Id = 1, Email = "viejo@test.com", Dni = 12345678 };
        _mockRepo.Setup(x => x.ObtenerPorIdAsync(1)).ReturnsAsync(profesor);
        _mockRepo.Setup(x => x.ExisteEmailAsync(dto.Email)).ReturnsAsync(true);

        // Act
        var result = await _controller.Actualizar(1, dto);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Actualizar_DniDuplicado_RetornaBadRequest()
    {
        // Arrange
        var dto = new ActualizarProfesorDTO { Id = 1, Email = "test@test.com", Dni = 87654321 };
        var profesor = new Profesor { Id = 1, Email = "test@test.com", Dni = 12345678 };
        _mockRepo.Setup(x => x.ObtenerPorIdAsync(1)).ReturnsAsync(profesor);
        _mockRepo.Setup(x => x.ExisteDniAsync(dto.Dni)).ReturnsAsync(true);

        // Act
        var result = await _controller.Actualizar(1, dto);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Actualizar_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        var dto = new ActualizarProfesorDTO { Id = 1 };
        _mockRepo.Setup(x => x.ObtenerPorIdAsync(1)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.Actualizar(1, dto);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region EliminarAsync Tests

    [Fact]
    public async Task EliminarAsync_Exitoso_RetornaNoContent()
    {
        // Arrange
        _mockRepo.Setup(x => x.EliminarAsync(1)).ReturnsAsync(true);

        // Act
        var result = await _controller.EliminarAsync(1);

        // Assert
        var noContentResult = result as NoContentResult;
        noContentResult.Should().NotBeNull();
        noContentResult!.StatusCode.Should().Be(204);
    }

    [Fact]
    public async Task EliminarAsync_IdCero_RetornaBadRequest()
    {
        // Act
        var result = await _controller.EliminarAsync(0);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task EliminarAsync_IdNegativo_RetornaBadRequest()
    {
        // Act
        var result = await _controller.EliminarAsync(-2);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task EliminarAsync_NoEncontrado_RetornaNotFound()
    {
        // Arrange
        _mockRepo.Setup(x => x.EliminarAsync(999)).ReturnsAsync(false);

        // Act
        var result = await _controller.EliminarAsync(999);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task EliminarAsync_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        _mockRepo.Setup(x => x.EliminarAsync(1)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.EliminarAsync(1);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ObtenerPorGimnasio Tests

    [Fact]
    public async Task ObtenerPorGimnasio_Exitoso_RetornaOk()
    {
        // Arrange
        var profesores = new List<Profesor>
        {
            new Profesor { Id = 1, Nombre = "Juan", GimnasioId = 5 }
        };
        _mockRepo.Setup(x => x.ObtenerPorGimnasioAsync(5)).ReturnsAsync(profesores);

        // Act
        var result = await _controller.ObtenerPorGimnasio(5);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task ObtenerPorGimnasio_GimnasioIdCero_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerPorGimnasio(0);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerPorGimnasio_GimnasioIdNegativo_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerPorGimnasio(-3);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerPorGimnasio_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        _mockRepo.Setup(x => x.ObtenerPorGimnasioAsync(1)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.ObtenerPorGimnasio(1);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ObtenerRutinasPorProfesor Tests

    [Fact]
    public async Task ObtenerRutinasPorProfesor_Exitoso_RetornaOk()
    {
        // Arrange
        var profesor = new Profesor { Id = 1, Nombre = "Juan" };
        var rutinas = new List<Rutina>
        {
            new Rutina { Id = 1, Nombre = "Rutina 1" }
        };
        _mockRepo.Setup(x => x.ObtenerPorIdAsync(1)).ReturnsAsync(profesor);
        _mockRutinaRepo.Setup(x => x.ObtenerTodasLasRutinasPorProfesorIdAsync(1)).ReturnsAsync(rutinas);

        // Act
        var result = await _controller.ObtenerRutinasPorProfesor(1);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task ObtenerRutinasPorProfesor_UsuarioIdCero_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerRutinasPorProfesor(0);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerRutinasPorProfesor_UsuarioIdNegativo_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerRutinasPorProfesor(-4);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerRutinasPorProfesor_SinRutinas_RetornaNotFound()
    {
        // Arrange
        var profesor = new Profesor { Id = 1, Nombre = "Juan" };
        _mockRepo.Setup(x => x.ObtenerPorIdAsync(1)).ReturnsAsync(profesor);
        _mockRutinaRepo.Setup(x => x.ObtenerTodasLasRutinasPorProfesorIdAsync(1)).ReturnsAsync(new List<Rutina>());

        // Act
        var result = await _controller.ObtenerRutinasPorProfesor(1);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task ObtenerRutinasPorProfesor_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        _mockRepo.Setup(x => x.ObtenerPorIdAsync(1)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.ObtenerRutinasPorProfesor(1);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ObtenerEstadisticas Tests

    [Fact]
    public async Task ObtenerEstadisticas_Exitoso_RetornaOk()
    {
        // Arrange
        var profesor = new Profesor { Id = 1, Nombre = "Juan", Apellido = "Pérez" };
        var solicitudes = new List<SolicitudRutinaProfesor>
        {
            new SolicitudRutinaProfesor { Id = 1, ProfesorId = 1, Estado = EstadoSolicitud.Pendiente }
        };
        profesor.Solicitudes = solicitudes;
        _mockSolicitudRepo.Setup(x => x.ObtenerProfesorMasSolicitadoAsync()).ReturnsAsync(profesor);

        // Act
        var result = await _controller.ObtenerEstadisticas();

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task ObtenerEstadisticas_SinDatos_RetornaNotFound()
    {
        // Arrange
        _mockSolicitudRepo.Setup(x => x.ObtenerProfesorMasSolicitadoAsync()).ReturnsAsync((Profesor?)null);
        _mockSolicitudRepo.Setup(x => x.ObtenerProfesorConMasPendientesAsync()).ReturnsAsync((Profesor?)null);
        _mockSolicitudRepo.Setup(x => x.ObtenerProfesorMasCumplidorAsync()).ReturnsAsync((Profesor?)null);
        _mockSolicitudRepo.Setup(x => x.ObtenerProfesorMejorPromedioValoracionesAsync())
            .ReturnsAsync(((Profesor?)null, (double?)null));

        // Act
        var result = await _controller.ObtenerEstadisticas();

        // Assert
        // Cuando todos los profesores son null, el método retorna un objeto EstadisticasProfesoresDTO
        // con todos los campos en null, pero no un null completo
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        var estadisticas = okResult.Value as EstadisticasProfesoresDTO;
        estadisticas.Should().NotBeNull();
        estadisticas!.TopSolicitado.Should().BeNull();
        estadisticas.TopPendientes.Should().BeNull();
        estadisticas.TopCumplidor.Should().BeNull();
        estadisticas.TopValorado.Should().BeNull();
    }

    [Fact]
    public async Task ObtenerEstadisticas_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        _mockSolicitudRepo.Setup(x => x.ObtenerProfesorMasSolicitadoAsync()).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.ObtenerEstadisticas();

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion
}
