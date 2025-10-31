using AutoMapper;
using FitRank_API.Application.DTOs.ProfesorDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.ProfesorCasosDeUso
{
    public class ObtenerProfesorPorIdCasoDeUso
    {
        private readonly IProfesorRepositorio _profesorRepositorio;
        private readonly IMapper _mapper;
        public ObtenerProfesorPorIdCasoDeUso(IProfesorRepositorio profesorRepositorio, IMapper mapper)
        {
            _profesorRepositorio = profesorRepositorio;
            _mapper = mapper;
        }
        public async Task<ProfesorDTO?> Ejecutar(long id)
        {
            var profesor = await _profesorRepositorio.ObtenerPorIdAsync(id);
            return profesor == null ? null : _mapper.Map<ProfesorDTO>(profesor);
        }
    }
}
