using System.ComponentModel.DataAnnotations;

namespace FitRank_API.Application.DTOs.EjercicioRealizadoDTOs;

public class ActualizarEjercicioRealizadoDTO
{
    public long Id { get; set; }
    public long EjercicioId { set; get; }
    public long SocioId { set; get; }
    public long RutinaId { set; get; }

}