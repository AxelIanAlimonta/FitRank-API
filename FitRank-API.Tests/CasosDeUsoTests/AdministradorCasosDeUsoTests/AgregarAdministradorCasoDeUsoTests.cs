using AutoMapper;
using FitRank_API.Application.CasosDeUso.AdministradorCasosDeUso;
using FitRank_API.Application.DTOs.AdministradorDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FluentAssertions;
using Moq;

namespace FitRank_API.Tests.CasosDeUsoTests.AdministradorCasosDeUsoTests
{
    public class AgregarAdministradorCasoDeUsoTests
    {
        private readonly Mock<IAdministradorRepositorio> _mockRepo;
        private readonly IMapper _mapper;
        private readonly AgregarAdministradorCasoDeUso _casoDeUso;

        public AgregarAdministradorCasoDeUsoTests()
        {
            _mockRepo = new Mock<IAdministradorRepositorio>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AgregarAdministradorDTO, Administrador>();
            });
            _mapper = config.CreateMapper();

            _casoDeUso = new AgregarAdministradorCasoDeUso(_mockRepo.Object, _mapper);
        }

        [Fact]
        public async Task Ejecutar_DebeCrearAdministradorCorrectamente()
        {
            // Arrange
            var dto = new AgregarAdministradorDTO
            {
                Nombre = "Admin",
                Apellido = "Principal",
                Email = "admin@test.com",
                Password = "AdminPass123!",
                Dni = 12345678
            };

            _mockRepo.Setup(r => r.AgregarAsync(It.IsAny<Administrador>())).ReturnsAsync((Administrador a) => a);

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Nombre.Should().Be("Admin");
            resultado.Email.Should().Be("admin@test.com");
        }

        [Fact]
        public async Task Ejecutar_DebeAsignarRolAdmin()
        {
            // Arrange
            var dto = new AgregarAdministradorDTO { Email = "test@test.com", Password = "Pass123!" };
            Administrador? adminGuardado = null;

            _mockRepo.Setup(r => r.AgregarAsync(It.IsAny<Administrador>()))
                .Callback<Administrador>(a => adminGuardado = a)
                .ReturnsAsync((Administrador a) => a);

            // Act
            await _casoDeUso.Ejecutar(dto);

            // Assert
            adminGuardado.Should().NotBeNull();
            adminGuardado!.Rol.Should().Be("Admin");
        }

        [Fact]
        public async Task Ejecutar_DebeMarcarComoActivado()
        {
            // Arrange
            var dto = new AgregarAdministradorDTO { Email = "test@test.com", Password = "Pass123!" };
            Administrador? adminGuardado = null;

            _mockRepo.Setup(r => r.AgregarAsync(It.IsAny<Administrador>()))
                .Callback<Administrador>(a => adminGuardado = a)
                .ReturnsAsync((Administrador a) => a);

            // Act
            await _casoDeUso.Ejecutar(dto);

            // Assert
            adminGuardado!.EsActivado.Should().BeTrue();
        }

        [Fact]
        public async Task Ejecutar_DebeHashearPassword()
        {
            // Arrange
            var dto = new AgregarAdministradorDTO
            {
                Email = "admin@test.com",
                Password = "MiPasswordSegura123!"
            };
            Administrador? adminGuardado = null;

            _mockRepo.Setup(r => r.AgregarAsync(It.IsAny<Administrador>()))
                .Callback<Administrador>(a => adminGuardado = a)
                .ReturnsAsync((Administrador a) => a);

            // Act
            await _casoDeUso.Ejecutar(dto);

            // Assert
            adminGuardado.Should().NotBeNull();
            adminGuardado!.PasswordHash.Should().NotBe(dto.Password);
            BCrypt.Net.BCrypt.Verify(dto.Password, adminGuardado.PasswordHash).Should().BeTrue();
        }

        [Fact]
        public async Task Ejecutar_DebeLlamarRepositorioParaAgregar()
        {
            // Arrange
            var dto = new AgregarAdministradorDTO { Email = "test@test.com", Password = "Pass123!" };
            _mockRepo.Setup(r => r.AgregarAsync(It.IsAny<Administrador>())).ReturnsAsync((Administrador a) => a);

            // Act
            await _casoDeUso.Ejecutar(dto);

            // Assert
            _mockRepo.Verify(r => r.AgregarAsync(It.Is<Administrador>(a =>
                a.Rol == "Admin" &&
                a.EsActivado == true &&
                !string.IsNullOrEmpty(a.PasswordHash)
            )), Times.Once);
        }
    }
}
