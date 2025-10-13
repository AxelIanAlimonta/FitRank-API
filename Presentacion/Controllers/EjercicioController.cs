using FitRank_API.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FitRank_API.Presentacion.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EjercicioController : ControllerBase
    {
        private readonly IEjercicioServicio _servicio;

        public EjercicioController(IEjercicioServicio servicio)
        {
            _servicio = servicio;
        }

        [HttpGet]
        public async Task<IActionResult> ListarEjercicios()
        {
            var ejercicios = await _servicio.ListarEjerciciosAsync();
            return Ok(ejercicios);
        }
    }
}
