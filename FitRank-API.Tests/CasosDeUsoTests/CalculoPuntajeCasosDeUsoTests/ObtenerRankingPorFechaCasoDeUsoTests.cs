using AutoMapper;
using FitRank_API.Application.CasosDeUso.CalculoPuntajeCasosDeUso;
using FitRank_API.Application.DTOs.RankingDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace FitRank_API.Tests.CasosDeUsoTests.CalculoPuntajeCasosDeUsoTests
{
    public class ObtenerRankingPorFechaCasoDeUsoTests
    {
        private readonly Mock<ISocioRepositorio> _mockSocioRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ObtenerRankingPorFechaCasoDeUso _casoDeUso;

        public ObtenerRankingPorFechaCasoDeUsoTests()
        {
            _mockSocioRepo = new Mock<ISocioRepositorio>();
            _mockMapper = new Mock<IMapper>();
            _casoDeUso = new ObtenerRankingPorFechaCasoDeUso(_mockSocioRepo.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarRankingOrdenadoPorPuntaje()
        {
            // Arrange
            var desde = new DateOnly(2024, 1, 1);
            var hasta = new DateOnly(2024, 1, 31);

            var socios = new List<Socio>
            {
                new Socio
                {
                    Id = 1,
                    Nombre = "Juan",
                    Apellido = "Pérez",
                    Entrenamientos = new List<Entrenamiento>
                    {
                        new Entrenamiento
                        {
                            Fecha = new DateTime(2024, 1, 15),
                            Actividades = new List<Actividad>
                            {
                                new Actividad { Punto = 100 }
                            }
                        }
                    }
                },
                new Socio
                {
                    Id = 2,
                    Nombre = "María",
                    Apellido = "García",
                    Entrenamientos = new List<Entrenamiento>
                    {
                        new Entrenamiento
                        {
                            Fecha = new DateTime(2024, 1, 20),
                            Actividades = new List<Actividad>
                            {
                                new Actividad { Punto = 150 }
                            }
                        }
                    }
                },
                new Socio
                {
                    Id = 3,
                    Nombre = "Carlos",
                    Apellido = "López",
                    Entrenamientos = new List<Entrenamiento>
                    {
                        new Entrenamiento
                        {
                            Fecha = new DateTime(2024, 1, 10),
                            Actividades = new List<Actividad>
                            {
                                new Actividad { Punto = 50 }
                            }
                        }
                    }
                }
            };

            _mockSocioRepo.Setup(r => r.ObtenerSociosParaRankingAsync(1))
                .ReturnsAsync(socios);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1, 10, desde, hasta);

            // Assert
            resultado.Should().HaveCount(3);
            resultado[0].SocioId.Should().Be(2); // María con 150 puntos
            resultado[0].PuntajeTotal.Should().Be(150);
            resultado[1].SocioId.Should().Be(1); // Juan con 100 puntos
            resultado[2].SocioId.Should().Be(3); // Carlos con 50 puntos
        }

        [Fact]
        public async Task Ejecutar_DebeFiltrarPorRangoDeFechas()
        {
            // Arrange
            var desde = new DateOnly(2024, 1, 15);
            var hasta = new DateOnly(2024, 1, 31);

            var socios = new List<Socio>
            {
                new Socio
                {
                    Id = 1,
                    Nombre = "Juan",
                    Apellido = "Pérez",
                    Entrenamientos = new List<Entrenamiento>
                    {
                        new Entrenamiento
                        {
                            Fecha = new DateTime(2024, 1, 10), // Fuera del rango
                            Actividades = new List<Actividad>
                            {
                                new Actividad { Punto = 100 }
                            }
                        },
                        new Entrenamiento
                        {
                            Fecha = new DateTime(2024, 1, 20), // Dentro del rango
                            Actividades = new List<Actividad>
                            {
                                new Actividad { Punto = 50 }
                            }
                        }
                    }
                }
            };

            _mockSocioRepo.Setup(r => r.ObtenerSociosParaRankingAsync(1))
                .ReturnsAsync(socios);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1, 10, desde, hasta);

            // Assert
            resultado.Should().HaveCount(1);
            resultado[0].PuntajeTotal.Should().Be(50); // Solo el entrenamiento dentro del rango
        }

        [Fact]
        public async Task Ejecutar_DebeLimitarCantidadDeResultados()
        {
            // Arrange
            var desde = new DateOnly(2024, 1, 1);
            var hasta = new DateOnly(2024, 1, 31);

            var socios = new List<Socio>();
            for (int i = 1; i <= 10; i++)
            {
                socios.Add(new Socio
                {
                    Id = i,
                    Nombre = $"Socio{i}",
                    Apellido = $"Apellido{i}",
                    Entrenamientos = new List<Entrenamiento>
                    {
                        new Entrenamiento
                        {
                            Fecha = new DateTime(2024, 1, 15),
                            Actividades = new List<Actividad>
                            {
                                new Actividad { Punto = i * 10 }
                            }
                        }
                    }
                });
            }

            _mockSocioRepo.Setup(r => r.ObtenerSociosParaRankingAsync(1))
                .ReturnsAsync(socios);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1, 5, desde, hasta);

            // Assert
            resultado.Should().HaveCount(5);
        }

        [Fact]
        public async Task Ejecutar_DebeExcluirSociosSinPuntaje()
        {
            // Arrange
            var desde = new DateOnly(2024, 1, 1);
            var hasta = new DateOnly(2024, 1, 31);

            var socios = new List<Socio>
            {
                new Socio
                {
                    Id = 1,
                    Nombre = "Juan",
                    Apellido = "Pérez",
                    Entrenamientos = new List<Entrenamiento>
                    {
                        new Entrenamiento
                        {
                            Fecha = new DateTime(2024, 1, 15),
                            Actividades = new List<Actividad>
                            {
                                new Actividad { Punto = 100 }
                            }
                        }
                    }
                },
                new Socio
                {
                    Id = 2,
                    Nombre = "María",
                    Apellido = "García",
                    Entrenamientos = new List<Entrenamiento>
                    {
                        new Entrenamiento
                        {
                            Fecha = new DateTime(2024, 2, 15), // Fuera del rango
                            Actividades = new List<Actividad>
                            {
                                new Actividad { Punto = 50 }
                            }
                        }
                    }
                }
            };

            _mockSocioRepo.Setup(r => r.ObtenerSociosParaRankingAsync(1))
                .ReturnsAsync(socios);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1, 10, desde, hasta);

            // Assert
            resultado.Should().HaveCount(1); // Solo Juan tiene puntos en el rango
            resultado[0].SocioId.Should().Be(1);
        }

        [Fact]
        public async Task Ejecutar_DebeIncluirFechasEnElDTO()
        {
            // Arrange
            var desde = new DateOnly(2024, 1, 1);
            var hasta = new DateOnly(2024, 1, 31);

            var socios = new List<Socio>
            {
                new Socio
                {
                    Id = 1,
                    Nombre = "Juan",
                    Apellido = "Pérez",
                    Entrenamientos = new List<Entrenamiento>
                    {
                        new Entrenamiento
                        {
                            Fecha = new DateTime(2024, 1, 15),
                            Actividades = new List<Actividad>
                            {
                                new Actividad { Punto = 100 }
                            }
                        }
                    }
                }
            };

            _mockSocioRepo.Setup(r => r.ObtenerSociosParaRankingAsync(1))
                .ReturnsAsync(socios);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1, 10, desde, hasta);

            // Assert
            resultado[0].Desde.Should().Be(desde);
            resultado[0].Hasta.Should().Be(hasta);
        }

        [Fact]
        public async Task Ejecutar_DebeConstruirNombreCompleto()
        {
            // Arrange
            var desde = new DateOnly(2024, 1, 1);
            var hasta = new DateOnly(2024, 1, 31);

            var socios = new List<Socio>
            {
                new Socio
                {
                    Id = 1,
                    Nombre = "Juan",
                    Apellido = "Pérez",
                    Entrenamientos = new List<Entrenamiento>
                    {
                        new Entrenamiento
                        {
                            Fecha = new DateTime(2024, 1, 15),
                            Actividades = new List<Actividad>
                            {
                                new Actividad { Punto = 100 }
                            }
                        }
                    }
                }
            };

            _mockSocioRepo.Setup(r => r.ObtenerSociosParaRankingAsync(1))
                .ReturnsAsync(socios);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1, 10, desde, hasta);

            // Assert
            resultado[0].NombreCompleto.Should().Be("Juan Pérez");
        }

        [Fact]
        public async Task Ejecutar_DebeSumarPuntajesDeVariosEntrenamientos()
        {
            // Arrange
            var desde = new DateOnly(2024, 1, 1);
            var hasta = new DateOnly(2024, 1, 31);

            var socios = new List<Socio>
            {
                new Socio
                {
                    Id = 1,
                    Nombre = "Juan",
                    Apellido = "Pérez",
                    Entrenamientos = new List<Entrenamiento>
                    {
                        new Entrenamiento
                        {
                            Fecha = new DateTime(2024, 1, 10),
                            Actividades = new List<Actividad>
                            {
                                new Actividad { Punto = 50 },
                                new Actividad { Punto = 30 }
                            }
                        },
                        new Entrenamiento
                        {
                            Fecha = new DateTime(2024, 1, 20),
                            Actividades = new List<Actividad>
                            {
                                new Actividad { Punto = 40 },
                                new Actividad { Punto = 60 }
                            }
                        }
                    }
                }
            };

            _mockSocioRepo.Setup(r => r.ObtenerSociosParaRankingAsync(1))
                .ReturnsAsync(socios);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1, 10, desde, hasta);

            // Assert
            resultado[0].PuntajeTotal.Should().Be(180);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarListaVacia_CuandoNoHaySocios()
        {
            // Arrange
            var desde = new DateOnly(2024, 1, 1);
            var hasta = new DateOnly(2024, 1, 31);

            _mockSocioRepo.Setup(r => r.ObtenerSociosParaRankingAsync(1))
                .ReturnsAsync(new List<Socio>());

            // Act
            var resultado = await _casoDeUso.Ejecutar(1, 10, desde, hasta);

            // Assert
            resultado.Should().BeEmpty();
        }
    }
}
