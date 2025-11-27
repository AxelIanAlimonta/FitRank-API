using FitRank_API.Application.CasosDeUso.ConfiguracionGrupoMuscular;
using FitRank_API.Application.DTOs.ConfiguracionGrupoMuscularDTOs;
using Microsoft.AspNetCore.Mvc;

namespace FitRank_API.Presentacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfiguracionGrupoMuscularController : ControllerBase
    {
        private readonly ObtenerTodasLasConfiguracionGrupoMuscularCasoDeUso _obtenerTodasLasConfiguracionGrupoMuscularCasoDeUso;
        private readonly ObtenerConfiguracionGrupoMuscularPorIdCasoDeUso _obtenerConfiguracionGrupoMuscularPorIdCasoDeUso;
        private readonly AgregarConfiguracionGrupoMuscularCasoDeUso _agregarConfiguracionGrupoMuscularCasoDeUso;
        private readonly ActualizarConfiguracionGrupoMuscularCasoDeUso _actualizarConfiguracionGrupoMuscularCasoDeUso;
        private readonly EliminarConfiguracionGrupoMuscularCasoDeUso _eliminarConfiguracionGrupoMuscularCasoDeUso;

        public ConfiguracionGrupoMuscularController(
            ObtenerTodasLasConfiguracionGrupoMuscularCasoDeUso obtenerTodasLasConfiguracionGrupoMuscularCasoDeUso,
            ObtenerConfiguracionGrupoMuscularPorIdCasoDeUso obtenerConfiguracionGrupoMuscularPorIdCasoDeUso,
            EliminarConfiguracionGrupoMuscularCasoDeUso eliminarConfiguracionGrupoMuscularCasoDeUso,
            ActualizarConfiguracionGrupoMuscularCasoDeUso actualizarConfiguracionGrupoMuscularCasoDeUso,
            AgregarConfiguracionGrupoMuscularCasoDeUso agregarConfiguracionGrupoMuscularCasoDeUso)
        {
            _obtenerTodasLasConfiguracionGrupoMuscularCasoDeUso = obtenerTodasLasConfiguracionGrupoMuscularCasoDeUso;
            _obtenerConfiguracionGrupoMuscularPorIdCasoDeUso = obtenerConfiguracionGrupoMuscularPorIdCasoDeUso;
            _eliminarConfiguracionGrupoMuscularCasoDeUso = eliminarConfiguracionGrupoMuscularCasoDeUso;
            _actualizarConfiguracionGrupoMuscularCasoDeUso = actualizarConfiguracionGrupoMuscularCasoDeUso;
            _agregarConfiguracionGrupoMuscularCasoDeUso = agregarConfiguracionGrupoMuscularCasoDeUso;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            try
            {
                var configuraciones = await _obtenerTodasLasConfiguracionGrupoMuscularCasoDeUso.Ejecutar();
                return Ok(configuraciones);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> ObtenerPorId(long id)
        {
            if (id <= 0)
                return BadRequest(new { Mensaje = "El ID debe ser mayor a cero." });

            try
            {
                var configuracion = await _obtenerConfiguracionGrupoMuscularPorIdCasoDeUso.Ejecutar(id);
                if (configuracion == null)
                {
                    return NotFound(new { Mensaje = "Configuración no encontrada." });
                }
                return Ok(configuracion);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Agregar([FromBody] AgregarConfiguracionGrupoMuscularDTO configuracion)
        {
            if (configuracion == null)
                return BadRequest(new { Mensaje = "El objeto de la solicitud no puede ser nulo." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var nuevaConfiguracion = await _agregarConfiguracionGrupoMuscularCasoDeUso.Ejecutar(configuracion);
                return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevaConfiguracion.Id }, nuevaConfiguracion);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Actualizar(long id, [FromBody] ConfiguracionGrupoMuscularDTO configuracion)
        {
            if (id <= 0)
                return BadRequest(new { Mensaje = "El ID debe ser mayor a cero." });

            if (configuracion == null)
                return BadRequest(new { Mensaje = "El objeto de la solicitud no puede ser nulo." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != configuracion.Id)
            {
                return BadRequest(new { Mensaje = "El ID de la URL no coincide con el ID de la configuración." });
            }

            try
            {
                var configuracionActualizada = await _actualizarConfiguracionGrupoMuscularCasoDeUso.Ejecutar(configuracion);
                if (configuracionActualizada == null)
                {
                    return NotFound(new { Mensaje = "Configuración no encontrada." });
                }
                return Ok(configuracionActualizada);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Eliminar(long id)
        {
            if (id <= 0)
                return BadRequest(new { Mensaje = "El ID debe ser mayor a cero." });

            try
            {
                await _eliminarConfiguracionGrupoMuscularCasoDeUso.Ejecutar(id);
                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }
    }
}
