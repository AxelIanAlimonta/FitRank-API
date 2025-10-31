using AutoMapper;
using FitRank_API.Application.DTOs;
using FitRank_API.Application.DTOs.SerieDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.SerieCasosDeUso
{
    public class ObtenerSeriePorIdCasoDeUso
    {
        private readonly ISerieRepositorio _repo;
        private readonly IMapper _mapper;

        public ObtenerSeriePorIdCasoDeUso(ISerieRepositorio repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<ObtenerSerieDTO?> Ejecutar(long id)
        {
            var serie = await _repo.ObtenerPorIdAsync(id);
            return serie == null ? null : _mapper.Map<ObtenerSerieDTO>(serie);
        }
    }
}
