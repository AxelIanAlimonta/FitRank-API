using FluentAssertions;
using FitRank_API.Application.CasosDeUso.Invitacion;
using FitRank_API.Application.CasosDeUso.MercadoPago;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json;

namespace FitRank_API.Tests.CasosDeUsoTests.InvitacionCasosDeUsoTests
{
    public class CrearPreferenciaMercadoPagoCasoDeUsoTests
    {
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly Mock<QrHelper> _mockQrHelper;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly HttpClient _httpClient;

        public CrearPreferenciaMercadoPagoCasoDeUsoTests()
        {
            _mockConfig = new Mock<IConfiguration>();
            _mockQrHelper = new Mock<QrHelper>(_mockConfig.Object);
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_mockHttpMessageHandler.Object);

            // Configuración de valores
            _mockConfig.Setup(c => c["MercadoPago:AccessToken"]).Returns("TEST-123456");
            _mockConfig.Setup(c => c["MercadoPago:SuccessUrl"]).Returns("http://localhost/success");
            _mockConfig.Setup(c => c["MercadoPago:FailureUrl"]).Returns("http://localhost/failure");
            _mockConfig.Setup(c => c["MercadoPago:PendingUrl"]).Returns("http://localhost/pending");
            _mockConfig.Setup(c => c["MercadoPago:NotificationUrl"]).Returns("http://localhost/webhook");
        }

        [Fact]
        public async Task DeberiaCrearPreferenciaYRetornarLinkYQr()
        {
            // Arrange
            var monto = 50000m;
            var email = "test@test.com";
            var invitacionId = 123L;

            var responseContent = new
            {
                init_point = "https://www.mercadopago.com.ar/checkout/v1/redirect?pref_id=123456"
            };

            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(responseContent))
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.Is<HttpRequestMessage>(req =>
                        req.Method == HttpMethod.Post &&
                        req.RequestUri.ToString() == "https://api.mercadopago.com/checkout/preferences"),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(responseMessage);

            _mockQrHelper.Setup(q => q.GenerarQrImage(It.IsAny<string>()))
                .ReturnsAsync("data:image/png;base64,testQrImage");

            var casoDeUso = new CrearPreferenciaMercadoPagoCasoDeUso(_mockConfig.Object, _mockQrHelper.Object, _httpClient);

            // Act
            var resultado = await casoDeUso.Ejecutar(monto, email, invitacionId);

            // Assert
            resultado.linkPago.Should().Contain("mercadopago.com");
            resultado.linkPago.Should().Contain("redirect?pref_id=123456");
            resultado.qrImage.Should().Contain("base64");
            _mockQrHelper.Verify(q => q.GenerarQrImage(It.Is<string>(s => s.Contains("mercadopago"))), Times.Once);
        }

        [Fact]
        public async Task DeberiaLanzarExcepcionCuandoMercadoPagoFalla()
        {
            // Arrange
            var monto = 50000m;
            var email = "test@test.com";
            var invitacionId = 123L;

            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.BadRequest,
                Content = new StringContent("{\"error\":\"Invalid request\"}")
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(responseMessage);

            var casoDeUso = new CrearPreferenciaMercadoPagoCasoDeUso(_mockConfig.Object, _mockQrHelper.Object, _httpClient);

            // Act
            Func<Task> act = async () => await casoDeUso.Ejecutar(monto, email, invitacionId);

            // Assert
            await act.Should().ThrowAsync<Exception>()
                .WithMessage("*Error MP*");
        }

        [Fact]
        public async Task DeberiaIncluirDatosCorrectosEnElPayload()
        {
            // Arrange
            var monto = 75000m;
            var email = "cliente@test.com";
            var invitacionId = 456L;

            HttpRequestMessage capturedRequest = null;

            var responseContent = new
            {
                init_point = "https://www.mercadopago.com.ar/checkout/v1/redirect?pref_id=789"
            };

            var responseMessage = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(responseContent))
            };

            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>()
                )
                .Callback<HttpRequestMessage, CancellationToken>((req, token) => capturedRequest = req)
                .ReturnsAsync(responseMessage);

            _mockQrHelper.Setup(q => q.GenerarQrImage(It.IsAny<string>()))
                .ReturnsAsync("data:image/png;base64,qr");

            var casoDeUso = new CrearPreferenciaMercadoPagoCasoDeUso(_mockConfig.Object, _mockQrHelper.Object, _httpClient);

            // Act
            await casoDeUso.Ejecutar(monto, email, invitacionId);

            // Assert
            capturedRequest.Should().NotBeNull();
            capturedRequest.Method.Should().Be(HttpMethod.Post);
            
            var content = await capturedRequest.Content.ReadAsStringAsync();
            content.Should().Contain("Pase FitRank");
            content.Should().Contain(monto.ToString());
            content.Should().Contain(email);
            content.Should().Contain(invitacionId.ToString());
            
            capturedRequest.Headers.Authorization.Should().NotBeNull();
            capturedRequest.Headers.Authorization.Scheme.Should().Be("Bearer");
            capturedRequest.Headers.Authorization.Parameter.Should().Be("TEST-123456");
        }
    }
}
