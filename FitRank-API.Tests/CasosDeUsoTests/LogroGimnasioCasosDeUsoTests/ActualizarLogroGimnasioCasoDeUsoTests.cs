using AutoMapper;
using FitRank_API.Application.CasosDeUso.LogroGimnasioCasosDeUso;
using FitRank_API.Application.DTOs.LogroGimnasioDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FluentAssertions;
using Moq;
using Xunit;

namespace FitRank_API.Tests.CasosDeUsoTests.LogroGimnasioCasosDeUsoTests
{
    public class ActualizarLogroGimnasioCasoDeUsoTests
    {
        private readonly Mock<ILogroGimnasioRepositorio> _mockLogroGimnasioRepo;
        private readonly Mock<ILogroRepositorio> _mockLogroRepo;
        private readonly Mock<IMapper> _mockMapper;
        private readonly ActualizarLogroGimnasioCasoDeUso _casoDeUso;

        public ActualizarLogroGimnasioCasoDeUsoTests()
        {
            _mockLogroGimnasioRepo = new Mock<ILogroGimnasioRepositorio>();
            _mockLogroRepo = new Mock<ILogroRepositorio>();
            _mockMapper = new Mock<IMapper>();
            _casoDeUso = new ActualizarLogroGimnasioCasoDeUso(
                _mockLogroGimnasioRepo.Object,
                _mockLogroRepo.Object,
                _mockMapper.Object);
        }

        [Fact]
        public async Task Ejecutar_DebeCrearNuevaConfiguracion_CuandoNoExiste()
        {
            // Arrange
            var dto = new ActualizarLogroGimnasioDTO
            {
                GimnasioId = 1,
                LogroId = 10,
                EstaActivo = true
            };

            var logro = new Logro
            {
                Id = 10,
                Nombre = "Logro Test",
                NombreClave = "logro_test",
                Descripcion = "Descripción test",
                Imagen = "test.png"
            };

            var nuevaEntidad = new LogroGimnasio
            {
                Id = 1,
                GimnasioId = 1,
                LogroId = 10,
                EstaActivo = true
            };

            var resultadoDTO = new LogroGimnasioDTO
            {
                GimnasioId = 1,
                LogroId = 10,
                EstaHabilitado = true,
                Nombre = "Logro Test",
                NombreClave = "logro_test",
                Descripcion = "Descripción test",
                Imagen = "test.png"
            };

            _mockLogroRepo.Setup(r => r.ObtenerLogroPorId(10))
                .ReturnsAsync(logro);

            _mockLogroGimnasioRepo.Setup(r => r.ObtenerPorGimnasioYLogroAsync(1, 10))
                .ReturnsAsync((LogroGimnasio)null);

            _mockMapper.Setup(m => m.Map<LogroGimnasio>(dto))
                .Returns(nuevaEntidad);

            _mockLogroGimnasioRepo.Setup(r => r.CrearAsync(It.IsAny<LogroGimnasio>()))
                .ReturnsAsync(nuevaEntidad);

            _mockMapper.Setup(m => m.Map<LogroGimnasioDTO>(It.IsAny<LogroGimnasio>()))
                .Returns(resultadoDTO);

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Should().NotBeNull();
            resultado!.GimnasioId.Should().Be(1);
            resultado.LogroId.Should().Be(10);
            resultado.EstaHabilitado.Should().BeTrue();
            _mockLogroGimnasioRepo.Verify(r => r.CrearAsync(It.IsAny<LogroGimnasio>()), Times.Once);
            _mockLogroGimnasioRepo.Verify(r => r.ActualizarAsync(It.IsAny<LogroGimnasio>()), Times.Never);
        }

        [Fact]
        public async Task Ejecutar_DebeActualizarConfiguracion_CuandoYaExiste()
        {
            // Arrange
            var dto = new ActualizarLogroGimnasioDTO
            {
                GimnasioId = 1,
                LogroId = 10,
                EstaActivo = false
            };

            var logro = new Logro
            {
                Id = 10,
                Nombre = "Logro Existente",
                NombreClave = "existente",
                Descripcion = "Descripción",
                Imagen = "img.png"
            };

            var existente = new LogroGimnasio
            {
                Id = 5,
                GimnasioId = 1,
                LogroId = 10,
                EstaActivo = true
            };

            var resultadoDTO = new LogroGimnasioDTO
            {
                GimnasioId = 1,
                LogroId = 10,
                EstaHabilitado = false,
                Nombre = "Logro Existente",
                NombreClave = "existente",
                Descripcion = "Descripción",
                Imagen = "img.png"
            };

            _mockLogroRepo.Setup(r => r.ObtenerLogroPorId(10))
                .ReturnsAsync(logro);

            _mockLogroGimnasioRepo.Setup(r => r.ObtenerPorGimnasioYLogroAsync(1, 10))
                .ReturnsAsync(existente);

            _mockLogroGimnasioRepo.Setup(r => r.ActualizarAsync(existente))
                .ReturnsAsync(existente);

            _mockMapper.Setup(m => m.Map<LogroGimnasioDTO>(It.IsAny<LogroGimnasio>()))
                .Returns(resultadoDTO);

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Should().NotBeNull();
            resultado!.EstaHabilitado.Should().BeFalse();
            existente.EstaActivo.Should().BeFalse();
            _mockLogroGimnasioRepo.Verify(r => r.ActualizarAsync(existente), Times.Once);
            _mockLogroGimnasioRepo.Verify(r => r.CrearAsync(It.IsAny<LogroGimnasio>()), Times.Never);
        }

        [Fact]
        public async Task Ejecutar_DebeRetornarNull_CuandoLogroNoExiste()
        {
            // Arrange
            var dto = new ActualizarLogroGimnasioDTO
            {
                GimnasioId = 1,
                LogroId = 999,
                EstaActivo = true
            };

            _mockLogroRepo.Setup(r => r.ObtenerLogroPorId(999))
                .ReturnsAsync((Logro)null);

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Should().BeNull();
            _mockLogroGimnasioRepo.Verify(r => r.ObtenerPorGimnasioYLogroAsync(It.IsAny<long>(), It.IsAny<long>()), Times.Never);
            _mockLogroGimnasioRepo.Verify(r => r.CrearAsync(It.IsAny<LogroGimnasio>()), Times.Never);
            _mockLogroGimnasioRepo.Verify(r => r.ActualizarAsync(It.IsAny<LogroGimnasio>()), Times.Never);
        }

        [Fact]
        public async Task Ejecutar_DebeAsignarLogroAEntidadCreada()
        {
            // Arrange
            var dto = new ActualizarLogroGimnasioDTO
            {
                GimnasioId = 1,
                LogroId = 10,
                EstaActivo = true
            };

            var logro = new Logro
            {
                Id = 10,
                Nombre = "Logro Test",
                NombreClave = "test",
                Descripcion = "Desc",
                Imagen = "img.png"
            };

            var nuevaEntidad = new LogroGimnasio
            {
                Id = 1,
                GimnasioId = 1,
                LogroId = 10,
                EstaActivo = true
            };

            _mockLogroRepo.Setup(r => r.ObtenerLogroPorId(10))
                .ReturnsAsync(logro);

            _mockLogroGimnasioRepo.Setup(r => r.ObtenerPorGimnasioYLogroAsync(1, 10))
                .ReturnsAsync((LogroGimnasio)null);

            _mockMapper.Setup(m => m.Map<LogroGimnasio>(dto))
                .Returns(nuevaEntidad);

            _mockLogroGimnasioRepo.Setup(r => r.CrearAsync(It.IsAny<LogroGimnasio>()))
                .ReturnsAsync(nuevaEntidad);

            _mockMapper.Setup(m => m.Map<LogroGimnasioDTO>(It.Is<LogroGimnasio>(lg => lg.Logro != null)))
                .Returns(new LogroGimnasioDTO { GimnasioId = 1, LogroId = 10, Nombre = "Logro Test" });

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Should().NotBeNull();
            _mockMapper.Verify(m => m.Map<LogroGimnasioDTO>(It.Is<LogroGimnasio>(lg => lg.Logro == logro)), Times.Once);
        }

        [Fact]
        public async Task Ejecutar_DebeAsignarLogroAEntidadActualizada()
        {
            // Arrange
            var dto = new ActualizarLogroGimnasioDTO
            {
                GimnasioId = 1,
                LogroId = 10,
                EstaActivo = true
            };

            var logro = new Logro
            {
                Id = 10,
                Nombre = "Logro Test",
                NombreClave = "test",
                Descripcion = "Desc",
                Imagen = "img.png"
            };

            var existente = new LogroGimnasio
            {
                Id = 5,
                GimnasioId = 1,
                LogroId = 10,
                EstaActivo = false
            };

            _mockLogroRepo.Setup(r => r.ObtenerLogroPorId(10))
                .ReturnsAsync(logro);

            _mockLogroGimnasioRepo.Setup(r => r.ObtenerPorGimnasioYLogroAsync(1, 10))
                .ReturnsAsync(existente);

            _mockLogroGimnasioRepo.Setup(r => r.ActualizarAsync(existente))
                .ReturnsAsync(existente);

            _mockMapper.Setup(m => m.Map<LogroGimnasioDTO>(It.Is<LogroGimnasio>(lg => lg.Logro != null)))
                .Returns(new LogroGimnasioDTO { GimnasioId = 1, LogroId = 10, Nombre = "Logro Test" });

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado.Should().NotBeNull();
            _mockMapper.Verify(m => m.Map<LogroGimnasioDTO>(It.Is<LogroGimnasio>(lg => lg.Logro == logro)), Times.Once);
        }

        [Fact]
        public async Task Ejecutar_DebeAsignarGimnasioIdYLogroIdCorrectamente()
        {
            // Arrange
            var dto = new ActualizarLogroGimnasioDTO
            {
                GimnasioId = 5,
                LogroId = 20,
                EstaActivo = true
            };

            var logro = new Logro { Id = 20, Nombre = "Logro", NombreClave = "logro", Descripcion = "Desc", Imagen = "img.png" };
            var nuevaEntidad = new LogroGimnasio { Id = 1, EstaActivo = true };

            _mockLogroRepo.Setup(r => r.ObtenerLogroPorId(20))
                .ReturnsAsync(logro);

            _mockLogroGimnasioRepo.Setup(r => r.ObtenerPorGimnasioYLogroAsync(5, 20))
                .ReturnsAsync((LogroGimnasio)null);

            _mockMapper.Setup(m => m.Map<LogroGimnasio>(dto))
                .Returns(nuevaEntidad);

            _mockLogroGimnasioRepo.Setup(r => r.CrearAsync(It.IsAny<LogroGimnasio>()))
                .ReturnsAsync(nuevaEntidad);

            _mockMapper.Setup(m => m.Map<LogroGimnasioDTO>(It.IsAny<LogroGimnasio>()))
                .Returns(new LogroGimnasioDTO());

            // Act
            await _casoDeUso.Ejecutar(dto);

            // Assert
            nuevaEntidad.GimnasioId.Should().Be(5);
            nuevaEntidad.LogroId.Should().Be(20);
        }

        [Fact]
        public async Task Ejecutar_DebeActivarLogro()
        {
            // Arrange
            var dto = new ActualizarLogroGimnasioDTO
            {
                GimnasioId = 1,
                LogroId = 10,
                EstaActivo = true
            };

            var logro = new Logro { Id = 10, Nombre = "Test", NombreClave = "test", Descripcion = "Desc", Imagen = "img.png" };
            var existente = new LogroGimnasio { Id = 1, GimnasioId = 1, LogroId = 10, EstaActivo = false };

            _mockLogroRepo.Setup(r => r.ObtenerLogroPorId(10))
                .ReturnsAsync(logro);

            _mockLogroGimnasioRepo.Setup(r => r.ObtenerPorGimnasioYLogroAsync(1, 10))
                .ReturnsAsync(existente);

            _mockLogroGimnasioRepo.Setup(r => r.ActualizarAsync(existente))
                .ReturnsAsync(existente);

            _mockMapper.Setup(m => m.Map<LogroGimnasioDTO>(It.IsAny<LogroGimnasio>()))
                .Returns(new LogroGimnasioDTO { EstaHabilitado = true });

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            existente.EstaActivo.Should().BeTrue();
            resultado!.EstaHabilitado.Should().BeTrue();
        }

        [Fact]
        public async Task Ejecutar_DebeDesactivarLogro()
        {
            // Arrange
            var dto = new ActualizarLogroGimnasioDTO
            {
                GimnasioId = 1,
                LogroId = 10,
                EstaActivo = false
            };

            var logro = new Logro { Id = 10, Nombre = "Test", NombreClave = "test", Descripcion = "Desc", Imagen = "img.png" };
            var existente = new LogroGimnasio { Id = 1, GimnasioId = 1, LogroId = 10, EstaActivo = true };

            _mockLogroRepo.Setup(r => r.ObtenerLogroPorId(10))
                .ReturnsAsync(logro);

            _mockLogroGimnasioRepo.Setup(r => r.ObtenerPorGimnasioYLogroAsync(1, 10))
                .ReturnsAsync(existente);

            _mockLogroGimnasioRepo.Setup(r => r.ActualizarAsync(existente))
                .ReturnsAsync(existente);

            _mockMapper.Setup(m => m.Map<LogroGimnasioDTO>(It.IsAny<LogroGimnasio>()))
                .Returns(new LogroGimnasioDTO { EstaHabilitado = false });

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            existente.EstaActivo.Should().BeFalse();
            resultado!.EstaHabilitado.Should().BeFalse();
        }

        [Fact]
        public async Task Ejecutar_DebeLlamarRepositoriosEnOrdenCorrecto()
        {
            // Arrange
            var dto = new ActualizarLogroGimnasioDTO
            {
                GimnasioId = 1,
                LogroId = 10,
                EstaActivo = true
            };

            var logro = new Logro { Id = 10, Nombre = "Test", NombreClave = "test", Descripcion = "Desc", Imagen = "img.png" };
            var nuevaEntidad = new LogroGimnasio { Id = 1, GimnasioId = 1, LogroId = 10, EstaActivo = true };

            var callOrder = new List<string>();

            _mockLogroRepo.Setup(r => r.ObtenerLogroPorId(10))
                .ReturnsAsync(logro)
                .Callback(() => callOrder.Add("ObtenerLogro"));

            _mockLogroGimnasioRepo.Setup(r => r.ObtenerPorGimnasioYLogroAsync(1, 10))
                .ReturnsAsync((LogroGimnasio)null)
                .Callback(() => callOrder.Add("ObtenerLogroGimnasio"));

            _mockMapper.Setup(m => m.Map<LogroGimnasio>(dto))
                .Returns(nuevaEntidad);

            _mockLogroGimnasioRepo.Setup(r => r.CrearAsync(It.IsAny<LogroGimnasio>()))
                .ReturnsAsync(nuevaEntidad)
                .Callback(() => callOrder.Add("Crear"));

            _mockMapper.Setup(m => m.Map<LogroGimnasioDTO>(It.IsAny<LogroGimnasio>()))
                .Returns(new LogroGimnasioDTO());

            // Act
            await _casoDeUso.Ejecutar(dto);

            // Assert
            callOrder.Should().ContainInOrder("ObtenerLogro", "ObtenerLogroGimnasio", "Crear");
        }

        [Fact]
        public async Task Ejecutar_DebeIncluirDatosDelLogroEnDTO()
        {
            // Arrange
            var dto = new ActualizarLogroGimnasioDTO
            {
                GimnasioId = 1,
                LogroId = 10,
                EstaActivo = true
            };

            var logro = new Logro
            {
                Id = 10,
                Nombre = "Nombre Logro",
                NombreClave = "nombre_logro",
                Descripcion = "Descripción detallada",
                Imagen = "ruta/imagen.png"
            };

            var nuevaEntidad = new LogroGimnasio { Id = 1, GimnasioId = 1, LogroId = 10, EstaActivo = true };

            var resultadoDTO = new LogroGimnasioDTO
            {
                GimnasioId = 1,
                LogroId = 10,
                EstaHabilitado = true,
                Nombre = "Nombre Logro",
                NombreClave = "nombre_logro",
                Descripcion = "Descripción detallada",
                Imagen = "ruta/imagen.png"
            };

            _mockLogroRepo.Setup(r => r.ObtenerLogroPorId(10))
                .ReturnsAsync(logro);

            _mockLogroGimnasioRepo.Setup(r => r.ObtenerPorGimnasioYLogroAsync(1, 10))
                .ReturnsAsync((LogroGimnasio)null);

            _mockMapper.Setup(m => m.Map<LogroGimnasio>(dto))
                .Returns(nuevaEntidad);

            _mockLogroGimnasioRepo.Setup(r => r.CrearAsync(It.IsAny<LogroGimnasio>()))
                .ReturnsAsync(nuevaEntidad);

            _mockMapper.Setup(m => m.Map<LogroGimnasioDTO>(It.IsAny<LogroGimnasio>()))
                .Returns(resultadoDTO);

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto);

            // Assert
            resultado!.Nombre.Should().Be("Nombre Logro");
            resultado.NombreClave.Should().Be("nombre_logro");
            resultado.Descripcion.Should().Be("Descripción detallada");
            resultado.Imagen.Should().Be("ruta/imagen.png");
        }
    }
}
