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

    [HttpGet]
    public async Task<IActionResult> ObtenerTodo()
    {
        try
        {
            var ejerciciosAsignados = await _obtenerEjerciciosAsignadosCasoDeUso.Ejecutar();
            return Ok(ejerciciosAsignados);
        }
        catch (Exception)
        {
            return StatusCode(500, new { Mensaje = "Error interno del servidor." });
        }
    }

    [HttpGet]
    [Route("{id:long}")]
    public async Task<IActionResult> ObtenerPorId(long id)
    {
        if (id <= 0)
            return BadRequest(new { Mensaje = "El ID debe ser mayor a cero." });

        try
        {
            var ejercicioAsignado = await _obtenerEjercicioAsignadoPorIdCasoDeUso.Ejecutar(id);
            if (ejercicioAsignado == null)
            {
                return NotFound(new { Mensaje = "Ejercicio asignado no encontrado." });
            }
            return Ok(ejercicioAsignado);
        }
        catch (Exception)
        {
            return StatusCode(500, new { Mensaje = "Error interno del servidor." });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Agregar([FromBody] AgregarEjercicioAsignadoDTO ejercicioAsignadoDTO)
    {
        if (ejercicioAsignadoDTO == null)
        {
            return BadRequest(new { Mensaje = "El objeto de la solicitud no puede ser nulo." });
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
        catch (Exception)
        {
            return StatusCode(500, new { Mensaje = "Error interno del servidor." });
        }
    }

    [HttpPut]
    [Route("{id:long}")]
    public async Task<IActionResult> Actualizar(long id, [FromBody] ActualizarEjercicioAsignadoDTO ejercicioAsignadoDTO)
    {
        if (id <= 0)
            return BadRequest(new { Mensaje = "El ID debe ser mayor a cero." });

        if (ejercicioAsignadoDTO == null)
        {
            return BadRequest(new { Mensaje = "El objeto de la solicitud no puede ser nulo." });
        }

        if (id != ejercicioAsignadoDTO.Id)
        {
            return BadRequest(new { Mensaje = "El ID de la URL no coincide con el ID del ejercicio asignado." });
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
                return NotFound(new { Mensaje = "Ejercicio asignado no encontrado." });
            }
            return Ok(ejercicioAsignadoActualizado);
        }
        catch (Exception)
        {
            return StatusCode(500, new { Mensaje = "Error interno del servidor." });
        }
    }

    [HttpDelete]
    [Route("{id:long}")]
    public async Task<IActionResult> Eliminar(long id)
    {
        if (id <= 0)
            return BadRequest(new { Mensaje = "El ID debe ser mayor a cero." });

        try
        {
            var resultado = await _eliminarEjercicioAsignadoCasoDeUso.Ejecutar(id);
            if (!resultado)
            {
                return NotFound(new { Mensaje = "Ejercicio asignado no encontrado." });
            }
            return NoContent();
        }
        catch (Exception)
        {
            return StatusCode(500, new { Mensaje = "Error interno del servidor." });
        }
    }
}
