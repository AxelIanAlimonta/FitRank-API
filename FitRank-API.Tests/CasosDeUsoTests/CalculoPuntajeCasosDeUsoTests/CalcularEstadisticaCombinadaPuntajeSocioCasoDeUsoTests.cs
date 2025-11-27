using FitRank_API.Application.DTOs.CalcularPuntajeDTOs;
using FitRank_API.Application.UseCases;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace FitRank_API.Tests.CasosDeUsoTests.CalculoPuntajeCasosDeUsoTests
{
    public class CalcularEstadisticaCombinadaPuntajeSocioCasoDeUsoTests
    {
        private readonly Mock<ISocioRepositorio> _mockSocioRepo;
        private readonly Mock<IActividadRepositorio> _mockActividadRepo;
        private readonly CalcularEstadisticaCombinadaPuntajeSocioCasoDeUso _casoDeUso;

        public CalcularEstadisticaCombinadaPuntajeSocioCasoDeUsoTests()
        {
            _mockSocioRepo = new Mock<ISocioRepositorio>();
            _mockActividadRepo = new Mock<IActividadRepositorio>();
            _casoDeUso = new CalcularEstadisticaCombinadaPuntajeSocioCasoDeUso(_mockSocioRepo.Object, _mockActividadRepo.Object);
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
                            new Actividad { Id = 1, Punto = 10.5, EjercicioAsignado = new EjercicioAsignado { Ejercicio = new Ejercicio { GrupoMuscularId = 1 } } },
                            new Actividad { Id = 2, Punto = 15.0, EjercicioAsignado = new EjercicioAsignado { Ejercicio = new Ejercicio { GrupoMuscularId = 1 } } },
                            new Actividad { Id = 3, Punto = 8.5, EjercicioAsignado = new EjercicioAsignado { Ejercicio = new Ejercicio { GrupoMuscularId = 2 } } }
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
            resultado!.SocioId.Should().Be(1);
            resultado.PuntajeTotal.Should().Be(34.0);
            resultado.PuntajePorGrupo.Should().HaveCount(2);
            resultado.PuntajePorGrupo.First(p => p.GrupoMuscularId == 1).Puntaje.Should().Be(25.5);
            resultado.PuntajePorGrupo.First(p => p.GrupoMuscularId == 2).Puntaje.Should().Be(8.5);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarNull_CuandoSocioNoExiste()
        {
            // Arrange
            _mockSocioRepo.Setup(r => r.ObtenerSocioConEntrenamientosAsync(999))
                .ReturnsAsync((Socio?)null);

            // Act
            var resultado = await _casoDeUso.Ejecutar(999);

            // Assert
            resultado.Should().BeNull();
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarNull_CuandoSocioNoTieneEntrenamientos()
        {
            // Arrange
            var socio = new Socio
            {
                Id = 1,
                Nombre = "Juan",
                Entrenamientos = new List<Entrenamiento>()
            };

            _mockSocioRepo.Setup(r => r.ObtenerSocioConEntrenamientosAsync(1))
                .ReturnsAsync(socio);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1);

            // Assert
            resultado.Should().BeNull();
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarNull_CuandoNoHayActividades()
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
                    }
                }
            };

            _mockSocioRepo.Setup(r => r.ObtenerSocioConEntrenamientosAsync(1))
                .ReturnsAsync(socio);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1);

            // Assert
            resultado.Should().BeNull();
        }

        [Fact]
        public async Task Ejecutar_DebeCalcularCorrectamentePuntajeConVariosEntrenamientos()
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
                            new Actividad { Id = 1, Punto = 10, EjercicioAsignado = new EjercicioAsignado { Ejercicio = new Ejercicio { GrupoMuscularId = 1 } } }
                        }
                    },
                    new Entrenamiento
                    {
                        Id = 2,
                        Actividades = new List<Actividad>
                        {
                            new Actividad { Id = 2, Punto = 20, EjercicioAsignado = new EjercicioAsignado { Ejercicio = new Ejercicio { GrupoMuscularId = 1 } } }
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
            resultado!.PuntajeTotal.Should().Be(30);
        }

        [Fact]
        public async Task Ejecutar_DebeManjejarActividadesSinPuntos()
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
                            new Actividad { Id = 1, Punto = null, EjercicioAsignado = new EjercicioAsignado { Ejercicio = new Ejercicio { GrupoMuscularId = 1 } } },
                            new Actividad { Id = 2, Punto = 10, EjercicioAsignado = new EjercicioAsignado { Ejercicio = new Ejercicio { GrupoMuscularId = 1 } } }
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
            resultado!.PuntajeTotal.Should().Be(10);
        }
    }
}
