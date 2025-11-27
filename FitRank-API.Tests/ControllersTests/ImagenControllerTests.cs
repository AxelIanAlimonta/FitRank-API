using FitRank_API.Application.DTOs.ImagenDTOs;
using FitRank_API.Application.Interfaces;
using FitRank_API.Presentacion.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace FitRank_API.tests.ControllersTests;

public class ImagenControllerTests
{
    private readonly ImagenController _controller;
    private readonly Mock<IImagenService> _mockImagenService;
    private readonly Mock<ILogger<ImagenController>> _mockLogger;

    public ImagenControllerTests()
    {
        _mockImagenService = new Mock<IImagenService>();
        _mockLogger = new Mock<ILogger<ImagenController>>();
        _controller = new ImagenController(_mockImagenService.Object, _mockLogger.Object);
    }

    #region SubirImagen Tests

    [Fact]
    public async Task SubirImagen_Exitoso_RetornaOk()
    {
        // Arrange
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.Length).Returns(1024);
        mockFile.Setup(f => f.FileName).Returns("test.jpg");
        mockFile.Setup(f => f.ContentType).Returns("image/jpeg");

        var request = new SubirImagenRequestDto { Archivo = mockFile.Object };
        var response = new ImagenUploadResponseDto
        {
            Key = "imagenes/test.jpg",
            Url = "https://ejemplo.com/imagenes/test.jpg",
            NombreArchivo = "test.jpg",
            TamanoBytes = 1024,
            ContentType = "image/jpeg",
            FechaSubida = DateTime.UtcNow
        };

        _mockImagenService.Setup(x => x.SubirImagenAsync(It.IsAny<IFormFile>(), It.IsAny<string>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.SubirImagen(request, "imagenes");

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(response);
    }

    [Fact]
    public async Task SubirImagen_ArchivoNulo_RetornaBadRequest()
    {
        // Arrange
        var request = new SubirImagenRequestDto { Archivo = null! };

        // Act
        var result = await _controller.SubirImagen(request, "imagenes");

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task SubirImagen_ArchivoVacio_RetornaBadRequest()
    {
        // Arrange
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.Length).Returns(0);
        var request = new SubirImagenRequestDto { Archivo = mockFile.Object };

        // Act
        var result = await _controller.SubirImagen(request, "imagenes");

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task SubirImagen_CarpetaVacia_RetornaBadRequest()
    {
        // Arrange
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.Length).Returns(1024);
        var request = new SubirImagenRequestDto { Archivo = mockFile.Object };

        // Act
        var result = await _controller.SubirImagen(request, "");

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task SubirImagen_CarpetaNull_RetornaBadRequest()
    {
        // Arrange
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.Length).Returns(1024);
        var request = new SubirImagenRequestDto { Archivo = mockFile.Object };

        // Act
        var result = await _controller.SubirImagen(request, null!);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task SubirImagen_ArgumentException_RetornaBadRequest()
    {
        // Arrange
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.Length).Returns(1024);
        var request = new SubirImagenRequestDto { Archivo = mockFile.Object };

        _mockImagenService.Setup(x => x.SubirImagenAsync(It.IsAny<IFormFile>(), It.IsAny<string>()))
            .ThrowsAsync(new ArgumentException("Tipo de archivo no permitido"));

        // Act
        var result = await _controller.SubirImagen(request, "imagenes");

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task SubirImagen_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.Length).Returns(1024);
        var request = new SubirImagenRequestDto { Archivo = mockFile.Object };

        _mockImagenService.Setup(x => x.SubirImagenAsync(It.IsAny<IFormFile>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception());

        // Act
        var result = await _controller.SubirImagen(request, "imagenes");

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ObtenerImagen Tests

    [Fact]
    public async Task ObtenerImagen_Exitoso_RetornaOk()
    {
        // Arrange
        var key = "imagenes/test.jpg";
        var response = new ImagenResponseDto
        {
            Key = key,
            Url = "https://ejemplo.com/" + key,
            NombreArchivo = "test.jpg",
            TamanoBytes = 1024
        };

        _mockImagenService.Setup(x => x.ObtenerImagenAsync(key)).ReturnsAsync(response);

        // Act
        var result = await _controller.ObtenerImagen(key);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        okResult.Value.Should().BeEquivalentTo(response);
    }

    [Fact]
    public async Task ObtenerImagen_KeyVacio_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerImagen("");

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerImagen_KeyNull_RetornaBadRequest()
    {
        // Act
        var result = await _controller.ObtenerImagen(null!);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ObtenerImagen_NoEncontrada_RetornaNotFound()
    {
        // Arrange
        var key = "imagenes/noexiste.jpg";
        _mockImagenService.Setup(x => x.ObtenerImagenAsync(key))
            .ThrowsAsync(new FileNotFoundException());

        // Act
        var result = await _controller.ObtenerImagen(key);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task ObtenerImagen_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        var key = "imagenes/test.jpg";
        _mockImagenService.Setup(x => x.ObtenerImagenAsync(key)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.ObtenerImagen(key);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ListarImagenes Tests

    [Fact]
    public async Task ListarImagenes_Exitoso_RetornaOk()
    {
        // Arrange
        var imagenes = new List<ImagenResponseDto>
        {
            new ImagenResponseDto { Key = "imagenes/test1.jpg", NombreArchivo = "test1.jpg" },
            new ImagenResponseDto { Key = "imagenes/test2.jpg", NombreArchivo = "test2.jpg" }
        };

        _mockImagenService.Setup(x => x.ListarImagenesAsync(null)).ReturnsAsync(imagenes);

        // Act
        var result = await _controller.ListarImagenes(null);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        var returnedList = okResult.Value as List<ImagenResponseDto>;
        returnedList.Should().HaveCount(2);
    }

    [Fact]
    public async Task ListarImagenes_ConCarpeta_RetornaOk()
    {
        // Arrange
        var carpeta = "perfiles";
        var imagenes = new List<ImagenResponseDto>
        {
            new ImagenResponseDto { Key = "perfiles/avatar1.jpg", NombreArchivo = "avatar1.jpg" }
        };

        _mockImagenService.Setup(x => x.ListarImagenesAsync(carpeta)).ReturnsAsync(imagenes);

        // Act
        var result = await _controller.ListarImagenes(carpeta);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task ListarImagenes_ListaVacia_RetornaOk()
    {
        // Arrange
        _mockImagenService.Setup(x => x.ListarImagenesAsync(null))
            .ReturnsAsync(new List<ImagenResponseDto>());

        // Act
        var result = await _controller.ListarImagenes(null);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
        var returnedList = okResult.Value as List<ImagenResponseDto>;
        returnedList.Should().BeEmpty();
    }

    [Fact]
    public async Task ListarImagenes_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        _mockImagenService.Setup(x => x.ListarImagenesAsync(null)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.ListarImagenes(null);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region EliminarImagen Tests

    [Fact]
    public async Task EliminarImagen_Exitoso_RetornaOk()
    {
        // Arrange
        var key = "imagenes/test.jpg";
        _mockImagenService.Setup(x => x.EliminarImagenAsync(key)).ReturnsAsync(true);

        // Act
        var result = await _controller.EliminarImagen(key);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task EliminarImagen_KeyVacio_RetornaBadRequest()
    {
        // Act
        var result = await _controller.EliminarImagen("");

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task EliminarImagen_KeyNull_RetornaBadRequest()
    {
        // Act
        var result = await _controller.EliminarImagen(null!);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task EliminarImagen_NoEncontrada_RetornaNotFound()
    {
        // Arrange
        var key = "imagenes/noexiste.jpg";
        _mockImagenService.Setup(x => x.EliminarImagenAsync(key)).ReturnsAsync(false);

        // Act
        var result = await _controller.EliminarImagen(key);

        // Assert
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult.Should().NotBeNull();
        notFoundResult!.StatusCode.Should().Be(404);
    }

    [Fact]
    public async Task EliminarImagen_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        var key = "imagenes/test.jpg";
        _mockImagenService.Setup(x => x.EliminarImagenAsync(key)).ThrowsAsync(new Exception());

        // Act
        var result = await _controller.EliminarImagen(key);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ActualizarImagen Tests

    [Fact]
    public async Task ActualizarImagen_Exitoso_RetornaOk()
    {
        // Arrange
        var key = "imagenes/test.jpg";
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.Length).Returns(1024);
        var request = new SubirImagenRequestDto { Archivo = mockFile.Object };
        var response = new ImagenUploadResponseDto
        {
            Key = key,
            Url = "https://ejemplo.com/" + key
        };

        _mockImagenService.Setup(x => x.ActualizarImagenAsync(key, It.IsAny<IFormFile>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.ActualizarImagen(key, request);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task ActualizarImagen_KeyVacio_RetornaBadRequest()
    {
        // Arrange
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.Length).Returns(1024);
        var request = new SubirImagenRequestDto { Archivo = mockFile.Object };

        // Act
        var result = await _controller.ActualizarImagen("", request);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ActualizarImagen_KeyNull_RetornaBadRequest()
    {
        // Arrange
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.Length).Returns(1024);
        var request = new SubirImagenRequestDto { Archivo = mockFile.Object };

        // Act
        var result = await _controller.ActualizarImagen(null!, request);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ActualizarImagen_ArchivoNulo_RetornaBadRequest()
    {
        // Arrange
        var key = "imagenes/test.jpg";
        var request = new SubirImagenRequestDto { Archivo = null! };

        // Act
        var result = await _controller.ActualizarImagen(key, request);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ActualizarImagen_ArchivoVacio_RetornaBadRequest()
    {
        // Arrange
        var key = "imagenes/test.jpg";
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.Length).Returns(0);
        var request = new SubirImagenRequestDto { Archivo = mockFile.Object };

        // Act
        var result = await _controller.ActualizarImagen(key, request);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ActualizarImagen_ArgumentException_RetornaBadRequest()
    {
        // Arrange
        var key = "imagenes/test.jpg";
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.Length).Returns(1024);
        var request = new SubirImagenRequestDto { Archivo = mockFile.Object };

        _mockImagenService.Setup(x => x.ActualizarImagenAsync(key, It.IsAny<IFormFile>()))
            .ThrowsAsync(new ArgumentException("Tipo de archivo no permitido"));

        // Act
        var result = await _controller.ActualizarImagen(key, request);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public async Task ActualizarImagen_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        var key = "imagenes/test.jpg";
        var mockFile = new Mock<IFormFile>();
        mockFile.Setup(f => f.Length).Returns(1024);
        var request = new SubirImagenRequestDto { Archivo = mockFile.Object };

        _mockImagenService.Setup(x => x.ActualizarImagenAsync(key, It.IsAny<IFormFile>()))
            .ThrowsAsync(new Exception());

        // Act
        var result = await _controller.ActualizarImagen(key, request);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion

    #region ObtenerUrlPublica Tests

    [Fact]
    public void ObtenerUrlPublica_Exitoso_RetornaOk()
    {
        // Arrange
        var key = "imagenes/test.jpg";
        var url = "https://ejemplo.com/" + key;
        _mockImagenService.Setup(x => x.ObtenerUrlPublica(key)).Returns(url);

        // Act
        var result = _controller.ObtenerUrlPublica(key);

        // Assert
        var okResult = result as OkObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(200);
    }

    [Fact]
    public void ObtenerUrlPublica_KeyVacio_RetornaBadRequest()
    {
        // Act
        var result = _controller.ObtenerUrlPublica("");

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public void ObtenerUrlPublica_KeyNull_RetornaBadRequest()
    {
        // Act
        var result = _controller.ObtenerUrlPublica(null!);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.Should().NotBeNull();
        badRequestResult!.StatusCode.Should().Be(400);
    }

    [Fact]
    public void ObtenerUrlPublica_ExcepcionGenerica_RetornaInternalServerError()
    {
        // Arrange
        var key = "imagenes/test.jpg";
        _mockImagenService.Setup(x => x.ObtenerUrlPublica(key)).Throws(new Exception());

        // Act
        var result = _controller.ObtenerUrlPublica(key);

        // Assert
        var statusCodeResult = result as ObjectResult;
        statusCodeResult.Should().NotBeNull();
        statusCodeResult!.StatusCode.Should().Be(500);
    }

    #endregion
}
