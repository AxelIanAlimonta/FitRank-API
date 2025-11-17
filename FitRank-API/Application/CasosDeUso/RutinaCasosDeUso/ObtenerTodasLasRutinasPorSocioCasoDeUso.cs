using AutoMapper;
using FitRank_API.Application.DTOs.RutinaDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.RutinaCasosDeUso
{
    public class ObtenerTodasLasRutinasPorSocioCasoDeUso
    {
        private readonly IRutinaRepositorio _rutinaRepositorio;
        private readonly ISocioRepositorio _socioRepositorio;
        private readonly IMapper _mapper;



        public ObtenerTodasLasRutinasPorSocioCasoDeUso(IRutinaRepositorio rutinaRepositorio, ISocioRepositorio socioRepositorio, IMapper mapper)
        {
            _rutinaRepositorio = rutinaRepositorio;
            _socioRepositorio = socioRepositorio;
            _mapper = mapper;
        }


        public async Task<List<ObtenerRutinaDTO>> Ejecutar(long socioId)
        {
            var socio = await _socioRepositorio.ObtenerPorIdAsync(socioId);
            if (socio == null)
            {
                throw new Exception("Socio no encontrado");
            }
            var rutinas = await _rutinaRepositorio.ObtenerPorSocioIdAsync(socioId);
            return _mapper.Map<List<ObtenerRutinaDTO>>(rutinas);
        }

    }
}
