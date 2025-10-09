using FitRank_API.Application.DTOs.EjercicioRealizado;
using FitRank_API.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FitRank_API.Presentacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]


  
        public class EjercicioRealizadoController : ControllerBase
        {
            private readonly IEjercicioRealizado _ejercicioRealizadoService;

            public EjercicioRealizadoController(IEjercicioRealizado ejercicioRealizadoService)
            {
                _ejercicioRealizadoService = ejercicioRealizadoService;
            }

        [HttpGet("Usuario/{usuarioId}")]
        public async Task<ActionResult<IEnumerable<EjercicioRealizadoDTOSalida>>> GetEjerciciosPorUsuario(int usuarioId)
        {
            var ejercicios = await _ejercicioRealizadoService.GetByUsuarioAsync(usuarioId);
            if (ejercicios == null || !ejercicios.Any())
                return NotFound("No se encontraron ejercicios para este usuario.");

            return Ok(ejercicios);
        }


        [HttpPost("registrar")]
            public async Task<IActionResult> RegistrarEjercicio([FromBody] EjercicioRealizadoDTOEntrada dto)
            {
                if (dto == null)
                {
                    return BadRequest("El objeto DTO no puede ser nulo.");
                }
                try
                {
                    var resultado = await _ejercicioRealizadoService.RegistrarEjercicioAsync(dto);
                    return Ok(resultado);
                }
                catch (Exception ex)
                {
                    // Manejo de errores (puedes registrar el error o devolver un mensaje específico)
                    return StatusCode(500, $"Error al registrar el ejercicio: {ex.Message}");
                }
            }


        }
    }
