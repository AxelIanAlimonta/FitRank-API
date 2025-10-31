using AutoMapper;
using FitRank_API.Application.DTOs;
using FitRank_API.Application.DTOs.ActividadDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.UseCases.Actividad
{
    public class AgregarActividadCasoDeUso
    {
        private readonly IActividadRepositorio _repo;
        private readonly IMapper _mapper;

        public AgregarActividadCasoDeUso(IActividadRepositorio repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<ObtenerActividadDTO> Ejecutar(AgregarActividadDTO dto)
        {
            var nueva = _mapper.Map<Domain.Entities.Actividad>(dto);
            var creada = await _repo.AgregarAsync(nueva);
            return _mapper.Map<ObtenerActividadDTO>(creada);
        }
    }
}
