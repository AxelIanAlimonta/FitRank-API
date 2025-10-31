using FitRank_API.Application.CasosDeUso.NotificacionCasosDeUso;
using FitRank_API.Application.DTOs.NotificacionDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class NotificacionController : ControllerBase
{
    private readonly AgregarNotificacionCasoDeUso _agregarCaso;
    private readonly ObtenerNotificacionPorUsuarioCasoDeUso _obtenerCaso;

    public NotificacionController(
        AgregarNotificacionCasoDeUso agregarCaso,
        ObtenerNotificacionPorUsuarioCasoDeUso obtenerCaso)
    {
        _agregarCaso = agregarCaso;
        _obtenerCaso = obtenerCaso;
    }

    [HttpPost]
    public async Task<IActionResult> Crear([FromBody] AgregarNotificacionDTO dto)
    {
        var usuarioId = long.Parse(User.FindFirst("id")!.Value);
        var resultado = await _agregarCaso.Ejecutar(usuarioId, dto);
        return CreatedAtAction(nameof(Crear), resultado);
    }

    [HttpGet("usuario")]
    public async Task<IActionResult> ObtenerPorUsuario()
    {
        var usuarioId = long.Parse(User.FindFirst("id")!.Value);
        var notificaciones = await _obtenerCaso.Ejecutar(usuarioId);
        return Ok(notificaciones);
    }
}
