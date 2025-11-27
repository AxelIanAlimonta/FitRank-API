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
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult> ObtenerPorId(long id)
        {
            if (id <= 0)
                return BadRequest(new { Mensaje = "El ID debe ser mayor a cero." });

            try
            {
                var gimnasio = await _obtenerGimnasioPorIdCasoDeUso.Ejecutar(id);
                if (gimnasio == null)
                {
                    return NotFound(new { Mensaje = "Gimnasio no encontrado." });
                }
                return Ok(gimnasio);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpPost]
        public async Task<ActionResult> Agregar([FromBody] AgregarGimnasioDTO crearGimnasioDTO)
        {
            if (crearGimnasioDTO == null)
            {
                return BadRequest(new { Mensaje = "El objeto de la solicitud no puede ser nulo." });
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
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Actualizar(long id, [FromBody] ActualizarGimnasioDTO actualizarGimnasioDTO)
        {
            if (id <= 0)
                return BadRequest(new { Mensaje = "El ID debe ser mayor a cero." });

            if (actualizarGimnasioDTO == null)
            {
                return BadRequest(new { Mensaje = "El objeto de la solicitud no puede ser nulo." });
            }

            if (id != actualizarGimnasioDTO.Id)
            {
                return BadRequest(new { Mensaje = "El ID de la URL no coincide con el ID del gimnasio." });
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
                    return NotFound(new { Mensaje = "Gimnasio no encontrado." });
                }

                await _hub.Clients
                    .Group($"gimnasio-{gimnasioActualizado.Id}")
                    .SendAsync("ThemeUpdated", new
                    {
                        colorPrincipal = gimnasioActualizado.ColorPrincipal,
                        colorSecundario = gimnasioActualizado.ColorSecundario,
                        logoUrl = gimnasioActualizado.LogoUrl
                    });

                return Ok(gimnasioActualizado);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Eliminar(long id)
        {
            if (id <= 0)
                return BadRequest(new { Mensaje = "El ID debe ser mayor a cero." });

            try
            {
                var eliminado = await _eliminarGimnasioCasoDeUso.Ejecutar(id);
                if (!eliminado)
                {
                    return NotFound(new { Mensaje = "Gimnasio no encontrado." });
                }
                return NoContent();
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpGet("mi-gimnasio")]
        public async Task<ActionResult> ObtenerMiGimnasio()
        {
            try
            {
                var groupClaim = User.FindFirst("groupsid");
                if (groupClaim == null || !long.TryParse(groupClaim.Value, out var gimnasioId))
                {
                    return BadRequest(new { Mensaje = "ID de gimnasio inválido en el token." });
                }

                var gimnasio = await _obtenerGimnasioPorIdCasoDeUso.Ejecutar(gimnasioId);

                if (gimnasio == null)
                    return NotFound(new { Mensaje = "Gimnasio no encontrado." });

                return Ok(gimnasio);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        [HttpPut("personalizacion")]
        public async Task<IActionResult> ActualizarPersonalizacion([FromBody] ActualizarPersonalizacionDTO dto)
        {
            if (dto == null)
                return BadRequest(new { Mensaje = "El objeto de la solicitud no puede ser nulo." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _actualizarPersonalizacion.Ejecutar(dto);

                if (result == null)
                    return NotFound(new { Mensaje = "Gimnasio no encontrado." });

                await _hub.Clients
                    .Group($"gimnasio-{result.Id}")
                    .SendAsync("ThemeUpdated", new
                    {
                        colorPrincipal = result.ColorPrincipal,
                        colorSecundario = result.ColorSecundario,
                        logoUrl = result.LogoUrl
                    });

                return Ok(result);
            }
            catch (Exception)
            {
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }
    }
}
