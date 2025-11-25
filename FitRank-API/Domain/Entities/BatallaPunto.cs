using FitRank_API.Domain.Enums;

namespace FitRank_API.Domain.Entities;
public class BatallaPunto
{
    public int Id { get; set; }

    public int SocioAId { get; set; }
    public int SocioBId { get; set; }

    public BatallaTipo Tipo { get; set; } // Puntos, MiniObjetivos

    public DateTime FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }    // <-- nullable

    // Progreso por puntos
    public double PuntosA { get; set; }
    public double PuntosB { get; set; }

    // Mini-objetivos
    public int ObjetivosTotales { get; set; }
    public int ObjetivosCumplidosA { get; set; }
    public int ObjetivosCumplidosB { get; set; }
    public string? ObjetivosJson { get; set; }

    public BatallaEstado Estado { get; set; } // Pendiente, Activa, Finalizada
    public int? GanadorId { get; set; }
}

