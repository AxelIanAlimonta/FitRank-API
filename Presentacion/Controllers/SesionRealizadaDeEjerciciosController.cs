using FitRank_API.Application.CasosDeUso.SesionRealizadaDeEjercicios;
using FitRank_API.Application.DTOs.SesionRealizadaDeEjercicios;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FitRank_API.Presentacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SesionRealizadaDeEjerciciosController : ControllerBase
    {
        private readonly ObtenerTodasLasSesionesRealizadasDeEjerciciosCasoDeUso obtenerTodasLasSesionesRealizadasDeEjercicios;
        private readonly ObtenerSesionRealizadaDeEjerciciosPorIdCasoDeUso obtenerSesionRealizadaDeEjerciciosPorIdCasoDeUso;
        private readonly AgregarSesionRealizadaDeEjerciciosCasoDeUso agregarSesionRealizadaDeEjercicios;
        private readonly ActualizarSesionRealizadaDeEjerciciosCasoDeUso actualizarSesionRealizadaDeEjercicios;
        private readonly EliminarSesionRealizadaDeEjerciciosCasoDeUso eliminarSesionRealizadaDeEjercicios;

        public SesionRealizadaDeEjerciciosController(ObtenerTodasLasSesionesRealizadasDeEjerciciosCasoDeUso obtenerTodasLasSesionesRealizadasDeEjercicios,
                ObtenerSesionRealizadaDeEjerciciosPorIdCasoDeUso obtenerSesionRealizadaDeEjerciciosPorIdCasoDeUso,
                EliminarSesionRealizadaDeEjerciciosCasoDeUso eliminarSesionRealizadaDeEjercicios,
                ActualizarSesionRealizadaDeEjerciciosCasoDeUso actualizarSesionRealizadaDeEjercicios,
                AgregarSesionRealizadaDeEjerciciosCasoDeUso agregarSesionRealizadaDeEjercicios)
        {
            this.obtenerTodasLasSesionesRealizadasDeEjercicios = obtenerTodasLasSesionesRealizadasDeEjercicios;
            this.obtenerSesionRealizadaDeEjerciciosPorIdCasoDeUso = obtenerSesionRealizadaDeEjerciciosPorIdCasoDeUso;
            this.eliminarSesionRealizadaDeEjercicios = eliminarSesionRealizadaDeEjercicios;
            this.actualizarSesionRealizadaDeEjercicios = actualizarSesionRealizadaDeEjercicios;
            this.agregarSesionRealizadaDeEjercicios = agregarSesionRealizadaDeEjercicios;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var sesionesRealizadasDeEjercicios = await obtenerTodasLasSesionesRealizadasDeEjercicios.Ejecutar();
            return Ok(sesionesRealizadasDeEjercicios);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> ObtenerPorId(long id)
        {
            var sesionRealizadaDeEjercicios = await obtenerSesionRealizadaDeEjerciciosPorIdCasoDeUso.Ejecutar(id);
            if (sesionRealizadaDeEjercicios == null)
            {
                return NotFound();
            }
            return Ok(sesionRealizadaDeEjercicios);
        }

        [HttpPost]
        public async Task<IActionResult> Agregar([FromBody] AgregarSesionRealizadaDeEjerciciosDTO sesionRealizadaDeEjercicios)
        {
            var nuevaSesionRealizadaDeEjercicios = await agregarSesionRealizadaDeEjercicios.Ejecutar(sesionRealizadaDeEjercicios);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevaSesionRealizadaDeEjercicios.Id }, nuevaSesionRealizadaDeEjercicios);
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> Actualizar(long id, [FromBody] SesionRealizadaDeEjerciciosDTO sesionRealizadaDeEjercicios)
        {
            if (id != sesionRealizadaDeEjercicios.Id)
            {
                return BadRequest();
            }
            var sesionActualizada = await actualizarSesionRealizadaDeEjercicios.Ejecutar(sesionRealizadaDeEjercicios);
            if (sesionActualizada == null)
            {
                return NotFound("El ID de la sesion realizada de ejercicio no coincide.");
            }
            return Ok(sesionActualizada);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> Eliminar(long id)
        {
            await eliminarSesionRealizadaDeEjercicios.Ejecutar(id);
            return NoContent();
        }
    }
}
