//using System.Security.Claims;
//using AutoMapper;
//using FitRank_API.Application.CasosDeUso.AdministradorCasosDeUso;
//using FitRank_API.Application.CasosDeUso.Invitacion;
//using FitRank_API.Application.CasosDeUso.Invitacion.RegistrarInvitacionCasoDeUso;
//using FitRank_API.Application.CasosDeUso.SocioCasosDeUso;
//using FitRank_API.Application.DTOs.AdministradorDTOs;
//using FitRank_API.Application.DTOs.Invitacion;
//using FitRank_API.Application.DTOs.QR;
//using FitRank_API.Application.DTOs.UsuarioDTOs;
//using FitRank_API.Domain.Entities;
//using FitRank_API.Infrastructure.Interfaces;
//using FitRank_API.Presentacion.Controllers;
//using FluentAssertions;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Moq;
//using Xunit;

//namespace FitRank_API.tests.ControllersTests;

//public class AdminControllerTests
//{
//    private readonly AdminController _adminController;
//    private readonly Mock<AgregarInvitacionCasoDeUso> _mockAgregar;
//    private readonly Mock<FallbackEfectivoCasoDeUso> _mockFallback;
//    private readonly Mock<EnviarEmailQrCasoDeUso> _mockEnviarEmailQr;
//    private readonly Mock<AgregarAdministradorCasoDeUso> _mockAgregarAdmin;
//    private readonly Mock<EliminarAdministradorCasoDeUso> _mockEliminarAdmin;
//    private readonly Mock<ValidarQrCasoDeUso> _mockValidarQr;
//    private readonly Mock<ObtenerAdministradorCasoDeUso> _mockObtenerAdmin;
//    private readonly Mock<BorrarSocioCompletoCasoDeUso> _mockBorrarSocioCompleto;

//    public AdminControllerTests()
//    {
//        var mockRepositorio = new Mock<IAdministradorRepositorio>();
//        var mockMapper = new Mock<IMapper>();

//        _mockAgregar = new Mock<AgregarInvitacionCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
//        _mockFallback = new Mock<FallbackEfectivoCasoDeUso>(mockRepositorio.Object);
//        _mockEnviarEmailQr = new Mock<EnviarEmailQrCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
//        _mockAgregarAdmin = new Mock<AgregarAdministradorCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
//        _mockEliminarAdmin = new Mock<EliminarAdministradorCasoDeUso>(mockRepositorio.Object);
//        _mockValidarQr = new Mock<ValidarQrCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
//        _mockObtenerAdmin = new Mock<ObtenerAdministradorCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
//        _mockBorrarSocioCompleto = new Mock<BorrarSocioCompletoCasoDeUso>(mockRepositorio.Object);

//        _adminController = new AdminController(
//            _mockAgregar.Object,
//            _mockFallback.Object,
//            _mockEnviarEmailQr.Object,
//            _mockAgregarAdmin.Object,
//            _mockEliminarAdmin.Object,
//            _mockValidarQr.Object,
//            _mockObtenerAdmin.Object,
//            _mockBorrarSocioCompleto.Object
//            );
//    }

//    private void SetUserWithAdminId(int adminId)
//    {
//        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, adminId.ToString()) };
//        var identity = new ClaimsIdentity(claims, "TestAuth");
//        var user = new ClaimsPrincipal(identity);
//        _adminController.ControllerContext = new ControllerContext
//        {
//            HttpContext = new DefaultHttpContext { User = user }
//        };
//    }

//    //Agregar agrega administrador exitosamente
//    [Fact]
//    public async Task AgregarAdministrador_Exitosamente()
//    {
//        // Arrange
//        var nuevoAdmin = new AgregarAdministradorDTO
//        {
//            Nombre = "Carlos",
//            Apellido = "Lopez",
//            Email = "carlozlopez@gmail.com",
//            Telefono = "0987654321",
//            Dni = 87654321,
//            Password = "securepassword",
//            NombreUsuario = "carloz",
//            Cuil = "20-87654321-3",
//            Direccion = "Calle Falsa 123",
//            Localidad = "Ciudad"
//        };

//        var adminDevuelto = new Administrador
//        {
//            Id = 1,
//            Nombre = "Carlos",
//            Apellido = "Lopez",
//            Email = "carlozlopez@gmail.com",
//            Telefono = "0987654321",
//            Dni = 87654321,
//            NombreUsuario = "carloz",
//            Cuil = "20-87654321-3",
//            Direccion = "Calle Falsa 123",
//            Localidad = "Ciudad",
//            CuotaPagadaHasta = DateTime.UtcNow,
//            EsActivado = true,
//            FechaNacimiento = DateTime.UtcNow.AddYears(-30),
//            GimnasioId = 1,
//            FotoDePerfil = "foto",
//            Rol = "Administrador",
//            Sexo = "Masculino",
//            PasswordHash = "hashedpassword",
//            QrToken = "qrtoken",
//            Estado = "Activo"
//        };



//        _mockAgregarAdmin.Setup(x => x.Ejecutar(nuevoAdmin))
//            .ReturnsAsync(adminDevuelto);

//        // Act
//        var resultado = await _adminController.Agregar(nuevoAdmin);

//        // FluentAssert
//        var createdAtResultResult = resultado as CreatedAtActionResult;
//        createdAtResultResult.Should().NotBeNull();
//        createdAtResultResult!.StatusCode.Should().Be(201);
//        var adminResult = createdAtResultResult.Value as Administrador;
//        adminResult.Should().NotBeNull();
//        adminResult!.Id.Should().Be(adminDevuelto.Id);
//    }

//    [Fact]
//    public async Task GenerarInvitacion_Exitoso()
//    {
//        SetUserWithAdminId(1);
//        var dto = new GenerarInvitacionDTO { Email = "test@mail.com" };
//        var response = new InvitacionResponseDTO { Success = true, Mensaje = "ok" };
//        _mockAgregar.Setup(x => x.Ejecutar(dto, 1)).ReturnsAsync(response);

//        var result = await _adminController.GenerarInvitacion(dto);

//        var okResult = result as OkObjectResult;
//        okResult.Should().NotBeNull();
//        okResult!.StatusCode.Should().Be(200);
//        okResult.Value.Should().Be(response);
//    }

//    [Fact]
//    public async Task GenerarInvitacion_ModelStateInvalido()
//    {
//        _adminController.ModelState.AddModelError("Email", "Requerido");
//        var dto = new GenerarInvitacionDTO();
//        var result = await _adminController.GenerarInvitacion(dto);
//        var badRequest = result as BadRequestObjectResult;
//        badRequest.Should().NotBeNull();
//        badRequest!.StatusCode.Should().Be(400);
//    }

//    [Fact]
//    public async Task GenerarInvitacion_SinAdminId()
//    {
//        _adminController.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
//        var dto = new GenerarInvitacionDTO { Email = "test@mail.com" };
//        var result = await _adminController.GenerarInvitacion(dto);
//        var unauthorized = result as UnauthorizedObjectResult;
//        unauthorized.Should().NotBeNull();
//        unauthorized!.StatusCode.Should().Be(401);
//    }

//    [Fact]
//    public async Task GenerarInvitacion_NoSuccess()
//    {
//        SetUserWithAdminId(1);
//        var dto = new GenerarInvitacionDTO { Email = "test@mail.com" };
//        var response = new InvitacionResponseDTO { Success = false, Mensaje = "error" };
//        _mockAgregar.Setup(x => x.Ejecutar(dto, 1)).ReturnsAsync(response);

//        var result = await _adminController.GenerarInvitacion(dto);

//        var badRequest = result as BadRequestObjectResult;
//        badRequest.Should().NotBeNull();
//        badRequest!.StatusCode.Should().Be(400);
//        (badRequest.Value as dynamic).Mensaje.Should().Be("error");
//    }

//    [Fact]
//    public async Task FallbackEfectivo_Exitoso()
//    {
//        SetUserWithAdminId(1);
//        var dto = new FallbackEfectivoDTO { InvitacionId = 1 };
//        var response = new InvitacionResponseDTO { Success = true, Mensaje = "ok" };
//        _mockFallback.Setup(x => x.Ejecutar(dto, 1)).ReturnsAsync(response);

//        var result = await _adminController.FallbackEfectivo(dto);

//        var okResult = result as OkObjectResult;
//        okResult.Should().NotBeNull();
//        okResult!.StatusCode.Should().Be(200);
//        okResult.Value.Should().Be(response);
//    }

//    [Fact]
//    public async Task FallbackEfectivo_ModelStateInvalido()
//    {
//        _adminController.ModelState.AddModelError("InvitacionId", "Requerido");
//        var dto = new FallbackEfectivoDTO();
//        var result = await _adminController.FallbackEfectivo(dto);
//        var badRequest = result as BadRequestObjectResult;
//        badRequest.Should().NotBeNull();
//        badRequest!.StatusCode.Should().Be(400);
//    }

//    [Fact]
//    public async Task FallbackEfectivo_SinAdminId()
//    {
//        _adminController.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext() };
//        var dto = new FallbackEfectivoDTO();
//        var result = await _adminController.FallbackEfectivo(dto);
//        var unauthorized = result as UnauthorizedObjectResult;
//        unauthorized.Should().NotBeNull();
//        unauthorized!.StatusCode.Should().Be(401);
//    }

//    [Fact]
//    public async Task FallbackEfectivo_NoSuccess()
//    {
//        SetUserWithAdminId(1);
//        var dto = new FallbackEfectivoDTO { InvitacionId = 1 };
//        var response = new InvitacionResponseDTO { Success = false, Mensaje = "error" };
//        _mockFallback.Setup(x => x.Ejecutar(dto, 1)).ReturnsAsync(response);

//        var result = await _adminController.FallbackEfectivo(dto);

//        var badRequest = result as BadRequestObjectResult;
//        badRequest.Should().NotBeNull();
//        badRequest!.StatusCode.Should().Be(400);
//        (badRequest.Value as dynamic).Mensaje.Should().Be("error");
//    }

//    [Fact]
//    public async Task EnviarEmailQr_Exitoso()
//    {
//        var dto = new EmailDTO { UsuarioId = 1, EmailDestinatario = "mail@mail.com" };
//        var response = new EmailResponseDTO { Mensaje = "ok" };
//        _mockEnviarEmailQr.Setup(x => x.Ejecutar(dto)).ReturnsAsync(response);

//        var result = await _adminController.EnviarEmailQr(dto);

//        var okResult = result as OkObjectResult;
//        okResult.Should().NotBeNull();
//        okResult!.StatusCode.Should().Be(200);
//        okResult.Value.Should().Be(response);
//    }

//    [Fact]
//    public async Task EnviarEmailQr_ModelStateInvalido()
//    {
//        _adminController.ModelState.AddModelError("UsuarioId", "Requerido");
//        var dto = new EmailDTO();
//        var result = await _adminController.EnviarEmailQr(dto);
//        var badRequest = result as BadRequestObjectResult;
//        badRequest.Should().NotBeNull();
//        badRequest!.StatusCode.Should().Be(400);
//    }

//    [Fact]
//    public async Task EnviarEmailQr_NoSuccess()
//    {
//        var dto = new EmailDTO { UsuarioId = 1, EmailDestinatario = "mail@mail.com" };
//        var response = new EmailResponseDTO { Mensaje = "error" };
//        _mockEnviarEmailQr.Setup(x => x.Ejecutar(dto)).ReturnsAsync(response);

//        var result = await _adminController.EnviarEmailQr(dto);

//        var badRequest = result as BadRequestObjectResult;
//        badRequest.Should().NotBeNull();
//        badRequest!.StatusCode.Should().Be(400);
//        (badRequest.Value as dynamic).Mensaje.Should().Be("error");
//    }

//    [Fact]
//    public async Task ValidarQr_Exitoso()
//    {
//        SetUserWithAdminId(1);
//        var dto = new QrValidationDTO { QrData = "data" };
//        var response = new QrValidationResponseDTO { Valido = true, Mensaje = "ok" };
//        _mockValidarQr.Setup(x => x.Ejecutar(dto, 1)).ReturnsAsync(response);

//        var result = await _adminController.ValidarQr(dto);

//        var okResult = result as OkObjectResult;
//        okResult.Should().NotBeNull();
//        okResult!.StatusCode.Should().Be(200);
//        okResult.Value.Should().Be(response);
//    }

//    [Fact]
//    public async Task ValidarQr_ModelStateInvalido()
//    {
//        SetUserWithAdminId(1);
//        _adminController.ModelState.AddModelError("QrData", "Requerido");
//        var dto = new QrValidationDTO();
//        var result = await _adminController.ValidarQr(dto);
//        var badRequest = result as BadRequestObjectResult;
//        badRequest.Should().NotBeNull();
//        badRequest!.StatusCode.Should().Be(400);
//    }

//    [Fact]
//    public async Task ValidarQr_NoValido()
//    {
//        SetUserWithAdminId(1);
//        var dto = new QrValidationDTO { QrData = "data" };
//        var response = new QrValidationResponseDTO { Valido = false, Mensaje = "error" };
//        _mockValidarQr.Setup(x => x.Ejecutar(dto, 1)).ReturnsAsync(response);

//        var result = await _adminController.ValidarQr(dto);

//        var badRequest = result as BadRequestObjectResult;
//        badRequest.Should().NotBeNull();
//        badRequest!.StatusCode.Should().Be(400);
//        (badRequest.Value as dynamic).Mensaje.Should().Be("error");
//    }

//    [Fact]
//    public async Task Eliminar_Exitoso()
//    {
//        _mockEliminarAdmin.Setup(x => x.Ejecutar(1)).ReturnsAsync(true);

//        var result = await _adminController.Eliminar(1);

//        var okResult = result as OkObjectResult;
//        okResult.Should().NotBeNull();
//        okResult!.StatusCode.Should().Be(200);
//        okResult.Value.Should().BeEquivalentTo(new { Mensaje = "Administrador eliminado correctamente" });
//    }

//    [Fact]
//    public async Task Eliminar_NoEncontrado()
//    {
//        _mockEliminarAdmin.Setup(x => x.Ejecutar(1)).ReturnsAsync(false);

//        var result = await _adminController.Eliminar(1);

//        var notFound = result as NotFoundObjectResult;
//        notFound.Should().NotBeNull();
//        notFound!.StatusCode.Should().Be(404);
//        (notFound.Value as dynamic).Mensaje.Should().Be("Administrador no encontrado");
//    }

//    [Fact]
//    public async Task ObtenerTodosLosAdministradores_Exitoso()
//    {
//        var admins = new List<ObtenerAdministradorDTO>
//        {
//            new ObtenerAdministradorDTO { Email = "admin@mail.com" }
//        };
//        _mockObtenerAdmin.Setup(x => x.Ejecutar()).ReturnsAsync(admins);

//        var result = await _adminController.ObtenerTodosLosAdministradores();

//        var okResult = result as OkObjectResult;
//        okResult.Should().NotBeNull();
//        okResult!.StatusCode.Should().Be(200);
//        okResult.Value.Should().BeEquivalentTo(admins);
//    }

//    [Fact]
//    public async Task BorrarCompleto_Exitoso()
//    {
//        _mockBorrarSocioCompleto.Setup(x => x.Ejecutar(1)).ReturnsAsync("ok");

//        var result = await _adminController.BorrarCompleto(1);

//        var okResult = result as OkObjectResult;
//        okResult.Should().NotBeNull();
//        okResult!.StatusCode.Should().Be(200);
//        okResult.Value.Should().Be("ok");
//    }
//}