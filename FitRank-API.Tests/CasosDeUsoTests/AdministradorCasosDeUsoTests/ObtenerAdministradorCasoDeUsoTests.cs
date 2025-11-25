using AutoMapper;
using FluentAssertions;
using FitRank_API.Application.CasosDeUso.AdministradorCasosDeUso;
using FitRank_API.Application.Mappings;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using Moq;

namespace FitRank_API.Tests.CasosDeUsoTests.AdministradorCasosDeUsoTests
{
    public class ObtenerAdministradorCasoDeUsoTests
    {
        private readonly Mock<IAdministradorRepositorio> _mockRepositorio;
        private readonly IMapper _mapper;
        private readonly ObtenerAdministradorCasoDeUso _casoDeUso;

        public ObtenerAdministradorCasoDeUsoTests()
        {
            _mockRepositorio = new Mock<IAdministradorRepositorio>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AdminProfile>();
                cfg.AddProfile<UsuarioProfile>();
            });
            _mapper = config.CreateMapper();
            _casoDeUso = new ObtenerAdministradorCasoDeUso(_mockRepositorio.Object, _mapper);
        }

        [Fact]
        public async Task DeberiaRetornarTodosLosAdministradores()
        {
            // Arrange
            var administradores = new List<Administrador>
            {
                new Administrador { Id = 1, Nombre = "Admin1", Apellido = "Test1", Email = "admin1@test.com", Cuil = "20-12345678-9" },
                new Administrador { Id = 2, Nombre = "Admin2", Apellido = "Test2", Email = "admin2@test.com", Cuil = "20-87654321-9" }
            };

            _mockRepositorio.Setup(r => r.ObtenerTodosAsync())
                .ReturnsAsync(administradores);

            // Act
            var resultado = await _casoDeUso.Ejecutar();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(2);
            resultado.First().Id.Should().Be(1);
            resultado.Last().Id.Should().Be(2);
            _mockRepositorio.Verify(r => r.ObtenerTodosAsync(), Times.Once);
        }

        [Fact]
        public async Task DeberiaRetornarListaVaciaCuandoNoHayAdministradores()
        {
            // Arrange
            var administradoresVacios = new List<Administrador>();

            _mockRepositorio.Setup(r => r.ObtenerTodosAsync())
                .ReturnsAsync(administradoresVacios);

            // Act
            var resultado = await _casoDeUso.Ejecutar();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();
            _mockRepositorio.Verify(r => r.ObtenerTodosAsync(), Times.Once);
        }
    }
}
