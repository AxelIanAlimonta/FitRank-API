using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text;
using FitRank_API.Application.CasosDeUso.Invitacion;

namespace FitRank_API.Application.CasosDeUso.MercadoPago
{
    public class CrearPreferenciaMercadoPagoCasoDeUso
    {
        private readonly IConfiguration _config;
        private readonly QrHelper _qrHelper;
        private readonly HttpClient _http;

        public CrearPreferenciaMercadoPagoCasoDeUso(IConfiguration config, QrHelper qrHelper, HttpClient httpClient = null)
        {
            _config = config;
            _qrHelper = qrHelper;
            _http = httpClient ?? new HttpClient();
        }

        public virtual async Task<(string linkPago, string qrImage)> Ejecutar(decimal monto, string email, long invitacionId)
        {
            var payload = new
            {
                items = new[]
                {
                    new {
                        title = "Pase FitRank",
                        quantity = 1,
                        unit_price = monto
                    }
                },
                payer = new
                {
                    email = email
                },
                external_reference = invitacionId.ToString(),
                back_urls = new
                {
                    success = _config["MercadoPago:SuccessUrl"],
                    failure = _config["MercadoPago:FailureUrl"],
                    pending = _config["MercadoPago:PendingUrl"]
                },
                auto_return = "approved",
                notification_url = _config["MercadoPago:NotificationUrl"]
            };

            string json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            _http.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _config["MercadoPago:AccessToken"]);

            var response = await _http.PostAsync(
                "https://api.mercadopago.com/checkout/preferences",
                content
            );

            var responseStr = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new Exception($"Error MP: {responseStr}");

            var preference = JsonSerializer.Deserialize<PreferenceResponse>(responseStr);

            string linkPago = preference.init_point;

            string qr = await _qrHelper.GenerarQrImage(linkPago);

            return (linkPago, qr);
        }
    }

    public class PreferenceResponse
    {
        public string init_point { get; set; }
    }
}
