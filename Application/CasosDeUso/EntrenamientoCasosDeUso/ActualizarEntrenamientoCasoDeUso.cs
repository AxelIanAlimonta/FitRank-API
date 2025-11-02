using AutoMapper;
using FitRank_API.Application.DTOs;
using FitRank_API.Application.DTOs.EntrenamientoDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.UseCases.Entrenamiento
{
    public class ActualizarEntrenamientoCasoDeUso
    {
        private readonly IEntrenamientoRepositorio _repo;
        private readonly IMapper _mapper;

        public ActualizarEntrenamientoCasoDeUso(IEntrenamientoRepositorio repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task Ejecutar(ActualizarEntrenamientoDTO dto)
        {
            var ent = await _repo.ObtenerPorIdAsync(dto.Id);
            if (ent == null)
                throw new Exception("Entrenamiento no encontrado");

            _mapper.Map(dto, ent);
            await _repo.ActualizarAsync(ent);
        }
    }
}
