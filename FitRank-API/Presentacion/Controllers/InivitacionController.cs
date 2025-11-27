using FitRank_API.Application.CasosDeUso.Invitacion;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FitRank_API.Presentacion.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class InvitacionController : ControllerBase
    {
        private readonly ObtenerInvitacionesCasoDeUso _obtenerInvitacionesCasoDeUso;

        public InvitacionController(ObtenerInvitacionesCasoDeUso obtenerInvitacionesCasoDeUso)
        {
            _obtenerInvitacionesCasoDeUso = obtenerInvitacionesCasoDeUso;
        }

        [HttpGet("todas")]
        public async Task<IActionResult> ObtenerTodas()
        {
            try
            {
                var adminIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrWhiteSpace(adminIdString) || !int.TryParse(adminIdString, out var adminId) || adminId <= 0)
                {
                    return BadRequest(new { Mensaje = "ID de administrador inválido en el token." });
                }

                var invitaciones = await _obtenerInvitacionesCasoDeUso.Ejecutar(adminId);
                return Ok(invitaciones);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }
    }
}
