using AutoMapper;
using FitRank_API.Application.CasosDeUso.LogroSocioCasosDeUso;
using FitRank_API.Application.DTOs.LogroSocioDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace FitRank_API.Tests.CasosDeUsoTests.LogroSocioCasosDeUsoTests
{
    public class ObtenerLogrosDisponiblesPorSocioCasoDeUsoTests
    {
        private readonly Mock<ILogroGimnasioRepositorio> _mockLogroGimnasioRepo;
        private readonly Mock<ILogroSocioRepositorio> _mockLogroSocioRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ObtenerLogrosDisponiblesPorSocioCasoDeUso _casoDeUso;

        public ObtenerLogrosDisponiblesPorSocioCasoDeUsoTests()
        {
            _mockLogroGimnasioRepo = new Mock<ILogroGimnasioRepositorio>();
            _mockLogroSocioRepo = new Mock<ILogroSocioRepositorio>();
            _mockMapper = new Mock<IMapper>();
            _casoDeUso = new ObtenerLogrosDisponiblesPorSocioCasoDeUso(
                _mockLogroGimnasioRepo.Object,
                _mockLogroSocioRepo.Object,
                _mockMapper.Object);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarLogrosDisponibles_CuandoSocioNoTieneTodosLosLogros()
        {
            // Arrange
            var logrosGimnasio = new List<LogroGimnasio>
            {
                new LogroGimnasio
                {
                    Id = 1,
                    LogroId = 1,
                    GimnasioId = 1,
                    EstaActivo = true,
                    Logro = new Logro
                    {
                        Id = 1,
                        Nombre = "Primera Victoria",
                        NombreClave = "primera_victoria",
                        Descripcion = "Completar primer entrenamiento",
                        Imagen = "victoria.png"
                    }
                },
                new LogroGimnasio
                {
                    Id = 2,
                    LogroId = 2,
                    GimnasioId = 1,
                    EstaActivo = true,
                    Logro = new Logro
                    {
                        Id = 2,
                        Nombre = "Constancia",
                        NombreClave = "constancia",
                        Descripcion = "7 días seguidos",
                        Imagen = "constancia.png"
                    }
                }
            };

            var logrosSocio = new List<LogroSocio>
            {
                new LogroSocio
                {
                    Id = 1,
                    LogroId = 1,
                    SocioId = 1,
                    GimnasioId = 1,
                    FechaObtenido = DateTime.Now
                }
            };

            _mockLogroGimnasioRepo.Setup(r => r.ObtenerPorGimnasioAsync(1))
                .ReturnsAsync(logrosGimnasio);

            _mockLogroSocioRepo.Setup(r => r.ObtenerPorSocioYGimnasioAsync(1, 1))
                .ReturnsAsync(logrosSocio);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1, 1);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(1);
            resultado.First().LogroId.Should().Be(2);
            resultado.First().Nombre.Should().Be("Constancia");
            resultado.First().NombreClave.Should().Be("constancia");
            resultado.First().Descripcion.Should().Be("7 días seguidos");
            resultado.First().Imagen.Should().Be("constancia.png");
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarTodosLosLogros_CuandoSocioNoTieneNinguno()
        {
            // Arrange
            var logrosGimnasio = new List<LogroGimnasio>
            {
                new LogroGimnasio
                {
                    Id = 1,
                    LogroId = 1,
                    GimnasioId = 1,
                    EstaActivo = true,
                    Logro = new Logro
                    {
                        Id = 1,
                        Nombre = "Logro 1",
                        NombreClave = "logro_1",
                        Descripcion = "Desc 1",
                        Imagen = "img1.png"
                    }
                },
                new LogroGimnasio
                {
                    Id = 2,
                    LogroId = 2,
                    GimnasioId = 1,
                    EstaActivo = true,
                    Logro = new Logro
                    {
                        Id = 2,
                        Nombre = "Logro 2",
                        NombreClave = "logro_2",
                        Descripcion = "Desc 2",
                        Imagen = "img2.png"
                    }
                }
            };

            _mockLogroGimnasioRepo.Setup(r => r.ObtenerPorGimnasioAsync(1))
                .ReturnsAsync(logrosGimnasio);

            _mockLogroSocioRepo.Setup(r => r.ObtenerPorSocioYGimnasioAsync(1, 1))
                .ReturnsAsync(new List<LogroSocio>());

            // Act
            var resultado = await _casoDeUso.Ejecutar(1, 1);

            // Assert
            resultado.Should().HaveCount(2);
            resultado.Select(l => l.LogroId).Should().Contain(new[] { 1L, 2L });
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarVacio_CuandoSocioTieneTodosLosLogros()
        {
            // Arrange
            var logrosGimnasio = new List<LogroGimnasio>
            {
                new LogroGimnasio
                {
                    Id = 1,
                    LogroId = 1,
                    GimnasioId = 1,
                    EstaActivo = true,
                    Logro = new Logro
                    {
                        Id = 1,
                        Nombre = "Logro 1",
                        NombreClave = "logro_1",
                        Descripcion = "Desc 1",
                        Imagen = "img1.png"
                    }
                }
            };

            var logrosSocio = new List<LogroSocio>
            {
                new LogroSocio
                {
                    Id = 1,
                    LogroId = 1,
                    SocioId = 1,
                    GimnasioId = 1,
                    FechaObtenido = DateTime.Now
                }
            };

            _mockLogroGimnasioRepo.Setup(r => r.ObtenerPorGimnasioAsync(1))
                .ReturnsAsync(logrosGimnasio);

            _mockLogroSocioRepo.Setup(r => r.ObtenerPorSocioYGimnasioAsync(1, 1))
                .ReturnsAsync(logrosSocio);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1, 1);

            // Assert
            resultado.Should().BeEmpty();
        }

        [Fact]
        public async Task Ejecutar_DebeExcluirLogrosInactivos()
        {
            // Arrange
            var logrosGimnasio = new List<LogroGimnasio>
            {
                new LogroGimnasio
                {
                    Id = 1,
                    LogroId = 1,
                    GimnasioId = 1,
                    EstaActivo = true,
                    Logro = new Logro
                    {
                        Id = 1,
                        Nombre = "Activo",
                        NombreClave = "activo",
                        Descripcion = "Logro activo",
                        Imagen = "activo.png"
                    }
                },
                new LogroGimnasio
                {
                    Id = 2,
                    LogroId = 2,
                    GimnasioId = 1,
                    EstaActivo = false,
                    Logro = new Logro
                    {
                        Id = 2,
                        Nombre = "Inactivo",
                        NombreClave = "inactivo",
                        Descripcion = "Logro inactivo",
                        Imagen = "inactivo.png"
                    }
                }
            };

            _mockLogroGimnasioRepo.Setup(r => r.ObtenerPorGimnasioAsync(1))
                .ReturnsAsync(logrosGimnasio);

            _mockLogroSocioRepo.Setup(r => r.ObtenerPorSocioYGimnasioAsync(1, 1))
                .ReturnsAsync(new List<LogroSocio>());

            // Act
            var resultado = await _casoDeUso.Ejecutar(1, 1);

            // Assert
            resultado.Should().HaveCount(1);
            resultado.First().Nombre.Should().Be("Activo");
        }

        [Fact]
        public async Task Ejecutar_DebeExcluirLogrosConLogroNulo()
        {
            // Arrange
            var logrosGimnasio = new List<LogroGimnasio>
            {
                new LogroGimnasio
                {
                    Id = 1,
                    LogroId = 1,
                    GimnasioId = 1,
                    EstaActivo = true,
                    Logro = new Logro
                    {
                        Id = 1,
                        Nombre = "Con Logro",
                        NombreClave = "con_logro",
                        Descripcion = "Tiene logro asociado",
                        Imagen = "logro.png"
                    }
                },
                new LogroGimnasio
                {
                    Id = 2,
                    LogroId = 2,
                    GimnasioId = 1,
                    EstaActivo = true,
                    Logro = null
                }
            };

            _mockLogroGimnasioRepo.Setup(r => r.ObtenerPorGimnasioAsync(1))
                .ReturnsAsync(logrosGimnasio);

            _mockLogroSocioRepo.Setup(r => r.ObtenerPorSocioYGimnasioAsync(1, 1))
                .ReturnsAsync(new List<LogroSocio>());

            // Act
            var resultado = await _casoDeUso.Ejecutar(1, 1);

            // Assert
            resultado.Should().HaveCount(1);
            resultado.First().Nombre.Should().Be("Con Logro");
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarVacio_CuandoNoHayLogrosHabilitados()
        {
            // Arrange
            _mockLogroGimnasioRepo.Setup(r => r.ObtenerPorGimnasioAsync(1))
                .ReturnsAsync(new List<LogroGimnasio>());

            _mockLogroSocioRepo.Setup(r => r.ObtenerPorSocioYGimnasioAsync(1, 1))
                .ReturnsAsync(new List<LogroSocio>());

            // Act
            var resultado = await _casoDeUso.Ejecutar(1, 1);

            // Assert
            resultado.Should().BeEmpty();
        }

        [Fact]
        public async Task Ejecutar_DebeManejarVariosLogrosDelSocio()
        {
            // Arrange
            var logrosGimnasio = new List<LogroGimnasio>
            {
                new LogroGimnasio
                {
                    Id = 1,
                    LogroId = 1,
                    GimnasioId = 1,
                    EstaActivo = true,
                    Logro = new Logro { Id = 1, Nombre = "Logro 1", NombreClave = "logro_1", Descripcion = "Desc 1", Imagen = "img1.png" }
                },
                new LogroGimnasio
                {
                    Id = 2,
                    LogroId = 2,
                    GimnasioId = 1,
                    EstaActivo = true,
                    Logro = new Logro { Id = 2, Nombre = "Logro 2", NombreClave = "logro_2", Descripcion = "Desc 2", Imagen = "img2.png" }
                },
                new LogroGimnasio
                {
                    Id = 3,
                    LogroId = 3,
                    GimnasioId = 1,
                    EstaActivo = true,
                    Logro = new Logro { Id = 3, Nombre = "Logro 3", NombreClave = "logro_3", Descripcion = "Desc 3", Imagen = "img3.png" }
                },
                new LogroGimnasio
                {
                    Id = 4,
                    LogroId = 4,
                    GimnasioId = 1,
                    EstaActivo = true,
                    Logro = new Logro { Id = 4, Nombre = "Logro 4", NombreClave = "logro_4", Descripcion = "Desc 4", Imagen = "img4.png" }
                }
            };

            var logrosSocio = new List<LogroSocio>
            {
                new LogroSocio { Id = 1, LogroId = 1, SocioId = 1, GimnasioId = 1, FechaObtenido = DateTime.Now },
                new LogroSocio { Id = 2, LogroId = 3, SocioId = 1, GimnasioId = 1, FechaObtenido = DateTime.Now }
            };

            _mockLogroGimnasioRepo.Setup(r => r.ObtenerPorGimnasioAsync(1))
                .ReturnsAsync(logrosGimnasio);

            _mockLogroSocioRepo.Setup(r => r.ObtenerPorSocioYGimnasioAsync(1, 1))
                .ReturnsAsync(logrosSocio);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1, 1);

            // Assert
            resultado.Should().HaveCount(2);
            resultado.Select(l => l.LogroId).Should().Contain(new[] { 2L, 4L });
            resultado.Select(l => l.LogroId).Should().NotContain(new[] { 1L, 3L });
        }

        [Fact]
        public async Task Ejecutar_DebeMapearCorrectamentePropiedades()
        {
            // Arrange
            var logrosGimnasio = new List<LogroGimnasio>
            {
                new LogroGimnasio
                {
                    Id = 1,
                    LogroId = 10,
                    GimnasioId = 1,
                    EstaActivo = true,
                    Logro = new Logro
                    {
                        Id = 10,
                        Nombre = "Nombre Test",
                        NombreClave = "nombre_test",
                        Descripcion = "Descripción Test",
                        Imagen = "imagen_test.png"
                    }
                }
            };

            _mockLogroGimnasioRepo.Setup(r => r.ObtenerPorGimnasioAsync(1))
                .ReturnsAsync(logrosGimnasio);

            _mockLogroSocioRepo.Setup(r => r.ObtenerPorSocioYGimnasioAsync(1, 1))
                .ReturnsAsync(new List<LogroSocio>());

            // Act
            var resultado = await _casoDeUso.Ejecutar(1, 1);

            // Assert
            var dto = resultado.First();
            dto.LogroId.Should().Be(10);
            dto.Nombre.Should().Be("Nombre Test");
            dto.NombreClave.Should().Be("nombre_test");
            dto.Descripcion.Should().Be("Descripción Test");
            dto.Imagen.Should().Be("imagen_test.png");
        }

        [Fact]
        public async Task Ejecutar_DebeLlamarRepositoriosConParametrosCorrectos()
        {
            // Arrange
            var socioId = 5;
            var gimnasioId = 10;

            _mockLogroGimnasioRepo.Setup(r => r.ObtenerPorGimnasioAsync(gimnasioId))
                .ReturnsAsync(new List<LogroGimnasio>());

            _mockLogroSocioRepo.Setup(r => r.ObtenerPorSocioYGimnasioAsync(socioId, gimnasioId))
                .ReturnsAsync(new List<LogroSocio>());

            // Act
            await _casoDeUso.Ejecutar(socioId, gimnasioId);

            // Assert
            _mockLogroGimnasioRepo.Verify(r => r.ObtenerPorGimnasioAsync(gimnasioId), Times.Once);
            _mockLogroSocioRepo.Verify(r => r.ObtenerPorSocioYGimnasioAsync(socioId, gimnasioId), Times.Once);
        }

        [Fact]
        public async Task Ejecutar_DebeFiltrarCorrectamentePorGimnasio()
        {
            // Arrange - El repositorio filtra por gimnasio, así que el socio no tiene este logro en el gym 1
            var logrosGimnasio = new List<LogroGimnasio>
            {
                new LogroGimnasio
                {
                    Id = 1,
                    LogroId = 1,
                    GimnasioId = 1,
                    EstaActivo = true,
                    Logro = new Logro { Id = 1, Nombre = "Logro Gym 1", NombreClave = "gym1", Descripcion = "Desc", Imagen = "img.png" }
                }
            };

            // El socio no tiene logros en este gimnasio
            var logrosSocio = new List<LogroSocio>();

            _mockLogroGimnasioRepo.Setup(r => r.ObtenerPorGimnasioAsync(1))
                .ReturnsAsync(logrosGimnasio);

            _mockLogroSocioRepo.Setup(r => r.ObtenerPorSocioYGimnasioAsync(1, 1))
                .ReturnsAsync(logrosSocio);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1, 1);

            // Assert - Debe retornar el logro porque el socio no lo tiene en este gimnasio
            resultado.Should().HaveCount(1);
            resultado.First().Nombre.Should().Be("Logro Gym 1");
        }
    }
}
