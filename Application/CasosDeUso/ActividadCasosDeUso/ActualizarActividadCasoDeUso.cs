using AutoMapper;
using FitRank_API.Application.DTOs;
using FitRank_API.Application.DTOs.ActividadDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.UseCases.Actividad
{
    public class ActualizarActividadCasoDeUso
    {
        private readonly IActividadRepositorio _repo;
        private readonly IMapper _mapper;

        public ActualizarActividadCasoDeUso(IActividadRepositorio repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public virtual async Task<ObtenerActividadDTO?> Ejecutar(ActualizarActividadDTO dto)
        {
            var actividadExistente = await _repo.ObtenerPorIdAsync(dto.Id);
            if (actividadExistente == null)
            {
                return null;
            }

            _mapper.Map(dto, actividadExistente);

            var actividadActualizada = await _repo.ActualizarAsync(actividadExistente);

            return _mapper.Map<ObtenerActividadDTO>(actividadActualizada);
        }
    }
}
