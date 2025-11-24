using AutoMapper;
using FluentAssertions;
using FitRank_API.Application.CasosDeUso.AsistenciaCasosDeUso;
using FitRank_API.Application.Mappings;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using Moq;

namespace FitRank_API.Tests.CasosDeUsoTests.AsistenciaCasosDeUsoTests
{
    public class ObtenerAsistenciasPorUsuarioCasoDeUsoTests
    {
        private readonly Mock<IAsistenciaRepositorio> _mockRepositorio;
        private readonly IMapper _mapper;
        private readonly ObtenerAsistenciasPorUsuarioCasoDeUso _casoDeUso;

        public ObtenerAsistenciasPorUsuarioCasoDeUsoTests()
        {
            _mockRepositorio = new Mock<IAsistenciaRepositorio>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AsistenciaProfile>();
            });
            _mapper = config.CreateMapper();
            _casoDeUso = new ObtenerAsistenciasPorUsuarioCasoDeUso(_mockRepositorio.Object, _mapper);
        }

        [Fact]
        public async Task DeberiaRetornarAsistenciasDelUsuario()
        {
            // Arrange
            var usuarioId = 1;
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

            _mockRepositorio.Setup(r => r.ObtenerPorUsuarioAsync(usuarioId))
                .ReturnsAsync(asistencias);

            // Act
            var resultado = await _casoDeUso.Ejecutar(usuarioId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(2);
            resultado.First().AsistenciaId.Should().Be(1);
            resultado.Last().AsistenciaId.Should().Be(2);
            _mockRepositorio.Verify(r => r.ObtenerPorUsuarioAsync(usuarioId), Times.Once);
        }

        [Fact]
        public async Task DeberiaRetornarListaVaciaCuandoNoHayAsistencias()
        {
            // Arrange
            var usuarioId = 1;
            var asistenciasVacias = new List<Asistencia>();

            _mockRepositorio.Setup(r => r.ObtenerPorUsuarioAsync(usuarioId))
                .ReturnsAsync(asistenciasVacias);

            // Act
            var resultado = await _casoDeUso.Ejecutar(usuarioId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();
            _mockRepositorio.Verify(r => r.ObtenerPorUsuarioAsync(usuarioId), Times.Once);
        }

        [Fact]
        public async Task DeberiaRetornarListaVaciaCuandoAsistenciasEsNull()
        {
            // Arrange
            var usuarioId = 1;

            _mockRepositorio.Setup(r => r.ObtenerPorUsuarioAsync(usuarioId))
                .ReturnsAsync((List<Asistencia>)null);

            // Act
            var resultado = await _casoDeUso.Ejecutar(usuarioId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();
            _mockRepositorio.Verify(r => r.ObtenerPorUsuarioAsync(usuarioId), Times.Once);
        }
    }
}
