using FitRank_API.Application.CasosDeUso.RankingCasosDeUso;
using FitRank_API.Application.DTOs.RankingDTOs;
using FitRank_API.Presentacion.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace FitRank_API.Tests.ControllersTests
{
    public class RankingControllerTests
    {
        private readonly Mock<ObtenerRankingGeneralCasoDeUso> _mockObtenerRankingCaso;
        private readonly Mock<ObtenerPosicionPorIdCasoDeUso> _mockObtenerPosicionCaso;
        private readonly RankingController _controller;

        public RankingControllerTests()
        {
            var mockRepositorio = new Mock<Domain.Interfaces.IRankingRepositorio>();
            var mockMapper = new Mock<AutoMapper.IMapper>();

            _mockObtenerRankingCaso = new Mock<ObtenerRankingGeneralCasoDeUso>(mockRepositorio.Object, mockMapper.Object);
            _mockObtenerPosicionCaso = new Mock<ObtenerPosicionPorIdCasoDeUso>(mockRepositorio.Object, mockMapper.Object);

            _controller = new RankingController(
                _mockObtenerRankingCaso.Object,
                _mockObtenerPosicionCaso.Object
            );
        }

        [Fact]
        public async Task ObtenerRankingGeneral_ConCantidadValida_RetornaOkResultConLista()
        {
            // Arrange
            int cantidad = 10;
            var rankingEsperado = new List<RankingDTO>
            {
                new RankingDTO { SocioId = 1, NombreCompleto = "Juan Perez", PuntajeTotal = 1000 },
                new RankingDTO { SocioId = 2, NombreCompleto = "Maria Lopez", PuntajeTotal = 900 }
            };

            _mockObtenerRankingCaso
                .Setup(caso => caso.Ejecutar(cantidad))
                .ReturnsAsync(rankingEsperado);

            // Act
            var result = await _controller.ObtenerRankingGeneral(cantidad);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(rankingEsperado);
        }

        [Fact]
        public async Task ObtenerRankingGeneral_ConListaVacia_RetornaNotFound()
        {
            // Arrange
            int cantidad = 10;
            var rankingVacio = new List<RankingDTO>();

            _mockObtenerRankingCaso
                .Setup(caso => caso.Ejecutar(cantidad))
                .ReturnsAsync(rankingVacio);

            // Act
            var result = await _controller.ObtenerRankingGeneral(cantidad);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task ObtenerRankingGeneral_ConCantidadCeroONegativa_RetornaBadRequest()
        {
            // Arrange
            int cantidadInvalida = 0;

            // Act
            var result = await _controller.ObtenerRankingGeneral(cantidadInvalida);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task ObtenerPosicionSocio_ConIdValido_RetornaOkResultConPosicion()
        {
            // Arrange
            long socioId = 1;
            var posicionEsperada = new PosicionDTO
            {
                SocioId = socioId,
                NombreCompleto = "Juan Perez",
                PuntajeTotal = 1000,
                Posicion = 5
            };

            _mockObtenerPosicionCaso
                .Setup(caso => caso.Ejecutar(socioId))
                .ReturnsAsync(posicionEsperada);

            // Act
            var result = await _controller.ObtenerPosicionSocio(socioId);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var okResult = result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(posicionEsperada);
        }

        [Fact]
        public async Task ObtenerPosicionSocio_ConIdInexistente_RetornaNotFound()
        {
            // Arrange
            long socioId = 999;

            _mockObtenerPosicionCaso
                .Setup(caso => caso.Ejecutar(socioId))
                .ReturnsAsync((PosicionDTO?)null);

            // Act
            var result = await _controller.ObtenerPosicionSocio(socioId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task ObtenerPosicionSocio_ConIdCeroONegativo_RetornaBadRequest()
        {
            // Arrange
            long idInvalido = 0;

            // Act
            var result = await _controller.ObtenerPosicionSocio(idInvalido);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }
    }
}
