using System.IdentityModel.Tokens.Jwt;

namespace FitRank_API.Application.Helpers
{
    public static class TokenInvitacionHelper
    {
        
        public static int? ParseIdFromTokenSimple(string token)
        {
            // Intentar parsear el token completo como número
            if (int.TryParse(token, out int id))
                return id;

            // Si el token tiene formato "simple_token_X", extraer el ID del final
            if (token.StartsWith("simple_token_"))
            {
                var idPart = token.Substring("simple_token_".Length);
                if (int.TryParse(idPart, out int parsedId))
                    return parsedId;
            }

            return null;
        }

        
        public static int? ParseIdFromJwt(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(token);
                var idClaim = jwt.Claims.FirstOrDefault(c => c.Type == "invitacionId")?.Value;

                if (int.TryParse(idClaim, out int invitacionId))
                    return invitacionId;

                return null;
            }
            catch
            {
                return null; 
            }
        }
    }
}
