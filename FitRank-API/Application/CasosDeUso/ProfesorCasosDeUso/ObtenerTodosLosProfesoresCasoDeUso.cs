using AutoMapper;
using FitRank_API.Application.DTOs.ProfesorDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.ProfesorCasosDeUso
{
    public class ObtenerTodosLosProfesoresCasoDeUso
    {
        private readonly IProfesorRepositorio _profesorRepositorio;
        private readonly IMapper _mapper;

        public ObtenerTodosLosProfesoresCasoDeUso(IProfesorRepositorio profesorRepositorio, IMapper mapper)
        {
            _profesorRepositorio = profesorRepositorio;
            _mapper = mapper;
        }

        public async Task<List<ProfesorDTO>> Ejecutar()
        {
            var profesores = await _profesorRepositorio.ObtenerTodosAsync();
            return _mapper.Map<List<ProfesorDTO>>(profesores);
        }
    }
}
