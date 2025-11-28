using AutoMapper;
using FitRank_API.Application.DTOs.JornadaDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.CasosDeUso.JornadaCasosDeUso
{
    public class AgregarJornadaCasoDeUso
    {
        private readonly IJornadaRepositorio _jornadaRepository;
        private readonly IMapper _mapper;
        public AgregarJornadaCasoDeUso(IJornadaRepositorio jornadaRepository, IMapper mapper)
        {
            _jornadaRepository = jornadaRepository;
            _mapper = mapper;
        }
        public virtual async Task<ObtenerJornadaDTO> Ejecutar(AgregarJornadaDTO crearJornadaDTO)
        {
            var nuevaJornada = _mapper.Map<Jornada>(crearJornadaDTO);
            var jornadaAgregada = await _jornadaRepository.AgregarJornadaAsync(nuevaJornada);
            return _mapper.Map<ObtenerJornadaDTO>(jornadaAgregada);
        }
    }
}
