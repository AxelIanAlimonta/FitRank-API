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

        public async Task Ejecutar(ActualizarActividadDTO dto)
        {
            var act = await _repo.ObtenerPorIdAsync(dto.Id);
            if (act == null)
                throw new Exception("Actividad no encontrada");

            _mapper.Map(dto, act);
            await _repo.ActualizarAsync(act);
        }
    }
}
