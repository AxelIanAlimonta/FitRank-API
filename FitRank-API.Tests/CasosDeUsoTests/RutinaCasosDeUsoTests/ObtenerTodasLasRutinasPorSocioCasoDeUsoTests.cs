using AutoMapper;
using FitRank_API.Application.CasosDeUso.RutinaCasosDeUso;
using FitRank_API.Application.DTOs.RutinaDTOs;
using FitRank_API.Application.Mappings;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace FitRank_API.Tests.CasosDeUsoTests.RutinaCasosDeUsoTests
{
    public class ObtenerTodasLasRutinasPorSocioCasoDeUsoTests
    {
        private readonly Mock<IRutinaRepositorio> _mockRutinaRepo;
        private readonly Mock<ISocioRepositorio> _mockSocioRepo;
        private readonly IMapper _mapper;
        private readonly ObtenerTodasLasRutinasPorSocioCasoDeUso _casoDeUso;

        public ObtenerTodasLasRutinasPorSocioCasoDeUsoTests()
        {
            _mockRutinaRepo = new Mock<IRutinaRepositorio>();
            _mockSocioRepo = new Mock<ISocioRepositorio>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<RutinaProfile>();
            });
            _mapper = config.CreateMapper();
            _casoDeUso = new ObtenerTodasLasRutinasPorSocioCasoDeUso(
                _mockRutinaRepo.Object,
                _mockSocioRepo.Object,
                _mapper);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarRutinasDelSocio()
        {
            // Arrange
            var socioId = 1L;
            var socio = new Socio { Id = socioId, Nombre = "Socio Test" };
            var rutinas = new List<Rutina>
            {
                new Rutina { Id = 1, Nombre = "Rutina 1", SocioId = socioId },
                new Rutina { Id = 2, Nombre = "Rutina 2", SocioId = socioId },
                new Rutina { Id = 3, Nombre = "Rutina 3", SocioId = socioId }
            };

            _mockSocioRepo.Setup(r => r.ObtenerPorIdAsync(socioId)).ReturnsAsync(socio);
            _mockRutinaRepo.Setup(r => r.ObtenerPorSocioIdAsync(socioId)).ReturnsAsync(rutinas);

            // Act
            var resultado = await _casoDeUso.Ejecutar(socioId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(3);
            resultado.First().Nombre.Should().Be("Rutina 1");
            resultado.All(r => r.SocioId == socioId).Should().BeTrue();
        }

        [Fact]
        public async Task Ejecutar_DebeLanzarExcepcion_CuandoSocioNoExiste()
        {
            // Arrange
            var socioId = 999L;
            _mockSocioRepo.Setup(r => r.ObtenerPorIdAsync(socioId))
                .ReturnsAsync((Socio?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _casoDeUso.Ejecutar(socioId));
            exception.Message.Should().Be("Socio no encontrado");
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarListaVacia_CuandoSocioNoTieneRutinas()
        {
            // Arrange
            var socioId = 1L;
            var socio = new Socio { Id = socioId };

            _mockSocioRepo.Setup(r => r.ObtenerPorIdAsync(socioId)).ReturnsAsync(socio);
            _mockRutinaRepo.Setup(r => r.ObtenerPorSocioIdAsync(socioId))
                .ReturnsAsync(new List<Rutina>());

            // Act
            var resultado = await _casoDeUso.Ejecutar(socioId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();
        }

        [Fact]
        public async Task Ejecutar_DebeVerificarExistenciaDelSocio()
        {
            // Arrange
            var socioId = 1L;
            var socio = new Socio { Id = socioId };

            _mockSocioRepo.Setup(r => r.ObtenerPorIdAsync(socioId)).ReturnsAsync(socio);
            _mockRutinaRepo.Setup(r => r.ObtenerPorSocioIdAsync(socioId))
                .ReturnsAsync(new List<Rutina>());

            // Act
            await _casoDeUso.Ejecutar(socioId);

            // Assert
            _mockSocioRepo.Verify(r => r.ObtenerPorIdAsync(socioId), Times.Once);
        }

        [Fact]
        public async Task Ejecutar_DebeLlamarRepositorioConSocioIdCorrecto()
        {
            // Arrange
            var socioId = 5L;
            var socio = new Socio { Id = socioId };

            _mockSocioRepo.Setup(r => r.ObtenerPorIdAsync(socioId)).ReturnsAsync(socio);
            _mockRutinaRepo.Setup(r => r.ObtenerPorSocioIdAsync(socioId))
                .ReturnsAsync(new List<Rutina>());

            // Act
            await _casoDeUso.Ejecutar(socioId);

            // Assert
            _mockRutinaRepo.Verify(r => r.ObtenerPorSocioIdAsync(socioId), Times.Once);
        }

        [Fact]
        public async Task Ejecutar_DebeMapearCorrectamenteAObtenerRutinaDTO()
        {
            // Arrange
            var socioId = 1L;
            var socio = new Socio { Id = socioId };
            var rutinas = new List<Rutina>
            {
                new Rutina
                {
                    Id = 10,
                    Nombre = "Rutina Test",
                    Descripcion = "Descripción Test",
                    TipoCreacion = "Manual",
                    Activa = true,
                    Favorita = false,
                    SocioId = socioId,
                    UsuarioId = 100
                }
            };

            _mockSocioRepo.Setup(r => r.ObtenerPorIdAsync(socioId)).ReturnsAsync(socio);
            _mockRutinaRepo.Setup(r => r.ObtenerPorSocioIdAsync(socioId)).ReturnsAsync(rutinas);

            // Act
            var resultado = await _casoDeUso.Ejecutar(socioId);

            // Assert
            resultado.Should().AllBeOfType<ObtenerRutinaDTO>();
            var dto = resultado.First();
            dto.Id.Should().Be(10);
            dto.Nombre.Should().Be("Rutina Test");
            dto.Descripcion.Should().Be("Descripción Test");
            dto.TipoCreacion.Should().Be("Manual");
            dto.Activa.Should().BeTrue();
            dto.Favorita.Should().BeFalse();
            dto.SocioId.Should().Be(socioId);
            dto.UsuarioId.Should().Be(100);
        }

        [Fact]
        public async Task Ejecutar_DebeManejarVariasRutinasDelSocio()
        {
            // Arrange
            var socioId = 1L;
            var socio = new Socio { Id = socioId };
            var rutinas = new List<Rutina>();
            for (int i = 1; i <= 8; i++)
            {
                rutinas.Add(new Rutina
                {
                    Id = i,
                    Nombre = $"Rutina {i}",
                    SocioId = socioId
                });
            }

            _mockSocioRepo.Setup(r => r.ObtenerPorIdAsync(socioId)).ReturnsAsync(socio);
            _mockRutinaRepo.Setup(r => r.ObtenerPorSocioIdAsync(socioId)).ReturnsAsync(rutinas);

            // Act
            var resultado = await _casoDeUso.Ejecutar(socioId);

            // Assert
            resultado.Should().HaveCount(8);
            resultado.Select(r => r.Id).Should().ContainInOrder(Enumerable.Range(1, 8).Select(i => (long)i));
        }

        [Fact]
        public async Task Ejecutar_DebeValidarSocioAntesDeObtenerRutinas()
        {
            // Arrange
            var socioId = 1L;
            var callOrder = new List<string>();

            _mockSocioRepo.Setup(r => r.ObtenerPorIdAsync(socioId))
                .Callback(() => callOrder.Add("ObtenerSocio"))
                .ReturnsAsync(new Socio { Id = socioId });

            _mockRutinaRepo.Setup(r => r.ObtenerPorSocioIdAsync(socioId))
                .Callback(() => callOrder.Add("ObtenerRutinas"))
                .ReturnsAsync(new List<Rutina>());

            // Act
            await _casoDeUso.Ejecutar(socioId);

            // Assert
            callOrder.Should().ContainInOrder("ObtenerSocio", "ObtenerRutinas");
        }
    }
}
