using System.ComponentModel.DataAnnotations;

namespace FitRank_API.Application.DTOs.EjercicioRealizadoDTOs;

public class AgregarEjercicioRealizadoDTO
{
    public long EjercicioId { get; set; }
    public long SocioId { get; set; }
    public long RutinaId { get; set; }

}

