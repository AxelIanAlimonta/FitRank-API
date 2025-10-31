using AutoMapper;
using FitRank_API.Application.DTOs.JornadaDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.JornadaCasosDeUso
{
    public class ObtenerTodasLasJornadasCasoDeUso
    {
        private readonly IJornadaRepositorio _jornadaRepository;
        private readonly IMapper _mapper;
        public ObtenerTodasLasJornadasCasoDeUso(IJornadaRepositorio jornadaRepositorio, IMapper mapper)
        {
            _jornadaRepository = jornadaRepositorio;
            _mapper = mapper;
        }

        public async Task<List<JornadaDTO>> Ejecutar()
        {
            var jornadas = await _jornadaRepository.ObtenerTodasLasJornadasAsync();
            return _mapper.Map<List<JornadaDTO>>(jornadas);
        }
    }
}
