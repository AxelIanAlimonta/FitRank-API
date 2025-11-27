using AutoMapper;
using FitRank_API.Application.CasosDeUso.SocioCasoDeUso;
using FitRank_API.Application.DTOs.SocioDTOs;
using FitRank_API.Application.Interfaces;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace FitRank_API.Tests.CasosDeUsoTests.SocioCasosDeUsoTests
{
    public class AgregarSocioCasoDeUsoTests
    {
        private readonly Mock<ISocioRepositorio> _mockRepo;
        private readonly Mock<IPasswordService> _mockPasswordService;
        private readonly IMapper _mapper;
        private readonly AgregarSocioCasoDeUso _casoDeUso;

        public AgregarSocioCasoDeUsoTests()
        {
            _mockRepo = new Mock<ISocioRepositorio>();
            _mockPasswordService = new Mock<IPasswordService>();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<AgregarSocioDTO, Socio>();
                cfg.CreateMap<Socio, SocioDTO>();
            });
            _mapper = config.CreateMapper();

            _casoDeUso = new AgregarSocioCasoDeUso(_mockRepo.Object, _mapper, _mockPasswordService.Object);
        }

        [Fact]
        public async Task Ejecutar_DebeAgregarSocioCorrectamente()
        {
            // Arrange
            var dto = new AgregarSocioDTO
            {
                NombreUsuario = "juanperez",
                Email = "juan@test.com",
                Password = "Password123!",
                Telefono = "123456789",
                Sexo = "Masculino",
                Altura = 175,
                Peso = 75,
                Nivel = "Intermedio"
            };

            var socioCreado = new Socio 
            { 
                Id = 1, 
                Email = dto.Email, 
                NombreUsuario = dto.NombreUsuario,
                Rol = "Socio",
                EsActivado = true
            };
            _mockRepo.Setup(r => r.AgregarAsync(It.IsAny<Socio>())).ReturnsAsync(socioCreado);
            _mockPasswordService.Setup(p => p.HashPassword(dto.Password)).Returns("hashed_password");

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.NombreUsuario.Should().Be(dto.NombreUsuario);
            _mockRepo.Verify(r => r.AgregarAsync(It.Is<Socio>(s =>
                s.Rol == "Socio" &&
                s.EsActivado == true &&
                !string.IsNullOrEmpty(s.PasswordHash)
            )), Times.Once);
        }

        [Fact]
        public async Task Ejecutar_DebeHashearPassword()
        {
            // Arrange
            var dto = new AgregarSocioDTO 
            { 
                Password = "MiPassword123!", 
                Email = "test@test.com",
                NombreUsuario = "testuser"
            };
            Socio? socioGuardado = null;
            _mockRepo.Setup(r => r.AgregarAsync(It.IsAny<Socio>()))
                .Callback<Socio>(s => socioGuardado = s)
                .ReturnsAsync((Socio s) => s);
            _mockPasswordService.Setup(p => p.HashPassword(dto.Password)).Returns("hashed_socio_password");

            // Act
            await _casoDeUso.Ejecutar(dto);

            // Assert
            socioGuardado.Should().NotBeNull();
            socioGuardado!.PasswordHash.Should().Be("hashed_socio_password");
            _mockPasswordService.Verify(p => p.HashPassword(dto.Password), Times.Once);
        }

        [Fact]
        public async Task Ejecutar_DebeAsignarRolSocio()
        {
            // Arrange
            var dto = new AgregarSocioDTO 
            { 
                Email = "test@test.com", 
                Password = "Pass123!",
                NombreUsuario = "testuser"
            };
            Socio? socioGuardado = null;
            _mockRepo.Setup(r => r.AgregarAsync(It.IsAny<Socio>()))
                .Callback<Socio>(s => socioGuardado = s)
                .ReturnsAsync((Socio s) => s);
            _mockPasswordService.Setup(p => p.HashPassword(dto.Password)).Returns("hashed_password");

            // Act
            await _casoDeUso.Ejecutar(dto);

            // Assert
            socioGuardado!.Rol.Should().Be("Socio");
        }

        [Fact]
        public async Task Ejecutar_DebeMarcarComoActivado()
        {
            // Arrange
            var dto = new AgregarSocioDTO 
            { 
                Email = "test@test.com", 
                Password = "Pass123!",
                NombreUsuario = "testuser"
            };
            Socio? socioGuardado = null;
            _mockRepo.Setup(r => r.AgregarAsync(It.IsAny<Socio>()))
                .Callback<Socio>(s => socioGuardado = s)
                .ReturnsAsync((Socio s) => s);
            _mockPasswordService.Setup(p => p.HashPassword(dto.Password)).Returns("hashed_password");

            // Act
            await _casoDeUso.Ejecutar(dto);

            // Assert
            socioGuardado!.EsActivado.Should().BeTrue();
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarSocioDTO()
        {
            // Arrange
            var dto = new AgregarSocioDTO 
            { 
                NombreUsuario = "testuser",
                Email = "test@test.com", 
                Password = "Pass123!" 
            };
            var socioCreado = new Socio 
            { 
                Id = 10, 
                NombreUsuario = "testuser", 
                Email = "test@test.com" 
            };
            _mockRepo.Setup(r => r.AgregarAsync(It.IsAny<Socio>())).ReturnsAsync(socioCreado);
            _mockPasswordService.Setup(p => p.HashPassword(dto.Password)).Returns("hashed_password");

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Id.Should().Be(10);
            resultado.NombreUsuario.Should().Be("testuser");
        }
    }
}
