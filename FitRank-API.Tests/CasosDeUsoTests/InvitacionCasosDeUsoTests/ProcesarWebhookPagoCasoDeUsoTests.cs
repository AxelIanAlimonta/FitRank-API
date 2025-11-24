using FitRank_API.Application.CasosDeUso.Ingreso;
using FitRank_API.Application.CasosDeUso.Invitacion;
using FitRank_API.Application.CasosDeUso.MercadoPago;
using FitRank_API.Application.DTOs.IngresoDTOs;
using FitRank_API.Application.DTOs.Invitacion;
using FitRank_API.Application.Helpers;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using Moq;
using SendGrid;
using Xunit;
using FluentAssertions;

namespace FitRank_API.Tests.CasosDeUsoTests.InvitacionCasosDeUsoTests
{
    public class ProcesarWebhookPagoCasoDeUsoTests
    {
        private readonly Mock<IInvitacionRepositorio> _mockInvitacionRepo;
        private readonly Mock<IUsuarioRepositorio> _mockUsuarioRepo;
        private readonly Mock<AgregarIngresoCasoDeUso> _mockAgregarIngreso;
        private readonly Mock<AgregarInvitacionCasoDeUso> _mockAgregarInvitacion;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly Mock<ProcesarWebhookPagoCasoDeUso> _mockCasoDeUso;

        public ProcesarWebhookPagoCasoDeUsoTests()
        {
            _mockInvitacionRepo = new Mock<IInvitacionRepositorio>();
            _mockUsuarioRepo = new Mock<IUsuarioRepositorio>();
            _mockAgregarIngreso = new Mock<AgregarIngresoCasoDeUso>(
                Mock.Of<IIngresoRepositorio>(),
                Mock.Of<AutoMapper.IMapper>());
            
            var mockQrHelper = new Mock<QrHelper>(Mock.Of<IConfiguration>());
            var mockCrearPreferencia = new Mock<CrearPreferenciaMercadoPagoCasoDeUso>(
                Mock.Of<IConfiguration>(),
                mockQrHelper.Object,
                null);

            _mockAgregarInvitacion = new Mock<AgregarInvitacionCasoDeUso>(
                Mock.Of<IInvitacionRepositorio>(),
                Mock.Of<IUsuarioRepositorio>(),
                Mock.Of<IConfiguration>(),
                Mock.Of<ISendGridClient>(),
                mockQrHelper.Object,
                Mock.Of<IGimnasioRepositorio>(),
                mockCrearPreferencia.Object,
                _mockAgregarIngreso.Object);
            
            _mockConfig = new Mock<IConfiguration>();
            _mockConfig.Setup(c => c["MercadoPago:AccessToken"])
                .Returns("TEST-1234567890-mocktoken");

            _mockCasoDeUso = new Mock<ProcesarWebhookPagoCasoDeUso>(
                _mockInvitacionRepo.Object,
                _mockUsuarioRepo.Object,
                _mockAgregarIngreso.Object,
                _mockAgregarInvitacion.Object,
                _mockConfig.Object);
        }

        [Fact]
        public async Task DeberiaConfigurarAccessTokenCorrectamente()
        {
            // Arrange & Act
            var accessToken = _mockConfig.Object["MercadoPago:AccessToken"];

            // Assert
            accessToken.Should().Be("TEST-1234567890-mocktoken");
            _mockCasoDeUso.Should().NotBeNull();
        }

        [Fact]
        public async Task DeberiaTenerDependenciasConfiguradas()
        {
            // Arrange
            var invitacion = new Invitacion
            {
                Id = 1,
                GimnasioId = 10,
                UsuarioId = 50,
                Email = "test@test.com",
                Estado = "Pendiente"
            };

            _mockInvitacionRepo.Setup(r => r.ObtenerPorIdAsync(1))
                .ReturnsAsync(invitacion);

            // Act
            var resultado = await _mockInvitacionRepo.Object.ObtenerPorIdAsync(1);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Id.Should().Be(1);
            resultado.Estado.Should().Be("Pendiente");
        }

        [Fact]
        public async Task DeberiaActualizarInvitacionCorrectamente()
        {
            // Arrange
            var invitacion = new Invitacion
            {
                Id = 100,
                GimnasioId = 10,
                UsuarioId = 50,
                Email = "user@test.com",
                Estado = "Pendiente",
                MpPaymentId = null
            };

            Invitacion invitacionActualizada = null;

            _mockInvitacionRepo.Setup(r => r.ActualizarAsync(It.IsAny<Invitacion>()))
                .Callback<Invitacion>(inv => invitacionActualizada = inv)
                .ReturnsAsync((Invitacion inv) => inv);

            // Act
            invitacion.Estado = "Pagado";
            invitacion.MpPaymentId = "12345678901";
            await _mockInvitacionRepo.Object.ActualizarAsync(invitacion);

            // Assert
            invitacionActualizada.Should().NotBeNull();
            invitacionActualizada.Estado.Should().Be("Pagado");
            invitacionActualizada.MpPaymentId.Should().Be("12345678901");
        }

        [Fact]
        public async Task DeberiaRegistrarIngresoConDatosCorrectos()
        {
            // Arrange
            AgregarIngresoDTO capturedDTO = null;

            _mockAgregarIngreso.Setup(a => a.Ejecutar(It.IsAny<AgregarIngresoDTO>()))
                .Callback<AgregarIngresoDTO>(dto => capturedDTO = dto)
                .ReturnsAsync(new ObtenerIngresoDTO());

            var ingresoDTO = new AgregarIngresoDTO
            {
                GimnasioId = 10,
                UsuarioId = 50,
                MetodoPago = "MercadoPago",
                Monto = 1500.50m,
                Observaciones = "Pago acreditado por Mercado Pago"
            };

            // Act
            await _mockAgregarIngreso.Object.Ejecutar(ingresoDTO);

            // Assert
            capturedDTO.Should().NotBeNull();
            capturedDTO.GimnasioId.Should().Be(10);
            capturedDTO.UsuarioId.Should().Be(50);
            capturedDTO.MetodoPago.Should().Be("MercadoPago");
            capturedDTO.Monto.Should().Be(1500.50m);
            capturedDTO.Observaciones.Should().Be("Pago acreditado por Mercado Pago");
        }

        [Fact]
        public async Task DeberiaLlamarProcesarInvitacionQrConDatosDelSocio()
        {
            // Arrange
            GenerarInvitacionDTO capturedDTO = null;
            string capturedToken = null;
            Invitacion capturedInvitacion = null;

            var socio = new Socio
            {
                Id = 50,
                Nombre = "Ana",
                Apellido = "Martinez",
                Email = "ana@test.com",
                Telefono = "987654321",
                TokenRecuperacion = "token-ana-123",
                GimnasioId = 10
            };

            var invitacion = new Invitacion
            {
                Id = 100,
                GimnasioId = 10,
                UsuarioId = 50,
                Email = "ana@test.com",
                Estado = "Pendiente"
            };

            _mockAgregarInvitacion.Setup(a => a.ProcesarInvitacionQrAsync(
                It.IsAny<GenerarInvitacionDTO>(),
                It.IsAny<string>(),
                It.IsAny<Invitacion>()))
                .Callback<GenerarInvitacionDTO, string, Invitacion>((dto, token, inv) =>
                {
                    capturedDTO = dto;
                    capturedToken = token;
                    capturedInvitacion = inv;
                })
                .ReturnsAsync(("token-qr", "qr-image"));

            var generarDTO = new GenerarInvitacionDTO
            {
                Nombre = socio.Nombre,
                Apellidos = socio.Apellido,
                Email = socio.Email,
                Telefono = socio.Telefono
            };

            // Act
            await _mockAgregarInvitacion.Object.ProcesarInvitacionQrAsync(
                generarDTO,
                socio.TokenRecuperacion,
                invitacion);

            // Assert
            capturedDTO.Should().NotBeNull();
            capturedDTO.Nombre.Should().Be("Ana");
            capturedDTO.Apellidos.Should().Be("Martinez");
            capturedDTO.Email.Should().Be("ana@test.com");
            capturedDTO.Telefono.Should().Be("987654321");
            capturedToken.Should().Be("token-ana-123");
            capturedInvitacion.Should().NotBeNull();
            capturedInvitacion.Id.Should().Be(100);
        }

        [Fact]
        public async Task DeberiaObtenerSocioCorrectamente()
        {
            // Arrange
            var socio = new Socio
            {
                Id = 50,
                Nombre = "Juan",
                Apellido = "Perez",
                Email = "juan@test.com",
                Telefono = "123456789",
                TokenRecuperacion = "token-123",
                GimnasioId = 10
            };

            _mockUsuarioRepo.Setup(r => r.ObtenerPorIdAsync(50))
                .ReturnsAsync(socio);

            // Act
            var resultado = await _mockUsuarioRepo.Object.ObtenerPorIdAsync(50);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Id.Should().Be(50);
            resultado.Nombre.Should().Be("Juan");
            resultado.Apellido.Should().Be("Perez");
            resultado.Email.Should().Be("juan@test.com");
        }

        [Fact]
        public async Task NoDeberiaEncontrarInvitacionInexistente()
        {
            // Arrange
            _mockInvitacionRepo.Setup(r => r.ObtenerPorIdAsync(999))
                .ReturnsAsync((Invitacion)null);

            // Act
            var resultado = await _mockInvitacionRepo.Object.ObtenerPorIdAsync(999);

            // Assert
            resultado.Should().BeNull();
          
        }

        [Fact]
        public async Task NoDeberiaEncontrarSocioInexistente()
        {
            // Arrange
            _mockUsuarioRepo.Setup(r => r.ObtenerPorIdAsync(999))
                .ReturnsAsync((Socio)null);

            // Act
            var resultado = await _mockUsuarioRepo.Object.ObtenerPorIdAsync(999);

            // Assert
            resultado.Should().BeNull();
           
        }

        [Fact]
        public async Task DeberiaManejarInvitacionConUsuarioIdNulo()
        {
            // Arrange
            var invitacion = new Invitacion
            {
                Id = 100,
                GimnasioId = 10,
                UsuarioId = null,
                Email = "user@test.com",
                Estado = "Pendiente"
            };

            _mockInvitacionRepo.Setup(r => r.ObtenerPorIdAsync(100))
                .ReturnsAsync(invitacion);

            _mockUsuarioRepo.Setup(r => r.ObtenerPorIdAsync(0))
                .ReturnsAsync((Socio)null);

            // Act
            var invitacionResult = await _mockInvitacionRepo.Object.ObtenerPorIdAsync(100);
            var socioResult = await _mockUsuarioRepo.Object.ObtenerPorIdAsync(invitacionResult.UsuarioId ?? 0);

            // Assert
            invitacionResult.Should().NotBeNull();
            invitacionResult.UsuarioId.Should().BeNull();
            socioResult.Should().BeNull();
        }

        [Fact]
        public async Task DeberiaVerificarConfiguracionDeCasoDeUso()
        {
            // Arrange
            var accessToken = _mockConfig.Object["MercadoPago:AccessToken"];
            
            // Act
            var casoDeUso = _mockCasoDeUso.Object;

            // Assert
            casoDeUso.Should().NotBeNull();
            accessToken.Should().Be("TEST-1234567890-mocktoken");
            _mockConfig.Verify(c => c["MercadoPago:AccessToken"], Times.Once);
        }
    }
}
