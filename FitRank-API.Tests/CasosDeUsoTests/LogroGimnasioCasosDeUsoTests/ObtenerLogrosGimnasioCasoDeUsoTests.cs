using AutoMapper;
using FluentAssertions;
using FitRank_API.Application.CasosDeUso.LogroGimnasioCasosDeUso;
using FitRank_API.Application.Mappings;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using Moq;

namespace FitRank_API.Tests.CasosDeUsoTests.LogroGimnasioCasosDeUsoTests
{
    public class ObtenerLogrosGimnasioCasoDeUsoTests
    {
        private readonly Mock<ILogroGimnasioRepositorio> _mockRepositorio;
        private readonly IMapper _mapper;
        private readonly ObtenerLogrosGimnasioCasoDeUso _casoDeUso;

        public ObtenerLogrosGimnasioCasoDeUsoTests()
        {
            _mockRepositorio = new Mock<ILogroGimnasioRepositorio>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<LogroProfile>();
            });
            _mapper = config.CreateMapper();
            _casoDeUso = new ObtenerLogrosGimnasioCasoDeUso(_mockRepositorio.Object, _mapper);
        }

        [Fact]
        public async Task DeberiaRetornarTodosLosLogrosDeUnGimnasio()
        {
            // Arrange
            var gimnasioId = 1L;
            var logrosGimnasio = new List<LogroGimnasio>
            {
                new LogroGimnasio 
                { 
                    Id = 1, 
                    GimnasioId = gimnasioId, 
                    LogroId = 10, 
                    EstaActivo = true,
                    Logro = new Logro 
                    { 
                        Id = 10, 
                        Nombre = "Logro 1", 
                        NombreClave = "logro_1", 
                        Descripcion = "Desc 1", 
                        Imagen = "img1.png" 
                    }
                },
                new LogroGimnasio 
                { 
                    Id = 2, 
                    GimnasioId = gimnasioId, 
                    LogroId = 20, 
                    EstaActivo = false,
                    Logro = new Logro 
                    { 
                        Id = 20, 
                        Nombre = "Logro 2", 
                        NombreClave = "logro_2", 
                        Descripcion = "Desc 2", 
                        Imagen = "img2.png" 
                    }
                }
            };

            _mockRepositorio.Setup(r => r.ObtenerPorGimnasioAsync(gimnasioId))
                .ReturnsAsync(logrosGimnasio);

            // Act
            var resultado = await _casoDeUso.Ejecutar(gimnasioId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(2);
            resultado.First().LogroId.Should().Be(10);
            resultado.Last().LogroId.Should().Be(20);
            _mockRepositorio.Verify(r => r.ObtenerPorGimnasioAsync(gimnasioId), Times.Once);
        }

        [Fact]
        public async Task DeberiaRetornarListaVaciaCuandoNoHayLogros()
        {
            // Arrange
            var gimnasioId = 1L;
            var logrosVacios = new List<LogroGimnasio>();

            _mockRepositorio.Setup(r => r.ObtenerPorGimnasioAsync(gimnasioId))
                .ReturnsAsync(logrosVacios);

            // Act
            var resultado = await _casoDeUso.Ejecutar(gimnasioId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();
            _mockRepositorio.Verify(r => r.ObtenerPorGimnasioAsync(gimnasioId), Times.Once);
        }
    }
}
