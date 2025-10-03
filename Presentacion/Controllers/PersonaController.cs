using FitRank_API.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Presentacion.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PersonaController : ControllerBase
{
    private readonly IPersonaService _personaService;
    public PersonaController(IPersonaService personaService)
    {
        _personaService = personaService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var personas = await _personaService.GetAllAsync();
        return Ok(personas);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(long id)
    {
        var persona = await _personaService.GetByIdAsync(id);
        if (persona == null)
        {
            return NotFound();
        }
        return Ok(persona);
    }

    [HttpPost]
    public async Task<IActionResult> Add([FromBody] Persona persona)
    {
        await _personaService.AddAsync(persona);
        return CreatedAtAction(nameof(GetById), new { id = persona.Id }, persona);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(long id, [FromBody] Persona persona)
    {
        if (id != persona.Id)
        {
            return BadRequest();
        }
        await _personaService.UpdateAsync(persona);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        await _personaService.DeleteAsync(id);
        return NoContent();
    }

}
