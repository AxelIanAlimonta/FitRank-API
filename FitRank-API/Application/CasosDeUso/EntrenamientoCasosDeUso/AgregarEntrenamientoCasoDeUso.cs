using AutoMapper;
using FitRank_API.Application.DTOs;
using FitRank_API.Application.DTOs.EntrenamientoDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.UseCases.Entrenamiento
{
    public class AgregarEntrenamientoCasoDeUso
    {
        private readonly IEntrenamientoRepositorio _repo;
        private readonly IMapper _mapper;

        public AgregarEntrenamientoCasoDeUso(IEntrenamientoRepositorio repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<ObtenerEntrenamientoDTO> Ejecutar(AgregarEntrenamientoDTO dto)
        {
            var nuevo = _mapper.Map<Domain.Entities.Entrenamiento>(dto);
            var creado = await _repo.AgregarAsync(nuevo);
            return _mapper.Map<ObtenerEntrenamientoDTO>(creado);
        }
    }
}
