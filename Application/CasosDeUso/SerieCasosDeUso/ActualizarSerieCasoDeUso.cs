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

        public async Task Ejecutar(ActualizarSerieDTO dto)
        {
            var serie = await _repo.ObtenerPorIdAsync(dto.Id);
            if (serie == null)
                throw new Exception("Serie no encontrada.");

            _mapper.Map(dto, serie);
            await _repo.ActualizarAsync(serie);
        }
    }
}
