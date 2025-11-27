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
    public class ObtenerTodasLasRutinasPorProfesorCasoDeUsoTests
    {
        private readonly Mock<IRutinaRepositorio> _mockRutinaRepo;
        private readonly Mock<IProfesorRepositorio> _mockProfesorRepo;
        private readonly IMapper _mapper;
        private readonly ObtenerTodasLasRutinasPorProfesorCasoDeUso _casoDeUso;

        public ObtenerTodasLasRutinasPorProfesorCasoDeUsoTests()
        {
            _mockRutinaRepo = new Mock<IRutinaRepositorio>();
            _mockProfesorRepo = new Mock<IProfesorRepositorio>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<RutinaProfile>();
            });
            _mapper = config.CreateMapper();
            _casoDeUso = new ObtenerTodasLasRutinasPorProfesorCasoDeUso(
                _mockRutinaRepo.Object,
                _mapper,
                _mockProfesorRepo.Object);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarRutinasDelProfesor()
        {
            // Arrange
            var profesorId = 1L;
            var profesor = new Profesor { Id = profesorId, Nombre = "Profesor Test" };
            var rutinas = new List<Rutina>
            {
                new Rutina { Id = 1, Nombre = "Rutina 1", UsuarioId = profesorId },
                new Rutina { Id = 2, Nombre = "Rutina 2", UsuarioId = profesorId }
            };

            _mockProfesorRepo.Setup(r => r.ObtenerPorIdAsync(profesorId)).ReturnsAsync(profesor);
            _mockRutinaRepo.Setup(r => r.ObtenerTodasLasRutinasPorProfesorIdAsync(profesorId))
                .ReturnsAsync(rutinas);

            // Act
            var resultado = await _casoDeUso.Ejecutar(profesorId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(2);
            resultado.First().Nombre.Should().Be("Rutina 1");
        }

        [Fact]
        public async Task Ejecutar_DebeLanzarExcepcion_CuandoProfesorNoExiste()
        {
            // Arrange
            var profesorId = 999L;
            _mockProfesorRepo.Setup(r => r.ObtenerPorIdAsync(profesorId))
                .ReturnsAsync((Profesor?)null);

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _casoDeUso.Ejecutar(profesorId));
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarListaVacia_CuandoProfesorNoTieneRutinas()
        {
            // Arrange
            var profesorId = 1L;
            var profesor = new Profesor { Id = profesorId, Nombre = "Profesor Test" };

            _mockProfesorRepo.Setup(r => r.ObtenerPorIdAsync(profesorId)).ReturnsAsync(profesor);
            _mockRutinaRepo.Setup(r => r.ObtenerTodasLasRutinasPorProfesorIdAsync(profesorId))
                .ReturnsAsync(new List<Rutina>());

            // Act
            var resultado = await _casoDeUso.Ejecutar(profesorId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();
        }

        [Fact]
        public async Task Ejecutar_DebeVerificarExistenciaDelProfesor()
        {
            // Arrange
            var profesorId = 1L;
            var profesor = new Profesor { Id = profesorId };

            _mockProfesorRepo.Setup(r => r.ObtenerPorIdAsync(profesorId)).ReturnsAsync(profesor);
            _mockRutinaRepo.Setup(r => r.ObtenerTodasLasRutinasPorProfesorIdAsync(profesorId))
                .ReturnsAsync(new List<Rutina>());

            // Act
            await _casoDeUso.Ejecutar(profesorId);

            // Assert
            _mockProfesorRepo.Verify(r => r.ObtenerPorIdAsync(profesorId), Times.Once);
        }

        [Fact]
        public async Task Ejecutar_DebeLlamarRepositorioConProfesorIdCorrecto()
        {
            // Arrange
            var profesorId = 5L;
            var profesor = new Profesor { Id = profesorId };

            _mockProfesorRepo.Setup(r => r.ObtenerPorIdAsync(profesorId)).ReturnsAsync(profesor);
            _mockRutinaRepo.Setup(r => r.ObtenerTodasLasRutinasPorProfesorIdAsync(profesorId))
                .ReturnsAsync(new List<Rutina>());

            // Act
            await _casoDeUso.Ejecutar(profesorId);

            // Assert
            _mockRutinaRepo.Verify(r => r.ObtenerTodasLasRutinasPorProfesorIdAsync(profesorId), Times.Once);
        }

        [Fact]
        public async Task Ejecutar_DebeMapearCorrectamenteARutinaProfesorDTO()
        {
            // Arrange
            var profesorId = 1L;
            var profesor = new Profesor { Id = profesorId };
            var rutinas = new List<Rutina>
            {
                new Rutina
                {
                    Id = 10,
                    Nombre = "Rutina Test",
                    Descripcion = "Descripción Test",
                    UsuarioId = profesorId
                }
            };

            _mockProfesorRepo.Setup(r => r.ObtenerPorIdAsync(profesorId)).ReturnsAsync(profesor);
            _mockRutinaRepo.Setup(r => r.ObtenerTodasLasRutinasPorProfesorIdAsync(profesorId))
                .ReturnsAsync(rutinas);

            // Act
            var resultado = await _casoDeUso.Ejecutar(profesorId);

            // Assert
            resultado.Should().AllBeOfType<RutinaProfesorDTO>();
            resultado.First().Id.Should().Be(10);
            resultado.First().Nombre.Should().Be("Rutina Test");
        }

        [Fact]
        public async Task Ejecutar_DebeManejarVariasRutinasDelProfesor()
        {
            // Arrange
            var profesorId = 1L;
            var profesor = new Profesor { Id = profesorId };
            var rutinas = new List<Rutina>();
            for (int i = 1; i <= 7; i++)
            {
                rutinas.Add(new Rutina
                {
                    Id = i,
                    Nombre = $"Rutina {i}",
                    UsuarioId = profesorId
                });
            }

            _mockProfesorRepo.Setup(r => r.ObtenerPorIdAsync(profesorId)).ReturnsAsync(profesor);
            _mockRutinaRepo.Setup(r => r.ObtenerTodasLasRutinasPorProfesorIdAsync(profesorId))
                .ReturnsAsync(rutinas);

            // Act
            var resultado = await _casoDeUso.Ejecutar(profesorId);

            // Assert
            resultado.Should().HaveCount(7);
            resultado.Select(r => r.Id).Should().ContainInOrder(Enumerable.Range(1, 7).Select(i => (long)i));
        }

        [Fact]
        public async Task Ejecutar_DebeLanzarExcepcionConMensajeEspecifico()
        {
            // Arrange
            var profesorId = 999L;
            _mockProfesorRepo.Setup(r => r.ObtenerPorIdAsync(profesorId))
                .ReturnsAsync((Profesor?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _casoDeUso.Ejecutar(profesorId));
            exception.Message.Should().Be("Profesor no encontrado");
        }
    }
}
