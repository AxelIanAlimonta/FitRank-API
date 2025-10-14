using FitRank_API.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using FitRank_API.Application.DTOs.Ejercicionamespace;

namespace FitRank_API.Presentacion.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EjercicioController : ControllerBase
{

    private readonly IEjercicioService _ejercicioService;
    public EjercicioController(IEjercicioService ejercicioService)
    {
        _ejercicioService = ejercicioService;
    }

    [HttpGet]
    public async Task<ActionResult<List<EjercicioDTO>>> GetAll()
    {
        var ejercicios = await _ejercicioService.GetAllAsync();
        return Ok(ejercicios);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<EjercicioDTO>> GetById(long id)
    {
        try
        {
            var ejercicio = await _ejercicioService.GetByIdAsync(id);
            return Ok(ejercicio);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }



    [HttpPost]
    public async Task<ActionResult<EjercicioDTO>> Create([FromBody] CrearEjercicioDTO crearEjercicioDto)
    {
        var ejercicio = await _ejercicioService.CreateAsync(crearEjercicioDto);
        if (ejercicio == null)
        {
            return BadRequest("Could not create ejercicio");
        }
        return CreatedAtAction(nameof(GetById), new { id = ejercicio.Id }, ejercicio);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<EjercicioDTO>> Update(long id, [FromBody] EjercicioDTO ejercicioDto)
    {
        if (id != ejercicioDto.Id)
        {
            return BadRequest("ID mismatch");
        }
        try
        {
            var updatedEjercicio = await _ejercicioService.UpdateAsync(id, ejercicioDto);
            return Ok(updatedEjercicio);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    //eliminar ejercicio
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(long id)
    {
        try
        {
            var result = await _ejercicioService.DeleteAsync(id);
            if (result)
            {
                return NoContent();
            }
            else
            {
                return NotFound();
            }
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

}
