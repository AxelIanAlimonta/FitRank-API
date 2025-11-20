using AutoMapper;
using FitRank_API.Application.DTOs.RutinaDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.RutinaCasosDeUso
{
    public class ObtenerTodasLasRutinasPorProfesorCasoDeUso
    {
        private readonly IRutinaRepositorio _rutinaRepositorio;
        private readonly IMapper _mapper;
        private readonly IProfesorRepositorio _profesorRepositorio;

        public ObtenerTodasLasRutinasPorProfesorCasoDeUso(
            IRutinaRepositorio rutinaRepositorio,
            IMapper mapper,
            IProfesorRepositorio profesorRepositorio)
        {
            _rutinaRepositorio = rutinaRepositorio;
            _mapper = mapper;
            _profesorRepositorio = profesorRepositorio;
        }

        public async Task<List<RutinaProfesorDTO>> Ejecutar(long profesorId)
        {
            var profesor = await _profesorRepositorio.ObtenerPorIdAsync(profesorId);
            if (profesor == null)
                throw new Exception("Profesor no encontrado");

            var rutinas = await _rutinaRepositorio.ObtenerTodasLasRutinasPorProfesorIdAsync(profesorId);
            return _mapper.Map<List<RutinaProfesorDTO>>(rutinas);
        }
    }
}
