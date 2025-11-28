using FitRank_API.Application.CasosDeUso.CalculoPuntajeCasosDeUso;
using FitRank_API.Application.DTOs.PuntajeDTOs;
using FitRank_API.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace FitRank_API.Tests.CasosDeUsoTests.CalculoPuntajeCasosDeUsoTests
{
    public class ObtenerRankingSociosCasoDeUsoTests
    {
        private readonly Mock<ISocioRepositorio> _mockSocioRepo;
        private readonly ObtenerRankingSociosCasoDeUso _casoDeUso;

        public ObtenerRankingSociosCasoDeUsoTests()
        {
            _mockSocioRepo = new Mock<ISocioRepositorio>();
            _casoDeUso = new ObtenerRankingSociosCasoDeUso(_mockSocioRepo.Object);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarRankingGeneral()
        {
            // Arrange
            var ranking = new List<SocioRankingDto>
            {
                new SocioRankingDto { SocioId = 1, NombreCompleto = "Juan Pérez", PuntajeTotal = 100 },
                new SocioRankingDto { SocioId = 2, NombreCompleto = "María García", PuntajeTotal = 150 },
                new SocioRankingDto { SocioId = 3, NombreCompleto = "Carlos López", PuntajeTotal = 80 }
            };

            _mockSocioRepo.Setup(r => r.ObtenerRankingGeneralAsync(1, 10))
                .ReturnsAsync(ranking);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1, 10);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(3);
            resultado.Should().BeEquivalentTo(ranking);
        }

        [Fact]
        public async Task Ejecutar_DebeLimitarCantidadDeResultados()
        {
            // Arrange
            var ranking = new List<SocioRankingDto>
            {
                new SocioRankingDto { SocioId = 1, NombreCompleto = "Juan Pérez", PuntajeTotal = 100 },
                new SocioRankingDto { SocioId = 2, NombreCompleto = "María García", PuntajeTotal = 150 },
                new SocioRankingDto { SocioId = 3, NombreCompleto = "Carlos López", PuntajeTotal = 80 }
            };

            _mockSocioRepo.Setup(r => r.ObtenerRankingGeneralAsync(1, 2))
                .ReturnsAsync(ranking.Take(2).ToList());

            // Act
            var resultado = await _casoDeUso.Ejecutar(1, 2);

            // Assert
            resultado.Should().HaveCount(2);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarListaVacia_CuandoNoHaySocios()
        {
            // Arrange
            _mockSocioRepo.Setup(r => r.ObtenerRankingGeneralAsync(1, 10))
                .ReturnsAsync(new List<SocioRankingDto>());

            // Act
            var resultado = await _casoDeUso.Ejecutar(1, 10);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();
        }

        [Fact]
        public async Task Ejecutar_DebeLlamarRepositorioConParametrosCorrectos()
        {
            // Arrange
            var gimnasioId = 5L;
            var cantidad = 20;
            var ranking = new List<SocioRankingDto>();

            _mockSocioRepo.Setup(r => r.ObtenerRankingGeneralAsync(gimnasioId, cantidad))
                .ReturnsAsync(ranking);

            // Act
            await _casoDeUso.Ejecutar(gimnasioId, cantidad);

            // Assert
            _mockSocioRepo.Verify(r => r.ObtenerRankingGeneralAsync(gimnasioId, cantidad), Times.Once);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarRankingConPropiedadesCompletas()
        {
            // Arrange
            var ranking = new List<SocioRankingDto>
            {
                new SocioRankingDto
                {
                    SocioId = 1,
                    NombreCompleto = "Juan Pérez",
                    PuntajeTotal = 100
                }
            };

            _mockSocioRepo.Setup(r => r.ObtenerRankingGeneralAsync(1, 10))
                .ReturnsAsync(ranking);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1, 10);

            // Assert
            resultado[0].SocioId.Should().Be(1);
            resultado[0].NombreCompleto.Should().Be("Juan Pérez");
            resultado[0].PuntajeTotal.Should().Be(100);
        }

        [Fact]
        public async Task Ejecutar_DebePermitirCantidadesVariables()
        {
            // Arrange
            var rankingCompleto = new List<SocioRankingDto>();
            for (int i = 1; i <= 100; i++)
            {
                rankingCompleto.Add(new SocioRankingDto
                {
                    SocioId = i,
                    NombreCompleto = $"Socio {i}",
                    PuntajeTotal = i * 10
                });
            }

            _mockSocioRepo.Setup(r => r.ObtenerRankingGeneralAsync(1, 50))
                .ReturnsAsync(rankingCompleto.Take(50).ToList());

            // Act
            var resultado = await _casoDeUso.Ejecutar(1, 50);

            // Assert
            resultado.Should().HaveCount(50);
        }

        [Fact]
        public async Task Ejecutar_DebeManejarGimnasiosDiferentes()
        {
            // Arrange
            var gimnasio1Ranking = new List<SocioRankingDto>
            {
                new SocioRankingDto { SocioId = 1, NombreCompleto = "Socio Gym 1", PuntajeTotal = 100 }
            };

            var gimnasio2Ranking = new List<SocioRankingDto>
            {
                new SocioRankingDto { SocioId = 2, NombreCompleto = "Socio Gym 2", PuntajeTotal = 200 }
            };

            _mockSocioRepo.Setup(r => r.ObtenerRankingGeneralAsync(1, 10))
                .ReturnsAsync(gimnasio1Ranking);

            _mockSocioRepo.Setup(r => r.ObtenerRankingGeneralAsync(2, 10))
                .ReturnsAsync(gimnasio2Ranking);

            // Act
            var resultado1 = await _casoDeUso.Ejecutar(1, 10);
            var resultado2 = await _casoDeUso.Ejecutar(2, 10);

            // Assert
            resultado1[0].SocioId.Should().Be(1);
            resultado2[0].SocioId.Should().Be(2);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarRankingOrdenadoPorRepositorio()
        {
            // Arrange - El repositorio ya debe retornar ordenado
            var ranking = new List<SocioRankingDto>
            {
                new SocioRankingDto { SocioId = 2, NombreCompleto = "María García", PuntajeTotal = 150 },
                new SocioRankingDto { SocioId = 1, NombreCompleto = "Juan Pérez", PuntajeTotal = 100 },
                new SocioRankingDto { SocioId = 3, NombreCompleto = "Carlos López", PuntajeTotal = 80 }
            };

            _mockSocioRepo.Setup(r => r.ObtenerRankingGeneralAsync(1, 10))
                .ReturnsAsync(ranking);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1, 10);

            // Assert
            resultado.Should().ContainInOrder(ranking);
            resultado[0].PuntajeTotal.Should().Be(150);
            resultado[1].PuntajeTotal.Should().Be(100);
            resultado[2].PuntajeTotal.Should().Be(80);
        }
    }
}
