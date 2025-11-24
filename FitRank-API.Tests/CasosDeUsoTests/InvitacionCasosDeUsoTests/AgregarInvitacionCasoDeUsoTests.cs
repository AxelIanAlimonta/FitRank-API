using AutoMapper;
using FluentAssertions;
using FitRank_API.Application.CasosDeUso.Ingreso;
using FitRank_API.Application.CasosDeUso.Invitacion;
using FitRank_API.Application.CasosDeUso.MercadoPago;
using FitRank_API.Application.DTOs.Invitacion;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;

namespace FitRank_API.Tests.CasosDeUsoTests.InvitacionCasosDeUsoTests
{
    public class AgregarInvitacionCasoDeUsoTests
    {
        private readonly Mock<IInvitacionRepositorio> _mockInvitacionRepo;
        private readonly Mock<IUsuarioRepositorio> _mockUsuarioRepo;
        private readonly Mock<IGimnasioRepositorio> _mockGimnasioRepo;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly Mock<ISendGridClient> _mockSendGridClient;
        private readonly Mock<QrHelper> _mockQrHelper;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;
        private readonly Mock<CrearPreferenciaMercadoPagoCasoDeUso> _mockCrearPreferencia;
        private readonly Mock<AgregarIngresoCasoDeUso> _mockAgregarIngreso;
        private readonly AgregarInvitacionCasoDeUso _casoDeUso;

        public AgregarInvitacionCasoDeUsoTests()
        {
            _mockInvitacionRepo = new Mock<IInvitacionRepositorio>();
            _mockUsuarioRepo = new Mock<IUsuarioRepositorio>();
            _mockGimnasioRepo = new Mock<IGimnasioRepositorio>();
            _mockConfig = new Mock<IConfiguration>();
            _mockSendGridClient = new Mock<ISendGridClient>();
            _mockQrHelper = new Mock<QrHelper>(_mockConfig.Object);
            
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            
            _mockCrearPreferencia = new Mock<CrearPreferenciaMercadoPagoCasoDeUso>(_mockConfig.Object, _mockQrHelper.Object, _httpClient);
            _mockAgregarIngreso = new Mock<AgregarIngresoCasoDeUso>(Mock.Of<IIngresoRepositorio>(), Mock.Of<IMapper>());

            // Configurar valores de configuración
            _mockConfig.Setup(c => c["FrontendUrl"]).Returns("http://localhost:3000");
            _mockConfig.Setup(c => c["Email:From"]).Returns("noreply@fitrank.com");

            _casoDeUso = new AgregarInvitacionCasoDeUso(
                _mockInvitacionRepo.Object,
                _mockUsuarioRepo.Object,
                _mockConfig.Object,
                _mockSendGridClient.Object,
                _mockQrHelper.Object,
                _mockGimnasioRepo.Object,
                _mockCrearPreferencia.Object,
                _mockAgregarIngreso.Object
            );
        }

        [Fact]
        public async Task DeberiaCrearInvitacionConMetodoEfectivo()
        {
            // Arrange
            var adminId = 1;
            var gimnasioId = 10L;
            var dto = new GenerarInvitacionDTO
            {
                Nombre = "Juan",
                Apellidos = "Perez",
                Dni = 12345678,
                Email = "juan@test.com",
                Telefono = "123456789",
                MetodoPago = "Efectivo",
                Monto = 50000m,
                Periodo = "Monthly"
            };

            var gimnasio = new Gimnasio { Id = gimnasioId, Nombre = "Gym Test" };
            var invitacionCreada = new Invitacion
            {
                Id = 1,
                GimnasioId = gimnasioId,
                Email = dto.Email,
                Estado = "Pagado",
                MetodoPago = "Efectivo",
                CuotaPagadaHasta = DateTime.Now.AddMonths(1),
                CreadaEn = DateTime.Now,
                ExpiraEn = DateTime.Now.AddHours(24)
            };

            var socioCreado = new Socio
            {
                Id = 100,
                Nombre = dto.Nombre,
                Apellido = dto.Apellidos,
                Email = dto.Email,
                GimnasioId = gimnasioId
            };

            _mockGimnasioRepo.Setup(r => r.ObtenerPorAdministradorIdAsync(adminId))
                .ReturnsAsync(gimnasio);

            _mockInvitacionRepo.Setup(r => r.AgregarAsync(It.IsAny<Invitacion>()))
                .Callback<Invitacion>(inv => inv.Id = 1)
                .ReturnsAsync(invitacionCreada);

            _mockUsuarioRepo.Setup(r => r.AgregarAsync(It.IsAny<Socio>()))
                .ReturnsAsync(socioCreado);

            _mockInvitacionRepo.Setup(r => r.ActualizarAsync(It.IsAny<Invitacion>()))
                .ReturnsAsync(invitacionCreada);

            _mockUsuarioRepo.Setup(r => r.ObtenerPorEmailAsync(dto.Email))
                .ReturnsAsync(socioCreado);

            _mockGimnasioRepo.Setup(r => r.ObtenerGimnasioPorId(gimnasioId))
                .ReturnsAsync(gimnasio);

            _mockQrHelper.Setup(q => q.GenerarQrDePaseJWT(It.IsAny<Socio>(), gimnasioId))
                .Returns("fake-jwt-token");

            _mockQrHelper.Setup(q => q.GenerarQrImage(It.IsAny<string>()))
                .ReturnsAsync("data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==");

            _mockSendGridClient.Setup(s => s.SendEmailAsync(
                It.IsAny<SendGridMessage>(),
                It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SendGrid.Response(System.Net.HttpStatusCode.OK, null, null));

            _mockAgregarIngreso.Setup(a => a.Ejecutar(It.IsAny<FitRank_API.Application.DTOs.IngresoDTOs.AgregarIngresoDTO>()))
                .ReturnsAsync(new FitRank_API.Application.DTOs.IngresoDTOs.ObtenerIngresoDTO());

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto, adminId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Success.Should().BeTrue();
            resultado.InvitacionId.Should().Be(1);
            resultado.TokenInvitacion.Should().Be("fake-jwt-token");
            resultado.QrImage.Should().Contain("base64");
            resultado.Mensaje.Should().Contain(dto.Email);

            _mockGimnasioRepo.Verify(r => r.ObtenerPorAdministradorIdAsync(adminId), Times.Once);
     
        }

        [Fact]
        public async Task DeberiaCrearInvitacionConMetodoMercadoPago()
        {
            // Arrange
            var adminId = 1;
            var gimnasioId = 10L;
            var dto = new GenerarInvitacionDTO
            {
                Nombre = "Maria",
                Apellidos = "Lopez",
                Dni = 87654321,
                Email = "maria@test.com",
                Telefono = "987654321",
                MetodoPago = "MercadoPago",
                Monto = 75000m,
                Periodo = "Yearly"
            };

            var gimnasio = new Gimnasio { Id = gimnasioId, Nombre = "Gym Test" };
            var invitacionCreada = new Invitacion
            {
                Id = 2,
                GimnasioId = gimnasioId,
                Email = dto.Email,
                Estado = "Pendiente",
                MetodoPago = "MercadoPago",
                CuotaPagadaHasta = DateTime.Now.AddYears(1),
                CreadaEn = DateTime.Now,
                ExpiraEn = DateTime.Now.AddHours(24)
            };

            var socioCreado = new Socio
            {
                Id = 200,
                Nombre = dto.Nombre,
                Apellido = dto.Apellidos,
                Email = dto.Email,
                GimnasioId = gimnasioId
            };

            _mockGimnasioRepo.Setup(r => r.ObtenerPorAdministradorIdAsync(adminId))
                .ReturnsAsync(gimnasio);

            _mockInvitacionRepo.Setup(r => r.AgregarAsync(It.IsAny<Invitacion>()))
                .Callback<Invitacion>(inv => inv.Id = 2)
                .ReturnsAsync(invitacionCreada);

            _mockUsuarioRepo.Setup(r => r.AgregarAsync(It.IsAny<Socio>()))
                .ReturnsAsync(socioCreado);

            _mockInvitacionRepo.Setup(r => r.ActualizarAsync(It.IsAny<Invitacion>()))
                .ReturnsAsync(invitacionCreada);

            _mockCrearPreferencia.Setup(c => c.Ejecutar(dto.Monto ?? 0, dto.Email, invitacionCreada.Id))
                .ReturnsAsync(("https://mercadopago.com/checkout/v1/redirect?pref_id=123", "data:image/png;base64,mpQrImage"));

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto, adminId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Success.Should().BeTrue();
            resultado.InvitacionId.Should().Be(2);
            resultado.TokenInvitacion.Should().BeNull();
            resultado.LinkPago.Should().Contain("mercadopago.com");
            resultado.QrImage.Should().Contain("mpQrImage");
            resultado.Mensaje.Should().Contain("Mercado Pago");

           
        }

        [Fact]
        public async Task DeberiaLanzarExcepcionCuandoGimnasioNoExiste()
        {
            // Arrange
            var adminId = 999;
            var dto = new GenerarInvitacionDTO
            {
                Nombre = "Test",
                Apellidos = "User",
                Email = "test@test.com",
                MetodoPago = "Efectivo"
            };

            _mockGimnasioRepo.Setup(r => r.ObtenerPorAdministradorIdAsync(adminId))
                .ReturnsAsync((Gimnasio)null);

            // Act
            Func<Task> act = async () => await _casoDeUso.Ejecutar(dto, adminId);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("No se encontró un gimnasio asociado al administrador.");

            
        }

        [Fact]
        public async Task DeberiaLanzarExcepcionCuandoMetodoPagoNoSoportado()
        {
            // Arrange
            var adminId = 1;
            var gimnasioId = 10L;
            var dto = new GenerarInvitacionDTO
            {
                Nombre = "Test",
                Apellidos = "User",
                Email = "test@test.com",
                MetodoPago = "Bitcoin" // Método no soportado
            };

            var gimnasio = new Gimnasio { Id = gimnasioId, Nombre = "Gym Test" };
            var invitacionCreada = new Invitacion
            {
                Id = 3,
                GimnasioId = gimnasioId,
                Email = dto.Email
            };

            var socioCreado = new Socio
            {
                Id = 300,
                Nombre = dto.Nombre,
                Email = dto.Email
            };

            _mockGimnasioRepo.Setup(r => r.ObtenerPorAdministradorIdAsync(adminId))
                .ReturnsAsync(gimnasio);

            _mockInvitacionRepo.Setup(r => r.AgregarAsync(It.IsAny<Invitacion>()))
                .ReturnsAsync(invitacionCreada);

            _mockUsuarioRepo.Setup(r => r.AgregarAsync(It.IsAny<Socio>()))
                .ReturnsAsync(socioCreado);

            _mockInvitacionRepo.Setup(r => r.ActualizarAsync(It.IsAny<Invitacion>()))
                .ReturnsAsync(invitacionCreada);

            // Act
            Func<Task> act = async () => await _casoDeUso.Ejecutar(dto, adminId);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Método de pago no soportado.");
        }

        [Fact]
        public async Task DeberiaCalcularCuotaPagadaHastaCorrectamenteParaMensual()
        {
            // Arrange
            var adminId = 1;
            var gimnasioId = 10L;
            var dto = new GenerarInvitacionDTO
            {
                Nombre = "Test",
                Apellidos = "User",
                Email = "test@test.com",
                MetodoPago = "Efectivo",
                Periodo = "Monthly"
            };

            var gimnasio = new Gimnasio { Id = gimnasioId, Nombre = "Gym Test" };
            Invitacion invitacionCapturada = null;

            _mockGimnasioRepo.Setup(r => r.ObtenerPorAdministradorIdAsync(adminId))
                .ReturnsAsync(gimnasio);

            _mockInvitacionRepo.Setup(r => r.AgregarAsync(It.IsAny<Invitacion>()))
                .Callback<Invitacion>(inv => invitacionCapturada = inv)
                .ReturnsAsync((Invitacion inv) => inv);

            _mockUsuarioRepo.Setup(r => r.AgregarAsync(It.IsAny<Socio>()))
                .ReturnsAsync(new Socio { Id = 1, Email = dto.Email, GimnasioId = gimnasioId });

            _mockInvitacionRepo.Setup(r => r.ActualizarAsync(It.IsAny<Invitacion>()))
                .ReturnsAsync((Invitacion inv) => inv);

            _mockUsuarioRepo.Setup(r => r.ObtenerPorEmailAsync(dto.Email))
                .ReturnsAsync(new Socio { Id = 1, Email = dto.Email, GimnasioId = gimnasioId });

            _mockGimnasioRepo.Setup(r => r.ObtenerGimnasioPorId(gimnasioId))
                .ReturnsAsync(gimnasio);

            _mockQrHelper.Setup(q => q.GenerarQrDePaseJWT(It.IsAny<Socio>(), gimnasioId))
                .Returns("token");

            _mockQrHelper.Setup(q => q.GenerarQrImage(It.IsAny<string>()))
                .ReturnsAsync("data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==");

            _mockSendGridClient.Setup(s => s.SendEmailAsync(It.IsAny<SendGridMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SendGrid.Response(System.Net.HttpStatusCode.OK, null, null));

            _mockAgregarIngreso.Setup(a => a.Ejecutar(It.IsAny<FitRank_API.Application.DTOs.IngresoDTOs.AgregarIngresoDTO>()))
                .ReturnsAsync(new FitRank_API.Application.DTOs.IngresoDTOs.ObtenerIngresoDTO());

            // Act
            var resultado = await _casoDeUso.Ejecutar(dto, adminId);

            // Assert
            invitacionCapturada.Should().NotBeNull();
            invitacionCapturada.CuotaPagadaHasta.Should().NotBeNull();
            invitacionCapturada.CuotaPagadaHasta.Value.Should().BeCloseTo(DateTime.Now.AddMonths(1), TimeSpan.FromSeconds(5));
        }

        [Fact]
        public async Task DeberiaCalcularCuotaPagadaHastaCorrectamenteParaAnual()
        {
            // Arrange
            var adminId = 1;
            var gimnasioId = 10L;
            var dto = new GenerarInvitacionDTO
            {
                Nombre = "Test",
                Apellidos = "User",
                Email = "test@test.com",
                MetodoPago = "Efectivo",
                Periodo = "Yearly"
            };

            var gimnasio = new Gimnasio { Id = gimnasioId, Nombre = "Gym Test" };
            Invitacion invitacionCapturada = null;

            _mockGimnasioRepo.Setup(r => r.ObtenerPorAdministradorIdAsync(adminId))
                .ReturnsAsync(gimnasio);

            _mockInvitacionRepo.Setup(r => r.AgregarAsync(It.IsAny<Invitacion>()))
                .Callback<Invitacion>(inv => invitacionCapturada = inv)
                .ReturnsAsync((Invitacion inv) => inv);

            _mockUsuarioRepo.Setup(r => r.AgregarAsync(It.IsAny<Socio>()))
                .ReturnsAsync(new Socio { Id = 1, Email = dto.Email, GimnasioId = gimnasioId });

            _mockInvitacionRepo.Setup(r => r.ActualizarAsync(It.IsAny<Invitacion>()))
                .ReturnsAsync((Invitacion inv) => inv);

            _mockUsuarioRepo.Setup(r => r.ObtenerPorEmailAsync(dto.Email))
                .ReturnsAsync(new Socio { Id = 1, Email = dto.Email, GimnasioId = gimnasioId });

            _mockGimnasioRepo.Setup(r => r.ObtenerGimnasioPorId(gimnasioId))
                .ReturnsAsync(gimnasio);

            _mockQrHelper.Setup(q => q.GenerarQrDePaseJWT(It.IsAny<Socio>(), gimnasioId))
                .Returns("token");

            _mockQrHelper.Setup(q => q.GenerarQrImage(It.IsAny<string>()))
                .ReturnsAsync("data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mNk+M9QDwADhgGAWjR9awAAAABJRU5ErkJggg==");

            _mockSendGridClient.Setup(s => s.SendEmailAsync(It.IsAny<SendGridMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new SendGrid.Response(System.Net.HttpStatusCode.OK, null, null));

            _mockAgregarIngreso.Setup(a => a.Ejecutar(It.IsAny<FitRank_API.Application.DTOs.IngresoDTOs.AgregarIngresoDTO>()))
                .ReturnsAsync(new FitRank_API.Application.DTOs.IngresoDTOs.ObtenerIngresoDTO());

            // Act
            await _casoDeUso.Ejecutar(dto, adminId);

            // Assert
            invitacionCapturada.Should().NotBeNull();
            invitacionCapturada.CuotaPagadaHasta.Should().NotBeNull();
            invitacionCapturada.CuotaPagadaHasta.Value.Should().BeCloseTo(DateTime.Now.AddYears(1), TimeSpan.FromSeconds(5));
        }
    }
}
