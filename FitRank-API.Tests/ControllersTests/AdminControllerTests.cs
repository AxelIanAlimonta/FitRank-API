using System.Security.Claims;
using AutoMapper;
using FitRank_API.Application.CasosDeUso.AdministradorCasosDeUso;
using FitRank_API.Application.CasosDeUso.Invitacion;
using FitRank_API.Application.CasosDeUso.Invitacion.RegistrarInvitacionCasoDeUso;
using FitRank_API.Application.CasosDeUso.SocioCasosDeUso;
using FitRank_API.Application.CasosDeUso.UsuarioCasosDeUso;
using FitRank_API.Application.DTOs.AdministradorDTOs;
using FitRank_API.Application.DTOs.Asistencia;
using FitRank_API.Application.DTOs.Invitacion;
using FitRank_API.Application.DTOs.QR;
using FitRank_API.Application.DTOs.UsuarioDTOs;
using FitRank_API.Application.Helpers;
using FitRank_API.Application.Interfaces;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using FitRank_API.Presentacion.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace FitRank_API.Tests.ControllersTests;

public class AdminControllerTests
{
    private readonly AdminController _adminController;
    private readonly Mock<AgregarInvitacionCasoDeUso> _mockAgregar;
    private readonly Mock<FallbackEfectivoCasoDeUso> _mockFallback;
    private readonly Mock<EnviarEmailQrCasoDeUso> _mockEnviarEmailQr;
    private readonly Mock<AgregarAdministradorCasoDeUso> _mockAgregarAdmin;
    private readonly Mock<EliminarAdministradorCasoDeUso> _mockEliminarAdmin;
    private readonly Mock<ValidarQrCasoDeUso> _mockValidarQr;
    private readonly Mock<ObtenerAdministradorCasoDeUso> _mockObtenerAdmin;
    private readonly Mock<BorrarSocioCompletoCasoDeUso> _mockBorrarSocioCompleto;

    public AdminControllerTests()
    {
        var mockInvitacionRepo = new Mock<IInvitacionRepositorio>();
        var mockUsuarioRepo = new Mock<IUsuarioRepositorio>();
        var mockAdminRepo = new Mock<IAdministradorRepositorio>();
        var mockAsistenciaRepo = new Mock<IAsistenciaRepositorio>();
        var mockGimnasioRepo = new Mock<IGimnasioRepositorio>();
        var mockMapper = new Mock<IMapper>();
        var mockPasswordService = new Mock<IPasswordService>();
        var mockConfig = new Mock<IConfiguration>();
        var mockQrHelper = new Mock<QrHelper>(mockConfig.Object);

        _mockAgregar = new Mock<AgregarInvitacionCasoDeUso>(null, null, null, null, null, null, null, null);
        _mockFallback = new Mock<FallbackEfectivoCasoDeUso>(null, null, null, null);
        _mockEnviarEmailQr = new Mock<EnviarEmailQrCasoDeUso>(null, null, null);
        _mockAgregarAdmin = new Mock<AgregarAdministradorCasoDeUso>(mockAdminRepo.Object, mockMapper.Object, mockPasswordService.Object);
        _mockEliminarAdmin = new Mock<EliminarAdministradorCasoDeUso>(mockAdminRepo.Object);
        _mockValidarQr = new Mock<ValidarQrCasoDeUso>(mockUsuarioRepo.Object, mockAsistenciaRepo.Object, mockConfig.Object, mockQrHelper.Object, mockGimnasioRepo.Object);
        _mockObtenerAdmin = new Mock<ObtenerAdministradorCasoDeUso>(mockAdminRepo.Object, mockMapper.Object);
        _mockBorrarSocioCompleto = new Mock<BorrarSocioCompletoCasoDeUso>(null);

        _adminController = new AdminController(
            _mockAgregar.Object,
            _mockFallback.Object,
            _mockEnviarEmailQr.Object,
            _mockAgregarAdmin.Object,
            _mockEliminarAdmin.Object,
            _mockValidarQr.Object,
            _mockObtenerAdmin.Object,
            _mockBorrarSocioCompleto.Object
        );
    }

    private void SetUserWithAdminId(int adminId)
    {
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, adminId.ToString()) };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var user = new ClaimsPrincipal(identity);
        _adminController.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    #region GenerarInvitacion Tests

    [Fact]
    public async Task GenerarInvitacion_Exitoso()
    {
        SetUserWithAdminId(1);
        var dto = new GenerarInvitacionDTO { Email = "test@mail.com", Nombre = "Test" };
        var response = new InvitacionResponseDTO { Success = true, Mensaje = "ok" };
        _mockAgregar.Setup(x => x.Ejecutar(dto, 1)).ReturnsAsync(response);

        var result = await _adminController.GenerarInvitacion(dto);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().Be(response);
    }

    [Fact]
    public async Task GenerarInvitacion_DtoNulo_RetornaBadRequest()
    {
        SetUserWithAdminId(1);

        var result = await _adminController.GenerarInvitacion(null);

        var badRequest = result as BadRequestObjectResult;
        badRequest.Should().NotBeNull();
        badRequest!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task GenerarInvitacion_ModelStateInvalido()
    {
        SetUserWithAdminId(1);
        _adminController.ModelState.AddModelError("Email", "Requerido");
        var dto = new GenerarInvitacionDTO();

        var result = await _adminController.GenerarInvitacion(dto);

        var badRequest = result as BadRequestObjectResult;
        badRequest.Should().NotBeNull();
        badRequest!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task GenerarInvitacion_SinAdminId()
    {
        _adminController.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        var dto = new GenerarInvitacionDTO { Email = "test@mail.com", Nombre = "Test" };

        var result = await _adminController.GenerarInvitacion(dto);

        var unauthorized = result as UnauthorizedObjectResult;
        unauthorized.Should().NotBeNull();
        unauthorized!.StatusCode.Should().Be(401);
    }

    [Fact]
    public async Task GenerarInvitacion_NoSuccess()
    {
        SetUserWithAdminId(1);
        var dto = new GenerarInvitacionDTO { Email = "test@mail.com", Nombre = "Test" };
        var response = new InvitacionResponseDTO { Success = false, Mensaje = "error" };
        _mockAgregar.Setup(x => x.Ejecutar(dto, 1)).ReturnsAsync(response);

        var result = await _adminController.GenerarInvitacion(dto);

        var badRequest = result as BadRequestObjectResult;
        badRequest.Should().NotBeNull();
        badRequest!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task GenerarInvitacion_EmailDuplicado_RetornaBadRequest()
    {
        SetUserWithAdminId(1);
        var dto = new GenerarInvitacionDTO { Email = "duplicado@mail.com", Nombre = "Test" };
        var ex = new Exception("EMAIL_DUPLICADO");
        ex.Data["socioId"] = 123;
        _mockAgregar.Setup(x => x.Ejecutar(dto, 1)).ThrowsAsync(ex);

        var result = await _adminController.GenerarInvitacion(dto);

        var badRequest = result as BadRequestObjectResult;
        badRequest.Should().NotBeNull();
        badRequest!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task GenerarInvitacion_DniDuplicado_RetornaBadRequest()
    {
        SetUserWithAdminId(1);
        var dto = new GenerarInvitacionDTO { Email = "test@mail.com", Nombre = "Test", Dni = 12345678 };
        var ex = new Exception("DNI_DUPLICADO");
        ex.Data["socioId"] = 456;
        _mockAgregar.Setup(x => x.Ejecutar(dto, 1)).ThrowsAsync(ex);

        var result = await _adminController.GenerarInvitacion(dto);

        var badRequest = result as BadRequestObjectResult;
        badRequest.Should().NotBeNull();
        badRequest!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task GenerarInvitacion_ExcepcionGenerica_RetornaInternalServerError()
    {
        SetUserWithAdminId(1);
        var dto = new GenerarInvitacionDTO { Email = "test@mail.com", Nombre = "Test" };
        _mockAgregar.Setup(x => x.Ejecutar(dto, 1)).ThrowsAsync(new Exception("Error genérico"));

        var result = await _adminController.GenerarInvitacion(dto);

        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region FallbackEfectivo Tests

    [Fact]
    public async Task FallbackEfectivo_Exitoso()
    {
        SetUserWithAdminId(1);
        var dto = new FallbackEfectivoDTO { InvitacionId = 1 };
        var response = new InvitacionResponseDTO { Success = true, Mensaje = "ok" };
        _mockFallback.Setup(x => x.Ejecutar(dto, 1)).ReturnsAsync(response);

        var result = await _adminController.FallbackEfectivo(dto);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().Be(response);
    }

    [Fact]
    public async Task FallbackEfectivo_DtoNulo_RetornaBadRequest()
    {
        SetUserWithAdminId(1);

        var result = await _adminController.FallbackEfectivo(null);

        var badRequest = result as BadRequestObjectResult;
        badRequest.Should().NotBeNull();
        badRequest!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task FallbackEfectivo_ModelStateInvalido()
    {
        SetUserWithAdminId(1);
        _adminController.ModelState.AddModelError("InvitacionId", "Requerido");
        var dto = new FallbackEfectivoDTO();

        var result = await _adminController.FallbackEfectivo(dto);

        var badRequest = result as BadRequestObjectResult;
        badRequest.Should().NotBeNull();
        badRequest!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task FallbackEfectivo_SinAdminId()
    {
        _adminController.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
        var dto = new FallbackEfectivoDTO();

        var result = await _adminController.FallbackEfectivo(dto);

        var unauthorized = result as UnauthorizedObjectResult;
        unauthorized.Should().NotBeNull();
        unauthorized!.StatusCode.Should().Be(401);
    }

    [Fact]
    public async Task FallbackEfectivo_NoSuccess()
    {
        SetUserWithAdminId(1);
        var dto = new FallbackEfectivoDTO { InvitacionId = 1 };
        var response = new InvitacionResponseDTO { Success = false, Mensaje = "error" };
        _mockFallback.Setup(x => x.Ejecutar(dto, 1)).ReturnsAsync(response);

        var result = await _adminController.FallbackEfectivo(dto);

        var badRequest = result as BadRequestObjectResult;
        badRequest.Should().NotBeNull();
        badRequest!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task FallbackEfectivo_ExcepcionGenerica_RetornaInternalServerError()
    {
        SetUserWithAdminId(1);
        var dto = new FallbackEfectivoDTO { InvitacionId = 1 };
        _mockFallback.Setup(x => x.Ejecutar(dto, 1)).ThrowsAsync(new Exception());

        var result = await _adminController.FallbackEfectivo(dto);

        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region EnviarEmailQr Tests

    [Fact]
    public async Task EnviarEmailQr_Exitoso()
    {
        var dto = new EmailDTO { UsuarioId = 1, EmailDestinatario = "mail@mail.com" };
        var response = new EmailResponseDTO { Success = true, Mensaje = "ok" };
        _mockEnviarEmailQr.Setup(x => x.Ejecutar(dto)).ReturnsAsync(response);

        var result = await _adminController.EnviarEmailQr(dto);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().Be(response);
    }

    [Fact]
    public async Task EnviarEmailQr_DtoNulo_RetornaBadRequest()
    {
        var result = await _adminController.EnviarEmailQr(null);

        var badRequest = result as BadRequestObjectResult;
        badRequest.Should().NotBeNull();
        badRequest!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task EnviarEmailQr_ModelStateInvalido()
    {
        _adminController.ModelState.AddModelError("UsuarioId", "Requerido");
        var dto = new EmailDTO();

        var result = await _adminController.EnviarEmailQr(dto);

        var badRequest = result as BadRequestObjectResult;
        badRequest.Should().NotBeNull();
        badRequest!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task EnviarEmailQr_NoSuccess()
    {
        var dto = new EmailDTO { UsuarioId = 1, EmailDestinatario = "mail@mail.com" };
        var response = new EmailResponseDTO { Success = false, Mensaje = "error" };
        _mockEnviarEmailQr.Setup(x => x.Ejecutar(dto)).ReturnsAsync(response);

        var result = await _adminController.EnviarEmailQr(dto);

        var badRequest = result as BadRequestObjectResult;
        badRequest.Should().NotBeNull();
        badRequest!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task EnviarEmailQr_ExcepcionGenerica_RetornaInternalServerError()
    {
        var dto = new EmailDTO { UsuarioId = 1, EmailDestinatario = "mail@mail.com" };
        _mockEnviarEmailQr.Setup(x => x.Ejecutar(dto)).ThrowsAsync(new Exception());

        var result = await _adminController.EnviarEmailQr(dto);

        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ValidarQr Tests

    [Fact]
    public async Task ValidarQr_Exitoso()
    {
        SetUserWithAdminId(1);
        var dto = new QrValidationDTO { QrData = "data" };
        var response = new QrValidationResponseDTO { Valido = true, Mensaje = "ok" };
        _mockValidarQr.Setup(x => x.Ejecutar(dto, 1)).ReturnsAsync(response);

        var result = await _adminController.ValidarQr(dto);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().Be(response);
    }

    [Fact]
    public async Task ValidarQr_DtoNulo_RetornaBadRequest()
    {
        SetUserWithAdminId(1);

        var result = await _adminController.ValidarQr(null);

        var badRequest = result as BadRequestObjectResult;
        badRequest.Should().NotBeNull();
        badRequest!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ValidarQr_ModelStateInvalido()
    {
        SetUserWithAdminId(1);
        _adminController.ModelState.AddModelError("QrData", "Requerido");
        var dto = new QrValidationDTO();

        var result = await _adminController.ValidarQr(dto);

        var badRequest = result as BadRequestObjectResult;
        badRequest.Should().NotBeNull();
        badRequest!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ValidarQr_NoValido()
    {
        SetUserWithAdminId(1);
        var dto = new QrValidationDTO { QrData = "data" };
        var response = new QrValidationResponseDTO { Valido = false, Mensaje = "error" };
        _mockValidarQr.Setup(x => x.Ejecutar(dto, 1)).ReturnsAsync(response);

        var result = await _adminController.ValidarQr(dto);

        var badRequest = result as BadRequestObjectResult;
        badRequest.Should().NotBeNull();
        badRequest!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ValidarQr_ExcepcionGenerica_RetornaInternalServerError()
    {
        SetUserWithAdminId(1);
        var dto = new QrValidationDTO { QrData = "data" };
        _mockValidarQr.Setup(x => x.Ejecutar(dto, 1)).ThrowsAsync(new Exception());

        var result = await _adminController.ValidarQr(dto);

        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region AgregarAdministrador Tests

    [Fact]
    public async Task AgregarAdministrador_Exitosamente()
    {
        var nuevoAdmin = new AgregarAdministradorDTO
        {
            Nombre = "Carlos",
            Apellido = "Lopez",
            Email = "carlozlopez@gmail.com",
            Telefono = "0987654321",
            Dni = 87654321,
            Password = "securepassword",
            NombreUsuario = "carloz",
            Cuil = "20-87654321-3",
            Direccion = "Calle Falsa 123",
            Localidad = "Ciudad"
        };

        var adminDevuelto = new Administrador
        {
            Id = 1,
            Nombre = "Carlos",
            Apellido = "Lopez",
            Email = "carlozlopez@gmail.com",
            Telefono = "0987654321",
            Dni = 87654321,
            NombreUsuario = "carloz",
            Cuil = "20-87654321-3",
            Direccion = "Calle Falsa 123",
            Localidad = "Ciudad"
        };

        _mockAgregarAdmin.Setup(x => x.Ejecutar(nuevoAdmin)).ReturnsAsync(adminDevuelto);

        var resultado = await _adminController.Agregar(nuevoAdmin);

        var createdAtResult = resultado as CreatedAtActionResult;
        createdAtResult.Should().NotBeNull();
        createdAtResult!.StatusCode.Should().Be(201);
        var adminResult = createdAtResult.Value as Administrador;
        adminResult.Should().NotBeNull();
        adminResult!.Id.Should().Be(adminDevuelto.Id);
    }

    [Fact]
    public async Task AgregarAdministrador_DtoNulo_RetornaBadRequest()
    {
        var result = await _adminController.Agregar(null);

        var badRequest = result as BadRequestObjectResult;
        badRequest.Should().NotBeNull();
        badRequest!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task AgregarAdministrador_ModelStateInvalido_RetornaBadRequest()
    {
        _adminController.ModelState.AddModelError("Email", "Requerido");
        var dto = new AgregarAdministradorDTO();

        var result = await _adminController.Agregar(dto);

        var badRequest = result as BadRequestObjectResult;
        badRequest.Should().NotBeNull();
        badRequest!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task AgregarAdministrador_ExcepcionGenerica_RetornaInternalServerError()
    {
        var dto = new AgregarAdministradorDTO { Email = "test@mail.com" };
        _mockAgregarAdmin.Setup(x => x.Ejecutar(dto)).ThrowsAsync(new Exception());

        var result = await _adminController.Agregar(dto);

        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region Eliminar Tests

    [Fact]
    public async Task Eliminar_Exitoso()
    {
        _mockEliminarAdmin.Setup(x => x.Ejecutar(1)).ReturnsAsync(true);

        var result = await _adminController.Eliminar(1);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task Eliminar_NoEncontrado()
    {
        _mockEliminarAdmin.Setup(x => x.Ejecutar(1)).ReturnsAsync(false);

        var result = await _adminController.Eliminar(1);

        var notFound = result as NotFoundObjectResult;
        notFound.Should().NotBeNull();
        notFound!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task Eliminar_IdCero_RetornaBadRequest()
    {
        var result = await _adminController.Eliminar(0);

        var badRequest = result as BadRequestObjectResult;
        badRequest.Should().NotBeNull();
        badRequest!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Eliminar_IdNegativo_RetornaBadRequest()
    {
        var result = await _adminController.Eliminar(-5);

        var badRequest = result as BadRequestObjectResult;
        badRequest.Should().NotBeNull();
        badRequest!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Eliminar_ExcepcionGenerica_RetornaInternalServerError()
    {
        _mockEliminarAdmin.Setup(x => x.Ejecutar(1)).ThrowsAsync(new Exception());

        var result = await _adminController.Eliminar(1);

        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ObtenerTodosLosAdministradores Tests

    [Fact]
    public async Task ObtenerTodosLosAdministradores_Exitoso()
    {
        var admins = new List<ObtenerAdministradorDTO>
        {
            new ObtenerAdministradorDTO { Email = "admin@mail.com" }
        };
        _mockObtenerAdmin.Setup(x => x.Ejecutar()).ReturnsAsync(admins);

        var result = await _adminController.ObtenerTodosLosAdministradores();

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(admins);
    }

    [Fact]
    public async Task ObtenerTodosLosAdministradores_ExcepcionGenerica_RetornaInternalServerError()
    {
        _mockObtenerAdmin.Setup(x => x.Ejecutar()).ThrowsAsync(new Exception());

        var result = await _adminController.ObtenerTodosLosAdministradores();

        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region BorrarCompleto Tests

    [Fact]
    public async Task BorrarCompleto_Exitoso()
    {
        _mockBorrarSocioCompleto.Setup(x => x.Ejecutar(1)).ReturnsAsync("ok");

        var result = await _adminController.BorrarCompleto(1);

        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().Be("ok");
    }

    [Fact]
    public async Task BorrarCompleto_IdCero_RetornaBadRequest()
    {
        var result = await _adminController.BorrarCompleto(0);

        var badRequest = result as BadRequestObjectResult;
        badRequest.Should().NotBeNull();
        badRequest!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task BorrarCompleto_IdNegativo_RetornaBadRequest()
    {
        var result = await _adminController.BorrarCompleto(-10);

        var badRequest = result as BadRequestObjectResult;
        badRequest.Should().NotBeNull();
        badRequest!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task BorrarCompleto_ExcepcionGenerica_RetornaInternalServerError()
    {
        _mockBorrarSocioCompleto.Setup(x => x.Ejecutar(1)).ThrowsAsync(new Exception());

        var result = await _adminController.BorrarCompleto(1);

        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion
}