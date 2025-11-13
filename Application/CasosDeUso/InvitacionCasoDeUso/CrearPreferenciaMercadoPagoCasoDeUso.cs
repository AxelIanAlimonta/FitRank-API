using MercadoPago.Client.Preference;
using MercadoPago.Resource.Preference;

namespace FitRank_API.Application.CasosDeUso.MercadoPago
{

    public class CrearPreferenciaMercadoPagoCasoDeUso
    {
        private readonly IConfiguration _config;


        public CrearPreferenciaMercadoPagoCasoDeUso(IConfiguration config)
        {
            _config = config;
        }
        public async Task<string> Ejecutar(decimal monto, string emailSocio, long invitacionId)
        {
            var request = new PreferenceRequest
            {
                Items = new List<PreferenceItemRequest>
                {
                    new PreferenceItemRequest
                    {
                        Title = "Pase FitRank",
                        Quantity = 1,
                        UnitPrice = monto
                    }
                },
                Payer = new PreferencePayerRequest
                {
                    Email = emailSocio
                },
                BackUrls = new PreferenceBackUrlsRequest
                {
                    Success = _config["MercadoPago:SuccessUrl"],
                    Failure = _config["MercadoPago:FailureUrl"],
                    Pending = _config["MercadoPago:PendingUrl"]
                },
                AutoReturn = "approved",
               /* NotificationUrl = "https://fitrank-api.onrender.com/api/mercadopago/webhook"*/
                NotificationUrl = _config["MercadoPago:NotificationUrl"]

            };

            var client = new PreferenceClient();
            Preference preference = await client.CreateAsync(request);

            return preference.InitPoint; // 🔹 URL del pago
        }
    }
}
