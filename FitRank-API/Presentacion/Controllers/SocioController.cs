using Microsoft.AspNetCore.Mvc;
//using casos de uso
using FitRank_API.Application.CasosDeUso.SocioCasoDeUso;
using FitRank_API.Application.DTOs.SocioDTOs;
using FitRank_API.Application.CasosDeUso.SocioCasosDeUso;


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
    private readonly CambiarParticipacionRankingCasoDeUso _cambiarParticipacionRankingCasoDeUso;
    private readonly ObtenerSocioConMedidasCasoDeUso _obtenerSocioConMedidasCasoDeUso;
    private readonly EditarPerfilSocioCasoDeUso _editarPerfilCasoDeUso;

 
    public SocioController(
        ObtenerSociosCasoDeUso obtenerSociosCasoDeUso,
        ObtenerSocioPorIdCasoDeUso obtenerSocioPorIdCasoDeUso,
        AgregarSocioCasoDeUso agregarSocioCasoDeUso,
        ActualizarSocioCasoDeUso actualizarSocioCasoDeUso,
        EliminarSocioCasoDeUso eliminarSocioCasoDeUso,
        CambiarParticipacionRankingCasoDeUso cambiarParticipacionRankingCasoDeUso,
        ObtenerSocioConMedidasCasoDeUso obtenerSocioConMedidasCasoDeUso,
        EditarPerfilSocioCasoDeUso editarPerfilCasoDeUso
        )
    {
        _obtenerSociosCasoDeUso = obtenerSociosCasoDeUso;
        _obtenerSocioPorIdCasoDeUso = obtenerSocioPorIdCasoDeUso;
        _agregarSocioCasoDeUso = agregarSocioCasoDeUso;
        _actualizarSocioCasoDeUso = actualizarSocioCasoDeUso;
        _eliminarSocioCasoDeUso = eliminarSocioCasoDeUso;
        _cambiarParticipacionRankingCasoDeUso = cambiarParticipacionRankingCasoDeUso;
        _obtenerSocioConMedidasCasoDeUso = obtenerSocioConMedidasCasoDeUso;
        _editarPerfilCasoDeUso = editarPerfilCasoDeUso;
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

    [HttpPut("socio/{socioId}/participacion-ranking")]
    public async Task<IActionResult> CambiarParticipacionRanking(long socioId, [FromBody] CambiarParticipacionRankingDTO body)
    {
        var ok = await _cambiarParticipacionRankingCasoDeUso.Ejecutar(socioId, body.ParticipaEnRanking);

        if (!ok)
            return NotFound(new { mensaje = "Socio no encontrado" });

        return Ok(new { mensaje = "Participación actualizada", participa = body.ParticipaEnRanking });
    }
    [HttpGet("completo/{id}")]
    public async Task<IActionResult> ObtenerSocioCompleto(long id)
    {
        var result = await _obtenerSocioConMedidasCasoDeUso.Ejecutar(id);

        if (result == null)
            return NotFound("No existe el socio");

        return Ok(result);
    }

    [HttpPut("editar-perfil/{socioId}")]
    public async Task<IActionResult> EditarPerfil(long socioId, [FromBody] EditarPerfilSocioDTO dto)
    {
        var ok = await _editarPerfilCasoDeUso.Ejecutar(socioId, dto);

        if (!ok)
            return NotFound(new { mensaje = "Socio no encontrado" });

        return Ok(new { mensaje = "Perfil actualizado correctamente" });
    }

}
