using AutoMapper;
using FitRank_API.Application.CasosDeUso.NotificacionCasoDeUso;
using FitRank_API.Application.DTOs.NotificacionDTOs;
using FitRank_API.Application.Mappings;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace FitRank_API.Tests.CasosDeUsoTests.NotificacionCasosDeUsoTests
{
    public class ObtenerUsuariosParaNotificacionCasoDeUsoTests
    {
        private readonly Mock<IUsuarioRepositorio> _mockRepo;
        private readonly IMapper _mapper;
        private readonly ObtenerUsuariosParaNotificacionCasoDeUso _casoDeUso;

        public ObtenerUsuariosParaNotificacionCasoDeUsoTests()
        {
            _mockRepo = new Mock<IUsuarioRepositorio>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<UsuarioProfile>());
            _mapper = config.CreateMapper();
            _casoDeUso = new ObtenerUsuariosParaNotificacionCasoDeUso(_mockRepo.Object, _mapper);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarTodosLosUsuarios()
        {
            // Arrange
            var usuarios = new List<Usuario>
            {
                new Socio { Id = 1, Nombre = "Socio1", Email = "socio1@test.com", NombreUsuario = "socio1" },
                new Profesor { Id = 2, Nombre = "Profesor1", Email = "prof1@test.com", NombreUsuario = "prof1" },
                new Administrador { Id = 3, Nombre = "Admin1", Email = "admin1@test.com", NombreUsuario = "admin1" }
            };

            _mockRepo.Setup(r => r.ObtenerTodosAsync())
                .ReturnsAsync(usuarios);

            // Act
            var resultado = await _casoDeUso.Ejecutar();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(3);
            resultado.Should().Contain(u => u.NombreCompleto == "Socio1");
            resultado.Should().Contain(u => u.NombreCompleto == "Profesor1");
            resultado.Should().Contain(u => u.NombreCompleto == "Admin1");
            _mockRepo.Verify(r => r.ObtenerTodosAsync(), Times.Once);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarListaVaciaSiNoHayUsuarios()
        {
            // Arrange
            _mockRepo.Setup(r => r.ObtenerTodosAsync())
                .ReturnsAsync(new List<Usuario>());

            // Act
            var resultado = await _casoDeUso.Ejecutar();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();
            _mockRepo.Verify(r => r.ObtenerTodosAsync(), Times.Once);
        }

        [Fact]
        public async Task Ejecutar_DebeMappearCorrectamenteLasPropiedades()
        {
            // Arrange
            var usuarios = new List<Usuario>
            {
                new Socio
                {
                    Id = 10,
                    Nombre = "Juan",
                    Apellido = "Pérez",
                    Email = "juan@test.com",
                    NombreUsuario = "juanp"
                }
            };

            _mockRepo.Setup(r => r.ObtenerTodosAsync())
                .ReturnsAsync(usuarios);

            // Act
            var resultado = await _casoDeUso.Ejecutar();

            // Assert
            var usuario = resultado.First();
            usuario.Id.Should().Be(10);
            usuario.NombreCompleto.Should().Be("Juan");
        }

        [Fact]
        public async Task Ejecutar_DebeIncluirTodosLosTiposDeUsuario()
        {
            // Arrange
            var usuarios = new List<Usuario>
            {
                new Socio { Id = 1, Nombre = "Socio", Email = "socio@test.com", NombreUsuario = "socio" },
                new Profesor { Id = 2, Nombre = "Profesor", Email = "profesor@test.com", NombreUsuario = "profesor" },
                new Administrador { Id = 3, Nombre = "Admin", Email = "admin@test.com", NombreUsuario = "admin" }
            };

            _mockRepo.Setup(r => r.ObtenerTodosAsync())
                .ReturnsAsync(usuarios);

            // Act
            var resultado = await _casoDeUso.Ejecutar();

            // Assert
            resultado.Should().HaveCount(3);
            resultado.Should().OnlyHaveUniqueItems(u => u.Id);
        }
    }
}
