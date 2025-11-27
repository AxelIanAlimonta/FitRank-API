using AutoMapper;
using FluentAssertions;
using FitRank_API.Application.CasosDeUso.AsistenciaCasosDeUso;
using FitRank_API.Application.Mappings;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using Moq;
using Xunit;

namespace FitRank_API.Tests.CasosDeUsoTests.AsistenciaCasosDeUsoTests
{
    public class ObtenerAsistenciasDetalladasPorUsuarioCasoDeUsoTests
    {
        private readonly Mock<IAsistenciaRepositorio> _mockAsistenciaRepo;
        private readonly Mock<IUsuarioRepositorio> _mockUsuarioRepo;
        private readonly IMapper _mapper;
        private readonly ObtenerAsistenciasDetalladasPorUsuarioCasoDeUso _casoDeUso;

        public ObtenerAsistenciasDetalladasPorUsuarioCasoDeUsoTests()
        {
            _mockAsistenciaRepo = new Mock<IAsistenciaRepositorio>();
            _mockUsuarioRepo = new Mock<IUsuarioRepositorio>();
            
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AsistenciaProfile>();
                cfg.AddProfile<UsuarioProfile>();
                cfg.AddProfile<SocioProfile>();
            });
            _mapper = config.CreateMapper();
            
            _casoDeUso = new ObtenerAsistenciasDetalladasPorUsuarioCasoDeUso(
                _mockAsistenciaRepo.Object,
                _mockUsuarioRepo.Object,
                _mapper);
        }

        [Fact]
        public async Task DeberiaRetornarDetalleCompletoDelSocio()
        {
            // Arrange
            var usuarioId = 1L;
            var gimnasio = new Gimnasio { Id = 10, Nombre = "Gimnasio Test" };
            
            var socio = new Socio
            {
                Id = usuarioId,
                Nombre = "Juan",
                Apellido = "Perez",
                Email = "juan@test.com",
                Telefono = "123456789",
                GimnasioId = 10,
                Gimnasio = gimnasio
            };

            var asistencias = new List<Asistencia>
            {
                new Asistencia 
                { 
                    Id = 1, 
                    UsuarioId = usuarioId, 
                    Fecha = new DateTime(2024, 1, 1),
                    Presente = true,
                    HoraEntrada = new DateTime(2024, 1, 1, 10, 0, 0)
                },
                new Asistencia 
                { 
                    Id = 2, 
                    UsuarioId = usuarioId, 
                    Fecha = new DateTime(2024, 1, 2),
                    Presente = true,
                    HoraEntrada = new DateTime(2024, 1, 2, 11, 0, 0)
                }
            };

            _mockUsuarioRepo.Setup(r => r.ObtenerSocioConGimnasioPorIdAsync(usuarioId))
                .ReturnsAsync(socio);

            _mockAsistenciaRepo.Setup(r => r.ObtenerPorUsuarioAsync(usuarioId))
                .ReturnsAsync(asistencias);

            // Act
            var resultado = await _casoDeUso.Ejecutar(usuarioId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Exito.Should().BeTrue();
            resultado.Mensaje.Should().Be("Detalle obtenido correctamente.");
            resultado.Socio.Should().NotBeNull();
            resultado.Socio.Nombre.Should().Be("Juan");
            resultado.Socio.Apellido.Should().Be("Perez");
            resultado.Asistencias.Should().HaveCount(2);
            
            _mockUsuarioRepo.Verify(r => r.ObtenerSocioConGimnasioPorIdAsync(usuarioId), Times.Once);
            _mockAsistenciaRepo.Verify(r => r.ObtenerPorUsuarioAsync(usuarioId), Times.Once);
        }

        [Fact]
        public async Task DeberiaRetornarErrorCuandoSocioNoExiste()
        {
            // Arrange
            var usuarioId = 999L;

            _mockUsuarioRepo.Setup(r => r.ObtenerSocioConGimnasioPorIdAsync(usuarioId))
                .ReturnsAsync((Socio?)null);

            // Act
            var resultado = await _casoDeUso.Ejecutar(usuarioId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Exito.Should().BeFalse();
            resultado.Mensaje.Should().Be("No se encontró el socio solicitado.");
            resultado.Socio.Should().BeNull();
            resultado.Asistencias.Should().BeEmpty();
            
            _mockUsuarioRepo.Verify(r => r.ObtenerSocioConGimnasioPorIdAsync(usuarioId), Times.Once);
            _mockAsistenciaRepo.Verify(r => r.ObtenerPorUsuarioAsync(It.IsAny<long>()), Times.Never);
        }

        [Fact]
        public async Task DeberiaRetornarSocioSinAsistencias()
        {
            // Arrange
            var usuarioId = 1L;
            var gimnasio = new Gimnasio { Id = 10, Nombre = "Gimnasio Test" };
            
            var socio = new Socio
            {
                Id = usuarioId,
                Nombre = "Maria",
                Apellido = "Lopez",
                Email = "maria@test.com",
                GimnasioId = 10,
                Gimnasio = gimnasio
            };

            var asistenciasVacias = new List<Asistencia>();

            _mockUsuarioRepo.Setup(r => r.ObtenerSocioConGimnasioPorIdAsync(usuarioId))
                .ReturnsAsync(socio);

            _mockAsistenciaRepo.Setup(r => r.ObtenerPorUsuarioAsync(usuarioId))
                .ReturnsAsync(asistenciasVacias);

            // Act
            var resultado = await _casoDeUso.Ejecutar(usuarioId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Exito.Should().BeTrue();
            resultado.Socio.Should().NotBeNull();
            resultado.Socio.Nombre.Should().Be("Maria");
            resultado.Asistencias.Should().NotBeNull();
            resultado.Asistencias.Should().BeEmpty();
        }

        [Fact]
        public async Task DeberiaMostrarInformacionDelGimnasio()
        {
            // Arrange
            var usuarioId = 1L;
            var gimnasio = new Gimnasio 
            { 
                Id = 10, 
                Nombre = "FitGym",
                Direccion = "Calle Test 123",
                Telefono = "555-1234"
            };
            
            var socio = new Socio
            {
                Id = usuarioId,
                Nombre = "Carlos",
                Apellido = "Gomez",
                Email = "carlos@test.com",
                GimnasioId = 10,
                Gimnasio = gimnasio
            };

            _mockUsuarioRepo.Setup(r => r.ObtenerSocioConGimnasioPorIdAsync(usuarioId))
                .ReturnsAsync(socio);

            _mockAsistenciaRepo.Setup(r => r.ObtenerPorUsuarioAsync(usuarioId))
                .ReturnsAsync(new List<Asistencia>());

            // Act
            var resultado = await _casoDeUso.Ejecutar(usuarioId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Exito.Should().BeTrue();
            resultado.Socio.Should().NotBeNull();
            resultado.Socio.GimnasioId.Should().Be(10);
        }

        [Fact]
        public async Task DeberiaMostrarAsistenciasConDetalles()
        {
            // Arrange
            var usuarioId = 1L;
            var gimnasio = new Gimnasio { Id = 10, Nombre = "Gimnasio Test" };
            
            var socio = new Socio
            {
                Id = usuarioId,
                Nombre = "Ana",
                Apellido = "Martinez",
                Email = "ana@test.com",
                GimnasioId = 10,
                Gimnasio = gimnasio
            };

            var asistencias = new List<Asistencia>
            {
                new Asistencia 
                { 
                    Id = 1, 
                    UsuarioId = usuarioId, 
                    Fecha = new DateTime(2024, 1, 1),
                    Presente = true,
                    HoraEntrada = new DateTime(2024, 1, 1, 10, 0, 0),
                    HoraSalida = new DateTime(2024, 1, 1, 12, 0, 0),
                    Observaciones = "Entrenamiento completo"
                }
            };

            _mockUsuarioRepo.Setup(r => r.ObtenerSocioConGimnasioPorIdAsync(usuarioId))
                .ReturnsAsync(socio);

            _mockAsistenciaRepo.Setup(r => r.ObtenerPorUsuarioAsync(usuarioId))
                .ReturnsAsync(asistencias);

            // Act
            var resultado = await _casoDeUso.Ejecutar(usuarioId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Exito.Should().BeTrue();
            resultado.Asistencias.Should().HaveCount(1);
            resultado.Asistencias[0].Fecha.Should().Be(new DateTime(2024, 1, 1));
        }
    }
}
