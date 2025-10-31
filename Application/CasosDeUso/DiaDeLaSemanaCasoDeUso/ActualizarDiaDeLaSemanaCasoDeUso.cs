using AutoMapper;
using FitRank_API.Application.DTOs.DiaDeLaSemanaDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.DiaDeLaSemanaCasoDeUso
{
    public class ActualizarDiaDeLaSemanaCasoDeUso
    {
        private readonly IDiaDeLaSemanaRepositorio _diaDeLaSemanaRepositorio;
        private readonly IMapper _mapper;

        public ActualizarDiaDeLaSemanaCasoDeUso(IDiaDeLaSemanaRepositorio diaDeLaSemanaRepositorio, IMapper mapper)
        {
            _diaDeLaSemanaRepositorio = diaDeLaSemanaRepositorio;
            _mapper = mapper;
        }

        public async Task<ObtenerDiaDeLaSemanaDTO?> Ejecutar(ActualizarDiaDeLaSemanaDTO actualizarDiaDeLaSemanaDTO)
        {
            var diaDeLaSemanaEntidad = _mapper.Map<DiaDeLaSemana>(actualizarDiaDeLaSemanaDTO);
            var diaDeLaSemanaActualizado = await _diaDeLaSemanaRepositorio.ActualizarDiaDeLaSemanaAsync(diaDeLaSemanaEntidad);
            if (diaDeLaSemanaActualizado == null)
            {
                return null;
            }
            return _mapper.Map<ObtenerDiaDeLaSemanaDTO>(diaDeLaSemanaActualizado);
        }
    }
}
