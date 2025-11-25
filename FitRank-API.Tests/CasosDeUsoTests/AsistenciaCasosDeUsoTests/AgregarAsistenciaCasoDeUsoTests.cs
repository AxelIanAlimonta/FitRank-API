using AutoMapper;
using FluentAssertions;
using FitRank_API.Application.CasosDeUso.AsistenciaCasosDeUso;
using FitRank_API.Application.DTOs.Asistencia;
using FitRank_API.Application.Mappings;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using Moq;
using Xunit;

namespace FitRank_API.Tests.CasosDeUsoTests.AsistenciaCasosDeUsoTests
{
    public class AgregarAsistenciaCasoDeUsoTests
    {
        private readonly Mock<IAsistenciaRepositorio> _mockAsistenciaRepo;
        private readonly Mock<IUsuarioRepositorio> _mockUsuarioRepo;
        private readonly IMapper _mapper;
        private readonly AgregarAsistenciaCasoDeUso _casoDeUso;

        public AgregarAsistenciaCasoDeUsoTests()
        {
            _mockAsistenciaRepo = new Mock<IAsistenciaRepositorio>();
            _mockUsuarioRepo = new Mock<IUsuarioRepositorio>();
            
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AsistenciaProfile>();
            });
            _mapper = config.CreateMapper();
            
            _casoDeUso = new AgregarAsistenciaCasoDeUso(
                _mockAsistenciaRepo.Object,
                _mockUsuarioRepo.Object,
                _mapper);
        }

        [Fact]
        public async Task DeberiaRegistrarAsistenciaCorrectamente()
        {
            // Arrange
            var usuario = new Socio
            {
                Id = 1,
                Nombre = "Juan",
                Apellido = "Perez",
                Email = "juan@test.com"
            };

            var dto = new AgregarAsistenciaDTO
            {
                UsuarioId = 1,
                GimnasioId = 10
            };

            Asistencia asistenciaCapturada = null;

            _mockUsuarioRepo.Setup(r => r.ObtenerPorIdAsync(1))
                .ReturnsAsync(usuario);

            _mockAsistenciaRepo.Setup(r => r.AgregarAsync(It.IsAny<Asistencia>()))
                .Callback<Asistencia>(a => asistenciaCapturada = a)
                .ReturnsAsync((Asistencia a) => a);

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Success.Should().BeTrue();
            resultado.Mensaje.Should().Be("Asistencia registrada correctamente.");
            resultado.NombreUsuario.Should().Be("Juan Perez");
            
            asistenciaCapturada.Should().NotBeNull();
            asistenciaCapturada.UsuarioId.Should().Be(1);
            asistenciaCapturada.GimnasioId.Should().Be(10);
            asistenciaCapturada.Presente.Should().BeTrue();
            asistenciaCapturada.Fecha.Date.Should().Be(DateTime.UtcNow.Date);
            
            _mockUsuarioRepo.Verify(r => r.ObtenerPorIdAsync(1), Times.Once);
            _mockAsistenciaRepo.Verify(r => r.AgregarAsync(It.IsAny<Asistencia>()), Times.Once);
        }

        [Fact]
        public async Task DeberiaRetornarErrorCuandoUsuarioNoExiste()
        {
            // Arrange
            var dto = new AgregarAsistenciaDTO
            {
                UsuarioId = 999,
                GimnasioId = 10
            };

            _mockUsuarioRepo.Setup(r => r.ObtenerPorIdAsync(999))
                .ReturnsAsync((Socio)null);

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Success.Should().BeFalse();
            resultado.Mensaje.Should().Be("Usuario no encontrado.");
            
            _mockUsuarioRepo.Verify(r => r.ObtenerPorIdAsync(999), Times.Once);
            _mockAsistenciaRepo.Verify(r => r.AgregarAsync(It.IsAny<Asistencia>()), Times.Never);
        }

        [Fact]
        public async Task DeberiaEstablecerFechaYHoraActual()
        {
            // Arrange
            var usuario = new Socio
            {
                Id = 1,
                Nombre = "Maria",
                Apellido = "Lopez"
            };

            var dto = new AgregarAsistenciaDTO
            {
                UsuarioId = 1,
                GimnasioId = 5
            };

            Asistencia asistenciaCapturada = null;

            _mockUsuarioRepo.Setup(r => r.ObtenerPorIdAsync(1))
                .ReturnsAsync(usuario);

            _mockAsistenciaRepo.Setup(r => r.AgregarAsync(It.IsAny<Asistencia>()))
                .Callback<Asistencia>(a => asistenciaCapturada = a)
                .ReturnsAsync((Asistencia a) => a);

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            asistenciaCapturada.Should().NotBeNull();
            asistenciaCapturada.Fecha.Date.Should().Be(DateTime.UtcNow.Date);
            asistenciaCapturada.HoraEntrada.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            asistenciaCapturada.Presente.Should().BeTrue();
        }

        [Fact]
        public async Task DeberiaAsignarGimnasioCorrectamente()
        {
            // Arrange
            var usuario = new Socio
            {
                Id = 1,
                Nombre = "Carlos",
                Apellido = "Gomez"
            };

            var dto = new AgregarAsistenciaDTO
            {
                UsuarioId = 1,
                GimnasioId = 25
            };

            Asistencia asistenciaCapturada = null;

            _mockUsuarioRepo.Setup(r => r.ObtenerPorIdAsync(1))
                .ReturnsAsync(usuario);

            _mockAsistenciaRepo.Setup(r => r.AgregarAsync(It.IsAny<Asistencia>()))
                .Callback<Asistencia>(a => asistenciaCapturada = a)
                .ReturnsAsync((Asistencia a) => a);

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Success.Should().BeTrue();
            asistenciaCapturada.Should().NotBeNull();
            asistenciaCapturada.GimnasioId.Should().Be(25);
        }

        [Fact]
        public async Task DeberiaCombinarNombreCompletoDelUsuario()
        {
            // Arrange
            var usuario = new Socio
            {
                Id = 1,
                Nombre = "Ana",
                Apellido = "Martinez"
            };

            var dto = new AgregarAsistenciaDTO
            {
                UsuarioId = 1,
                GimnasioId = 10
            };

            _mockUsuarioRepo.Setup(r => r.ObtenerPorIdAsync(1))
                .ReturnsAsync(usuario);

            _mockAsistenciaRepo.Setup(r => r.AgregarAsync(It.IsAny<Asistencia>()))
                .ReturnsAsync((Asistencia a) => a);

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Success.Should().BeTrue();
            resultado.NombreUsuario.Should().Be("Ana Martinez");
        }
    }
}
