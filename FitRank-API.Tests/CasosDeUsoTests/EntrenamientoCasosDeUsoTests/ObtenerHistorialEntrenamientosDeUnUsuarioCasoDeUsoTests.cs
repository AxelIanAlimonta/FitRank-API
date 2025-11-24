using AutoMapper;
using FitRank_API.Application.CasosDeUso.EntrenamientoCasosDeUso;
using FitRank_API.Application.DTOs.EntrenamientoDTOs;
using FitRank_API.Infrastructure.Interfaces;
using FluentAssertions;
using Moq;

namespace FitRank_API.Tests.CasosDeUsoTests.EntrenamientoCasosDeUsoTests
{
    public class ObtenerHistorialEntrenamientosDeUnUsuarioCasoDeUsoTests
    {
        private readonly Mock<IEntrenamientoRepositorio> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ObtenerHistorialEntrenamientosDeUnUsuarioCasoDeUso _casoDeUso;

        public ObtenerHistorialEntrenamientosDeUnUsuarioCasoDeUsoTests()
        {
            _mockRepo = new Mock<IEntrenamientoRepositorio>();
            _mockMapper = new Mock<IMapper>();
            _casoDeUso = new ObtenerHistorialEntrenamientosDeUnUsuarioCasoDeUso(_mockRepo.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task EjecutarAsync_DebeRetornarHistorialVacio()
        {
            // Arrange
            var socioId = 1L;
            _mockRepo.Setup(r => r.ObtenerHistorialCompletoPorSocioAsync(socioId))
                .ReturnsAsync(new List<Domain.Entities.Entrenamiento>());
            _mockMapper.Setup(m => m.Map<List<EntrenamientoHistorialDTO>>(It.IsAny<object>()))
                .Returns(new List<EntrenamientoHistorialDTO>());

            // Act
            var resultado = await _casoDeUso.EjecutarAsync(socioId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();
        }

        [Fact]
        public async Task EjecutarAsync_DebeLlamarRepositorio()
        {
            // Arrange
            var socioId = 5L;
            _mockRepo.Setup(r => r.ObtenerHistorialCompletoPorSocioAsync(socioId))
                .ReturnsAsync(new List<Domain.Entities.Entrenamiento>());
            _mockMapper.Setup(m => m.Map<List<EntrenamientoHistorialDTO>>(It.IsAny<object>()))
                .Returns(new List<EntrenamientoHistorialDTO>());

            // Act
            await _casoDeUso.EjecutarAsync(socioId);

            // Assert
            _mockRepo.Verify(r => r.ObtenerHistorialCompletoPorSocioAsync(socioId), Times.Once);
        }
    }
}
