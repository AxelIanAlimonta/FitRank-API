using FitRank_API.Domain.Entities.TuProyecto.Models;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Presentacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TorneosController : ControllerBase
    {
        private readonly FitRankDbContext _context;

        public TorneosController(FitRankDbContext context)
        {
            _context = context;
        }

        // POST: api/torneos
        [HttpPost]
        public async Task<IActionResult> CrearTorneo([FromBody] Torneo nuevoTorneo)
        {
            if (string.IsNullOrWhiteSpace(nuevoTorneo.Nombre))
                return BadRequest("El nombre del torneo es obligatorio.");

            var torneo = new Torneo
            {
                Nombre = nuevoTorneo.Nombre,
                ParticipantesJson = "[]",
                Participantes = new List<Participante>()
            };

            _context.Torneos.Add(torneo);
            await _context.SaveChangesAsync();

            return Ok(torneo);
        }

        // POST: api/torneos/{id}/participantes
        [HttpPost("{id}/participantes")]
        public async Task<IActionResult> AgregarParticipante(int id, [FromBody] Participante participante)
        {
            var torneo = await _context.Torneos.FindAsync(id);
            if (torneo == null)
                return NotFound("Torneo no encontrado.");

            var participantes = torneo.Participantes;
            participantes.Add(participante);
            torneo.Participantes = participantes;

            _context.Entry(torneo).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(torneo.Participantes.OrderByDescending(p => p.Puntaje));
        }

        // GET: api/torneos/{id}/ranking
        [HttpGet("{id}/ranking")]
        public async Task<IActionResult> ObtenerRanking(int id)
        {
            var torneo = await _context.Torneos.FindAsync(id);
            if (torneo == null)
                return NotFound("Torneo no encontrado.");

            var ranking = torneo.Participantes.OrderByDescending(p => p.Puntaje).ToList();
            return Ok(ranking);
        }
    }
}
