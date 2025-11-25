using AutoMapper;
using FluentAssertions;
using FitRank_API.Application.CasosDeUso.RankingCasosDeUso;
using FitRank_API.Application.DTOs.RankingDTOs;
using FitRank_API.Application.Mappings;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using Moq;

namespace FitRank_API.Tests.CasosDeUsoTests.RankingCasosDeUsoTests
{
    public class ObtenerPosicionPorIdCasoDeUsoTests
    {
        private readonly Mock<IRankingRepositorio> _mockRepositorio;
        private readonly IMapper _mapper;
        private readonly ObtenerPosicionPorIdCasoDeUso _casoDeUso;

        public ObtenerPosicionPorIdCasoDeUsoTests()
        {
            _mockRepositorio = new Mock<IRankingRepositorio>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<RankingProfile>();
                cfg.AddProfile<SocioProfile>();
            });
            _mapper = config.CreateMapper();
            _casoDeUso = new ObtenerPosicionPorIdCasoDeUso(_mockRepositorio.Object, _mapper);
        }

        [Fact]
        public async Task DeberiaRetornarPosicionCuandoSocioExiste()
        {
            // Arrange
            long socioId = 1;
            var posicionDTO = new PosicionDTO
            {
                SocioId = 1,
                NombreCompleto = "Juan Pérez",
                PuntajeTotal = 100,
                Posicion = 1
            };

            _mockRepositorio.Setup(r => r.ObtenerPosicionPorIdAsync(socioId))
                .ReturnsAsync(posicionDTO);

            // Act
            var resultado = await _casoDeUso.Ejecutar(socioId);

            // Assert
            resultado.Should().NotBeNull();
            resultado!.SocioId.Should().Be(1);
            resultado.NombreCompleto.Should().Be("Juan Pérez");
            resultado.PuntajeTotal.Should().Be(100);
            _mockRepositorio.Verify(r => r.ObtenerPosicionPorIdAsync(socioId), Times.Once);
        }

        [Fact]
        public async Task DeberiaRetornarNullCuandoSocioNoExiste()
        {
            // Arrange
            long socioId = 999;
            _mockRepositorio.Setup(r => r.ObtenerPosicionPorIdAsync(socioId))
                .ReturnsAsync((PosicionDTO)null!);

            // Act
            var resultado = await _casoDeUso.Ejecutar(socioId);

            // Assert
            resultado.Should().BeNull();
            _mockRepositorio.Verify(r => r.ObtenerPosicionPorIdAsync(socioId), Times.Once);
        }

        [Fact]
        public async Task DebeMapearCorrectamenteTodosLosCampos()
        {
            // Arrange
            long socioId = 50;
            var posicionDTO = new PosicionDTO
            {
                SocioId = 50,
                NombreCompleto = "Roberto González",
                PuntajeTotal = 450,
                Posicion = 5
            };

            _mockRepositorio.Setup(r => r.ObtenerPosicionPorIdAsync(socioId))
                .ReturnsAsync(posicionDTO);

            // Act
            var resultado = await _casoDeUso.Ejecutar(socioId);

            // Assert
            resultado.Should().NotBeNull();
            resultado!.SocioId.Should().Be(50);
            resultado.NombreCompleto.Should().Be("Roberto González");
            resultado.PuntajeTotal.Should().Be(450);
            resultado.Posicion.Should().Be(5);
        }

        [Fact]
        public async Task DebeLlamarRepositorioConSocioIdCorrecto()
        {
            // Arrange
            long socioId = 123;
            var posicionDTO = new PosicionDTO
            {
                SocioId = 123,
                NombreCompleto = "Test User",
                PuntajeTotal = 100,
                Posicion = 10
            };

            _mockRepositorio.Setup(r => r.ObtenerPosicionPorIdAsync(socioId))
                .ReturnsAsync(posicionDTO);

            // Act
            await _casoDeUso.Ejecutar(socioId);

            // Assert
            _mockRepositorio.Verify(r => r.ObtenerPosicionPorIdAsync(socioId), Times.Once);
        }

        [Fact]
        public async Task DeberiaRetornarPosicionesEnDiferentesRangos()
        {
            // Arrange
            long socioId1 = 1;
            long socioId2 = 100;

            var posicion1 = new PosicionDTO { SocioId = 1, NombreCompleto = "Primero", PuntajeTotal = 1000, Posicion = 1 };
            var posicion100 = new PosicionDTO { SocioId = 100, NombreCompleto = "Centésimo", PuntajeTotal = 10, Posicion = 100 };

            _mockRepositorio.Setup(r => r.ObtenerPosicionPorIdAsync(socioId1))
                .ReturnsAsync(posicion1);
            _mockRepositorio.Setup(r => r.ObtenerPosicionPorIdAsync(socioId2))
                .ReturnsAsync(posicion100);

            // Act
            var resultado1 = await _casoDeUso.Ejecutar(socioId1);
            var resultado2 = await _casoDeUso.Ejecutar(socioId2);

            // Assert
            resultado1!.Posicion.Should().Be(1);
            resultado2!.Posicion.Should().Be(100);
        }

        [Fact]
        public async Task DeberiaRetornarTipoPosicionDTO()
        {
            // Arrange
            long socioId = 25;
            var posicionDTO = new PosicionDTO
            {
                SocioId = 25,
                NombreCompleto = "Usuario Test",
                PuntajeTotal = 200,
                Posicion = 15
            };

            _mockRepositorio.Setup(r => r.ObtenerPosicionPorIdAsync(socioId))
                .ReturnsAsync(posicionDTO);

            // Act
            var resultado = await _casoDeUso.Ejecutar(socioId);

            // Assert
            resultado.Should().BeOfType<PosicionDTO>();
        }
    }
}
