using FitRank_API.Application.DTOs.GimnasioDTOs;
using FitRank_API.Domain.Entities;
using static System.Net.Mime.MediaTypeNames;

public interface IGimnasioRepositorio
{
    Task<List<Gimnasio>> ObtenerTodosLosGimnasios();
    Task<Gimnasio?> ObtenerGimnasioPorId(long id);
    Task<Gimnasio> AgregarGimnasio(Gimnasio gimnasio);
    Task<Gimnasio?> ActualizarGimnasio(long id, ActualizarGimnasioDTO gimnasioDTO);
    Task<bool> EliminarGimnasio(long id);
    Task<Gimnasio?> ObtenerPorAdministradorIdAsync(long adminId);
}
