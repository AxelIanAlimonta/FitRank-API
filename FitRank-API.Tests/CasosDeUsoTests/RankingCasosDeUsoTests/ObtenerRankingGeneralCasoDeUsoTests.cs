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
    }
}
