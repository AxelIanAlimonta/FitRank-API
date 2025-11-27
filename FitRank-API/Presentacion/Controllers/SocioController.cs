using Microsoft.AspNetCore.Mvc;
using FitRank_API.Application.CasosDeUso.SocioCasoDeUso;
using FitRank_API.Application.DTOs.SocioDTOs;
using FitRank_API.Application.CasosDeUso.SocioCasosDeUso;

namespace FitRank_API.Presentacion.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SocioController : ControllerBase
{
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
        EditarPerfilSocioCasoDeUso editarPerfilCasoDeUso)
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
    public async Task<IActionResult> ObtenerTodos()
    {
        try
        {
            var socios = await _obtenerSociosCasoDeUso.Ejecutar();
            return Ok(socios);
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
            return BadRequest(new { Mensaje = "El ID del socio debe ser mayor a cero." });

        try
        {
            var socio = await _obtenerSocioPorIdCasoDeUso.Ejecutar(id);
            if (socio == null)
                return NotFound(new { Mensaje = $"El socio con ID {id} no fue encontrado." });

            return Ok(socio);
        }
        catch (Exception)
        {
            return StatusCode(500, new { Mensaje = "Error interno del servidor." });
        }
    }

    [HttpPost]
    public async Task<IActionResult> Agregar([FromBody] AgregarSocioDTO socio)
    {
        if (socio == null)
            return BadRequest(new { Mensaje = "El objeto socio no puede ser nulo." });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var nuevoSocio = await _agregarSocioCasoDeUso.Ejecutar(socio);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = nuevoSocio.Id }, nuevoSocio);
        }
        catch (Exception)
        {
            return StatusCode(500, new { Mensaje = "Error interno del servidor." });
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Actualizar(long id, [FromBody] SocioDTO socio)
    {
        if (id <= 0)
            return BadRequest(new { Mensaje = "El ID del socio debe ser mayor a cero." });

        if (socio == null)
            return BadRequest(new { Mensaje = "El objeto socio no puede ser nulo." });

        if (id != socio.Id)
            return BadRequest(new { Mensaje = "El ID del socio no coincide con el ID del objeto." });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var socioActualizado = await _actualizarSocioCasoDeUso.Ejecutar(socio);
            if (socioActualizado == null)
                return NotFound(new { Mensaje = $"El socio con ID {id} no fue encontrado." });

            return Ok(socioActualizado);
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
            return BadRequest(new { Mensaje = "El ID del socio debe ser mayor a cero." });

        try
        {
            var resultado = await _eliminarSocioCasoDeUso.Ejecutar(id);
            if (!resultado)
                return NotFound(new { Mensaje = $"El socio con ID {id} no fue encontrado." });

            return NoContent();
        }
        catch (Exception)
        {
            return StatusCode(500, new { Mensaje = "Error interno del servidor." });
        }
    }

    [HttpPut("socio/{socioId}/participacion-ranking")]
    public async Task<IActionResult> CambiarParticipacionRanking(long socioId, [FromBody] CambiarParticipacionRankingDTO body)
    {
        if (socioId <= 0)
            return BadRequest(new { Mensaje = "El ID del socio debe ser mayor a cero." });

        if (body == null)
            return BadRequest(new { Mensaje = "El objeto de participación no puede ser nulo." });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var ok = await _cambiarParticipacionRankingCasoDeUso.Ejecutar(socioId, body.ParticipaEnRanking);
            if (!ok)
                return NotFound(new { Mensaje = $"El socio con ID {socioId} no fue encontrado." });

            return Ok(new { Mensaje = "Participación actualizada correctamente.", participa = body.ParticipaEnRanking });
        }
        catch (Exception)
        {
            return StatusCode(500, new { Mensaje = "Error interno del servidor." });
        }
    }

    [HttpGet("completo/{id}")]
    public async Task<IActionResult> ObtenerSocioCompleto(long id)
    {
        if (id <= 0)
            return BadRequest(new { Mensaje = "El ID del socio debe ser mayor a cero." });

        try
        {
            var result = await _obtenerSocioConMedidasCasoDeUso.Ejecutar(id);
            if (result == null)
                return NotFound(new { Mensaje = $"El socio con ID {id} no fue encontrado." });

            return Ok(result);
        }
        catch (Exception)
        {
            return StatusCode(500, new { Mensaje = "Error interno del servidor." });
        }
    }

    [HttpPut("editar-perfil/{socioId}")]
    public async Task<IActionResult> EditarPerfil(long socioId, [FromBody] EditarPerfilSocioDTO dto)
    {
        if (socioId <= 0)
            return BadRequest(new { Mensaje = "El ID del socio debe ser mayor a cero." });

        if (dto == null)
            return BadRequest(new { Mensaje = "El objeto de perfil no puede ser nulo." });

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var ok = await _editarPerfilCasoDeUso.Ejecutar(socioId, dto);
            if (!ok)
                return NotFound(new { Mensaje = $"El socio con ID {socioId} no fue encontrado." });

            return Ok(new { Mensaje = "Perfil actualizado correctamente." });
        }
        catch (Exception)
        {
            return StatusCode(500, new { Mensaje = "Error interno del servidor." });
        }
    }
}
