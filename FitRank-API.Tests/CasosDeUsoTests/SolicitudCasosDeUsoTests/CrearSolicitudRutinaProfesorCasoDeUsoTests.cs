using FitRank_API.Application.CasosDeUso.SolicitudCasosDeUso;
using FitRank_API.Application.DTOs.SolicitudDTO;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using FluentAssertions;
using Moq;

namespace FitRank_API.Tests.CasosDeUsoTests.SolicitudCasosDeUsoTests
{
    public class CrearSolicitudRutinaProfesorCasoDeUsoTests
    {
        private readonly Mock<ISolicitudRutinaProfesorRepositorio> _mockRepo;
        private readonly CrearSolicitudRutinaProfesorCasoDeUso _casoDeUso;

        public CrearSolicitudRutinaProfesorCasoDeUsoTests()
        {
            _mockRepo = new Mock<ISolicitudRutinaProfesorRepositorio>();
            _casoDeUso = new CrearSolicitudRutinaProfesorCasoDeUso(_mockRepo.Object);
        }

        [Fact]
        public async Task EjecutarAsync_DebeCrearSolicitudCorrectamente()
        {
            // Arrange
            var dto = new CrearSolicitudRutinaProfesorDTO
            {
                MensajeSocio = "Necesito una rutina personalizada",
                NombreSocio = "Juan Pérez",
                Edad = 30,
                PesoKg = 75.5,
                AlturaCm = 175,
                Nivel = "Intermedio",
                SesionesPorSemana = 4,
                MinutosPorSesion = 60,
                Objetivo = "Aumento de masa muscular",
                CalidadAlimentacion = 3,
                HorasSuenio = 7,
                DolorLumbar = false,
                DolorRodilla = false,
                DolorHombro = false,
                CirugiaReciente = false,
                Sincope = false,
                Embarazo = false,
                Hipertension = false,
                HipertensionControlada = false,
                Diabetes = false,
                DolorToracico = false,
                FrecuenciaCardiacaReposo = 65
            };

            var socioId = 1L;

            _mockRepo.Setup(r => r.AgregarAsync(It.IsAny<SolicitudRutinaProfesor>()))
                .Callback<SolicitudRutinaProfesor>(s => s.Id = 10)
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _casoDeUso.EjecutarAsync(dto, socioId);

            // Assert
            resultado.Should().Be(10);
            _mockRepo.Verify(r => r.AgregarAsync(It.Is<SolicitudRutinaProfesor>(s =>
                s.SocioId == socioId &&
                s.MensajeSocio == dto.MensajeSocio &&
                s.Estado == EstadoSolicitud.Pendiente &&
                s.Nivel == dto.Nivel
            )), Times.Once);
        }

        [Fact]
        public async Task EjecutarAsync_DebeAsignarEstadoPendiente()
        {
            // Arrange
            var dto = new CrearSolicitudRutinaProfesorDTO
            {
                NombreSocio = "Test",
                Edad = 25,
                Nivel = "Principiante"
            };

            SolicitudRutinaProfesor? solicitudCreada = null;
            _mockRepo.Setup(r => r.AgregarAsync(It.IsAny<SolicitudRutinaProfesor>()))
                .Callback<SolicitudRutinaProfesor>(s =>
                {
                    solicitudCreada = s;
                    s.Id = 1;
                })
                .Returns(Task.CompletedTask);

            // Act
            await _casoDeUso.EjecutarAsync(dto, 1L);

            // Assert
            solicitudCreada.Should().NotBeNull();
            solicitudCreada!.Estado.Should().Be(EstadoSolicitud.Pendiente);
        }

        [Fact]
        public async Task EjecutarAsync_DebeCopiarTodosLosDatosMedicos()
        {
            // Arrange
            var dto = new CrearSolicitudRutinaProfesorDTO
            {
                NombreSocio = "Test",
                Edad = 40,
                Nivel = "Avanzado",
                DolorLumbar = true,
                DolorRodilla = true,
                DolorHombro = false,
                CirugiaReciente = true,
                Sincope = false,
                Embarazo = false,
                Hipertension = true,
                HipertensionControlada = true,
                Diabetes = false,
                DolorToracico = false,
                FrecuenciaCardiacaReposo = 70
            };

            SolicitudRutinaProfesor? solicitudCreada = null;
            _mockRepo.Setup(r => r.AgregarAsync(It.IsAny<SolicitudRutinaProfesor>()))
                .Callback<SolicitudRutinaProfesor>(s => solicitudCreada = s)
                .Returns(Task.CompletedTask);

            // Act
            await _casoDeUso.EjecutarAsync(dto, 1L);

            // Assert
            solicitudCreada.Should().NotBeNull();
            solicitudCreada!.DolorLumbar.Should().BeTrue();
            solicitudCreada.DolorRodilla.Should().BeTrue();
            solicitudCreada.DolorHombro.Should().BeFalse();
            solicitudCreada.CirugiaReciente.Should().BeTrue();
            solicitudCreada.Hipertension.Should().BeTrue();
            solicitudCreada.HipertensionControlada.Should().BeTrue();
            solicitudCreada.FrecuenciaCardiacaReposo.Should().Be(70);
        }

        [Fact]
        public async Task EjecutarAsync_DebeCopiarDatosDeEntrenamiento()
        {
            // Arrange
            var dto = new CrearSolicitudRutinaProfesorDTO
            {
                NombreSocio = "María",
                Edad = 28,
                Nivel = "Intermedio",
                SesionesPorSemana = 5,
                MinutosPorSesion = 90,
                Objetivo = "Pérdida de grasa",
                CalidadAlimentacion = 5,
                HorasSuenio = 8
            };

            SolicitudRutinaProfesor? solicitudCreada = null;
            _mockRepo.Setup(r => r.AgregarAsync(It.IsAny<SolicitudRutinaProfesor>()))
                .Callback<SolicitudRutinaProfesor>(s => solicitudCreada = s)
                .Returns(Task.CompletedTask);

            // Act
            await _casoDeUso.EjecutarAsync(dto, 1L);

            // Assert
            solicitudCreada.Should().NotBeNull();
            solicitudCreada!.SesionesPorSemana.Should().Be(5);
            solicitudCreada.MinutosPorSesion.Should().Be(90);
            solicitudCreada.Objetivo.Should().Be("Pérdida de grasa");
            solicitudCreada.CalidadAlimentacion.Should().Be(5);
            solicitudCreada.HorasSuenio.Should().Be(8);
        }

        [Fact]
        public async Task EjecutarAsync_DebeCopiarDatosAntropometricos()
        {
            // Arrange
            var dto = new CrearSolicitudRutinaProfesorDTO
            {
                NombreSocio = "Carlos",
                Edad = 35,
                PesoKg = 82.3,
                AlturaCm = 180,
                Nivel = "Avanzado"
            };

            SolicitudRutinaProfesor? solicitudCreada = null;
            _mockRepo.Setup(r => r.AgregarAsync(It.IsAny<SolicitudRutinaProfesor>()))
                .Callback<SolicitudRutinaProfesor>(s => solicitudCreada = s)
                .Returns(Task.CompletedTask);

            // Act
            await _casoDeUso.EjecutarAsync(dto, 1L);

            // Assert
            solicitudCreada.Should().NotBeNull();
            solicitudCreada!.NombreSocio.Should().Be("Carlos");
            solicitudCreada.Edad.Should().Be(35);
            solicitudCreada.PesoKg.Should().Be(82.3);
            solicitudCreada.AlturaCm.Should().Be(180);
        }

        [Fact]
        public async Task EjecutarAsync_DebeAsociarSocioCorrectamente()
        {
            // Arrange
            var dto = new CrearSolicitudRutinaProfesorDTO
            {
                NombreSocio = "Test",
                Edad = 30,
                Nivel = "Intermedio"
            };

            var socioId = 25L;

            SolicitudRutinaProfesor? solicitudCreada = null;
            _mockRepo.Setup(r => r.AgregarAsync(It.IsAny<SolicitudRutinaProfesor>()))
                .Callback<SolicitudRutinaProfesor>(s => solicitudCreada = s)
                .Returns(Task.CompletedTask);

            // Act
            await _casoDeUso.EjecutarAsync(dto, socioId);

            // Assert
            solicitudCreada.Should().NotBeNull();
            solicitudCreada!.SocioId.Should().Be(socioId);
        }

        [Fact]
        public async Task EjecutarAsync_DebeRetornarIdDeSolicitudCreada()
        {
            // Arrange
            var dto = new CrearSolicitudRutinaProfesorDTO
            {
                NombreSocio = "Test",
                Edad = 30,
                Nivel = "Intermedio"
            };

            _mockRepo.Setup(r => r.AgregarAsync(It.IsAny<SolicitudRutinaProfesor>()))
                .Callback<SolicitudRutinaProfesor>(s => s.Id = 42)
                .Returns(Task.CompletedTask);

            // Act
            var resultado = await _casoDeUso.EjecutarAsync(dto, 1L);

            // Assert
            resultado.Should().Be(42);
        }
    }
}
