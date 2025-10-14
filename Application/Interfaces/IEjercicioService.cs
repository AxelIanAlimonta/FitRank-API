

using FitRank_API.Application.DTOs.Ejercicionamespace;

namespace FitRank_API.Application.Interfaces;

public interface IEjercicioService
{
    //crud
    Task<List<EjercicioDTO>> GetAllAsync();
    Task<EjercicioDTO> GetByIdAsync(long id);
    Task<EjercicioDTO?> CreateAsync(CrearEjercicioDTO ejercicioDto);
    Task<EjercicioDTO> UpdateAsync(long id, EjercicioDTO ejercicioDto);
    Task<bool> DeleteAsync(long id);


}
