using AutoMapper;
using FitRank_API.Application.DTOs.DificultadDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.CasosDeUso.DificultadCasosDeUso
{
    public class AgregarDificultadCasoDeUso
    {
        private readonly IDificultadRepositorio _dificultadRepositorio;
        private readonly IMapper _mapper;

        public AgregarDificultadCasoDeUso(IDificultadRepositorio dificultadRepositorio, IMapper mapper)
        {
            _dificultadRepositorio = dificultadRepositorio;
            _mapper = mapper;
        }

        public virtual async Task<DificultadDTO> Ejecutar(AgregarDificultadDTO agregarDificultadDTO)
        {
            var dificultadEntidad = _mapper.Map<Dificultad>(agregarDificultadDTO);
            var dificultadCreada = await _dificultadRepositorio.AgregarAsync(dificultadEntidad);
            return _mapper.Map<DificultadDTO>(dificultadCreada);
        }
    }
}
