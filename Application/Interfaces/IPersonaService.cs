using FitRank_API.Application.DTOs.Persona;

namespace FitRank_API.Application.Interfaces;

public interface IPersonaService
{
    Task<List<PersonaDTO>> GetAllAsync();
    Task<PersonaDTO> AddAsync(CreatePersonaDTO persona);
    Task UpdateAsync(UpdatePersonaDTO persona);
    Task DeleteAsync(long id);
    Task<PersonaDTO?> GetByIdAsync(long id);
}
