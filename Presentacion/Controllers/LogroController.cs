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
    public LogroController(ObtenerLogrosCasoDeUso obtenerLogrosCasoDeUso,
        AgregarLogroCasoDeUso agregarLogroCasoDeUso,
        ActualizarLogroCasoDeUso actualizarLogroCasoDeUso,
        EliminarLogroCasoDeUso eliminarLogroCasoDeUso,
        ObtenerLogroPorIdCasoDeUso obtenerLogroPorIdCasoDeUso)
    {
        _obtenerLogrosCasoDeUso = obtenerLogrosCasoDeUso;
        _agregarLogroCasoDeUso = agregarLogroCasoDeUso;
        _actualizarLogroCasoDeUso = actualizarLogroCasoDeUso;
        _eliminarLogroCasoDeUso = eliminarLogroCasoDeUso;
        _obtenerLogroPorIdCasoDeUso = obtenerLogroPorIdCasoDeUso;
    }

    [HttpGet]
    public async Task<IActionResult> ObtenerTodos()
    {
        var logros = await _obtenerLogrosCasoDeUso.Ejecutar();
        return Ok(logros);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> ObtenerPorId(long id)
    {
        var logro = await _obtenerLogroPorIdCasoDeUso.Ejecutar(id);
        if (logro == null)
        {
            return NotFound();
        }
        return Ok(logro);
    }

    [HttpPost]
    public async Task<IActionResult> Agregar([FromBody] AgregarLogroDTO crearLogroDTO)
    {
        var logroCreado = await _agregarLogroCasoDeUso.Ejecutar(crearLogroDTO);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = logroCreado.Id }, logroCreado);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(long id, [FromBody] ActualizarLogroDTO actualizarLogroDTO)
    {
        if (id != actualizarLogroDTO.Id)
        {
            return BadRequest("El ID del logro no coincide con el ID proporcionado en la ruta.");
        }

        var logroActualizado = await _actualizarLogroCasoDeUso.Ejecutar(actualizarLogroDTO);
        if (logroActualizado == null)
        {
            return NotFound();
        }

        return Ok(logroActualizado);

    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Eliminar(long id)
    {
        var exito = await _eliminarLogroCasoDeUso.Ejecutar(id);
        if (!exito)
        {
            return NotFound();
        }
        return NoContent();
    }
}
