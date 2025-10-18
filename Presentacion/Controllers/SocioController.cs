using Microsoft.AspNetCore.Mvc;
//using casos de uso
using FitRank_API.Application.CasosDeUso.SocioCasoDeUso;
using FitRank_API.Application.DTOs.SocioDTOs;


namespace FitRank_API.Presentacion.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SocioController : ControllerBase
{
    //traeme todos los casos de uso de socio
    private readonly ObtenerSociosCasoDeUso _obtenerSociosCasoDeUso;
    private readonly ObtenerSocioPorIdCasoDeUso _obtenerSocioPorIdCasoDeUso;
    private readonly AgregarSocioCasoDeUso _agregarSocioCasoDeUso;
    private readonly ActualizarSocioCasoDeUso _actualizarSocioCasoDeUso;
    private readonly EliminarSocioCasoDeUso _eliminarSocioCasoDeUso;

    public SocioController(ObtenerSociosCasoDeUso obtenerSociosCasoDeUso,
        ObtenerSocioPorIdCasoDeUso obtenerSocioPorIdCasoDeUso,
        AgregarSocioCasoDeUso agregarSocioCasoDeUso,
        ActualizarSocioCasoDeUso actualizarSocioCasoDeUso,
        EliminarSocioCasoDeUso eliminarSocioCasoDeUso)
    {
        _obtenerSociosCasoDeUso = obtenerSociosCasoDeUso;
        _obtenerSocioPorIdCasoDeUso = obtenerSocioPorIdCasoDeUso;
        _agregarSocioCasoDeUso = agregarSocioCasoDeUso;
        _actualizarSocioCasoDeUso = actualizarSocioCasoDeUso;
        _eliminarSocioCasoDeUso = eliminarSocioCasoDeUso;
    }

    [HttpGet]
    public async Task<IActionResult> obtenerTodos()
    {
        var socios = await _obtenerSociosCasoDeUso.Ejecutar();
        return Ok(socios);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> obtenerPorId(long id)
    {
        var socio = await _obtenerSocioPorIdCasoDeUso.Ejecutar(id);
        if (socio == null)
        {
            return NotFound();
        }
        return Ok(socio);
    }

    [HttpPost]
    public async Task<IActionResult> agregar([FromBody] AgregarSocioDTO socio)
    {
        var nuevoSocio = await _agregarSocioCasoDeUso.Ejecutar(socio);
        return CreatedAtAction(nameof(obtenerPorId), new { id = nuevoSocio.Id }, nuevoSocio);


    }

    [HttpPut("{id}")]
    public async Task<IActionResult> actualizar(long id, [FromBody] SocioDTO socio)
    {
        if (id != socio.Id)
        {
            return BadRequest();
        }
        var socioActualizado = await _actualizarSocioCasoDeUso.Ejecutar(socio);
        if (socioActualizado == null)
        {
            return NotFound();
        }
        return Ok(socioActualizado);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> eliminar(long id)
    {
        var resultado = await _eliminarSocioCasoDeUso.Ejecutar(id);
        if (!resultado)
        {
            return NotFound();
        }
        return NoContent();
    }

}
