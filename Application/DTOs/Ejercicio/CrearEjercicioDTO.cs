using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Enums;

namespace FitRank_API.Application.DTOs.Ejercicionamespace;

public class CrearEjercicioDTO
{
    
    public int? RutinaId { get; set; }
    public int? MaquinaId { get; set; }
    public string Nombre { get; set; }
    public GrupoMuscular GrupoMuscular { get; set; }
    public Dificultad Dificultad { get; set; }
    public int Series { get; set; }
    public int Repeticiones { get; set; }
    public double Peso { get; set; }
    public int DescansoSegundos { get; set; }
    public bool EsSerieCompuesta { get; set; }
    public bool EsOpcional { get; set; }
    public DayOfWeek DiaAsignado { get; set; }
    public string Observaciones { get; set; }
    public string VideoUrl { get; set; }
    public string TipoEntrenamiento { get; set; }
}
