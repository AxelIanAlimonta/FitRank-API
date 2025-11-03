using FitRank_API.Application.Interfaces;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Domain.Entities;
using FitRank_API.Application.DTOs.Persona;
using AutoMapper;

namespace FitRank_API.Application.Services
{
    public class PersonaServiceImpl : IPersonaService
    {

        private readonly IPersonaRepository _personaRepository;
        private readonly IMapper _mapper;

        public PersonaServiceImpl(IPersonaRepository personaRepository, IMapper mapper)
        {
            _personaRepository = personaRepository;
            _mapper = mapper;
        }

        public async Task<List<PersonaDTO>> GetAllAsync()
        {
            var personas = await _personaRepository.GetAllAsync();
            return _mapper.Map<List<PersonaDTO>>(personas);
        }

        public async Task<PersonaDTO> AddAsync(CreatePersonaDTO persona)
        {
            var newPersona = _mapper.Map<Persona>(persona);
            await _personaRepository.AddAsync(newPersona);
            return _mapper.Map<PersonaDTO>(newPersona);

        }

        public async Task UpdateAsync(UpdatePersonaDTO persona)
        {
            var updatedPersona = _mapper.Map<Persona>(persona);
            await _personaRepository.UpdateAsync(updatedPersona);

        }

        public async Task DeleteAsync(long id)
        {
            await _personaRepository.DeleteAsync(id);
        }

        public async Task<PersonaDTO?> GetByIdAsync(long id)
        {
            var persona = await _personaRepository.GetByIdAsync(id);
            return _mapper.Map<PersonaDTO?>(persona);
        }


    }
}
