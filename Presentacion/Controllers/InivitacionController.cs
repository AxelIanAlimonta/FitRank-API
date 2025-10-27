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


        public InvitacionController(
            ObtenerInvitacionesCasoDeUso obtenerInvitacionesCasoDeUso
            )
        {
            _obtenerInvitacionesCasoDeUso = obtenerInvitacionesCasoDeUso;
            
        }

       
        [HttpGet("todas")]
        public async Task<IActionResult> ObtenerTodas()
        {
            var adminId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var invitaciones = await _obtenerInvitacionesCasoDeUso.Ejecutar(adminId);
            return Ok(invitaciones);
        }

        
    }
}
