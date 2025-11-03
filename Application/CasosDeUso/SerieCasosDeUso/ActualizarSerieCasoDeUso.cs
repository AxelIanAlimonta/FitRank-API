using AutoMapper;
using FitRank_API.Application.DTOs;
using FitRank_API.Application.DTOs.SerieDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.SerieCasosDeUso
{
    public class ActualizarSerieCasoDeUso
    {
        private readonly ISerieRepositorio _repo;
        private readonly IMapper _mapper;

        public ActualizarSerieCasoDeUso(ISerieRepositorio repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public virtual async Task<ObtenerSerieDTO?> Ejecutar(ActualizarSerieDTO dto)
        {
            var serieExistente = await _repo.ObtenerPorIdAsync(dto.Id);
            if (serieExistente == null)
            {
                return null;
            }

            _mapper.Map(dto, serieExistente);

            var serieActualizada = await _repo.ActualizarAsync(serieExistente);

            return _mapper.Map<ObtenerSerieDTO>(serieActualizada);
        }
    }
}
