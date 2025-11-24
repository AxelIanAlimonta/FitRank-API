using FitRank_API.Application.CasosDeUso.CalculoPuntajeCasosDeUso;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace FitRank_API.Tests.CasosDeUsoTests.CalculoPuntajeCasosDeUsoTests
{
    public class ObtenerPuntajePorGrupoMuscularSocioCasoDeUsoTests
    {
        private readonly Mock<ISocioRepositorio> _mockSocioRepo;
        private readonly ObtenerPuntajePorGrupoMuscularSocioCasoDeUso _casoDeUso;

        public ObtenerPuntajePorGrupoMuscularSocioCasoDeUsoTests()
        {
            _mockSocioRepo = new Mock<ISocioRepositorio>();
            _casoDeUso = new ObtenerPuntajePorGrupoMuscularSocioCasoDeUso(_mockSocioRepo.Object);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarPuntajesPorGrupoMuscular_CuandoHayActividades()
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
                            new Actividad
                            {
                                Id = 1,
                                Punto = 10,
                                Serie = new Serie
                                {
                                    EjercicioAsignado = new EjercicioAsignado
                                    {
                                        Ejercicio = new Ejercicio
                                        {
                                            GrupoMuscular = new GrupoMuscular { Nombre = "Pecho" }
                                        }
                                    }
                                }
                            },
                            new Actividad
                            {
                                Id = 2,
                                Punto = 15,
                                Serie = new Serie
                                {
                                    EjercicioAsignado = new EjercicioAsignado
                                    {
                                        Ejercicio = new Ejercicio
                                        {
                                            GrupoMuscular = new GrupoMuscular { Nombre = "Pecho" }
                                        }
                                    }
                                }
                            },
                            new Actividad
                            {
                                Id = 3,
                                Punto = 20,
                                Serie = new Serie
                                {
                                    EjercicioAsignado = new EjercicioAsignado
                                    {
                                        Ejercicio = new Ejercicio
                                        {
                                            GrupoMuscular = new GrupoMuscular { Nombre = "Espalda" }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            _mockSocioRepo.Setup(r => r.ObtenerSocioConEntrenamientosAsync(1))
                .ReturnsAsync(socio);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(2);
            resultado["Pecho"].Should().Be(25);
            resultado["Espalda"].Should().Be(20);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarDiccionarioVacio_CuandoSocioNoExiste()
        {
            // Arrange
            _mockSocioRepo.Setup(r => r.ObtenerSocioConEntrenamientosAsync(999))
                .ReturnsAsync((Socio?)null);

            // Act
            var resultado = await _casoDeUso.Ejecutar(999);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarDiccionarioVacio_CuandoNoHayEntrenamientos()
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
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();
        }

        [Fact]
        public async Task Ejecutar_DebeAgruparCorrectamentePuntajesPorGrupo()
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
                            new Actividad { Id = 1, Punto = 5, Serie = new Serie { EjercicioAsignado = new EjercicioAsignado { Ejercicio = new Ejercicio { GrupoMuscular = new GrupoMuscular { Nombre = "Brazos" } } } } },
                            new Actividad { Id = 2, Punto = 10, Serie = new Serie { EjercicioAsignado = new EjercicioAsignado { Ejercicio = new Ejercicio { GrupoMuscular = new GrupoMuscular { Nombre = "Brazos" } } } } },
                            new Actividad { Id = 3, Punto = 15, Serie = new Serie { EjercicioAsignado = new EjercicioAsignado { Ejercicio = new Ejercicio { GrupoMuscular = new GrupoMuscular { Nombre = "Brazos" } } } } }
                        }
                    }
                }
            };

            _mockSocioRepo.Setup(r => r.ObtenerSocioConEntrenamientosAsync(1))
                .ReturnsAsync(socio);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1);

            // Assert
            resultado.Should().HaveCount(1);
            resultado["Brazos"].Should().Be(30);
        }

        [Fact]
        public async Task Ejecutar_DebeUsarDesconocido_CuandoGrupoMuscularEsNull()
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
                            new Actividad
                            {
                                Id = 1,
                                Punto = 10,
                                Serie = new Serie
                                {
                                    EjercicioAsignado = new EjercicioAsignado
                                    {
                                        Ejercicio = new Ejercicio
                                        {
                                            GrupoMuscular = null
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            _mockSocioRepo.Setup(r => r.ObtenerSocioConEntrenamientosAsync(1))
                .ReturnsAsync(socio);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1);

            // Assert
            resultado.Should().ContainKey("Desconocido");
            resultado["Desconocido"].Should().Be(10);
        }

        [Fact]
        public async Task Ejecutar_DebeManjejarPuntosNull()
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
                            new Actividad
                            {
                                Id = 1,
                                Punto = null,
                                Serie = new Serie
                                {
                                    EjercicioAsignado = new EjercicioAsignado
                                    {
                                        Ejercicio = new Ejercicio
                                        {
                                            GrupoMuscular = new GrupoMuscular { Nombre = "Piernas" }
                                        }
                                    }
                                }
                            },
                            new Actividad
                            {
                                Id = 2,
                                Punto = 20,
                                Serie = new Serie
                                {
                                    EjercicioAsignado = new EjercicioAsignado
                                    {
                                        Ejercicio = new Ejercicio
                                        {
                                            GrupoMuscular = new GrupoMuscular { Nombre = "Piernas" }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            _mockSocioRepo.Setup(r => r.ObtenerSocioConEntrenamientosAsync(1))
                .ReturnsAsync(socio);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1);

            // Assert
            resultado["Piernas"].Should().Be(20);
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
                            new Actividad { Id = 1, Punto = 10, Serie = new Serie { EjercicioAsignado = new EjercicioAsignado { Ejercicio = new Ejercicio { GrupoMuscular = new GrupoMuscular { Nombre = "Abdomen" } } } } }
                        }
                    },
                    new Entrenamiento
                    {
                        Id = 2,
                        Actividades = new List<Actividad>
                        {
                            new Actividad { Id = 2, Punto = 15, Serie = new Serie { EjercicioAsignado = new EjercicioAsignado { Ejercicio = new Ejercicio { GrupoMuscular = new GrupoMuscular { Nombre = "Abdomen" } } } } }
                        }
                    }
                }
            };

            _mockSocioRepo.Setup(r => r.ObtenerSocioConEntrenamientosAsync(1))
                .ReturnsAsync(socio);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1);

            // Assert
            resultado["Abdomen"].Should().Be(25);
        }
    }
}
