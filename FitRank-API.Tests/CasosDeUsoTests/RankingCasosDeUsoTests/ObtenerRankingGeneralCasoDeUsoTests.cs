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
    public class ObtenerRankingGeneralCasoDeUsoTests
    {
        private readonly Mock<IRankingRepositorio> _mockRepositorio;
        private readonly IMapper _mapper;
        private readonly ObtenerRankingGeneralCasoDeUso _casoDeUso;

        public ObtenerRankingGeneralCasoDeUsoTests()
        {
            _mockRepositorio = new Mock<IRankingRepositorio>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<RankingProfile>();
                cfg.AddProfile<SocioProfile>();
            });
            _mapper = config.CreateMapper();
            _casoDeUso = new ObtenerRankingGeneralCasoDeUso(_mockRepositorio.Object, _mapper);
        }

        [Fact]
        public async Task DeberiaRetornarTopSocios()
        {
            // Arrange
            int top = 10;
            var rankingDTOs = new List<RankingDTO>
            {
                new RankingDTO { SocioId = 1, NombreCompleto = "Juan Pérez", PuntajeTotal = 100 },
                new RankingDTO { SocioId = 2, NombreCompleto = "María García", PuntajeTotal = 95 },
                new RankingDTO { SocioId = 3, NombreCompleto = "Pedro López", PuntajeTotal = 90 }
            };

            _mockRepositorio.Setup(r => r.ObtenerTopSociosAsync(top))
                .ReturnsAsync(rankingDTOs);

            // Act
            var resultado = await _casoDeUso.Ejecutar(top);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(3);
            resultado.First().SocioId.Should().Be(1);
            resultado.First().NombreCompleto.Should().Be("Juan Pérez");
            resultado.Last().SocioId.Should().Be(3);
            _mockRepositorio.Verify(r => r.ObtenerTopSociosAsync(top), Times.Once);
        }

        [Fact]
        public async Task DeberiaRetornarListaVaciaCuandoNoHaySocios()
        {
            // Arrange
            int top = 10;
            var rankingVacio = new List<RankingDTO>();

            _mockRepositorio.Setup(r => r.ObtenerTopSociosAsync(top))
                .ReturnsAsync(rankingVacio);

            // Act
            var resultado = await _casoDeUso.Ejecutar(top);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();
            _mockRepositorio.Verify(r => r.ObtenerTopSociosAsync(top), Times.Once);
        }

        [Fact]
        public async Task DebeMapearCorrectamenteTodosLosCampos()
        {
            // Arrange
            int top = 5;
            var rankingDTOs = new List<RankingDTO>
            {
                new RankingDTO { SocioId = 10, NombreCompleto = "Carlos Rodríguez", PuntajeTotal = 250 },
                new RankingDTO { SocioId = 20, NombreCompleto = "Ana Martínez", PuntajeTotal = 200 }
            };

            _mockRepositorio.Setup(r => r.ObtenerTopSociosAsync(top))
                .ReturnsAsync(rankingDTOs);

            // Act
            var resultado = await _casoDeUso.Ejecutar(top);

            // Assert
            resultado.Should().HaveCount(2);
            resultado[0].SocioId.Should().Be(10);
            resultado[0].NombreCompleto.Should().Be("Carlos Rodríguez");
            resultado[0].PuntajeTotal.Should().Be(250);
            resultado[1].SocioId.Should().Be(20);
            resultado[1].NombreCompleto.Should().Be("Ana Martínez");
            resultado[1].PuntajeTotal.Should().Be(200);
        }

        [Fact]
        public async Task DebeLlamarRepositorioConTopCorrecto()
        {
            // Arrange
            int top = 20;
            var rankingDTOs = new List<RankingDTO>();

            _mockRepositorio.Setup(r => r.ObtenerTopSociosAsync(top))
                .ReturnsAsync(rankingDTOs);

            // Act
            await _casoDeUso.Ejecutar(top);

            // Assert
            _mockRepositorio.Verify(r => r.ObtenerTopSociosAsync(top), Times.Once);
        }

        [Fact]
        public async Task DeberiaRetornarExactamenteCantidadTopSolicitada()
        {
            // Arrange
            int top = 3;
            var rankingDTOs = new List<RankingDTO>
            {
                new RankingDTO { SocioId = 1, NombreCompleto = "Primero", PuntajeTotal = 300 },
                new RankingDTO { SocioId = 2, NombreCompleto = "Segundo", PuntajeTotal = 200 },
                new RankingDTO { SocioId = 3, NombreCompleto = "Tercero", PuntajeTotal = 100 }
            };

            _mockRepositorio.Setup(r => r.ObtenerTopSociosAsync(top))
                .ReturnsAsync(rankingDTOs);

            // Act
            var resultado = await _casoDeUso.Ejecutar(top);

            // Assert
            resultado.Should().HaveCount(3);
        }

        [Fact]
        public async Task DeberiaRetornarRankingEnElMismoOrdenDelRepositorio()
        {
            // Arrange
            int top = 5;
            var rankingDTOs = new List<RankingDTO>
            {
                new RankingDTO { SocioId = 5, NombreCompleto = "Quinto", PuntajeTotal = 150 },
                new RankingDTO { SocioId = 3, NombreCompleto = "Tercero", PuntajeTotal = 250 },
                new RankingDTO { SocioId = 1, NombreCompleto = "Primero", PuntajeTotal = 350 }
            };

            _mockRepositorio.Setup(r => r.ObtenerTopSociosAsync(top))
                .ReturnsAsync(rankingDTOs);

            // Act
            var resultado = await _casoDeUso.Ejecutar(top);

            // Assert
            resultado[0].SocioId.Should().Be(5);
            resultado[1].SocioId.Should().Be(3);
            resultado[2].SocioId.Should().Be(1);
        }
    }
}
