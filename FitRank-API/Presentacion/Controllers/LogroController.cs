using FitRank_API.Application.CasosDeUso.LogroCasosDeUso;
using FitRank_API.Application.DTOs.LogroDTOs;
using Microsoft.AspNetCore.Mvc;

namespace FitRank_API.Presentacion.Controllers;

[Route("api/[controller]")]
[ApiController]
public class LogroController : ControllerBase
{
    private readonly ObtenerLogrosCasoDeUso _obtenerLogrosCasoDeUso;
    private readonly AgregarLogroCasoDeUso _agregarLogroCasoDeUso;
    private readonly ActualizarLogroCasoDeUso _actualizarLogroCasoDeUso;
    private readonly EliminarLogroCasoDeUso _eliminarLogroCasoDeUso;
    private readonly ObtenerLogroPorIdCasoDeUso _obtenerLogroPorIdCasoDeUso;
    private readonly OtorgarLogroPorNombreClaveCasoDeUso _otorgarLogroPorNombreClaveCasoDeUso;

    public LogroController(
        ObtenerLogrosCasoDeUso obtenerLogrosCasoDeUso,
        AgregarLogroCasoDeUso agregarLogroCasoDeUso,
        ActualizarLogroCasoDeUso actualizarLogroCasoDeUso,
        EliminarLogroCasoDeUso eliminarLogroCasoDeUso,
        ObtenerLogroPorIdCasoDeUso obtenerLogroPorIdCasoDeUso,
        OtorgarLogroPorNombreClaveCasoDeUso otorgarLogroPorNombreClaveCasoDeUso)
    {
        _obtenerLogrosCasoDeUso = obtenerLogrosCasoDeUso;
        _agregarLogroCasoDeUso = agregarLogroCasoDeUso;
        _actualizarLogroCasoDeUso = actualizarLogroCasoDeUso;
        _eliminarLogroCasoDeUso = eliminarLogroCasoDeUso;
        _obtenerLogroPorIdCasoDeUso = obtenerLogroPorIdCasoDeUso;
        _otorgarLogroPorNombreClaveCasoDeUso = otorgarLogroPorNombreClaveCasoDeUso;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        try
        {
            var logros = await _obtenerLogrosCasoDeUso.Ejecutar();
            return Ok(logros);
        }
        catch (Exception)
        {
            return StatusCode(500, new { Mensaje = "Error interno del servidor." });
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(long id)
    {
        if (id <= 0)
            return BadRequest(new { Mensaje = "El ID debe ser mayor a cero." });

        try
        {
            var logro = await _obtenerLogroPorIdCasoDeUso.Ejecutar(id);
            if (logro == null)
            {
                return NotFound(new { Mensaje = "Logro no encontrado." });
            }
            return Ok(logro);
        }
        catch (Exception)
        {
            return StatusCode(500, new { Mensaje = "Error interno del servidor." });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Agregar([FromBody] AgregarLogroDTO crearLogroDTO)
    {
        if (crearLogroDTO == null)
        {
            return BadRequest(new { Mensaje = "El objeto de la solicitud no puede ser nulo." });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var logroCreado = await _agregarLogroCasoDeUso.Ejecutar(crearLogroDTO);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = logroCreado.Id }, logroCreado);
        }
        catch (Exception)
        {
            return StatusCode(500, new { Mensaje = "Error interno del servidor." });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(long id, [FromBody] ActualizarLogroDTO actualizarLogroDTO)
    {
        if (id <= 0)
            return BadRequest(new { Mensaje = "El ID debe ser mayor a cero." });

        if (actualizarLogroDTO == null)
        {
            return BadRequest(new { Mensaje = "El objeto de la solicitud no puede ser nulo." });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (id != actualizarLogroDTO.Id)
        {
            return BadRequest(new { Mensaje = "El ID de la URL no coincide con el ID del logro." });
        }

        try
        {
            var logroActualizado = await _actualizarLogroCasoDeUso.Ejecutar(actualizarLogroDTO);
            if (logroActualizado == null)
            {
                return NotFound(new { Mensaje = "Logro no encontrado." });
            }

            return Ok(logroActualizado);
        }
        catch (Exception)
        {
            return StatusCode(500, new { Mensaje = "Error interno del servidor." });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(long id)
    {
        if (id <= 0)
            return BadRequest(new { Mensaje = "El ID debe ser mayor a cero." });

        try
        {
            var exito = await _eliminarLogroCasoDeUso.Ejecutar(id);
            if (!exito)
            {
                return NotFound(new { Mensaje = "Logro no encontrado." });
            }
            return NoContent();
        }
        catch (Exception)
        {
            return StatusCode(500, new { Mensaje = "Error interno del servidor." });
        }
    }

    [HttpPost("otorgar")]
    public async Task<ActionResult<LogroOtorgadoDTO>> Otorgar([FromBody] OtorgarLogroPorNombreClaveDTO dto)
    {
        if (dto == null)
        {
            return BadRequest(new { Mensaje = "El objeto de la solicitud no puede ser nulo." });
        }

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var resultado = await _otorgarLogroPorNombreClaveCasoDeUso.Ejecutar(dto);
            if (!resultado.Otorgado)
                return BadRequest(resultado);

            return Ok(resultado);
        }
        catch (Exception)
        {
            return StatusCode(500, new { Mensaje = "Error interno del servidor." });
        }
    }
}
