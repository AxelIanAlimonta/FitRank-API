using AutoMapper;
using FitRank_API.Application.DTOs;
using FitRank_API.Application.DTOs.EntrenamientoDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.UseCases.Entrenamiento
{
    public class ObtenerEntrenamientosCasoDeUso
    {
        private readonly IEntrenamientoRepositorio _repo;
        private readonly IMapper _mapper;

        public ObtenerEntrenamientosCasoDeUso(IEntrenamientoRepositorio repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ObtenerEntrenamientoDTO>> Ejecutar()
        {
            var lista = await _repo.ObtenerTodosAsync();
            return _mapper.Map<IEnumerable<ObtenerEntrenamientoDTO>>(lista);
        }
    }
}
