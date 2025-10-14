using AutoMapper;
using FitRank_API.Application.DTOs.RutinaNamespace;
using FitRank_API.Application.DTOs.RutinaNameSpace;
using FitRank_API.Application.Interfaces;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.Services
{
    public class RutinaServicesImpl: IRutinaService
    {
        private readonly IRutinaRepository _rutinaRepository;
        private readonly IMapper _mapper;

        public RutinaServicesImpl(IRutinaRepository rutinaRepository, IMapper mapper)
        {
            _rutinaRepository = rutinaRepository;
            _mapper = mapper;
        }
        //RUTINAS
        public async Task<CrearRutinaDTO> CrearRutinaAsync(CrearRutinaDTO nuevaRutina)
        {
            var entidad = _mapper.Map<Rutina>(nuevaRutina);
            var creada = await _rutinaRepository.CrearRutinaAsync(entidad);
            return _mapper.Map<CrearRutinaDTO>(creada);
        }

        public async Task<List<RutinaDTO>> ListarRutinasAsync()
        {
            var rutinas = await _rutinaRepository.ListarRutinasAsync();
            return _mapper.Map<List<RutinaDTO>>(rutinas);
        }

        public async Task<List<RutinaDTO>> ListarRutinasPorUsuarioAsync(int usuarioId)
        {
            var rutina = await _rutinaRepository.ListarRutinasPorUsuarioAsync(usuarioId);
            return _mapper.Map<List<RutinaDTO>>(rutina);
        }

        public async Task<CrearRutinaDTO> ObtenerRutinaPorIdAsync(int id)
        {
            var rutina = await _rutinaRepository.ObtenerRutinaPorIdAsync(id);
            return _mapper.Map<CrearRutinaDTO>(rutina);
        }

        public async Task<EditarRutinaDTO> EditarRutinaAsync(int id, EditarRutinaDTO rutinaActualizada)
        {
            var rutina = await _rutinaRepository.ObtenerRutinaPorIdAsync(id);
            if (rutina == null)
            {
                return null;
            }

            _mapper.Map(rutinaActualizada, rutina); //Copia los valores del DTO sobre el objeto entidad

            var rutinaEditada = await _rutinaRepository.ActualizarRutinaAsync(rutina);
            return _mapper.Map<EditarRutinaDTO>(rutinaEditada);
        }

        public async Task<bool> EliminarRutinaAsync(int id)
        {
            return await _rutinaRepository.EliminarRutinaAsync(id);
        }

    }
}
