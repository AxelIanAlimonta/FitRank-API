using AutoMapper;
using FitRank_API.Application.DTOs.EjercicioDTOs.ObtenerEjercicioDTO;
using FitRank_API.Application.DTOs.RutinaDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.CasosDeUso.EjercicioCasosDeUso
{
    public class ObtenerEjerciciosPorGrupoMuscularCasoDeUso
    {
        private readonly IEjercicioRepositorio _ejercicioRepositorio;
        private readonly IMapper _mapper;

        public ObtenerEjerciciosPorGrupoMuscularCasoDeUso(IEjercicioRepositorio ejercicioRepositorio,
        IMapper mapper)
        {
            _ejercicioRepositorio = ejercicioRepositorio;
            _mapper = mapper;
        }
        public async Task<List<ObtenerEjercicioDTO>> Ejecutar(long grupoMuscularId)
        {
            var ejercicios = await _ejercicioRepositorio.ObtenerEjerciciosPorGrupoMuscularAsync(grupoMuscularId);
            return _mapper.Map<List<ObtenerEjercicioDTO>>(ejercicios);
        }
    }
}
