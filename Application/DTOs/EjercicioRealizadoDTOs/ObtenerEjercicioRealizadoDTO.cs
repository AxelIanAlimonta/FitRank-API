namespace FitRank_API.Application.DTOs.EjercicioRealizadoDTOs;

public class ObtenerEjercicioRealizadoDTO
{
    public long Id { get; set; }
    public long EjercicioId { get; set; }
    public long SocioId { get; set; }
    public long SesionRealizadaDeEjerciciosId { get; set; }

    // Datos Enriquecidos (Mapeados desde las propiedades de navegación)
    public string NombreEjercicio { get; set; }
    public string NombreRutina { get; set; }
    public string NombreSocio { get; set; }

    public ICollection<ObtenerSerieRealizadaDTO>? SeriesRealizadas { get; set; }
}
