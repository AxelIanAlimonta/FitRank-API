using AutoMapper;
using FitRank_API.Application.CasosDeUso.ProfesorCasosDeUso;
using FitRank_API.Application.DTOs.ProfesorDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FluentAssertions;
using Moq;

namespace FitRank_API.Tests.CasosDeUsoTests.ProfesorCasosDeUsoTests
{
    public class ObtenerTodosPorGimnasioCasoDeUsoTests
    {
        private readonly Mock<IProfesorRepositorio> _mockRepo;
        private readonly IMapper _mapper;
        private readonly ObtenerTodosPorGimnasioCasoDeUso _casoDeUso;

        public ObtenerTodosPorGimnasioCasoDeUsoTests()
        {
            _mockRepo = new Mock<IProfesorRepositorio>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Profesor, ProfesorDTO>();
            });
            _mapper = config.CreateMapper();

            _casoDeUso = new ObtenerTodosPorGimnasioCasoDeUso(_mockRepo.Object, _mapper);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarProfesoresDelGimnasio()
        {
            // Arrange
            long gimnasioId = 5;
            var profesores = new List<Profesor>
            {
                new Profesor { Id = 1, Nombre = "Juan", Apellido = "Pérez", GimnasioId = gimnasioId },
                new Profesor { Id = 2, Nombre = "María", Apellido = "González", GimnasioId = gimnasioId }
            };

            _mockRepo.Setup(r => r.ObtenerPorGimnasioAsync(gimnasioId)).ReturnsAsync(profesores);

            // Act
            var resultado = await _casoDeUso.Ejecutar(gimnasioId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(2);
            resultado[0].Nombre.Should().Be("Juan");
            resultado[1].Nombre.Should().Be("María");
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarListaVaciaSiNoHayProfesores()
        {
            // Arrange
            long gimnasioId = 10;
            _mockRepo.Setup(r => r.ObtenerPorGimnasioAsync(gimnasioId)).ReturnsAsync(new List<Profesor>());

            // Act
            var resultado = await _casoDeUso.Ejecutar(gimnasioId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();
        }

        [Fact]
        public async Task Ejecutar_DebeLlamarRepositorioConGimnasioIdCorrecto()
        {
            // Arrange
            long gimnasioId = 7;
            _mockRepo.Setup(r => r.ObtenerPorGimnasioAsync(gimnasioId)).ReturnsAsync(new List<Profesor>());

            // Act
            await _casoDeUso.Ejecutar(gimnasioId);

            // Assert
            _mockRepo.Verify(r => r.ObtenerPorGimnasioAsync(gimnasioId), Times.Once);
        }

        [Fact]
        public async Task Ejecutar_DebeMappearProfesoresADTO()
        {
            // Arrange
            long gimnasioId = 3;
            var profesor = new Profesor
            {
                Id = 100,
                Nombre = "Carlos",
                Apellido = "Rodríguez",
                Email = "carlos@test.com",
                Matricula = "MAT123",
                GimnasioId = gimnasioId
            };

            _mockRepo.Setup(r => r.ObtenerPorGimnasioAsync(gimnasioId)).ReturnsAsync(new List<Profesor> { profesor });

            // Act
            var resultado = await _casoDeUso.Ejecutar(gimnasioId);

            // Assert
            resultado.Should().HaveCount(1);
            resultado[0].Id.Should().Be(100);
            resultado[0].Nombre.Should().Be("Carlos");
            resultado[0].Apellido.Should().Be("Rodríguez");
            resultado[0].Email.Should().Be("carlos@test.com");
        }
    }
}
