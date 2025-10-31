using System.IdentityModel.Tokens.Jwt;

namespace FitRank_API.Application.Helpers
{
    public static class TokenInvitacionHelper
    {
        
        public static int? ParseIdFromTokenSimple(string token)
        {
            if (int.TryParse(token, out int id))
                return id;

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
