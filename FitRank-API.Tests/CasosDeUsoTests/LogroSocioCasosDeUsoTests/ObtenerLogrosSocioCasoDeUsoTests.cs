using AutoMapper;
using FluentAssertions;
using FitRank_API.Application.CasosDeUso.LogroSocioCasosDeUso;
using FitRank_API.Application.Mappings;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
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
        public async Task Ejecutar_DebeRetornarLogrosDelSocio()
        {
            // Arrange
            var logrosSocio = new List<LogroSocio>
            {
                new LogroSocio
                {
                    Id = 1,
                    LogroId = 1,
                    SocioId = 1,
                    GimnasioId = 1,
                    FechaObtenido = new DateTime(2024, 1, 1),
                    Logro = new Logro
                    {
                        Id = 1,
                        Nombre = "Primera Victoria",
                        NombreClave = "primera_victoria",
                        Descripcion = "Completar primer entrenamiento",
                        Imagen = "victoria.png"
                    }
                },
                new LogroSocio
                {
                    Id = 2,
                    LogroId = 2,
                    SocioId = 1,
                    GimnasioId = 1,
                    FechaObtenido = new DateTime(2024, 1, 15),
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

            _mockRepositorio.Setup(r => r.ObtenerPorSocioYGimnasioAsync(1, 1))
                .ReturnsAsync(logrosSocio);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1, 1);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(2);
            resultado.First().LogroId.Should().Be(1);
            resultado.First().Nombre.Should().Be("Primera Victoria");
            resultado.First().FechaOtorgado.Should().Be(new DateTime(2024, 1, 1));
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarListaVacia_CuandoSocioNoTieneLogros()
        {
            // Arrange
            _mockRepositorio.Setup(r => r.ObtenerPorSocioYGimnasioAsync(1, 1))
                .ReturnsAsync(new List<LogroSocio>());

            // Act
            var resultado = await _casoDeUso.Ejecutar(1, 1);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();
        }

        [Fact]
        public async Task Ejecutar_DebeLlamarRepositorioConParametrosCorrectos()
        {
            // Arrange
            var socioId = 5;
            var gimnasioId = 10;

            _mockRepositorio.Setup(r => r.ObtenerPorSocioYGimnasioAsync(socioId, gimnasioId))
                .ReturnsAsync(new List<LogroSocio>());

            // Act
            await _casoDeUso.Ejecutar(socioId, gimnasioId);

            // Assert
            _mockRepositorio.Verify(r => r.ObtenerPorSocioYGimnasioAsync(socioId, gimnasioId), Times.Once);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarLogrosSoloDelGimnasioEspecificado()
        {
            // Arrange
            var logrosSocio = new List<LogroSocio>
            {
                new LogroSocio
                {
                    Id = 1,
                    LogroId = 1,
                    SocioId = 1,
                    GimnasioId = 1,
                    FechaObtenido = DateTime.Now,
                    Logro = new Logro
                    {
                        Id = 1,
                        Nombre = "Logro Gym 1",
                        NombreClave = "gym1",
                        Descripcion = "Del gimnasio 1",
                        Imagen = "img1.png"
                    }
                }
            };

            _mockRepositorio.Setup(r => r.ObtenerPorSocioYGimnasioAsync(1, 1))
                .ReturnsAsync(logrosSocio);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1, 1);

            // Assert
            resultado.Should().HaveCount(1);
            resultado.First().Nombre.Should().Be("Logro Gym 1");
        }

        [Fact]
        public async Task Ejecutar_DebeIncluirFechaOtorgado_EnElDTO()
        {
            // Arrange
            var fechaOtorgado = new DateTime(2024, 6, 15, 10, 30, 0);
            var logrosSocio = new List<LogroSocio>
            {
                new LogroSocio
                {
                    Id = 1,
                    LogroId = 1,
                    SocioId = 1,
                    GimnasioId = 1,
                    FechaObtenido = fechaOtorgado,
                    Logro = new Logro
                    {
                        Id = 1,
                        Nombre = "Logro Test",
                        NombreClave = "test",
                        Descripcion = "Test",
                        Imagen = "test.png"
                    }
                }
            };

            _mockRepositorio.Setup(r => r.ObtenerPorSocioYGimnasioAsync(1, 1))
                .ReturnsAsync(logrosSocio);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1, 1);

            // Assert
            resultado.First().FechaOtorgado.Should().Be(fechaOtorgado);
        }

        [Fact]
        public async Task Ejecutar_DebeManejarVariosLogros()
        {
            // Arrange
            var logrosSocio = new List<LogroSocio>
            {
                new LogroSocio { Id = 1, LogroId = 1, SocioId = 1, GimnasioId = 1, FechaObtenido = new DateTime(2024, 1, 1), Logro = new Logro { Id = 1, Nombre = "Logro 1", NombreClave = "logro1", Descripcion = "Desc 1", Imagen = "img1.png" } },
                new LogroSocio { Id = 2, LogroId = 2, SocioId = 1, GimnasioId = 1, FechaObtenido = new DateTime(2024, 2, 1), Logro = new Logro { Id = 2, Nombre = "Logro 2", NombreClave = "logro2", Descripcion = "Desc 2", Imagen = "img2.png" } },
                new LogroSocio { Id = 3, LogroId = 3, SocioId = 1, GimnasioId = 1, FechaObtenido = new DateTime(2024, 3, 1), Logro = new Logro { Id = 3, Nombre = "Logro 3", NombreClave = "logro3", Descripcion = "Desc 3", Imagen = "img3.png" } }
            };

            _mockRepositorio.Setup(r => r.ObtenerPorSocioYGimnasioAsync(1, 1))
                .ReturnsAsync(logrosSocio);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1, 1);

            // Assert
            resultado.Should().HaveCount(3);
            resultado.Select(l => l.LogroId).Should().Contain(new[] { 1L, 2L, 3L });
        }

        [Fact]
        public async Task Ejecutar_DebeMapearTodasLasPropiedades()
        {
            // Arrange
            var logrosSocio = new List<LogroSocio>
            {
                new LogroSocio
                {
                    Id = 1,
                    LogroId = 10,
                    SocioId = 1,
                    GimnasioId = 1,
                    FechaObtenido = new DateTime(2024, 5, 20),
                    Logro = new Logro
                    {
                        Id = 10,
                        Nombre = "Nombre Completo",
                        NombreClave = "nombre_clave",
                        Descripcion = "Descripción detallada",
                        Imagen = "ruta/imagen.png"
                    }
                }
            };

            _mockRepositorio.Setup(r => r.ObtenerPorSocioYGimnasioAsync(1, 1))
                .ReturnsAsync(logrosSocio);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1, 1);

            // Assert
            var dto = resultado.First();
            dto.LogroId.Should().Be(10);
            dto.Nombre.Should().Be("Nombre Completo");
            dto.NombreClave.Should().Be("nombre_clave");
            dto.Descripcion.Should().Be("Descripción detallada");
            dto.Imagen.Should().Be("ruta/imagen.png");
            dto.FechaOtorgado.Should().Be(new DateTime(2024, 5, 20));
        }

        [Fact]
        public async Task Ejecutar_DebeManejarDiferentesSocios()
        {
            // Arrange
            var logrosSocio1 = new List<LogroSocio>
            {
                new LogroSocio { Id = 1, LogroId = 1, SocioId = 1, GimnasioId = 1, FechaObtenido = DateTime.Now, Logro = new Logro { Id = 1, Nombre = "Logro Socio 1", NombreClave = "socio1", Descripcion = "Desc", Imagen = "img.png" } }
            };

            var logrosSocio2 = new List<LogroSocio>
            {
                new LogroSocio { Id = 2, LogroId = 2, SocioId = 2, GimnasioId = 1, FechaObtenido = DateTime.Now, Logro = new Logro { Id = 2, Nombre = "Logro Socio 2", NombreClave = "socio2", Descripcion = "Desc", Imagen = "img.png" } }
            };

            _mockRepositorio.Setup(r => r.ObtenerPorSocioYGimnasioAsync(1, 1))
                .ReturnsAsync(logrosSocio1);

            _mockRepositorio.Setup(r => r.ObtenerPorSocioYGimnasioAsync(2, 1))
                .ReturnsAsync(logrosSocio2);

            // Act
            var resultado1 = await _casoDeUso.Ejecutar(1, 1);
            var resultado2 = await _casoDeUso.Ejecutar(2, 1);

            // Assert
            resultado1.First().Nombre.Should().Be("Logro Socio 1");
            resultado2.First().Nombre.Should().Be("Logro Socio 2");
        }

        [Fact]
        public async Task Ejecutar_DebeManejarDiferentesGimnasios()
        {
            // Arrange
            var logrosGym1 = new List<LogroSocio>
            {
                new LogroSocio { Id = 1, LogroId = 1, SocioId = 1, GimnasioId = 1, FechaObtenido = DateTime.Now, Logro = new Logro { Id = 1, Nombre = "Logro Gym 1", NombreClave = "gym1", Descripcion = "Desc", Imagen = "img.png" } }
            };

            var logrosGym2 = new List<LogroSocio>
            {
                new LogroSocio { Id = 2, LogroId = 2, SocioId = 1, GimnasioId = 2, FechaObtenido = DateTime.Now, Logro = new Logro { Id = 2, Nombre = "Logro Gym 2", NombreClave = "gym2", Descripcion = "Desc", Imagen = "img.png" } }
            };

            _mockRepositorio.Setup(r => r.ObtenerPorSocioYGimnasioAsync(1, 1))
                .ReturnsAsync(logrosGym1);

            _mockRepositorio.Setup(r => r.ObtenerPorSocioYGimnasioAsync(1, 2))
                .ReturnsAsync(logrosGym2);

            // Act
            var resultado1 = await _casoDeUso.Ejecutar(1, 1);
            var resultado2 = await _casoDeUso.Ejecutar(1, 2);

            // Assert
            resultado1.First().Nombre.Should().Be("Logro Gym 1");
            resultado2.First().Nombre.Should().Be("Logro Gym 2");
        }
    }
}
