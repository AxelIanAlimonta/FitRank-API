using AutoMapper;
using FitRank_API.Application.DTOs.DiaDeLaSemanaDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.DiaDeLaSemanaCasoDeUso
{
    public class ObtenerDiaDeLaSemanaPorIdCasoDeUso
    {
        private readonly IDiaDeLaSemanaRepositorio _diaDeLaSemanaRepositorio;
        private readonly IMapper _mapper;

        public ObtenerDiaDeLaSemanaPorIdCasoDeUso(IDiaDeLaSemanaRepositorio diaDeLaSemanaRepositorio, IMapper mapper)
        {
            _diaDeLaSemanaRepositorio = diaDeLaSemanaRepositorio;
            _mapper = mapper;
        }

        public virtual async Task<ObtenerDiaDeLaSemanaDTO?> Ejecutar(long id)
        {
            var diaDeLaSemana = await _diaDeLaSemanaRepositorio.ObtenerDiaDeLaSemanaPorIdAsync(id);
            if (diaDeLaSemana == null)
            {
                return null;
            }
            return _mapper.Map<ObtenerDiaDeLaSemanaDTO>(diaDeLaSemana);
        }
    }
}
