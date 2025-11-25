using AutoMapper;
using FluentAssertions;
using FitRank_API.Application.CasosDeUso.AsistenciaCasosDeUso;
using FitRank_API.Application.Mappings;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using Moq;

namespace FitRank_API.Tests.CasosDeUsoTests.AsistenciaCasosDeUsoTests
{
    public class ObtenerTodasLasAsistenciasCasoDeUsoTests
    {
        private readonly Mock<IAsistenciaRepositorio> _mockRepositorio;
        private readonly IMapper _mapper;
        private readonly ObtenerTodasLasAsistenciasCasoDeUso _casoDeUso;

        public ObtenerTodasLasAsistenciasCasoDeUsoTests()
        {
            _mockRepositorio = new Mock<IAsistenciaRepositorio>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AsistenciaProfile>();
                cfg.AddProfile<UsuarioProfile>();
            });
            _mapper = config.CreateMapper();
            _casoDeUso = new ObtenerTodasLasAsistenciasCasoDeUso(_mockRepositorio.Object, _mapper);
        }

        [Fact]
        public async Task DeberiaRetornarTodasLasAsistencias()
        {
            // Arrange
            var gimnasio = new Gimnasio { Id = 1, Nombre = "Gimnasio Test" };
            var usuario = new Socio 
            { 
                Id = 1, 
                Nombre = "Juan", 
                Apellido = "Perez", 
                Email = "juan@test.com",
                Gimnasio = gimnasio,
                GimnasioId = 1
            };

            var asistencias = new List<Asistencia>
            {
                new Asistencia 
                { 
                    Id = 1, 
                    UsuarioId = 1, 
                    Usuario = usuario, 
                    Fecha = new DateTime(2024, 1, 1), 
                    Presente = true,
                    HoraEntrada = new DateTime(2024, 1, 1, 10, 0, 0),
                    GimnasioId = 1,
                    Gimnasio = gimnasio
                },
                new Asistencia 
                { 
                    Id = 2, 
                    UsuarioId = 1, 
                    Usuario = usuario, 
                    Fecha = new DateTime(2024, 1, 2), 
                    Presente = true,
                    HoraEntrada = new DateTime(2024, 1, 2, 11, 0, 0),
                    GimnasioId = 1,
                    Gimnasio = gimnasio
                }
            };

            _mockRepositorio.Setup(r => r.ObtenerTodasConUsuarioAsync())
                .ReturnsAsync(asistencias);

            // Act
            var resultado = await _casoDeUso.Ejecutar();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(2);
            resultado.First().NombreSocio.Should().Be("Juan Perez");
            _mockRepositorio.Verify(r => r.ObtenerTodasConUsuarioAsync(), Times.Once);
        }

        [Fact]
        public async Task DeberiaRetornarListaVaciaCuandoNoHayAsistencias()
        {
            // Arrange
            var asistenciasVacias = new List<Asistencia>();

            _mockRepositorio.Setup(r => r.ObtenerTodasConUsuarioAsync())
                .ReturnsAsync(asistenciasVacias);

            // Act
            var resultado = await _casoDeUso.Ejecutar();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();
            _mockRepositorio.Verify(r => r.ObtenerTodasConUsuarioAsync(), Times.Once);
        }
    }
}
