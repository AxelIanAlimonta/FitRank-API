using FitRank_API.Application.CasosDeUso.FotoCasosDeUso;
using FitRank_API.Application.DTOs.FotoDTOs;
using FitRank_API.Presentacion.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

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
            var mockRepositorio = new Mock<Infrastructure.Interfaces.IFotoRepositorio>();
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
    }
}
