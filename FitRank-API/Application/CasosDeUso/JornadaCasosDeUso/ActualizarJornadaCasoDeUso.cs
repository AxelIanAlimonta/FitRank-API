using AutoMapper;
using FitRank_API.Application.DTOs.JornadaDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.CasosDeUso.JornadaCasosDeUso
{
    public class ActualizarJornadaCasoDeUso
    {
        private readonly IJornadaRepositorio _jornadaRepository;
        private readonly IMapper _mapper;
        public ActualizarJornadaCasoDeUso(IJornadaRepositorio jornadaRepository, IMapper mapper)
        {
            _jornadaRepository = jornadaRepository;
            _mapper = mapper;
        }

        public virtual async Task<ObtenerJornadaDTO?> Ejecutar(ActualizarJornadaDTO actualizarJornadaDTO)
        {
            var jornadaActualizada = _mapper.Map<Jornada>(actualizarJornadaDTO);
            var resultado = await _jornadaRepository.ActualizarJornadaAsync(jornadaActualizada);
            if (resultado == null)
            {
                return null;
            }
            return _mapper.Map<ObtenerJornadaDTO>(resultado);
        }
    }
}
