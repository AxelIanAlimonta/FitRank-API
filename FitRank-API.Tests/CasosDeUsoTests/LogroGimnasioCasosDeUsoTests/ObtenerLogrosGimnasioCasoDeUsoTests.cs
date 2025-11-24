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

        [Fact]
        public async Task Ejecutar_DebeIncluirEstadoActivoEnDTO()
        {
            // Arrange
            var logrosGimnasio = new List<LogroGimnasio>
            {
                new LogroGimnasio
                {
                    Id = 1,
                    GimnasioId = 1,
                    LogroId = 10,
                    EstaActivo = true,
                    Logro = new Logro
                    {
                        Id = 10,
                        Nombre = "Activo",
                        NombreClave = "activo",
                        Descripcion = "Logro activo",
                        Imagen = "activo.png"
                    }
                },
                new LogroGimnasio
                {
                    Id = 2,
                    GimnasioId = 1,
                    LogroId = 20,
                    EstaActivo = false,
                    Logro = new Logro
                    {
                        Id = 20,
                        Nombre = "Inactivo",
                        NombreClave = "inactivo",
                        Descripcion = "Logro inactivo",
                        Imagen = "inactivo.png"
                    }
                }
            };

            _mockRepositorio.Setup(r => r.ObtenerPorGimnasioAsync(1))
                .ReturnsAsync(logrosGimnasio);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1);

            // Assert
            resultado.First().EstaHabilitado.Should().BeTrue();
            resultado.Last().EstaHabilitado.Should().BeFalse();
        }

        [Fact]
        public async Task Ejecutar_DebeMapearTodasLasPropiedadesDelLogro()
        {
            // Arrange
            var logrosGimnasio = new List<LogroGimnasio>
            {
                new LogroGimnasio
                {
                    Id = 1,
                    GimnasioId = 1,
                    LogroId = 10,
                    EstaActivo = true,
                    Logro = new Logro
                    {
                        Id = 10,
                        Nombre = "Nombre Test",
                        NombreClave = "nombre_test",
                        Descripcion = "Descripción Test",
                        Imagen = "ruta/imagen.png"
                    }
                }
            };

            _mockRepositorio.Setup(r => r.ObtenerPorGimnasioAsync(1))
                .ReturnsAsync(logrosGimnasio);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1);

            // Assert
            var dto = resultado.First();
            dto.Nombre.Should().Be("Nombre Test");
            dto.NombreClave.Should().Be("nombre_test");
            dto.Descripcion.Should().Be("Descripción Test");
            dto.Imagen.Should().Be("ruta/imagen.png");
        }

        [Fact]
        public async Task Ejecutar_DebeManejarVariosLogros()
        {
            // Arrange
            var logrosGimnasio = new List<LogroGimnasio>();
            for (int i = 1; i <= 5; i++)
            {
                logrosGimnasio.Add(new LogroGimnasio
                {
                    Id = i,
                    GimnasioId = 1,
                    LogroId = i * 10,
                    EstaActivo = i % 2 == 0,
                    Logro = new Logro
                    {
                        Id = i * 10,
                        Nombre = $"Logro {i}",
                        NombreClave = $"logro_{i}",
                        Descripcion = $"Descripción {i}",
                        Imagen = $"img{i}.png"
                    }
                });
            }

            _mockRepositorio.Setup(r => r.ObtenerPorGimnasioAsync(1))
                .ReturnsAsync(logrosGimnasio);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1);

            // Assert
            resultado.Should().HaveCount(5);
            resultado.Select(l => l.LogroId).Should().Contain(new[] { 10L, 20L, 30L, 40L, 50L });
        }

        [Fact]
        public async Task Ejecutar_DebeManejarDiferentesGimnasios()
        {
            // Arrange
            var logrosGym1 = new List<LogroGimnasio>
            {
                new LogroGimnasio
                {
                    Id = 1,
                    GimnasioId = 1,
                    LogroId = 10,
                    EstaActivo = true,
                    Logro = new Logro { Id = 10, Nombre = "Logro Gym 1", NombreClave = "gym1", Descripcion = "Desc", Imagen = "img.png" }
                }
            };

            var logrosGym2 = new List<LogroGimnasio>
            {
                new LogroGimnasio
                {
                    Id = 2,
                    GimnasioId = 2,
                    LogroId = 20,
                    EstaActivo = true,
                    Logro = new Logro { Id = 20, Nombre = "Logro Gym 2", NombreClave = "gym2", Descripcion = "Desc", Imagen = "img.png" }
                }
            };

            _mockRepositorio.Setup(r => r.ObtenerPorGimnasioAsync(1))
                .ReturnsAsync(logrosGym1);

            _mockRepositorio.Setup(r => r.ObtenerPorGimnasioAsync(2))
                .ReturnsAsync(logrosGym2);

            // Act
            var resultado1 = await _casoDeUso.Ejecutar(1);
            var resultado2 = await _casoDeUso.Ejecutar(2);

            // Assert
            resultado1.First().Nombre.Should().Be("Logro Gym 1");
            resultado2.First().Nombre.Should().Be("Logro Gym 2");
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarTodosLosLogros_ActivosEInactivos()
        {
            // Arrange
            var logrosGimnasio = new List<LogroGimnasio>
            {
                new LogroGimnasio
                {
                    Id = 1,
                    GimnasioId = 1,
                    LogroId = 10,
                    EstaActivo = true,
                    Logro = new Logro { Id = 10, Nombre = "Activo", NombreClave = "activo", Descripcion = "Desc", Imagen = "img.png" }
                },
                new LogroGimnasio
                {
                    Id = 2,
                    GimnasioId = 1,
                    LogroId = 20,
                    EstaActivo = false,
                    Logro = new Logro { Id = 20, Nombre = "Inactivo", NombreClave = "inactivo", Descripcion = "Desc", Imagen = "img.png" }
                }
            };

            _mockRepositorio.Setup(r => r.ObtenerPorGimnasioAsync(1))
                .ReturnsAsync(logrosGimnasio);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1);

            // Assert
            resultado.Should().HaveCount(2);
            resultado.Should().Contain(l => l.EstaHabilitado == true);
            resultado.Should().Contain(l => l.EstaHabilitado == false);
        }

        [Fact]
        public async Task Ejecutar_DebeMapearGimnasioIdYLogroIdCorrectamente()
        {
            // Arrange
            var logrosGimnasio = new List<LogroGimnasio>
            {
                new LogroGimnasio
                {
                    Id = 1,
                    GimnasioId = 5,
                    LogroId = 100,
                    EstaActivo = true,
                    Logro = new Logro { Id = 100, Nombre = "Test", NombreClave = "test", Descripcion = "Desc", Imagen = "img.png" }
                }
            };

            _mockRepositorio.Setup(r => r.ObtenerPorGimnasioAsync(5))
                .ReturnsAsync(logrosGimnasio);

            // Act
            var resultado = await _casoDeUso.Ejecutar(5);

            // Assert
            resultado.First().GimnasioId.Should().Be(5);
            resultado.First().LogroId.Should().Be(100);
        }

        [Fact]
        public async Task Ejecutar_DebeUsarMapper_ParaConvertirEntidadesADTOs()
        {
            // Arrange
            var logrosGimnasio = new List<LogroGimnasio>
            {
                new LogroGimnasio
                {
                    Id = 1,
                    GimnasioId = 1,
                    LogroId = 10,
                    EstaActivo = true,
                    Logro = new Logro
                    {
                        Id = 10,
                        Nombre = "Test Mapper",
                        NombreClave = "test_mapper",
                        Descripcion = "Prueba de AutoMapper",
                        Imagen = "mapper.png"
                    }
                }
            };

            _mockRepositorio.Setup(r => r.ObtenerPorGimnasioAsync(1))
                .ReturnsAsync(logrosGimnasio);

            // Act
            var resultado = await _casoDeUso.Ejecutar(1);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().AllBeOfType<FitRank_API.Application.DTOs.LogroGimnasioDTOs.LogroGimnasioDTO>();
        }
    }
}
