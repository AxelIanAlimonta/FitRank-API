using FitRank_API.Application.CasosDeUso.BatallasCasosDeUso;
using FitRank_API.Application.DTOs.BatallaDTOs;
using FitRank_API.Application.UseCases.Batallas;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace FitRank_API.Presentacion.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BatallaController : ControllerBase
    {
        private readonly CrearBatallaCasoDeUso _crearBatallaCasoDeUso;
        private readonly AceptarBatallaCasoDeUso _aceptarBatallaCasoDeUso;
        private readonly RechazarBatallaCasoDeUso _rechazarBatallaCasoDeUso;
        private readonly ObtenerBatallasActivasCasoDeUso _obtenerActivasCasoDeUso;
        private readonly FinalizarBatallaCasoDeUso _finalizarBatallaCasoDeUso;
        private readonly ObtenerProgresoBatallaCasoDeUso _obtenerProgresoBatallaCasoDeUso;
        private readonly ObtenerHistorialBatallasCasoDeUso _obtenerHistorialBatallasCasoDeUso;
        private readonly ObtenerBatallasPendientesCasoDeUso _obtenerPendientesBatallasCasoDeUso;
        public BatallaController(
            CrearBatallaCasoDeUso crearBatallaCasoDeUso,
            AceptarBatallaCasoDeUso aceptarBatallaCasoDeUSo,
            RechazarBatallaCasoDeUso rechazarBatallaCasoDeUso,
            ObtenerBatallasActivasCasoDeUso obtenerBatallasActivasCasoDeUso,
            FinalizarBatallaCasoDeUso finalizarBatallaCasoDeUso,
            ObtenerProgresoBatallaCasoDeUso obtenerProgresoBatallaCaso,
            ObtenerHistorialBatallasCasoDeUso obtenerHistorialBatallasCaso,
            ObtenerBatallasPendientesCasoDeUso obtenerPendientesBatallasCasoDeUso)
        {
            _crearBatallaCasoDeUso = crearBatallaCasoDeUso;
            _aceptarBatallaCasoDeUso = aceptarBatallaCasoDeUSo;
            _rechazarBatallaCasoDeUso = rechazarBatallaCasoDeUso;
            _obtenerActivasCasoDeUso = obtenerBatallasActivasCasoDeUso;
            _finalizarBatallaCasoDeUso = finalizarBatallaCasoDeUso;
            _obtenerProgresoBatallaCasoDeUso = obtenerProgresoBatallaCaso;
            _obtenerHistorialBatallasCasoDeUso = obtenerHistorialBatallasCaso;
            _obtenerPendientesBatallasCasoDeUso = obtenerPendientesBatallasCasoDeUso;
        }
        [HttpPost("crear")]
        public async Task<IActionResult> Crear([FromBody] CrearBatallaDTO dto)
        {
            try
            {
                var batalla = await _crearBatallaCasoDeUso.Ejecutar(dto);
                return Ok(batalla);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = ex.Message });
            }
        }

        [HttpPost("aceptar/{id}")]
        public async Task<IActionResult> Aceptar(int id)
        {
            try
            {
                var ok = await _aceptarBatallaCasoDeUso.Ejecutar(id);
                return ok ? Ok() : NotFound(new { mensaje = "Batalla no encontrada o no se puede aceptar." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = ex.Message });
            }
        }



        [HttpPost("rechazar/{id}")]
        public async Task<IActionResult> Rechazar(int id)
        {
            try
            {
                var ok = await _rechazarBatallaCasoDeUso.Ejecutar(id);
                return ok ? Ok() : NotFound(new { mensaje = "Batalla no encontrada o no se puede rechazar." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = ex.Message });
            }
        }


        [HttpGet("activas/{socioId}")]
        public async Task<IActionResult> ObtenerActivas(int socioId)
        {
            try
            {
                var lista = await _obtenerActivasCasoDeUso.Ejecutar(socioId);
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = ex.Message });
            }
        }

        [HttpGet("progreso/{id}")]
        public async Task<IActionResult> Progreso(int id)
        {
            try
            {
                var progreso = await _obtenerProgresoBatallaCasoDeUso.Ejecutar(id);
                return progreso == null ? NotFound(new { mensaje = "Batalla no encontrada." }) : Ok(progreso);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = ex.Message });
            }
        }

        [HttpPost("{id}/finalizar")]
        public async Task<IActionResult> FinalizarBatalla(int id)
        {
            try
            {
                var resultado = await _finalizarBatallaCasoDeUso.Ejecutar(id);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        [HttpGet("historial/{socioId}")]
        public async Task<IActionResult> ObtenerHistorial(int socioId)
        {
            try
            {
                var resultado = await _obtenerHistorialBatallasCasoDeUso.Ejecutar(socioId);
                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = ex.Message });
            }
        }

        [HttpGet("pendientes/{socioId}")]
        public async Task<IActionResult> ObtenerPendientes(int socioId)
        {
            try
            {
                var lista = await _obtenerPendientesBatallasCasoDeUso.Ejecutar(socioId);
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { mensaje = ex.Message });
            }
        }



    }
}
