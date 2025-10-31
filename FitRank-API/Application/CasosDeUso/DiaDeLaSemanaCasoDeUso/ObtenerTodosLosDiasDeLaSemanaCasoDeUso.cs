using AutoMapper;
using FitRank_API.Application.DTOs.DiaDeLaSemanaDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.DiaDeLaSemanaCasoDeUso
{
    public class ObtenerTodosLosDiasDeLaSemanaCasoDeUso
    {
        private readonly IDiaDeLaSemanaRepositorio _diaDeLaSemanaRepositorio;
        private readonly IMapper _mapper;

        public ObtenerTodosLosDiasDeLaSemanaCasoDeUso(IDiaDeLaSemanaRepositorio diaDeLaSemanaRepositorio, IMapper mapper)
        {
            _diaDeLaSemanaRepositorio = diaDeLaSemanaRepositorio;
            _mapper = mapper;
        }

        public async Task<List<DiaDeLaSemanaDTO>> Ejecutar()
        {
            var diasDeLaSemana = await _diaDeLaSemanaRepositorio.ObtenerTodosLosDiasDeLaSemanaAsync();
            return _mapper.Map<List<DiaDeLaSemanaDTO>>(diasDeLaSemana);
        }
    }
}
