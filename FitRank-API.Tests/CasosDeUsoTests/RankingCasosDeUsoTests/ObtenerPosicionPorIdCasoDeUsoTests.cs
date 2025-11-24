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
    }
}
