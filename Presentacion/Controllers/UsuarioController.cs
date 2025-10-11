using FitRank_API.Application.DTOs.Usuario;
using FitRank_API.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FitRank_API.Presentacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]




    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;
        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }


        [HttpGet("{usuarioId}")]
        public async Task<ActionResult> GetUsuarioById(int usuarioId)
        {
            var usuario = await _usuarioService.GetUsuarioByIdAsync(usuarioId);
            if (usuario == null)
            {
                return NotFound();
            }
            return Ok(usuario);
        }

        [HttpPost("crear")]
        public async Task<ActionResult> CrearUsuario([FromBody] CrearUsuarioDTO usuarioDto)
        {
            if (usuarioDto == null)
            {
                return BadRequest("El objeto DTO no puede ser nulo.");
            }
            try
            {
                var nuevoUsuario = await _usuarioService.CrearUsuarioAsync(usuarioDto);
                return Ok(nuevoUsuario);
            }
            catch (Exception ex)
            {
                
                return StatusCode(500, $"Error al crear el usuario: {ex.Message}");
            }
        }





    }
}
