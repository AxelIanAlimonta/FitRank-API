using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FitRank_API.Application.CasosDeUso.EjercicioAsignadoCasoDeUso;
using FitRank_API.Application.DTOs.EjercicioAsignadoDTOs;

namespace FitRank_API.Presentacion.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EjercicioAsignadoController : ControllerBase
{
    private readonly ActualizarEjercicioAsignadoCasoDeUso _actualizarEjercicioAsignadoCasoDeUso;
    private readonly ObtenerEjercicioAsignadoPorIdCasoDeUso _obtenerEjercicioAsignadoPorIdCasoDeUso;
    private readonly AgregarEjercicioAsignadoCasoDeUso _agregarEjercicioAsignadoCasoDeUso;
    private readonly EliminarEjercicioAsignadoCasoDeUso _eliminarEjercicioAsignadoCasoDeUso;
    private readonly ObtenerEjerciciosAsignadosCasoDeUso _obtenerEjerciciosAsignadosCasoDeUso;

    public EjercicioAsignadoController(
        ActualizarEjercicioAsignadoCasoDeUso actualizarEjercicioAsignadoCasoDeUso,
        ObtenerEjercicioAsignadoPorIdCasoDeUso obtenerEjercicioRealizadoPorIdCasoDeUso,
        AgregarEjercicioAsignadoCasoDeUso agregarEjercicioAsignadoCasoDeUso,
        EliminarEjercicioAsignadoCasoDeUso eliminarEjercicioAsignadoCasoDeUso,
        ObtenerEjerciciosAsignadosCasoDeUso obtenerEjerciciosAsignadosCasoDeUso)
    {
        _actualizarEjercicioAsignadoCasoDeUso = actualizarEjercicioAsignadoCasoDeUso;
        _obtenerEjercicioAsignadoPorIdCasoDeUso = obtenerEjercicioRealizadoPorIdCasoDeUso;
        _agregarEjercicioAsignadoCasoDeUso = agregarEjercicioAsignadoCasoDeUso;
        _eliminarEjercicioAsignadoCasoDeUso = eliminarEjercicioAsignadoCasoDeUso;
        _obtenerEjerciciosAsignadosCasoDeUso = obtenerEjerciciosAsignadosCasoDeUso;
    }

    //get
    [HttpGet]
    public async Task<IActionResult> ObtenerTodo()
    {
        try
        {
            var ejerciciosAsignados = await _obtenerEjerciciosAsignadosCasoDeUso.Ejecutar();
            return Ok(ejerciciosAsignados);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error interno del servidor.");
        }
    }

    [HttpGet]
    [Route("{id:long}")]
    public async Task<IActionResult> ObtenerPorId(long id)
    {
        try
        {
            var ejercicioAsignado = await _obtenerEjercicioAsignadoPorIdCasoDeUso.Ejecutar(id);
            if (ejercicioAsignado == null)
            {
                return NotFound($"No se encontró ningún ejercicio asignado con ID {id}.");
            }
            return Ok(ejercicioAsignado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    //póst
    [HttpPost]
    public async Task<IActionResult> Agregar([FromBody] AgregarEjercicioAsignadoDTO ejercicioAsignadoDTO)
    {
        if (ejercicioAsignadoDTO == null)
        {
            return BadRequest("El cuerpo de la solicitud no puede ser nulo.");
        }
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        try
        {
            var nuevoEjercicioAsignado = await _agregarEjercicioAsignadoCasoDeUso.Ejecutar(ejercicioAsignadoDTO);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevoEjercicioAsignado.Id }, nuevoEjercicioAsignado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error interno del servidor");
        }
    }


    //put
    [HttpPut]
    [Route("{id:long}")]
    public async Task<IActionResult> Actualizar(long id, [FromBody] ActualizarEjercicioAsignadoDTO ejercicioAsignadoDTO)
    {
        if (ejercicioAsignadoDTO == null)
        {
            return BadRequest("El cuerpo de la solicitud no puede ser nulo.");
        }
        if (id != ejercicioAsignadoDTO.Id)
        {
            return BadRequest("El ID del ejercicio asignado no coincide con el ID proporcionado.");
        }
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var ejercicioAsignadoActualizado = await _actualizarEjercicioAsignadoCasoDeUso.Ejecutar(ejercicioAsignadoDTO);
            if (ejercicioAsignadoActualizado == null)
            {
                return NotFound($"No se encontró ningún ejercicio asignado con ID {id} para actualizar.");
            }
            return Ok(ejercicioAsignadoActualizado);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error interno del servidor.");
        }
    }


    [HttpDelete]
    [Route("{id:long}")]
    public async Task<IActionResult> Eliminar(long id)
    {
        try
        {
            var resultado = await _eliminarEjercicioAsignadoCasoDeUso.Ejecutar(id);
            if (!resultado)
            {
                return NotFound($"No se encontró ningún ejercicio asignado con ID {id} para eliminar.");
            }
            return NoContent();
        }
        catch (Exception ex)
        {
            return StatusCode(500, "Error interno del servidor.");
        }
    }
}
