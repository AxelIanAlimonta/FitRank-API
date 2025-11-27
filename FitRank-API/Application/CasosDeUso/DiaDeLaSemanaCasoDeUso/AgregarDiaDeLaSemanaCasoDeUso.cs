using AutoMapper;
using FitRank_API.Application.DTOs.DiaDeLaSemanaDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.CasosDeUso.DiaDeLaSemanaCasoDeUso
{
    public class AgregarDiaDeLaSemanaCasoDeUso
    {
        private readonly IDiaDeLaSemanaRepositorio _diaDeLaSemanaRepositorio;
        private readonly IMapper _mapper;

        public AgregarDiaDeLaSemanaCasoDeUso(IDiaDeLaSemanaRepositorio diaDeLaSemanaRepositorio, IMapper mapper)
        {
            _diaDeLaSemanaRepositorio = diaDeLaSemanaRepositorio;
            _mapper = mapper;
        }
        public virtual async Task<ObtenerDiaDeLaSemanaDTO?> Ejecutar(AgregarDiaDeLaSemanaDTO agregarDiaDeLaSemanaDTO)
        {
            var diaDeLaSemanaEntidad = _mapper.Map<DiaDeLaSemana>(agregarDiaDeLaSemanaDTO);
            var diaDeLaSemanaCreado = await _diaDeLaSemanaRepositorio.AgregarDiaDeLaSemanaAsync(diaDeLaSemanaEntidad);
            return _mapper.Map<ObtenerDiaDeLaSemanaDTO?>(diaDeLaSemanaCreado);
        }
    }
}
