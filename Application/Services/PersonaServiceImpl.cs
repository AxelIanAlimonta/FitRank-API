using FitRank_API.Application.Interfaces;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.Services
{
    public class PersonaServiceImpl : IPersonaService
    {
        private readonly IPersonaRepository _personaRepository;
        public PersonaServiceImpl(IPersonaRepository personaRepository)
        {
            _personaRepository = personaRepository;
        }

        public async Task<List<Domain.Entities.Persona>> GetAllAsync()
        {
            return await _personaRepository.GetAllAsync();
        }

        public async Task AddAsync(Domain.Entities.Persona persona)
        {
            await _personaRepository.AddAsync(persona);
        }

        public async Task UpdateAsync(Domain.Entities.Persona persona)
        {
            await _personaRepository.UpdateAsync(persona);
        }

        public async Task DeleteAsync(long id)
        {
            await _personaRepository.DeleteAsync(id);
        }

        public async Task<Domain.Entities.Persona?> GetByIdAsync(long id)
        {
            return await _personaRepository.GetByIdAsync(id);
        }


    }
}
