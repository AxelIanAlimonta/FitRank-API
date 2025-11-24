using AutoMapper;
using FitRank_API.Application.DTOs.DificultadDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.DificultadCasosDeUso
{
    public class ObtenerTodasLasDificultadesCasoDeUso
    {
        private readonly IDificultadRepositorio _dificultadRepositorio;
        private readonly IMapper _mapper;

        public ObtenerTodasLasDificultadesCasoDeUso(IDificultadRepositorio dificultadRepositorio, IMapper mapper)
        {
            _dificultadRepositorio = dificultadRepositorio;
            _mapper = mapper;
        }

        public virtual async Task<List<DificultadDTO>> Ejecutar()
        {
            var dificultades = await _dificultadRepositorio.ObtenerTodosAsync();
            return _mapper.Map<List<DificultadDTO>>(dificultades);
        }

    }
}
