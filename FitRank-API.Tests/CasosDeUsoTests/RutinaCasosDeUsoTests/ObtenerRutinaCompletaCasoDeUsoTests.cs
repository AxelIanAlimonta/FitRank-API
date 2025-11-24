using AutoMapper;
using FitRank_API.Application.CasosDeUso.RutinaCasosDeUso;
using FitRank_API.Application.DTOs.RutinaDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace FitRank_API.Tests.CasosDeUsoTests.RutinaCasosDeUsoTests
{
    public class ObtenerRutinaCompletaCasoDeUsoTests
    {
        private readonly Mock<IRutinaRepositorio> _mockRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ObtenerRutinaCompletaCasoDeUso _casoDeUso;

        public ObtenerRutinaCompletaCasoDeUsoTests()
        {
            _mockRepo = new Mock<IRutinaRepositorio>();
            _mockMapper = new Mock<IMapper>();
            _casoDeUso = new ObtenerRutinaCompletaCasoDeUso(_mockRepo.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarRutinaCompleta()
        {
            // Arrange
            var socioId = 1L;
            var rutinas = new List<Rutina>
            {
                new Rutina
                {
                    Id = 1,
                    Nombre = "Rutina Test",
                    Descripcion = "Descripción",
                    Favorita = true,
                    Activa = true,
                    Sesiones = new List<Sesion>
                    {
                        new Sesion
                        {
                            Id = 10,
                            Nombre = "Sesión 1",
                            NumeroDeSesion = 1,
                            EjerciciosAsignados = new List<EjercicioAsignado>
                            {
                                new EjercicioAsignado
                                {
                                    Id = 100,
                                    NumeroEjercicio = 1,
                                    Ejercicio = new Ejercicio
                                    {
                                        Id = 1000,
                                        Nombre = "Press Banca",
                                        Descripcion = "Ejercicio de pecho",
                                        UrlImagen = "img.jpg",
                                        UrlVideo = "video.mp4",
                                        DuracionEstimada = 30
                                    },
                                    Series = new List<Serie>
                                    {
                                        new Serie { Id = 1, Peso = 80, Repeticiones = 10, Duracion = TimeSpan.FromSeconds(60) }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            _mockRepo.Setup(r => r.ObtenerRutinasPorSocioAsync(socioId)).ReturnsAsync(rutinas);

            // Act
            var resultado = await _casoDeUso.Ejecutar(socioId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(1);
            var rutina = resultado.First();
            rutina.Id.Should().Be(1);
            rutina.Nombre.Should().Be("Rutina Test");
            rutina.Sesiones.Should().HaveCount(1);
            rutina.Sesiones.First().EjerciciosAsignados.Should().HaveCount(1);
            rutina.Sesiones.First().EjerciciosAsignados.First().Series.Should().HaveCount(1);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarListaVacia_CuandoNoHayRutinas()
        {
            // Arrange
            var socioId = 1L;
            _mockRepo.Setup(r => r.ObtenerRutinasPorSocioAsync(socioId))
                .ReturnsAsync(new List<Rutina>());

            // Act
            var resultado = await _casoDeUso.Ejecutar(socioId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();
        }

        [Fact]
        public async Task Ejecutar_DebeLlamarRepositorioConSocioIdCorrecto()
        {
            // Arrange
            var socioId = 5L;
            _mockRepo.Setup(r => r.ObtenerRutinasPorSocioAsync(socioId))
                .ReturnsAsync(new List<Rutina>());

            // Act
            await _casoDeUso.Ejecutar(socioId);

            // Assert
            _mockRepo.Verify(r => r.ObtenerRutinasPorSocioAsync(socioId), Times.Once);
        }

        [Fact]
        public async Task Ejecutar_DebeMapearDescripcionNulaAVacio()
        {
            // Arrange
            var socioId = 1L;
            var rutinas = new List<Rutina>
            {
                new Rutina
                {
                    Id = 1,
                    Nombre = "Rutina",
                    Descripcion = null,
                    Favorita = false,
                    Activa = true
                }
            };

            _mockRepo.Setup(r => r.ObtenerRutinasPorSocioAsync(socioId)).ReturnsAsync(rutinas);

            // Act
            var resultado = await _casoDeUso.Ejecutar(socioId);

            // Assert
            resultado.First().Descripcion.Should().Be("");
        }

        [Fact]
        public async Task Ejecutar_DebeManejarSesionesNulas()
        {
            // Arrange
            var socioId = 1L;
            var rutinas = new List<Rutina>
            {
                new Rutina
                {
                    Id = 1,
                    Nombre = "Rutina",
                    Descripcion = "Desc",
                    Favorita = false,
                    Activa = true,
                    Sesiones = null
                }
            };

            _mockRepo.Setup(r => r.ObtenerRutinasPorSocioAsync(socioId)).ReturnsAsync(rutinas);

            // Act
            var resultado = await _casoDeUso.Ejecutar(socioId);

            // Assert
            resultado.First().Sesiones.Should().NotBeNull();
            resultado.First().Sesiones.Should().BeEmpty();
        }

        [Fact]
        public async Task Ejecutar_DebeManejarEjerciciosAsignadosNulos()
        {
            // Arrange
            var socioId = 1L;
            var rutinas = new List<Rutina>
            {
                new Rutina
                {
                    Id = 1,
                    Nombre = "Rutina",
                    Sesiones = new List<Sesion>
                    {
                        new Sesion
                        {
                            Id = 10,
                            Nombre = "Sesión 1",
                            NumeroDeSesion = 1,
                            EjerciciosAsignados = null
                        }
                    }
                }
            };

            _mockRepo.Setup(r => r.ObtenerRutinasPorSocioAsync(socioId)).ReturnsAsync(rutinas);

            // Act
            var resultado = await _casoDeUso.Ejecutar(socioId);

            // Assert
            resultado.First().Sesiones.First().EjerciciosAsignados.Should().NotBeNull();
            resultado.First().Sesiones.First().EjerciciosAsignados.Should().BeEmpty();
        }

        [Fact]
        public async Task Ejecutar_DebeManejarSeriesNulas()
        {
            // Arrange
            var socioId = 1L;
            var rutinas = new List<Rutina>
            {
                new Rutina
                {
                    Id = 1,
                    Nombre = "Rutina",
                    Sesiones = new List<Sesion>
                    {
                        new Sesion
                        {
                            Id = 10,
                            Nombre = "Sesión 1",
                            EjerciciosAsignados = new List<EjercicioAsignado>
                            {
                                new EjercicioAsignado
                                {
                                    Id = 100,
                                    Ejercicio = new Ejercicio { Id = 1, Nombre = "Test" },
                                    Series = null
                                }
                            }
                        }
                    }
                }
            };

            _mockRepo.Setup(r => r.ObtenerRutinasPorSocioAsync(socioId)).ReturnsAsync(rutinas);

            // Act
            var resultado = await _casoDeUso.Ejecutar(socioId);

            // Assert
            resultado.First().Sesiones.First().EjerciciosAsignados.First().Series.Should().NotBeNull();
            resultado.First().Sesiones.First().EjerciciosAsignados.First().Series.Should().BeEmpty();
        }

        [Fact]
        public async Task Ejecutar_DebeMapearCorrectamenteDatosDelEjercicio()
        {
            // Arrange
            var socioId = 1L;
            var rutinas = new List<Rutina>
            {
                new Rutina
                {
                    Id = 1,
                    Nombre = "Rutina",
                    Sesiones = new List<Sesion>
                    {
                        new Sesion
                        {
                            Id = 10,
                            Nombre = "Sesión 1",
                            EjerciciosAsignados = new List<EjercicioAsignado>
                            {
                                new EjercicioAsignado
                                {
                                    Id = 100,
                                    NumeroEjercicio = 1,
                                    Ejercicio = new Ejercicio
                                    {
                                        Id = 1000,
                                        Nombre = "Sentadilla",
                                        Descripcion = "Ejercicio de piernas",
                                        UrlImagen = "sentadilla.jpg",
                                        UrlVideo = "sentadilla.mp4",
                                        DuracionEstimada = 45
                                    }
                                }
                            }
                        }
                    }
                }
            };

            _mockRepo.Setup(r => r.ObtenerRutinasPorSocioAsync(socioId)).ReturnsAsync(rutinas);

            // Act
            var resultado = await _casoDeUso.Ejecutar(socioId);

            // Assert
            var ejercicio = resultado.First().Sesiones.First().EjerciciosAsignados.First().Ejercicio;
            ejercicio.Id.Should().Be(1000);
            ejercicio.Nombre.Should().Be("Sentadilla");
            ejercicio.Descripcion.Should().Be("Ejercicio de piernas");
            ejercicio.UrlImagen.Should().Be("sentadilla.jpg");
            ejercicio.UrlVideo.Should().Be("sentadilla.mp4");
            ejercicio.DuracionEstimada.Should().Be(45);
        }

        [Fact]
        public async Task Ejecutar_DebeManejarDuracionEstimadaNula()
        {
            // Arrange
            var socioId = 1L;
            var rutinas = new List<Rutina>
            {
                new Rutina
                {
                    Id = 1,
                    Nombre = "Rutina",
                    Sesiones = new List<Sesion>
                    {
                        new Sesion
                        {
                            Id = 10,
                            Nombre = "Sesión 1",
                            EjerciciosAsignados = new List<EjercicioAsignado>
                            {
                                new EjercicioAsignado
                                {
                                    Id = 100,
                                    Ejercicio = new Ejercicio
                                    {
                                        Id = 1000,
                                        Nombre = "Test",
                                        DuracionEstimada = null
                                    }
                                }
                            }
                        }
                    }
                }
            };

            _mockRepo.Setup(r => r.ObtenerRutinasPorSocioAsync(socioId)).ReturnsAsync(rutinas);

            // Act
            var resultado = await _casoDeUso.Ejecutar(socioId);

            // Assert
            resultado.First().Sesiones.First().EjerciciosAsignados.First().Ejercicio.DuracionEstimada.Should().Be(0);
        }

        [Fact]
        public async Task Ejecutar_DebeManejarVariasRutinasConVariasSesiones()
        {
            // Arrange
            var socioId = 1L;
            var rutinas = new List<Rutina>();
            for (int r = 1; r <= 3; r++)
            {
                var sesiones = new List<Sesion>();
                for (int s = 1; s <= 2; s++)
                {
                    sesiones.Add(new Sesion
                    {
                        Id = r * 10 + s,
                        Nombre = $"Sesión {s}",
                        NumeroDeSesion = s
                    });
                }

                rutinas.Add(new Rutina
                {
                    Id = r,
                    Nombre = $"Rutina {r}",
                    Sesiones = sesiones
                });
            }

            _mockRepo.Setup(r => r.ObtenerRutinasPorSocioAsync(socioId)).ReturnsAsync(rutinas);

            // Act
            var resultado = await _casoDeUso.Ejecutar(socioId);

            // Assert
            resultado.Should().HaveCount(3);
            resultado.All(r => r.Sesiones.Count == 2).Should().BeTrue();
        }

        [Fact]
        public async Task Ejecutar_DebeMapearCorrectamenteDatosDeSeries()
        {
            // Arrange
            var socioId = 1L;
            var rutinas = new List<Rutina>
            {
                new Rutina
                {
                    Id = 1,
                    Nombre = "Rutina",
                    Sesiones = new List<Sesion>
                    {
                        new Sesion
                        {
                            Id = 10,
                            Nombre = "Sesión 1",
                            EjerciciosAsignados = new List<EjercicioAsignado>
                            {
                                new EjercicioAsignado
                                {
                                    Id = 100,
                                    Ejercicio = new Ejercicio { Id = 1, Nombre = "Test" },
                                    Series = new List<Serie>
                                    {
                                        new Serie { Id = 1, Peso = 100, Repeticiones = 8, Duracion = TimeSpan.FromSeconds(120) },
                                        new Serie { Id = 2, Peso = 90, Repeticiones = 10, Duracion = TimeSpan.FromSeconds(90) }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            _mockRepo.Setup(r => r.ObtenerRutinasPorSocioAsync(socioId)).ReturnsAsync(rutinas);

            // Act
            var resultado = await _casoDeUso.Ejecutar(socioId);

            // Assert
            var series = resultado.First().Sesiones.First().EjerciciosAsignados.First().Series;
            series.Should().HaveCount(2);
            series[0].Peso.Should().Be(100);
            series[0].Repeticiones.Should().Be(8);
            series[0].Duracion.Should().Be(TimeSpan.FromSeconds(120));
            series[1].Peso.Should().Be(90);
            series[1].Repeticiones.Should().Be(10);
            series[1].Duracion.Should().Be(TimeSpan.FromSeconds(90));
        }
    }
}
