//using AutoMapper;
//using FitRank_API.Application.CasosDeUso.CalculoPuntajeCasosDeUso;
//using FitRank_API.Application.DTOs.PuntajeDTOs;
//using FitRank_API.Domain.Entities;
//using FitRank_API.Infrastructure.Interfaces;
//using FluentAssertions;
//using Moq;
//using Xunit;

//namespace FitRank_API.Tests.CasosDeUsoTests.CalculoPuntajeCasosDeUsoTests
//{
//    public class ObtenerRankingPorGrupoMuscularCasoDeUsoTests
//    {
//        private readonly Mock<ISocioRepositorio> _mockSocioRepo;
//        private readonly Mock<IMapper> _mockMapper;
//        private readonly ObtenerRankingPorGrupoMuscularCasoDeUso _casoDeUso;

//        public ObtenerRankingPorGrupoMuscularCasoDeUsoTests()
//        {
//            _mockSocioRepo = new Mock<ISocioRepositorio>();
//            _mockMapper = new Mock<IMapper>();
//            _casoDeUso = new ObtenerRankingPorGrupoMuscularCasoDeUso(_mockSocioRepo.Object, _mockMapper.Object);
//        }

//        [Fact]
//        public async Task Ejecutar_DebeRetornarRankingOrdenadoPorPuntaje()
//        {
//            // Arrange
//            var grupoMuscular = "Pecho";
//            var socios = new List<Socio>
//            {
//                new Socio
//                {
//                    Id = 1,
//                    Nombre = "Juan",
//                    Apellido = "Pérez",
//                    Entrenamientos = new List<Entrenamiento>
//                    {
//                        new Entrenamiento
//                        {
//                            Actividades = new List<Actividad>
//                            {
//                                new Actividad
//                                {
//                                    Punto = 100,
//                                    Serie = new Serie
//                                    {
//                                        EjercicioAsignado = new EjercicioAsignado
//                                        {
//                                            Ejercicio = new Ejercicio
//                                            {
//                                                GrupoMuscular = new GrupoMuscular { Nombre = "Pecho" }
//                                            }
//                                        }
//                                    }
//                                }
//                            }
//                        }
//                    }
//                },
//                new Socio
//                {
//                    Id = 2,
//                    Nombre = "María",
//                    Apellido = "García",
//                    Entrenamientos = new List<Entrenamiento>
//                    {
//                        new Entrenamiento
//                        {
//                            Actividades = new List<Actividad>
//                            {
//                                new Actividad
//                                {
//                                    Punto = 150,
//                                    Serie = new Serie
//                                    {
//                                        EjercicioAsignado = new EjercicioAsignado
//                                        {
//                                            Ejercicio = new Ejercicio
//                                            {
//                                                GrupoMuscular = new GrupoMuscular { Nombre = "Pecho" }
//                                            }
//                                        }
//                                    }
//                                }
//                            }
//                        }
//                    }
//                }
//            };

//            _mockSocioRepo.Setup(r => r.ObtenerSociosParaRankingAsync(1))
//                .ReturnsAsync(socios);

//            // Act
//            var resultado = await _casoDeUso.Ejecutar(1, grupoMuscular, 10);

//            // Assert
//            resultado.Should().HaveCount(2);
//            resultado[0].SocioId.Should().Be(2); // María con 150 puntos
//            resultado[0].PuntajeTotal.Should().Be(150);
//            resultado[1].SocioId.Should().Be(1); // Juan con 100 puntos
//        }

//        [Fact]
//        public async Task Ejecutar_DebeFiltrarSoloPorGrupoMuscularEspecificado()
//        {
//            // Arrange
//            var grupoMuscular = "Pecho";
//            var socios = new List<Socio>
//            {
//                new Socio
//                {
//                    Id = 1,
//                    Nombre = "Juan",
//                    Apellido = "Pérez",
//                    Entrenamientos = new List<Entrenamiento>
//                    {
//                        new Entrenamiento
//                        {
//                            Actividades = new List<Actividad>
//                            {
//                                new Actividad
//                                {
//                                    Punto = 50,
//                                    Serie = new Serie
//                                    {
//                                        EjercicioAsignado = new EjercicioAsignado
//                                        {
//                                            Ejercicio = new Ejercicio
//                                            {
//                                                GrupoMuscular = new GrupoMuscular { Nombre = "Pecho" }
//                                            }
//                                        }
//                                    }
//                                },
//                                new Actividad
//                                {
//                                    Punto = 100,
//                                    Serie = new Serie
//                                    {
//                                        EjercicioAsignado = new EjercicioAsignado
//                                        {
//                                            Ejercicio = new Ejercicio
//                                            {
//                                                GrupoMuscular = new GrupoMuscular { Nombre = "Espalda" }
//                                            }
//                                        }
//                                    }
//                                }
//                            }
//                        }
//                    }
//                }
//            };

//            _mockSocioRepo.Setup(r => r.ObtenerSociosParaRankingAsync(1))
//                .ReturnsAsync(socios);

//            // Act
//            var resultado = await _casoDeUso.Ejecutar(1, grupoMuscular, 10);

//            // Assert
//            resultado[0].PuntajeTotal.Should().Be(50); // Solo puntos de Pecho
//        }

//        [Fact]
//        public async Task Ejecutar_DebeLimitarCantidadDeResultados()
//        {
//            // Arrange
//            var grupoMuscular = "Piernas";
//            var socios = new List<Socio>();
            
//            for (int i = 1; i <= 10; i++)
//            {
//                socios.Add(new Socio
//                {
//                    Id = i,
//                    Nombre = $"Socio{i}",
//                    Apellido = $"Apellido{i}",
//                    Entrenamientos = new List<Entrenamiento>
//                    {
//                        new Entrenamiento
//                        {
//                            Actividades = new List<Actividad>
//                            {
//                                new Actividad
//                                {
//                                    Punto = i * 10,
//                                    Serie = new Serie
//                                    {
//                                        EjercicioAsignado = new EjercicioAsignado
//                                        {
//                                            Ejercicio = new Ejercicio
//                                            {
//                                                GrupoMuscular = new GrupoMuscular { Nombre = "Piernas" }
//                                            }
//                                        }
//                                    }
//                                }
//                            }
//                        }
//                    }
//                });
//            }

//            _mockSocioRepo.Setup(r => r.ObtenerSociosParaRankingAsync(1))
//                .ReturnsAsync(socios);

//            // Act
//            var resultado = await _casoDeUso.Ejecutar(1, grupoMuscular, 3);

//            // Assert
//            resultado.Should().HaveCount(3);
//        }

//        [Fact]
//        public async Task Ejecutar_DebeExcluirSociosSinPuntajeEnGrupo()
//        {
//            // Arrange
//            var grupoMuscular = "Abdomen";
//            var socios = new List<Socio>
//            {
//                new Socio
//                {
//                    Id = 1,
//                    Nombre = "Juan",
//                    Apellido = "Pérez",
//                    Entrenamientos = new List<Entrenamiento>
//                    {
//                        new Entrenamiento
//                        {
//                            Actividades = new List<Actividad>
//                            {
//                                new Actividad
//                                {
//                                    Punto = 100,
//                                    Serie = new Serie
//                                    {
//                                        EjercicioAsignado = new EjercicioAsignado
//                                        {
//                                            Ejercicio = new Ejercicio
//                                            {
//                                                GrupoMuscular = new GrupoMuscular { Nombre = "Abdomen" }
//                                            }
//                                        }
//                                    }
//                                }
//                            }
//                        }
//                    }
//                },
//                new Socio
//                {
//                    Id = 2,
//                    Nombre = "María",
//                    Apellido = "García",
//                    Entrenamientos = new List<Entrenamiento>
//                    {
//                        new Entrenamiento
//                        {
//                            Actividades = new List<Actividad>
//                            {
//                                new Actividad
//                                {
//                                    Punto = 50,
//                                    Serie = new Serie
//                                    {
//                                        EjercicioAsignado = new EjercicioAsignado
//                                        {
//                                            Ejercicio = new Ejercicio
//                                            {
//                                                GrupoMuscular = new GrupoMuscular { Nombre = "Brazos" }
//                                            }
//                                        }
//                                    }
//                                }
//                            }
//                        }
//                    }
//                }
//            };

//            _mockSocioRepo.Setup(r => r.ObtenerSociosParaRankingAsync(1))
//                .ReturnsAsync(socios);

//            // Act
//            var resultado = await _casoDeUso.Ejecutar(1, grupoMuscular, 10);

//            // Assert
//            resultado.Should().HaveCount(1); // Solo Juan tiene puntos en Abdomen
//            resultado[0].SocioId.Should().Be(1);
//        }

//        [Fact]
//        public async Task Ejecutar_DebeSerCaseInsensitive()
//        {
//            // Arrange
//            var grupoMuscular = "PECHO";
//            var socios = new List<Socio>
//            {
//                new Socio
//                {
//                    Id = 1,
//                    Nombre = "Juan",
//                    Apellido = "Pérez",
//                    Entrenamientos = new List<Entrenamiento>
//                    {
//                        new Entrenamiento
//                        {
//                            Actividades = new List<Actividad>
//                            {
//                                new Actividad
//                                {
//                                    Punto = 100,
//                                    Serie = new Serie
//                                    {
//                                        EjercicioAsignado = new EjercicioAsignado
//                                        {
//                                            Ejercicio = new Ejercicio
//                                            {
//                                                GrupoMuscular = new GrupoMuscular { Nombre = "pecho" }
//                                            }
//                                        }
//                                    }
//                                }
//                            }
//                        }
//                    }
//                }
//            };

//            _mockSocioRepo.Setup(r => r.ObtenerSociosParaRankingAsync(1))
//                .ReturnsAsync(socios);

//            // Act
//            var resultado = await _casoDeUso.Ejecutar(1, grupoMuscular, 10);

//            // Assert
//            resultado.Should().HaveCount(1);
//            resultado[0].PuntajeTotal.Should().Be(100);
//        }

//        [Fact]
//        public async Task Ejecutar_DebeConstruirNombreCompleto()
//        {
//            // Arrange
//            var grupoMuscular = "Hombros";
//            var socios = new List<Socio>
//            {
//                new Socio
//                {
//                    Id = 1,
//                    Nombre = "Juan",
//                    Apellido = "Pérez",
//                    Entrenamientos = new List<Entrenamiento>
//                    {
//                        new Entrenamiento
//                        {
//                            Actividades = new List<Actividad>
//                            {
//                                new Actividad
//                                {
//                                    Punto = 100,
//                                    Serie = new Serie
//                                    {
//                                        EjercicioAsignado = new EjercicioAsignado
//                                        {
//                                            Ejercicio = new Ejercicio
//                                            {
//                                                GrupoMuscular = new GrupoMuscular { Nombre = "Hombros" }
//                                            }
//                                        }
//                                    }
//                                }
//                            }
//                        }
//                    }
//                }
//            };

//            _mockSocioRepo.Setup(r => r.ObtenerSociosParaRankingAsync(1))
//                .ReturnsAsync(socios);

//            // Act
//            var resultado = await _casoDeUso.Ejecutar(1, grupoMuscular, 10);

//            // Assert
//            resultado[0].NombreCompleto.Should().Be("Juan Pérez");
//            resultado[0].GrupoMuscular.Should().Be(grupoMuscular);
//        }

//        [Fact]
//        public async Task Ejecutar_DebeSumarPuntajesDeVariosEntrenamientos()
//        {
//            // Arrange
//            var grupoMuscular = "Brazos";
//            var socios = new List<Socio>
//            {
//                new Socio
//                {
//                    Id = 1,
//                    Nombre = "Juan",
//                    Apellido = "Pérez",
//                    Entrenamientos = new List<Entrenamiento>
//                    {
//                        new Entrenamiento
//                        {
//                            Actividades = new List<Actividad>
//                            {
//                                new Actividad
//                                {
//                                    Punto = 30,
//                                    Serie = new Serie
//                                    {
//                                        EjercicioAsignado = new EjercicioAsignado
//                                        {
//                                            Ejercicio = new Ejercicio
//                                            {
//                                                GrupoMuscular = new GrupoMuscular { Nombre = "Brazos" }
//                                            }
//                                        }
//                                    }
//                                },
//                                new Actividad
//                                {
//                                    Punto = 40,
//                                    Serie = new Serie
//                                    {
//                                        EjercicioAsignado = new EjercicioAsignado
//                                        {
//                                            Ejercicio = new Ejercicio
//                                            {
//                                                GrupoMuscular = new GrupoMuscular { Nombre = "Brazos" }
//                                            }
//                                        }
//                                    }
//                                }
//                            }
//                        },
//                        new Entrenamiento
//                        {
//                            Actividades = new List<Actividad>
//                            {
//                                new Actividad
//                                {
//                                    Punto = 50,
//                                    Serie = new Serie
//                                    {
//                                        EjercicioAsignado = new EjercicioAsignado
//                                        {
//                                            Ejercicio = new Ejercicio
//                                            {
//                                                GrupoMuscular = new GrupoMuscular { Nombre = "Brazos" }
//                                            }
//                                        }
//                                    }
//                                }
//                            }
//                        }
//                    }
//                }
//            };

//            _mockSocioRepo.Setup(r => r.ObtenerSociosParaRankingAsync(1))
//                .ReturnsAsync(socios);

//            // Act
//            var resultado = await _casoDeUso.Ejecutar(1, grupoMuscular, 10);

//            // Assert
//            resultado[0].PuntajeTotal.Should().Be(120);
//        }

//        [Fact]
//        public async Task Ejecutar_DebeRetornarListaVacia_CuandoNoHaySocios()
//        {
//            // Arrange
//            _mockSocioRepo.Setup(r => r.ObtenerSociosParaRankingAsync(1))
//                .ReturnsAsync(new List<Socio>());

//            // Act
//            var resultado = await _casoDeUso.Ejecutar(1, "Pecho", 10);

//            // Assert
//            resultado.Should().BeEmpty();
//        }
//    }
//}
