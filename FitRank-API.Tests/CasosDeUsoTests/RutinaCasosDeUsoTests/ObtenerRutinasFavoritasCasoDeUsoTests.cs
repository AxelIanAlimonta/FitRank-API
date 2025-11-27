using FitRank_API.Application.CasosDeUso.RutinaCasosDeUso;
using FitRank_API.Application.DTOs.RutinaDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace FitRank_API.Tests.CasosDeUsoTests.RutinaCasosDeUsoTests
{
    public class ObtenerRutinasFavoritasCasoDeUsoTests
    {
        private readonly Mock<IRutinaRepositorio> _mockRepo;
        private readonly ObtenerRutinasFavoritasCasoDeUso _casoDeUso;

        public ObtenerRutinasFavoritasCasoDeUsoTests()
        {
            _mockRepo = new Mock<IRutinaRepositorio>();
            _casoDeUso = new ObtenerRutinasFavoritasCasoDeUso(_mockRepo.Object);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarRutinasFavoritas()
        {
            // Arrange
            var socioId = 1L;
            var rutinas = new List<Rutina>
            {
                new Rutina
                {
                    Id = 1,
                    Nombre = "Rutina Favorita 1",
                    TipoCreacion = "Manual",
                    FechaCreacion = DateTime.UtcNow,
                    Descripcion = "Desc 1",
                    Activa = true,
                    Favorita = true,
                    SocioId = socioId,
                    UsuarioId = 10
                },
                new Rutina
                {
                    Id = 2,
                    Nombre = "Rutina Favorita 2",
                    TipoCreacion = "IA",
                    FechaCreacion = DateTime.UtcNow,
                    Descripcion = "Desc 2",
                    Activa = true,
                    Favorita = true,
                    SocioId = socioId,
                    UsuarioId = 10
                }
            };

            _mockRepo.Setup(r => r.ObtenerFavoritasPorSocioAsync(socioId)).ReturnsAsync(rutinas);

            // Act
            var resultado = await _casoDeUso.Ejecutar(socioId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(2);
            resultado.All(r => r.Favorita).Should().BeTrue();
            resultado.First().Nombre.Should().Be("Rutina Favorita 1");
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarListaVacia_CuandoNoHayFavoritas()
        {
            // Arrange
            var socioId = 1L;
            _mockRepo.Setup(r => r.ObtenerFavoritasPorSocioAsync(socioId))
                .ReturnsAsync(new List<Rutina>());

            // Act
            var resultado = await _casoDeUso.Ejecutar(socioId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();
        }

        [Fact]
        public async Task Ejecutar_DebeLlamarRepositorioConSocioIdCorrecto()
        {
            // Arrange
            var socioId = 5L;
            _mockRepo.Setup(r => r.ObtenerFavoritasPorSocioAsync(socioId))
                .ReturnsAsync(new List<Rutina>());

            // Act
            await _casoDeUso.Ejecutar(socioId);

            // Assert
            _mockRepo.Verify(r => r.ObtenerFavoritasPorSocioAsync(socioId), Times.Once);
        }

        [Fact]
        public async Task Ejecutar_DebeMapearCorrectamenteLasPropiedades()
        {
            // Arrange
            var socioId = 1L;
            var rutinas = new List<Rutina>
            {
                new Rutina
                {
                    Id = 10,
                    Nombre = "Rutina Test",
                    TipoCreacion = "Manual",
                    FechaCreacion = new DateTime(2025, 1, 1),
                    Descripcion = "Descripción Test",
                    Activa = true,
                    Favorita = true,
                    SocioId = socioId,
                    UsuarioId = 100
                }
            };

            _mockRepo.Setup(r => r.ObtenerFavoritasPorSocioAsync(socioId)).ReturnsAsync(rutinas);

            // Act
            var resultado = await _casoDeUso.Ejecutar(socioId);

            // Assert
            var dto = resultado.First();
            dto.Id.Should().Be(10);
            dto.Nombre.Should().Be("Rutina Test");
            dto.TipoCreacion.Should().Be("Manual");
            dto.FechaCreacion.Should().Be(new DateTime(2025, 1, 1));
            dto.Descripcion.Should().Be("Descripción Test");
            dto.Activa.Should().BeTrue();
            dto.Favorita.Should().BeTrue();
            dto.SocioId.Should().Be(socioId);
            dto.UsuarioId.Should().Be(100);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarListaDeObtenerRutinaDTO()
        {
            // Arrange
            var socioId = 1L;
            var rutinas = new List<Rutina>
            {
                new Rutina { Id = 1, Nombre = "Test", Favorita = true, SocioId = socioId }
            };

            _mockRepo.Setup(r => r.ObtenerFavoritasPorSocioAsync(socioId)).ReturnsAsync(rutinas);

            // Act
            var resultado = await _casoDeUso.Ejecutar(socioId);

            // Assert
            resultado.Should().AllBeOfType<ObtenerRutinaDTO>();
        }

        [Fact]
        public async Task Ejecutar_DebeFiltrarSoloPorSocioId()
        {
            // Arrange
            var socioId1 = 1L;
            var socioId2 = 2L;
            var rutinasSocio1 = new List<Rutina>
            {
                new Rutina { Id = 1, Nombre = "Rutina Socio 1", Favorita = true, SocioId = socioId1 }
            };
            var rutinasSocio2 = new List<Rutina>
            {
                new Rutina { Id = 2, Nombre = "Rutina Socio 2", Favorita = true, SocioId = socioId2 }
            };

            _mockRepo.Setup(r => r.ObtenerFavoritasPorSocioAsync(socioId1)).ReturnsAsync(rutinasSocio1);
            _mockRepo.Setup(r => r.ObtenerFavoritasPorSocioAsync(socioId2)).ReturnsAsync(rutinasSocio2);

            // Act
            var resultado1 = await _casoDeUso.Ejecutar(socioId1);
            var resultado2 = await _casoDeUso.Ejecutar(socioId2);

            // Assert
            resultado1.First().Nombre.Should().Be("Rutina Socio 1");
            resultado2.First().Nombre.Should().Be("Rutina Socio 2");
        }

        [Fact]
        public async Task Ejecutar_DebeManejarVariasRutinasFavoritas()
        {
            // Arrange
            var socioId = 1L;
            var rutinas = new List<Rutina>();
            for (int i = 1; i <= 5; i++)
            {
                rutinas.Add(new Rutina
                {
                    Id = i,
                    Nombre = $"Rutina Favorita {i}",
                    Favorita = true,
                    SocioId = socioId
                });
            }

            _mockRepo.Setup(r => r.ObtenerFavoritasPorSocioAsync(socioId)).ReturnsAsync(rutinas);

            // Act
            var resultado = await _casoDeUso.Ejecutar(socioId);

            // Assert
            resultado.Should().HaveCount(5);
            resultado.Select(r => r.Id).Should().ContainInOrder(1L, 2L, 3L, 4L, 5L);
        }
    }
}
