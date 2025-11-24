using AutoMapper;
using FluentAssertions;
using FitRank_API.Application.CasosDeUso.LogroSocioCasosDeUso;
using FitRank_API.Application.Mappings;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using Moq;

namespace FitRank_API.Tests.CasosDeUsoTests.LogroSocioCasosDeUsoTests
{
    public class ObtenerLogrosSocioCasoDeUsoTests
    {
        private readonly Mock<ILogroSocioRepositorio> _mockRepositorio;
        private readonly IMapper _mapper;
        private readonly ObtenerLogrosSocioCasoDeUso _casoDeUso;

        public ObtenerLogrosSocioCasoDeUsoTests()
        {
            _mockRepositorio = new Mock<ILogroSocioRepositorio>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<LogroProfile>();
            });
            _mapper = config.CreateMapper();
            _casoDeUso = new ObtenerLogrosSocioCasoDeUso(_mockRepositorio.Object, _mapper);
        }

        [Fact]
        public async Task DeberiaRetornarTodosLosLogrosDeUnSocioEnUnGimnasio()
        {
            // Arrange
            var socioId = 1;
            var gimnasioId = 2;
            var logrosSocio = new List<LogroSocio>
            {
                new LogroSocio 
                { 
                    Id = 1, 
                    SocioId = socioId, 
                    GimnasioId = gimnasioId, 
                    LogroId = 10,
                    FechaObtenido = new DateTime(2024, 1, 1),
                    Logro = new Logro 
                    { 
                        Id = 10, 
                        Nombre = "Logro Test 1", 
                        NombreClave = "logro_test_1", 
                        Descripcion = "Descripción 1", 
                        Imagen = "img1.png" 
                    }
                },
                new LogroSocio 
                { 
                    Id = 2, 
                    SocioId = socioId, 
                    GimnasioId = gimnasioId, 
                    LogroId = 20,
                    FechaObtenido = new DateTime(2024, 2, 1),
                    Logro = new Logro 
                    { 
                        Id = 20, 
                        Nombre = "Logro Test 2", 
                        NombreClave = "logro_test_2", 
                        Descripcion = "Descripción 2", 
                        Imagen = "img2.png" 
                    }
                }
            };

            _mockRepositorio.Setup(r => r.ObtenerPorSocioYGimnasioAsync(socioId, gimnasioId))
                .ReturnsAsync(logrosSocio);

            // Act
            var resultado = await _casoDeUso.Ejecutar(socioId, gimnasioId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(2);
            resultado.First().LogroId.Should().Be(10);
            resultado.Last().LogroId.Should().Be(20);
            _mockRepositorio.Verify(r => r.ObtenerPorSocioYGimnasioAsync(socioId, gimnasioId), Times.Once);
        }

        [Fact]
        public async Task DeberiaRetornarListaVaciaCuandoNoHayLogros()
        {
            // Arrange
            var socioId = 1;
            var gimnasioId = 2;
            var logrosVacios = new List<LogroSocio>();

            _mockRepositorio.Setup(r => r.ObtenerPorSocioYGimnasioAsync(socioId, gimnasioId))
                .ReturnsAsync(logrosVacios);

            // Act
            var resultado = await _casoDeUso.Ejecutar(socioId, gimnasioId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();
            _mockRepositorio.Verify(r => r.ObtenerPorSocioYGimnasioAsync(socioId, gimnasioId), Times.Once);
        }
    }
}
