using FitRank_API.Application.CasosDeUso.LogroCasosDeUso;
using FitRank_API.Application.DTOs.LogroDTOs;
using Microsoft.AspNetCore.Http;
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
    public LogroController(ObtenerLogrosCasoDeUso obtenerLogrosCasoDeUso,
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
        catch (Exception ex)
        {
            return StatusCode(500, "Ocurrió un error en el servidor.");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(long id)
    {
        try
        {
            var logro = await _obtenerLogroPorIdCasoDeUso.Ejecutar(id);
            if (logro == null)
            {
                return NotFound($"No se encontró ningún logro con ID {id}.");
            }
            return Ok(logro);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Ocurrió un error en el servidor.");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Agregar([FromBody] AgregarLogroDTO crearLogroDTO)
    {
        if (crearLogroDTO == null)
        {
            return BadRequest("El logro proporcionado es nulo.");
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
        catch (Exception ex)
        {
            return StatusCode(500, "Ocurrió un error en el servidor.");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(long id, [FromBody] ActualizarLogroDTO actualizarLogroDTO)
    {
        if (actualizarLogroDTO == null)
        {
            return BadRequest("El logro proporcionado es nulo.");
        }
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        if (id != actualizarLogroDTO.Id)
        {
            return BadRequest("El ID del logro no coincide con el ID proporcionado en la ruta.");
        }

        try
        {
            var logroActualizado = await _actualizarLogroCasoDeUso.Ejecutar(actualizarLogroDTO);
            if (logroActualizado == null)
            {
                return NotFound($"No se encontró ningún logro con ID {id} para actualizar.");
            }

            return Ok(logroActualizado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Ocurrió un error en el servidor.");

        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(long id)
    {
        try
        {
            var exito = await _eliminarLogroCasoDeUso.Ejecutar(id);
            if (!exito)
            {
                return NotFound("No se encontró ningún logro con el ID proporcionado para eliminar.");
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Ocurrió un error en el servidor.");
        }
    }

    //Para debug
    [HttpPost("otorgar")]
    public async Task<ActionResult<LogroOtorgadoDTO>> Otorgar([FromBody] OtorgarLogroPorNombreClaveDTO dto)
    {
        var resultado = await _otorgarLogroPorNombreClaveCasoDeUso.Ejecutar(dto);
        if (!resultado.Otorgado)
            return BadRequest(resultado);

        return Ok(resultado);
    }
}
