using AutoMapper;
using FitRank_API.Application.DTOs;
using FitRank_API.Application.DTOs.EntrenamientoDTOs;
using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.UseCases.Entrenamiento
{
    public class ObtenerEntrenamientoPorIdCasoDeUso
    {
        private readonly IEntrenamientoRepositorio _repo;
        private readonly IMapper _mapper;

        public ObtenerEntrenamientoPorIdCasoDeUso(IEntrenamientoRepositorio repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public virtual async Task<ObtenerEntrenamientoDTO?> Ejecutar(long id)
        {
            var ent = await _repo.ObtenerPorIdAsync(id);
            return ent == null ? null : _mapper.Map<ObtenerEntrenamientoDTO>(ent);
        }
    }
}
