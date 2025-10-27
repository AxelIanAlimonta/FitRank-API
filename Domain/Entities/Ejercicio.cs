using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FitRank_API.Domain.Entities;

public class Ejercicio
{
    [Key]
    public long Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Descripcion { get; set; } = string.Empty;

    [MaxLength(250)]
    public string UrlImagen { get; set; } = string.Empty;

    public int DuracionEstimada { get; set; } // en minutos o segundos según convengan

    [MaxLength(250)]
    public string UrlVideo { get; set; } = string.Empty;

    // 🔗 Relación con GrupoMuscular
    [ForeignKey("GrupoMuscular")]
    public long GrupoMuscularId { get; set; }
    public GrupoMuscular? GrupoMuscular { get; set; }

    // 🔗 Relación con Maquina
    //[ForeignKey("Maquina")]
    //public long? MaquinaId { get; set; } // puede ser null si no requiere máquina
    //public Maquina? Maquina { get; set; }

    // 🔗 Relación con EjercicioAsignado
    public ICollection<EjercicioAsignado>? EjerciciosAsignados { get; set; }
}
