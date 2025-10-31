using AutoMapper;
using FitRank_API.Application.DTOs;
using FitRank_API.Application.DTOs.SerieDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.UseCases.Serie
{
    public class ObtenerSeriesCasoDeUso
    {
        private readonly ISerieRepositorio _repo;
        private readonly IMapper _mapper;

        public ObtenerSeriesCasoDeUso(ISerieRepositorio repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ObtenerSerieDTO>> Ejecutar()
        {
            var series = await _repo.ObtenerTodasAsync();
            return _mapper.Map<IEnumerable<ObtenerSerieDTO>>(series);
        }
    }
}
