using FitRank_API.Application.CasosDeUso.Invitacion;
using FitRank_API.Application.DTOs.Invitacion;
using FitRank_API.Presentacion.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;
using AutoMapper;
using FitRank_API.Domain.Interfaces;

namespace FitRank_API.tests.ControllersTests;

public class InvitacionControllerTests
{
    private readonly InvitacionController _controller;
    private readonly Mock<ObtenerInvitacionesCasoDeUso> _mockObtenerInvitaciones;

    public InvitacionControllerTests()
    {
        var mockRepo = new Mock<IInvitacionRepositorio>();
        var mockMapper = new Mock<IMapper>();

        _mockObtenerInvitaciones = new Mock<ObtenerInvitacionesCasoDeUso>(mockRepo.Object, mockMapper.Object);

        _controller = new InvitacionController(_mockObtenerInvitaciones.Object);
    }

    #region ObtenerTodas Tests

    [Fact]
    public async Task ObtenerTodas_Exitoso_RetornaOk()
    {
        // Arrange
        var adminId = 1;
        var invitaciones = new List<InvitacionListadoDTO>
        {
            new InvitacionListadoDTO 
            { 
                Id = 1, 
                Email = "test1@test.com", 
                Estado = "Pendiente",
                MetodoPago = "Efectivo",
                CreadaEn = DateTime.Now,
                ExpiraEn = DateTime.Now.AddDays(1)
            },
            new InvitacionListadoDTO 
            { 
                Id = 2, 
                Email = "test2@test.com", 
                Estado = "Pagado",
                MetodoPago = "MercadoPago",
                CreadaEn = DateTime.Now,
                ExpiraEn = DateTime.Now.AddDays(1)
            }
        };

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, adminId.ToString())
        };
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        _mockObtenerInvitaciones.Setup(x => x.Ejecutar(adminId)).ReturnsAsync(invitaciones);

        // Act
        var result = await _controller.ObtenerTodas();

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        var returnedList = okResult.Value as List<InvitacionListadoDTO>;
        returnedList.Should().HaveCount(2);
        returnedList!.First().Email.Should().Be("test1@test.com");
    }

    [Fact]
    public async Task ObtenerTodas_ListaVacia_RetornaOk()
    {
        // Arrange
        var adminId = 1;
        var invitacionesVacias = new List<InvitacionListadoDTO>();

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, adminId.ToString())
        };
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        _mockObtenerInvitaciones.Setup(x => x.Ejecutar(adminId)).ReturnsAsync(invitacionesVacias);

        // Act
        var result = await _controller.ObtenerTodas();

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        var returnedList = okResult.Value as List<InvitacionListadoDTO>;
        returnedList.Should().BeEmpty();
    }

    [Fact]
    public async Task ObtenerTodas_ClaimNulo_RetornaBadRequest()
    {
        // Arrange
        var claims = new List<Claim>();
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        // Act
        var result = await _controller.ObtenerTodas();

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerTodas_ClaimVacio_RetornaBadRequest()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "")
        };
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        // Act
        var result = await _controller.ObtenerTodas();

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerTodas_ClaimInvalido_RetornaBadRequest()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "invalid")
        };
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        // Act
        var result = await _controller.ObtenerTodas();

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerTodas_ClaimConLetras_RetornaBadRequest()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "abc123")
        };
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        // Act
        var result = await _controller.ObtenerTodas();

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerTodas_ClaimNegativo_RetornaBadRequest()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, "-5")
        };
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        // Act
        var result = await _controller.ObtenerTodas();

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerTodas_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        var adminId = 1;
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, adminId.ToString())
        };
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        _mockObtenerInvitaciones.Setup(x => x.Ejecutar(adminId)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.ObtenerTodas();

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task ObtenerTodas_ConInvitacionesDiversas_RetornaTodasCorrectamente()
    {
        // Arrange
        var adminId = 5;
        var invitaciones = new List<InvitacionListadoDTO>
        {
            new InvitacionListadoDTO 
            { 
                Id = 10, 
                Email = "pendiente@test.com", 
                Estado = "Pendiente",
                MetodoPago = "MercadoPago"
            },
            new InvitacionListadoDTO 
            { 
                Id = 11, 
                Email = "pagado@test.com", 
                Estado = "Pagado",
                MetodoPago = "Efectivo"
            },
            new InvitacionListadoDTO 
            { 
                Id = 12, 
                Email = "expirado@test.com", 
                Estado = "Expirado",
                MetodoPago = "Efectivo"
            }
        };

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, adminId.ToString())
        };
        var identity = new ClaimsIdentity(claims);
        var claimsPrincipal = new ClaimsPrincipal(identity);
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        _mockObtenerInvitaciones.Setup(x => x.Ejecutar(adminId)).ReturnsAsync(invitaciones);

        // Act
        var result = await _controller.ObtenerTodas();

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        var returnedList = okResult!.Value as List<InvitacionListadoDTO>;
        returnedList.Should().HaveCount(3);
        returnedList.Should().Contain(i => i.Estado == "Pendiente");
        returnedList.Should().Contain(i => i.Estado == "Pagado");
        returnedList.Should().Contain(i => i.Estado == "Expirado");
    }

    #endregion
}
