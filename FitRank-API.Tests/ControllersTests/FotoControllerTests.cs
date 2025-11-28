using FitRank_API.Application.CasosDeUso.FotoCasosDeUso;
using FitRank_API.Application.DTOs.FotoDTOs;
using FitRank_API.Presentacion.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace FitRank_API.Tests.ControllersTests
{
    public class FotoControllerTests
    {
        private readonly Mock<AgregarFotoCasoDeUso> _mockAgregarCaso;
        private readonly Mock<ObtenerFotosPorSocioCasoDeUso> _mockObtenerPorSocioCaso;
        private readonly Mock<EliminarFotoCasoDeUso> _mockEliminarCaso;
        private readonly FotoController _controller;

        public FotoControllerTests()
        {
            var mockRepositorio = new Mock<Domain.Interfaces.IFotoRepositorio>();
            var mockMapper = new Mock<AutoMapper.IMapper>();

            _mockAgregarCaso = new Mock<AgregarFotoCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
            _mockObtenerPorSocioCaso = new Mock<ObtenerFotosPorSocioCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
            _mockEliminarCaso = new Mock<EliminarFotoCasoDeUso>(mockRepositorio.Object);

            _controller = new FotoController(
                _mockAgregarCaso.Object,
                _mockObtenerPorSocioCaso.Object,
                _mockEliminarCaso.Object
            );
        }

        #region Agregar Tests

        [Fact]
        public async Task Agregar_RetornaOkResultConFoto()
        {
            // Arrange
            var dto = new AgregarFotoDTO
            {
                SocioId = 1,
                Fecha = DateTime.UtcNow,
                UrlImagen = "https://example.com/foto.jpg"
            };

            var fotoEsperada = new ObtenerFotoDTO
            {
                Id = 1,
                SocioId = 1,
                Fecha = dto.Fecha,
                UrlImagen = dto.UrlImagen
            };

            _mockAgregarCaso
                .Setup(caso => caso.Ejecutar(dto))
                .ReturnsAsync(fotoEsperada);

            // Act
            var result = await _controller.Agregar(dto);

            // Assert
            result.Result.Should().BeOfType<OkObjectResult>();
            var okResult = result.Result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(fotoEsperada);
        }

        [Fact]
        public async Task Agregar_DtoNulo_RetornaBadRequest()
        {
            // Act
            var result = await _controller.Agregar(null);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task Agregar_ModelStateInvalido_RetornaBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("UrlImagen", "Requerido");
            var dto = new AgregarFotoDTO();

            // Act
            var result = await _controller.Agregar(dto);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task Agregar_ExcepcionGenerica_RetornaInternalServerError()
        {
            // Arrange
            var dto = new AgregarFotoDTO { SocioId = 1 };
            _mockAgregarCaso.Setup(x => x.Ejecutar(dto)).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.Agregar(dto);

            // Assert
            var statusCodeResult = result.Result as ObjectResult;
            statusCodeResult.Should().NotBeNull();
            statusCodeResult!.StatusCode.Should().Be(500);
        }

        #endregion

        #region ObtenerPorSocio Tests

        [Fact]
        public async Task ObtenerPorSocio_RetornaListaCompleta()
        {
            // Arrange
            long socioId = 1;
            var fotosEsperadas = new List<ObtenerFotoDTO>
            {
                new ObtenerFotoDTO { Id = 1, SocioId = socioId, Fecha = DateTime.UtcNow, UrlImagen = "url1" },
                new ObtenerFotoDTO { Id = 2, SocioId = socioId, Fecha = DateTime.UtcNow, UrlImagen = "url2" }
            };

            _mockObtenerPorSocioCaso
                .Setup(caso => caso.Ejecutar(socioId))
                .ReturnsAsync(fotosEsperadas);

            // Act
            var result = await _controller.ObtenerPorSocio(socioId);

            // Assert
            result.Should().BeOfType<ActionResult<List<ObtenerFotoDTO>>>();
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.Value.Should().BeEquivalentTo(fotosEsperadas);
        }

        [Fact]
        public async Task ObtenerPorSocio_RetornaListaVacia()
        {
            // Arrange
            long socioId = 999;
            var fotosEsperadas = new List<ObtenerFotoDTO>();

            _mockObtenerPorSocioCaso
                .Setup(caso => caso.Ejecutar(socioId))
                .ReturnsAsync(fotosEsperadas);

            // Act
            var result = await _controller.ObtenerPorSocio(socioId);

            // Assert
            result.Should().BeOfType<ActionResult<List<ObtenerFotoDTO>>>();
            var okResult = result.Result as OkObjectResult;
            okResult.Should().NotBeNull();
            var lista = okResult!.Value as List<ObtenerFotoDTO>;
            lista.Should().BeEmpty();
        }

        [Fact]
        public async Task ObtenerPorSocio_IdCero_RetornaBadRequest()
        {
            // Act
            var result = await _controller.ObtenerPorSocio(0);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task ObtenerPorSocio_IdNegativo_RetornaBadRequest()
        {
            // Act
            var result = await _controller.ObtenerPorSocio(-5);

            // Assert
            var badRequestResult = result.Result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task ObtenerPorSocio_ExcepcionGenerica_RetornaInternalServerError()
        {
            // Arrange
            _mockObtenerPorSocioCaso.Setup(x => x.Ejecutar(1)).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.ObtenerPorSocio(1);

            // Assert
            var statusCodeResult = result.Result as ObjectResult;
            statusCodeResult.Should().NotBeNull();
            statusCodeResult!.StatusCode.Should().Be(500);
        }

        #endregion

        #region Eliminar Tests

        [Fact]
        public async Task Eliminar_RetornaNoContent()
        {
            // Arrange
            long fotoId = 1;

            _mockEliminarCaso
                .Setup(caso => caso.Ejecutar(fotoId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Eliminar(fotoId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Eliminar_IdCero_RetornaBadRequest()
        {
            // Act
            var result = await _controller.Eliminar(0);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task Eliminar_IdNegativo_RetornaBadRequest()
        {
            // Act
            var result = await _controller.Eliminar(-3);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task Eliminar_ExcepcionGenerica_RetornaInternalServerError()
        {
            // Arrange
            _mockEliminarCaso.Setup(x => x.Ejecutar(1)).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.Eliminar(1);

            // Assert
            var statusCodeResult = result as ObjectResult;
            statusCodeResult.Should().NotBeNull();
            statusCodeResult!.StatusCode.Should().Be(500);
        }

        #endregion
    }
}
