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
        var ejerciciosAsignados = await _obtenerEjerciciosAsignadosCasoDeUso.Ejecutar();
        return Ok(ejerciciosAsignados);
    }

    [HttpGet]
    [Route("{id:long}")]
    public async Task<IActionResult> ObtenerPorId(long id)
    {
        var ejercicioAsignado = await _obtenerEjercicioAsignadoPorIdCasoDeUso.Ejecutar(id);
        if (ejercicioAsignado == null)
        {
            return NotFound();
        }
        return Ok(ejercicioAsignado);
    }

    //póst
    [HttpPost]
    public async Task<IActionResult> Agregar([FromBody] AgregarEjercicioAsignadoDTO ejercicioAsignadoDTO)
    {
        var nuevoEjercicioAsignado = await _agregarEjercicioAsignadoCasoDeUso.Ejecutar(ejercicioAsignadoDTO);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevoEjercicioAsignado.Id }, nuevoEjercicioAsignado);
    }


    //put
    [HttpPut]
    [Route("{id:long}")]
    public async Task<IActionResult> Actualizar(long id, [FromBody] ActualizarEjercicioAsignadoDTO ejercicioAsignadoDTO)
    {
        if (id != ejercicioAsignadoDTO.Id)
        {
            return BadRequest();
        }

        var ejercicioAsignadoActualizado = await _actualizarEjercicioAsignadoCasoDeUso.Ejecutar(ejercicioAsignadoDTO);
        if (ejercicioAsignadoActualizado == null)
        {
            return NotFound();
        }
        return Ok(ejercicioAsignadoActualizado);
    }


    [HttpDelete]
    [Route("{id:long}")]
    public async Task<IActionResult> Eliminar(long id)
    {
        var resultado = await _eliminarEjercicioAsignadoCasoDeUso.Ejecutar(id);
        if (!resultado)
        {
            return NotFound();
        }
        return NoContent();
    }
}
