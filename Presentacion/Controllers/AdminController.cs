using FitRank_API.Application.DTOs.Auth;
using FitRank_API.Application.DTOs.Auth.Invitacion;
using FitRank_API.Application.DTOs.QR;
using FitRank_API.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FitRank_API.Presentacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpPost("generar-invitacion")]
        public async Task<ActionResult<InvitacionResponseDTO>> GenerarInvitacion([FromBody] GenerarInvitacionDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            int adminId = 1; // Si necesitás pasar un admin fijo al servicio
            var result = await _adminService.GenerarInvitacionAsync(dto, adminId);

            if (!result.Success)
                return BadRequest(new { Mensaje = result.Mensaje });

            return Ok(result);
        }


        [HttpPost("fallback-efectivo")]
        public async Task<ActionResult<InvitacionResponseDTO>> FallbackEfectivo([FromBody] FallbackEfectivoDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var adminIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(adminIdClaim) || !int.TryParse(adminIdClaim, out int adminId))
                return Unauthorized(new { Mensaje = "Admin ID no válido" });

            var result = await _adminService.FallbackEfectivoAsync(dto, adminId);
            if (!result.Success)
                return BadRequest(new { Mensaje = result.Mensaje });

            return Ok(result);
        }


        [HttpPost("enviar-email-qr")]
        public async Task<ActionResult<EmailResponseDTO>> EnviarEmailQr([FromBody] EmailDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _adminService.EnviarEmailQrAsync(dto);
            if (!result.Success)
                return BadRequest(new { Mensaje = result.Mensaje });

            return Ok(result);
        }


        [HttpPost("validar-qr")]
        public async Task<ActionResult<QrValidationResponseDTO>> ValidarQr([FromBody] QrValidationDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var adminIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            int? adminId = string.IsNullOrEmpty(adminIdClaim) ? null : int.Parse(adminIdClaim);

            var result = await _adminService.ValidarQrAsync(dto, adminId);
            if (!result.Valido)
                return BadRequest(new { Mensaje = result.Mensaje });

            return Ok(result);
        }
    }
}
