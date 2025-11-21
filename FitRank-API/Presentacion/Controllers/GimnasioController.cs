using FitRank_API.Application.CasosDeUso.GimnasioCasosDeUso;
using FitRank_API.Application.DTOs.GimnasioDTOs;
using FitRank_API.Application.Hubs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace FitRank_API.Presentacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GimnasioController : ControllerBase
    {
        private readonly IHubContext<NotificacionesHub> _hub;
        private readonly ObtenerGimnasiosCasoDeUso _obtenerGimnasiosCasoDeUso;
        private readonly AgregarGimnasioCasoDeUso _agregarGimnasioCasoDeUso;
        private readonly ActualizarGimnasioCasoDeUso _actualizarGimnasioCasoDeUso;
        private readonly EliminarGimnasioCasoDeUso _eliminarGimnasioCasoDeUso;
        private readonly ObtenerGimnasioPorIdCasoDeUso _obtenerGimnasioPorIdCasoDeUso;
        private readonly ActualizarPersonalizacionGimnasioCasoDeUso _actualizarPersonalizacion;

       public GimnasioController(
            IHubContext<NotificacionesHub> hub,
            ObtenerGimnasiosCasoDeUso obtenerGimnasiosCasoDeUso,
            AgregarGimnasioCasoDeUso agregarGimnasioCasoDeUso,
            ActualizarGimnasioCasoDeUso actualizarGimnasioCasoDeUso,
            EliminarGimnasioCasoDeUso eliminarGimnasioCasoDeUso,
            ObtenerGimnasioPorIdCasoDeUso obtenerGimnasioPorIdCasoDeUso,
            ActualizarPersonalizacionGimnasioCasoDeUso actualizarPersonalizacion)
        {
            _hub = hub;
            _obtenerGimnasiosCasoDeUso = obtenerGimnasiosCasoDeUso;
            _agregarGimnasioCasoDeUso = agregarGimnasioCasoDeUso;
            _actualizarGimnasioCasoDeUso = actualizarGimnasioCasoDeUso;
            _eliminarGimnasioCasoDeUso = eliminarGimnasioCasoDeUso;
            _obtenerGimnasioPorIdCasoDeUso = obtenerGimnasioPorIdCasoDeUso;
            _actualizarPersonalizacion = actualizarPersonalizacion;
        }








        [HttpGet]
        public async Task<ActionResult> ObtenerTodos()
        {
            try
            {
                var gimnasios = await _obtenerGimnasiosCasoDeUso.Ejecutar();
                return Ok(gimnasios);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error en el servidor.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> ObtenerPorId(long id)
        {
            var gimnasio = await _obtenerGimnasioPorIdCasoDeUso.Ejecutar(id);
            if (gimnasio == null)
            {
                return NotFound();
            }
            return Ok(gimnasio);
        }

        [HttpPost]
        public async Task<ActionResult> Agregar([FromBody] AgregarGimnasioDTO crearGimnasioDTO)
        {
            if (crearGimnasioDTO == null)
            {
                return BadRequest("El gimnasio no puede ser nulo.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                var gimnasioCreado = await _agregarGimnasioCasoDeUso.Ejecutar(crearGimnasioDTO);
                return CreatedAtAction(nameof(ObtenerPorId), new { id = gimnasioCreado.Id }, gimnasioCreado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error en el servidor.");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Actualizar(long id, [FromBody] ActualizarGimnasioDTO actualizarGimnasioDTO)
        {
            if (actualizarGimnasioDTO == null)
            {
                return BadRequest("El gimnasio no puede ser nulo.");
            }
            if (id != actualizarGimnasioDTO.Id)
            {
                return BadRequest("El ID del gimnasio no coincide.");
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var gimnasioActualizado = await _actualizarGimnasioCasoDeUso.Ejecutar(actualizarGimnasioDTO);

                if (gimnasioActualizado == null)
                {
                    return NotFound("Gimnasio no encontrado.");
                }

             
                await _hub.Clients
                    .Group($"gimnasio-{gimnasioActualizado.Id}")
                    .SendAsync("ThemeUpdated", new
                    {
                        colorPrincipal = gimnasioActualizado.ColorPrincipal,
                        colorSecundario = gimnasioActualizado.ColorSecundario,
                        logoUrl = gimnasioActualizado.LogoUrl
                    });

                Console.WriteLine("🎨 ThemeUpdated enviado por SignalR ✔");

                return Ok(gimnasioActualizado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error en el servidor.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Eliminar(long id)
        {
            try
            {
                var eliminado = await _eliminarGimnasioCasoDeUso.Ejecutar(id);
                if (!eliminado)
                {
                    return NotFound();
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error en el servidor.");
            }
        }
        [HttpGet("mi-gimnasio")]
        public async Task<ActionResult> ObtenerMiGimnasio()
        {
            var gimnasioId = long.Parse(User.FindFirst("groupsid")!.Value);
            var gimnasio = await _obtenerGimnasioPorIdCasoDeUso.Ejecutar(gimnasioId);

            if (gimnasio == null)
                return NotFound();

            return Ok(gimnasio);
        }
        [HttpPut("personalizacion")]
        public async Task<IActionResult> ActualizarPersonalizacion(
                   [FromBody] ActualizarPersonalizacionDTO dto)
        {
            if (dto == null)
                return BadRequest(new { message = "El body no puede ser nulo." });

            var result = await _actualizarPersonalizacion.Ejecutar(dto);

            if (result == null)
                return NotFound(new { message = "No se encontró el gimnasio." });

            // 🔥 ENVIAR SIGNALR A TODO EL GIMNASIO
            await _hub.Clients
                .Group($"gimnasio-{result.Id}")
                .SendAsync("ThemeUpdated", new
                {
                    colorPrincipal = result.ColorPrincipal,
                    colorSecundario = result.ColorSecundario,
                    logoUrl = result.LogoUrl
                });

            Console.WriteLine("🎨 ThemeUpdated enviado por SignalR ✔");

            return Ok(result);
        }
    }

}
