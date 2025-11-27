using FitRank_API.Application.CasosDeUso.RankingCasosDeUso;
using FitRank_API.Application.DTOs.RankingDTOs;
using FitRank_API.Presentacion.Controllers;
using FitRank_API.Domain.Interfaces;
using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using FitRank_API.Application.Mappings;

namespace FitRank_API.Tests.ControllersTests
{
    public class RankingControllerTests
    {
        private readonly RankingController _controller;
        private readonly Mock<IRankingRepositorio> _mockRepo;
        private readonly IMapper _mapper;

        public RankingControllerTests()
        {
            _mockRepo = new Mock<IRankingRepositorio>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<RankingProfile>();
                cfg.AddProfile<SocioProfile>();
            });
            _mapper = config.CreateMapper();

            var obtenerRanking = new ObtenerRankingGeneralCasoDeUso(_mockRepo.Object, _mapper);
            var obtenerPosicion = new ObtenerPosicionPorIdCasoDeUso(_mockRepo.Object, _mapper);

            _controller = new RankingController(obtenerRanking, obtenerPosicion);
        }

        #region ObtenerRankingGeneral Tests

        [Fact]
        public async Task ObtenerRankingGeneral_Exitoso_RetornaOk()
        {
            // Arrange
            int cantidad = 10;
            var ranking = new List<RankingDTO>
            {
                new RankingDTO { SocioId = 1, NombreCompleto = "Juan Pérez", PuntajeTotal = 1000 },
                new RankingDTO { SocioId = 2, NombreCompleto = "María López", PuntajeTotal = 900 }
            };
            _mockRepo.Setup(x => x.ObtenerTopSociosAsync(cantidad)).ReturnsAsync(ranking);

            // Act
            var result = await _controller.ObtenerRankingGeneral(cantidad);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(ranking);
        }

        [Fact]
        public async Task ObtenerRankingGeneral_CantidadCero_RetornaBadRequest()
        {
            // Act
            var result = await _controller.ObtenerRankingGeneral(0);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task ObtenerRankingGeneral_CantidadNegativa_RetornaBadRequest()
        {
            // Act
            var result = await _controller.ObtenerRankingGeneral(-5);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task ObtenerRankingGeneral_ListaVacia_RetornaNotFound()
        {
            // Arrange
            int cantidad = 10;
            _mockRepo.Setup(x => x.ObtenerTopSociosAsync(cantidad)).ReturnsAsync(new List<RankingDTO>());

            // Act
            var result = await _controller.ObtenerRankingGeneral(cantidad);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult!.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task ObtenerRankingGeneral_ListaNula_RetornaNotFound()
        {
            // Arrange
            int cantidad = 10;
            _mockRepo.Setup(x => x.ObtenerTopSociosAsync(cantidad)).ReturnsAsync((List<RankingDTO>?)null);

            // Act
            var result = await _controller.ObtenerRankingGeneral(cantidad);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult!.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task ObtenerRankingGeneral_ExcepcionGenerica_RetornaInternalServerError()
        {
            // Arrange
            int cantidad = 10;
            _mockRepo.Setup(x => x.ObtenerTopSociosAsync(cantidad)).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.ObtenerRankingGeneral(cantidad);

            // Assert
            var statusCodeResult = result as ObjectResult;
            statusCodeResult.Should().NotBeNull();
            statusCodeResult!.StatusCode.Should().Be(500);
        }

        [Fact]
        public async Task ObtenerRankingGeneral_CantidadVariada_RetornaOk()
        {
            // Arrange
            int cantidad = 25;
            var ranking = new List<RankingDTO>();
            for (int i = 1; i <= 25; i++)
            {
                ranking.Add(new RankingDTO { SocioId = i, NombreCompleto = $"Socio {i}", PuntajeTotal = 1000 - i });
            }
            _mockRepo.Setup(x => x.ObtenerTopSociosAsync(cantidad)).ReturnsAsync(ranking);

            // Act
            var result = await _controller.ObtenerRankingGeneral(cantidad);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);
            var resultList = okResult.Value as List<RankingDTO>;
            resultList.Should().HaveCount(25);
        }

        [Fact]
        public async Task ObtenerRankingGeneral_ConRankingOrdenado_MantineOrden()
        {
            // Arrange
            int cantidad = 3;
            var ranking = new List<RankingDTO>
            {
                new RankingDTO { SocioId = 1, NombreCompleto = "Primero", PuntajeTotal = 1000 },
                new RankingDTO { SocioId = 2, NombreCompleto = "Segundo", PuntajeTotal = 900 },
                new RankingDTO { SocioId = 3, NombreCompleto = "Tercero", PuntajeTotal = 800 }
            };
            _mockRepo.Setup(x => x.ObtenerTopSociosAsync(cantidad)).ReturnsAsync(ranking);

            // Act
            var result = await _controller.ObtenerRankingGeneral(cantidad);

            // Assert
            var okResult = result as OkObjectResult;
            var resultList = okResult!.Value as List<RankingDTO>;
            resultList![0].SocioId.Should().Be(1);
            resultList[1].SocioId.Should().Be(2);
            resultList[2].SocioId.Should().Be(3);
        }

        #endregion

        #region ObtenerPosicionSocio Tests

        [Fact]
        public async Task ObtenerPosicionSocio_Exitoso_RetornaOk()
        {
            // Arrange
            long socioId = 1;
            var posicion = new PosicionDTO
            {
                SocioId = 1,
                NombreCompleto = "Juan Pérez",
                PuntajeTotal = 1000,
                Posicion = 5
            };
            _mockRepo.Setup(x => x.ObtenerPosicionPorIdAsync(socioId)).ReturnsAsync(posicion);

            // Act
            var result = await _controller.ObtenerPosicionSocio(socioId);

            // Assert
            var okResult = result as OkObjectResult;
            okResult.Should().NotBeNull();
            okResult!.StatusCode.Should().Be(200);
            okResult.Value.Should().BeEquivalentTo(posicion);
        }

        [Fact]
        public async Task ObtenerPosicionSocio_IdCero_RetornaBadRequest()
        {
            // Act
            var result = await _controller.ObtenerPosicionSocio(0);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task ObtenerPosicionSocio_IdNegativo_RetornaBadRequest()
        {
            // Act
            var result = await _controller.ObtenerPosicionSocio(-3);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            badRequestResult.Should().NotBeNull();
            badRequestResult!.StatusCode.Should().Be(400);
        }

        [Fact]
        public async Task ObtenerPosicionSocio_SocioNoEncontrado_RetornaNotFound()
        {
            // Arrange
            long socioId = 999;
            _mockRepo.Setup(x => x.ObtenerPosicionPorIdAsync(socioId)).ReturnsAsync((PosicionDTO?)null);

            // Act
            var result = await _controller.ObtenerPosicionSocio(socioId);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            notFoundResult.Should().NotBeNull();
            notFoundResult!.StatusCode.Should().Be(404);
        }

        [Fact]
        public async Task ObtenerPosicionSocio_ExcepcionGenerica_RetornaInternalServerError()
        {
            // Arrange
            long socioId = 1;
            _mockRepo.Setup(x => x.ObtenerPosicionPorIdAsync(socioId)).ThrowsAsync(new Exception());

            // Act
            var result = await _controller.ObtenerPosicionSocio(socioId);

            // Assert
            var statusCodeResult = result as ObjectResult;
            statusCodeResult.Should().NotBeNull();
            statusCodeResult!.StatusCode.Should().Be(500);
        }

        [Fact]
        public async Task ObtenerPosicionSocio_ConPosicionPrimera_RetornaOk()
        {
            // Arrange
            long socioId = 1;
            var posicion = new PosicionDTO
            {
                SocioId = 1,
                NombreCompleto = "El Mejor",
                PuntajeTotal = 5000,
                Posicion = 1
            };
            _mockRepo.Setup(x => x.ObtenerPosicionPorIdAsync(socioId)).ReturnsAsync(posicion);

            // Act
            var result = await _controller.ObtenerPosicionSocio(socioId);

            // Assert
            var okResult = result as OkObjectResult;
            var posicionResult = okResult!.Value as PosicionDTO;
            posicionResult!.Posicion.Should().Be(1);
        }

        [Fact]
        public async Task ObtenerPosicionSocio_ConPosicionUltima_RetornaOk()
        {
            // Arrange
            long socioId = 100;
            var posicion = new PosicionDTO
            {
                SocioId = 100,
                NombreCompleto = "El Último",
                PuntajeTotal = 10,
                Posicion = 100
            };
            _mockRepo.Setup(x => x.ObtenerPosicionPorIdAsync(socioId)).ReturnsAsync(posicion);

            // Act
            var result = await _controller.ObtenerPosicionSocio(socioId);

            // Assert
            var okResult = result as OkObjectResult;
            var posicionResult = okResult!.Value as PosicionDTO;
            posicionResult!.Posicion.Should().Be(100);
        }

        [Fact]
        public async Task ObtenerPosicionSocio_ConDiferentesSocios_RetornaPosicionesCorrectas()
        {
            // Arrange
            long socioId1 = 1;
            long socioId2 = 2;

            var posicion1 = new PosicionDTO { SocioId = 1, NombreCompleto = "Primero", PuntajeTotal = 1000, Posicion = 1 };
            var posicion2 = new PosicionDTO { SocioId = 2, NombreCompleto = "Segundo", PuntajeTotal = 900, Posicion = 2 };

            _mockRepo.Setup(x => x.ObtenerPosicionPorIdAsync(socioId1)).ReturnsAsync(posicion1);
            _mockRepo.Setup(x => x.ObtenerPosicionPorIdAsync(socioId2)).ReturnsAsync(posicion2);

            // Act
            var result1 = await _controller.ObtenerPosicionSocio(socioId1);
            var result2 = await _controller.ObtenerPosicionSocio(socioId2);

            // Assert
            var okResult1 = result1 as OkObjectResult;
            var okResult2 = result2 as OkObjectResult;
            var pos1 = okResult1!.Value as PosicionDTO;
            var pos2 = okResult2!.Value as PosicionDTO;
            pos1!.Posicion.Should().Be(1);
            pos2!.Posicion.Should().Be(2);
        }

        #endregion
    }
}
