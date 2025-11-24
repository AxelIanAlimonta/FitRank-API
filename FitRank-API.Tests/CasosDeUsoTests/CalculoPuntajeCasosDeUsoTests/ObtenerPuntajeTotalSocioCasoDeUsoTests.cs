using FitRank_API.Application.CasosDeUso.CalculoPuntajeCasosDeUso;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace FitRank_API.Tests.CasosDeUsoTests.CalculoPuntajeCasosDeUsoTests
{
    public class ObtenerPuntajeTotalSocioCasoDeUsoTests
    {
        private readonly Mock<ISocioRepositorio> _mockSocioRepo;
        private readonly ObtenerPuntajeTotalSocioCasoDeUso _casoDeUso;

        public ObtenerPuntajeTotalSocioCasoDeUsoTests()
        {
            _mockSocioRepo = new Mock<ISocioRepositorio>();
            _casoDeUso = new ObtenerPuntajeTotalSocioCasoDeUso(_mockSocioRepo.Object);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarPuntajeTotal_CuandoSocioTieneActividades()
        {
            // Arrange
            var socio = new Socio
            {
                Id = 1,
                Nombre = "Juan",
                Entrenamientos = new List<Entrenamiento>
                {
                    new Entrenamiento
                    {
                        Id = 1,
                        Actividades = new List<Actividad>
                        {
                            new Actividad { Id = 1, Punto = 10 },
                            new Actividad { Id = 2, Punto = 15 },
                            new Actividad { Id = 3, Punto = 20 }
                        }
                    }
                }
            };

            _mockSocioRepo.Setup(r => r.ObtenerSocioConEntrenamientosAsync(1))
                .ReturnsAsync(socio);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1);

            // Assert
            resultado.Should().Be(45);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarCero_CuandoSocioNoExiste()
        {
            // Arrange
            _mockSocioRepo.Setup(r => r.ObtenerSocioConEntrenamientosAsync(999))
                .ReturnsAsync((Socio?)null);

            // Act
            var resultado = await _casoDeUso.Ejecutar(999);

            // Assert
            resultado.Should().Be(0);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarCero_CuandoNoHayEntrenamientos()
        {
            // Arrange
            var socio = new Socio
            {
                Id = 1,
                Nombre = "Juan",
                Entrenamientos = null
            };

            _mockSocioRepo.Setup(r => r.ObtenerSocioConEntrenamientosAsync(1))
                .ReturnsAsync(socio);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1);

            // Assert
            resultado.Should().Be(0);
        }

        [Fact]
        public async Task Ejecutar_DebeSumarPuntajesDeMultiplesEntrenamientos()
        {
            // Arrange
            var socio = new Socio
            {
                Id = 1,
                Nombre = "Juan",
                Entrenamientos = new List<Entrenamiento>
                {
                    new Entrenamiento
                    {
                        Id = 1,
                        Actividades = new List<Actividad>
                        {
                            new Actividad { Id = 1, Punto = 10 },
                            new Actividad { Id = 2, Punto = 15 }
                        }
                    },
                    new Entrenamiento
                    {
                        Id = 2,
                        Actividades = new List<Actividad>
                        {
                            new Actividad { Id = 3, Punto = 20 },
                            new Actividad { Id = 4, Punto = 25 }
                        }
                    }
                }
            };

            _mockSocioRepo.Setup(r => r.ObtenerSocioConEntrenamientosAsync(1))
                .ReturnsAsync(socio);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1);

            // Assert
            resultado.Should().Be(70);
        }

        [Fact]
        public async Task Ejecutar_DebeIgnorarPuntosNull()
        {
            // Arrange
            var socio = new Socio
            {
                Id = 1,
                Nombre = "Juan",
                Entrenamientos = new List<Entrenamiento>
                {
                    new Entrenamiento
                    {
                        Id = 1,
                        Actividades = new List<Actividad>
                        {
                            new Actividad { Id = 1, Punto = null },
                            new Actividad { Id = 2, Punto = 10 },
                            new Actividad { Id = 3, Punto = null },
                            new Actividad { Id = 4, Punto = 20 }
                        }
                    }
                }
            };

            _mockSocioRepo.Setup(r => r.ObtenerSocioConEntrenamientosAsync(1))
                .ReturnsAsync(socio);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1);

            // Assert
            resultado.Should().Be(30);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarCero_CuandoTodasLasActividadesTienenPuntosNull()
        {
            // Arrange
            var socio = new Socio
            {
                Id = 1,
                Nombre = "Juan",
                Entrenamientos = new List<Entrenamiento>
                {
                    new Entrenamiento
                    {
                        Id = 1,
                        Actividades = new List<Actividad>
                        {
                            new Actividad { Id = 1, Punto = null },
                            new Actividad { Id = 2, Punto = null }
                        }
                    }
                }
            };

            _mockSocioRepo.Setup(r => r.ObtenerSocioConEntrenamientosAsync(1))
                .ReturnsAsync(socio);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1);

            // Assert
            resultado.Should().Be(0);
        }

        [Fact]
        public async Task Ejecutar_DebeManjejarEntrenamientosSinActividades()
        {
            // Arrange
            var socio = new Socio
            {
                Id = 1,
                Nombre = "Juan",
                Entrenamientos = new List<Entrenamiento>
                {
                    new Entrenamiento
                    {
                        Id = 1,
                        Actividades = new List<Actividad>()
                    },
                    new Entrenamiento
                    {
                        Id = 2,
                        Actividades = new List<Actividad>
                        {
                            new Actividad { Id = 1, Punto = 50 }
                        }
                    }
                }
            };

            _mockSocioRepo.Setup(r => r.ObtenerSocioConEntrenamientosAsync(1))
                .ReturnsAsync(socio);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1);

            // Assert
            resultado.Should().Be(50);
        }

        [Fact]
        public async Task Ejecutar_DebeManejarPuntajesDecimales()
        {
            // Arrange
            var socio = new Socio
            {
                Id = 1,
                Nombre = "Juan",
                Entrenamientos = new List<Entrenamiento>
                {
                    new Entrenamiento
                    {
                        Id = 1,
                        Actividades = new List<Actividad>
                        {
                            new Actividad { Id = 1, Punto = 10.5 },
                            new Actividad { Id = 2, Punto = 15.75 },
                            new Actividad { Id = 3, Punto = 20.25 }
                        }
                    }
                }
            };

            _mockSocioRepo.Setup(r => r.ObtenerSocioConEntrenamientosAsync(1))
                .ReturnsAsync(socio);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1);

            // Assert
            resultado.Should().Be(46.5);
        }
    }
}
