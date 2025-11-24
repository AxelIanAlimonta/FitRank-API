using FitRank_API.Application.CasosDeUso.EntrenamientoCasosDeUso;
using FitRank_API.Application.DTOs.EntrenamientoDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace FitRank_API.Tests.CasosDeUsoTests.EntrenamientoCasosDeUsoTests
{
    public class RegistrarEntrenamientoCasoDeUsoTests
    {
        private readonly Mock<IEntrenamientoRepositorio> _mockEntrenamientoRepo;
        private readonly Mock<IActividadRepositorio> _mockActividadRepo;
        private readonly FitRankDbContext _context;

        public RegistrarEntrenamientoCasoDeUsoTests()
        {
            _mockEntrenamientoRepo = new Mock<IEntrenamientoRepositorio>();
            _mockActividadRepo = new Mock<IActividadRepositorio>();
            
            var options = new DbContextOptionsBuilder<FitRankDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            
            _context = new FitRankDbContext(options);
        }

        [Fact]
        public void Constructor_DebeInicializarCorrectamente()
        {
            // Arrange & Act
            var casoDeUso = new RegistrarEntrenamientoCasoDeUso(
                _mockEntrenamientoRepo.Object,
                _mockActividadRepo.Object,
                _context
            );

            // Assert
            casoDeUso.Should().NotBeNull();
        }

        [Fact]
        public async Task Ejecutar_DebeCrearEntrenamientoConSocioId()
        {
            // Arrange
            var dto = new RegistrarEntrenamientoConActividadesDTO
            {
                SocioId = 5,
                Fecha = DateTime.UtcNow,
                Duracion = TimeSpan.FromMinutes(60),
                Actividades = new List<FitRank_API.Application.DTOs.ActividadDTOs.AgregarActividadDTO>()
            };

            var entrenamientoCreado = new Entrenamiento { Id = 1, SocioId = dto.SocioId };
            _mockEntrenamientoRepo.Setup(r => r.AgregarAsync(It.IsAny<Entrenamiento>()))
                .ReturnsAsync(entrenamientoCreado);

            var casoDeUso = new RegistrarEntrenamientoCasoDeUso(
                _mockEntrenamientoRepo.Object,
                _mockActividadRepo.Object,
                _context
            );

            // Act
            var resultado = await casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.EntrenamientoId.Should().Be(1);
        }
    }
}
