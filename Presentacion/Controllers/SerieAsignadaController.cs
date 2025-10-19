using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using FitRank_API.Application.CasosDeUso.SerieAsignadaCasoDeUso;
using FitRank_API.Application.DTOs.SerieAsignadaDTOs;

namespace FitRank_API.Presentacion.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SerieAsignadaController : ControllerBase
{
    private readonly ObtenerSerieAsignadaPorIdCasoDeUso _obtenerSerieAsignadaPorIdCasoDeUso;
    private readonly ActualizarSerieAsignadaCasoDeUso _actualizarSerieAsignadaCasoDeUso;
    private readonly EliminarSerieAsignadaCasoDeUso _eliminarSerieAsignadaCasoDeUso;
    private readonly ObtenerSeriesAsignadasCasoDeUso _obtenerSeriesAsignadasCasoDeUso;
    private readonly AgregarSerieAsignadaCasoDeUso _agregarSerieAsignadaCasoDeUso;

    public SerieAsignadaController(
        ObtenerSerieAsignadaPorIdCasoDeUso obtenerSerieAsignadaPorIdCasoDeUso,
        ActualizarSerieAsignadaCasoDeUso actualizarSerieAsignadaCasoDeUso,
        EliminarSerieAsignadaCasoDeUso eliminarSerieAsignadaCasoDeUso,
        ObtenerSeriesAsignadasCasoDeUso obtenerSeriesAsignadasCasoDeUso,
        AgregarSerieAsignadaCasoDeUso agregarSerieAsignadaCasoDeUso)
    {
        _obtenerSerieAsignadaPorIdCasoDeUso = obtenerSerieAsignadaPorIdCasoDeUso;
        _actualizarSerieAsignadaCasoDeUso = actualizarSerieAsignadaCasoDeUso;
        _eliminarSerieAsignadaCasoDeUso = eliminarSerieAsignadaCasoDeUso;
        _obtenerSeriesAsignadasCasoDeUso = obtenerSeriesAsignadasCasoDeUso;
        _agregarSerieAsignadaCasoDeUso = agregarSerieAsignadaCasoDeUso;
    }

    //get
    [HttpGet]
    public async Task<IActionResult> ObtenerTodas()
    {
        var seriesAsignadas = await _obtenerSeriesAsignadasCasoDeUso.Ejecutar();
        return Ok(seriesAsignadas);
    }

    [HttpGet]
    [Route("{id:long}")]
    public async Task<IActionResult> ObtenerPorId(long id)
    {
        var serieAsignada = await _obtenerSerieAsignadaPorIdCasoDeUso.Ejecutar(id);
        if (serieAsignada == null)
        {
            return NotFound();
        }
        return Ok(serieAsignada);
    }

    [HttpPost]
    public async Task<IActionResult> Agregar([FromBody] AgregarSerieAsignadaDTO serieAsignada)
    {
        var nuevaSerieAsignada = await _agregarSerieAsignadaCasoDeUso.Ejecutar(serieAsignada);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevaSerieAsignada.Id }, nuevaSerieAsignada);
    }

    [HttpPut]
    [Route("{id:long}")]
    public async Task<IActionResult> Actualizar(long id, [FromBody] ActualizarSerieAsignadaDTO serieAsignada)
    {
        if (id != serieAsignada.Id)
        {
            return BadRequest();
        }
        var serieAsignadaActualizada = await _actualizarSerieAsignadaCasoDeUso.Ejecutar(serieAsignada);
        if (serieAsignadaActualizada == null)
        {
            return NotFound();
        }
        return Ok(serieAsignadaActualizada);
    }

    [HttpDelete]
    [Route("{id:long}")]
    public async Task<IActionResult> Eliminar(long id)
    {
        var resultado = await _eliminarSerieAsignadaCasoDeUso.Ejecutar(id);
        if (!resultado)
        {
            return NotFound();
        }
        return NoContent();
    }
}
