using FitRank_API.Application.CasosDeUso.ConfiguracionGrupoMuscular;
using FitRank_API.Application.DTOs.ConfiguracionGrupoMuscularDTOs;
using Microsoft.AspNetCore.Mvc;

namespace FitRank_API.Presentacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ConfiguracionGrupoMuscularController : ControllerBase
    {
        // Implementation goes here
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
            var configuraciones = await _obtenerTodasLasConfiguracionGrupoMuscularCasoDeUso.Ejecutar();
            return Ok(configuraciones);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> ObtenerPorId(long id)
        {
            var configuracion = await _obtenerConfiguracionGrupoMuscularPorIdCasoDeUso.Ejecutar(id);
            if (configuracion == null)
            {
                return NotFound();
            }
            return Ok(configuracion);
        }

        [HttpPost]
        public async Task<IActionResult> Agregar([FromBody] AgregarConfiguracionGrupoMuscularDTO configuracion)
        {
            var nuevaConfiguracion = await _agregarConfiguracionGrupoMuscularCasoDeUso.Ejecutar(configuracion);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevaConfiguracion.Id }, nuevaConfiguracion);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Actualizar(long id, [FromBody] ConfiguracionGrupoMuscularDTO configuracion)
        {
            if (id != configuracion.Id)
            {
                return BadRequest();
            }
            var configuracionActualizada = await _actualizarConfiguracionGrupoMuscularCasoDeUso.Ejecutar(configuracion);
            if (configuracionActualizada == null)
            {
                return NotFound();
            }
            return Ok(configuracionActualizada);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Eliminar(long id)
        {
            await _eliminarConfiguracionGrupoMuscularCasoDeUso.Ejecutar(id);
            return NoContent();
        }
    }
}
