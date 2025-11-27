using AutoMapper;
using FitRank_API.Application.CasosDeUso.UsuarioCasosDeUso;
using FitRank_API.Application.DTOs.Invitacion;
using FitRank_API.Application.DTOs.UsuarioDTOs;
using FitRank_API.Application.DTOs.UsuarioDTOs.ValidarAuth;
using FitRank_API.Application.Interfaces;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using FitRank_API.Presentacion.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace FitRank_API.tests.ControllersTests;

public class AuthControllerTests
{
    private readonly AuthController _controller;
    private readonly Mock<LoginUsuarioCasoDeUso> _mockLogin;
    private readonly Mock<RegistrarUsuarioCasoDeUso> _mockRegister;
    private readonly Mock<ValidarTokenActivacionCasoDeUso> _mockValidarToken;
    private readonly Mock<ActivarCuentaCasoDeUso> _mockActivarCuenta;
    private readonly Mock<AgregarUsuarioConInvitacionCasoDeUso> _mockAgregarConInvitacion;
    private readonly Mock<GenerarTokenCasoDeUso> _mockGenerarToken;
    private readonly Mock<IConfiguration> _mockConfig;

    public AuthControllerTests()
    {
        var mockUsuarioRepo = new Mock<IUsuarioRepositorio>();
        var mockGimnasioRepo = new Mock<IGimnasioRepositorio>();
        var mockMapper = new Mock<IMapper>();
        var mockPasswordService = new Mock<IPasswordService>();
        _mockConfig = new Mock<IConfiguration>();

        _mockLogin = new Mock<LoginUsuarioCasoDeUso>(mockUsuarioRepo.Object, mockMapper.Object, mockPasswordService.Object);
        _mockGenerarToken = new Mock<GenerarTokenCasoDeUso>(_mockConfig.Object, mockGimnasioRepo.Object);
        _mockRegister = new Mock<RegistrarUsuarioCasoDeUso>(mockUsuarioRepo.Object, _mockGenerarToken.Object, _mockConfig.Object, mockMapper.Object, mockPasswordService.Object);
        _mockValidarToken = new Mock<ValidarTokenActivacionCasoDeUso>(mockUsuarioRepo.Object);
        _mockActivarCuenta = new Mock<ActivarCuentaCasoDeUso>(mockUsuarioRepo.Object, mockPasswordService.Object);
        _mockAgregarConInvitacion = new Mock<AgregarUsuarioConInvitacionCasoDeUso>(null, null, null, null, null);

        _controller = new AuthController(
            _mockLogin.Object,
            _mockRegister.Object,
            _mockValidarToken.Object,
            _mockActivarCuenta.Object,
            _mockAgregarConInvitacion.Object,
            _mockGenerarToken.Object,
            _mockConfig.Object
        );
    }

    #region Login Tests

    [Fact]
    public async Task Login_Exitoso_RetornaOkConToken()
    {
        // Arrange
        var dto = new LoginDTO { Email = "test@test.com", Password = "password123" };
        var usuario = new Usuario { Id = 1, Email = "test@test.com", Rol = "User" };
        var usuarioDto = new UsuarioAuthDTO { Id = 1, Email = "test@test.com", Rol = "User" };
        var token = "fake-jwt-token";

        _mockLogin.Setup(x => x.Ejecutar(dto)).ReturnsAsync((usuario, usuarioDto));
        _mockGenerarToken.Setup(x => x.Ejecutar(usuario)).Returns(token);

        // Act
        var result = await _controller.Login(dto);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        var response = okResult.Value as AuthResponseDTO;
        response.Should().NotBeNull();
        response!.Token.Should().Be(token);
        response.User.Should().BeEquivalentTo(usuarioDto);
    }

    [Fact]
    public async Task Login_DtoNulo_RetornaBadRequest()
    {
        // Act
        var result = await _controller.Login(null);

        // Assert
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Login_ModelStateInvalido_RetornaBadRequest()
    {
        // Arrange
        _controller.ModelState.AddModelError("Email", "Requerido");
        var dto = new LoginDTO();

        // Act
        var result = await _controller.Login(dto);

        // Assert
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Login_CredencialesInvalidas_RetornaUnauthorized()
    {
        // Arrange
        var dto = new LoginDTO { Email = "test@test.com", Password = "wrongpassword" };
        _mockLogin.Setup(x => x.Ejecutar(dto)).ReturnsAsync(((Usuario, UsuarioAuthDTO)?)null);

        // Act
        var result = await _controller.Login(dto);

        // Assert
        var unauthorizedResult = result.Result as UnauthorizedObjectResult;
        unauthorizedResult.Should().NotBeNull();
        unauthorizedResult!.StatusCode.Should().Be(401);
    }

    [Fact]
    public async Task Login_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        var dto = new LoginDTO { Email = "test@test.com", Password = "password123" };
        _mockLogin.Setup(x => x.Ejecutar(dto)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.Login(dto);

        // Assert
        var statusCodeResult = result.Result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region Register Tests

    [Fact]
    public async Task Register_Exitoso_RetornaOkConToken()
    {
        // Arrange
        var dto = new RegisterDTO 
        { 
            Email = "newuser@test.com", 
            Password = "password123",
            Nombre = "Test",
            Dni = 12345678
        };
        var response = new AuthResponseDTO 
        { 
            Token = "fake-token", 
            User = new UsuarioAuthDTO { Id = 1, Email = dto.Email } 
        };

        _mockRegister.Setup(x => x.Ejecutar(dto)).ReturnsAsync(response);

        // Act
        var result = await _controller.Register(dto);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(response);
    }

    [Fact]
    public async Task Register_DtoNulo_RetornaBadRequest()
    {
        // Act
        var result = await _controller.Register(null);

        // Assert
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Register_ModelStateInvalido_RetornaBadRequest()
    {
        // Arrange
        _controller.ModelState.AddModelError("Email", "Requerido");
        var dto = new RegisterDTO();

        // Act
        var result = await _controller.Register(dto);

        // Assert
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Register_EmailYaExiste_RetornaBadRequest()
    {
        // Arrange
        var dto = new RegisterDTO { Email = "existing@test.com", Password = "password123", Nombre = "Test" };
        _mockRegister.Setup(x => x.Ejecutar(dto)).ReturnsAsync((AuthResponseDTO?)null);

        // Act
        var result = await _controller.Register(dto);

        // Assert
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task Register_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        var dto = new RegisterDTO { Email = "test@test.com", Password = "password123", Nombre = "Test" };
        _mockRegister.Setup(x => x.Ejecutar(dto)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.Register(dto);

        // Assert
        var statusCodeResult = result.Result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region RegisterInvitacion Tests

    [Fact]
    public async Task AgregarUsuarioConInvitacion_Exitoso_RetornaOk()
    {
        // Arrange
        var dto = new RegisterInvitacionDTO 
        { 
            TokenInvitacion = "valid-token", 
            Password = "password123" 
        };
        var response = new AuthResponseDTO 
        { 
            Token = "fake-token", 
            User = new UsuarioAuthDTO { Id = 1 } 
        };

        _mockAgregarConInvitacion.Setup(x => x.Ejecutar(dto)).ReturnsAsync(response);

        // Act
        var result = await _controller.AgregarUsuarioConInvitacion(dto);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(response);
    }

    [Fact]
    public async Task AgregarUsuarioConInvitacion_DtoNulo_RetornaBadRequest()
    {
        // Act
        var result = await _controller.AgregarUsuarioConInvitacion(null);

        // Assert
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task AgregarUsuarioConInvitacion_ModelStateInvalido_RetornaBadRequest()
    {
        // Arrange
        _controller.ModelState.AddModelError("TokenInvitacion", "Requerido");
        var dto = new RegisterInvitacionDTO();

        // Act
        var result = await _controller.AgregarUsuarioConInvitacion(dto);

        // Assert
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task AgregarUsuarioConInvitacion_TokenInvalido_RetornaBadRequest()
    {
        // Arrange
        var dto = new RegisterInvitacionDTO { TokenInvitacion = "invalid-token", Password = "password123" };
        _mockAgregarConInvitacion.Setup(x => x.Ejecutar(dto)).ReturnsAsync((AuthResponseDTO?)null);

        // Act
        var result = await _controller.AgregarUsuarioConInvitacion(dto);

        // Assert
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task AgregarUsuarioConInvitacion_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        var dto = new RegisterInvitacionDTO { TokenInvitacion = "token", Password = "password123" };
        _mockAgregarConInvitacion.Setup(x => x.Ejecutar(dto)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.AgregarUsuarioConInvitacion(dto);

        // Assert
        var statusCodeResult = result.Result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ValidarActivacion Tests

    [Fact]
    public async Task ValidarActivacion_TokenValido_RetornaOk()
    {
        // Arrange
        var dto = new ValidarActivacionDTO { Token = "valid-token" };
        _mockValidarToken.Setup(x => x.Ejecutar(dto.Token)).ReturnsAsync(true);

        // Act
        var result = await _controller.ValidarActivacion(dto);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task ValidarActivacion_DtoNulo_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ValidarActivacion(null);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ValidarActivacion_ModelStateInvalido_RetornaBadRequest()
    {
        // Arrange
        _controller.ModelState.AddModelError("Token", "Requerido");
        var dto = new ValidarActivacionDTO();

        // Act
        var result = await _controller.ValidarActivacion(dto);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ValidarActivacion_TokenInvalido_RetornaBadRequest()
    {
        // Arrange
        var dto = new ValidarActivacionDTO { Token = "invalid-token" };
        _mockValidarToken.Setup(x => x.Ejecutar(dto.Token)).ReturnsAsync(false);

        // Act
        var result = await _controller.ValidarActivacion(dto);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ValidarActivacion_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        var dto = new ValidarActivacionDTO { Token = "token" };
        _mockValidarToken.Setup(x => x.Ejecutar(dto.Token)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.ValidarActivacion(dto);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ActivarCuenta Tests

    [Fact]
    public async Task ActivarCuenta_Exitoso_RetornaOk()
    {
        // Arrange
        var dto = new ActivarCuentaDTO { Token = "valid-token", Password = "newpassword123" };
        var email = "test@test.com";
        _mockActivarCuenta.Setup(x => x.Ejecutar(dto.Token, dto.Password)).ReturnsAsync(email);

        // Act
        var result = await _controller.ActivarCuenta(dto);

        // Assert
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        var response = okResult.Value as ActivarResponseDTO;
        response.Should().NotBeNull();
        response!.Email.Should().Be(email);
    }

    [Fact]
    public async Task ActivarCuenta_DtoNulo_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ActivarCuenta(null);

        // Assert
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ActivarCuenta_ModelStateInvalido_RetornaBadRequest()
    {
        // Arrange
        _controller.ModelState.AddModelError("Token", "Requerido");
        var dto = new ActivarCuentaDTO();

        // Act
        var result = await _controller.ActivarCuenta(dto);

        // Assert
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ActivarCuenta_TokenInvalido_RetornaBadRequest()
    {
        // Arrange
        var dto = new ActivarCuentaDTO { Token = "invalid-token", Password = "password123" };
        _mockActivarCuenta.Setup(x => x.Ejecutar(dto.Token, dto.Password)).ReturnsAsync((string?)null);

        // Act
        var result = await _controller.ActivarCuenta(dto);

        // Assert
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ActivarCuenta_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        var dto = new ActivarCuentaDTO { Token = "token", Password = "password123" };
        _mockActivarCuenta.Setup(x => x.Ejecutar(dto.Token, dto.Password)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.ActivarCuenta(dto);

        // Assert
        var statusCodeResult = result.Result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion
}
