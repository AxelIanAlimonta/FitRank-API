using AutoMapper;
using FitRank_API.Application.DTOs.ProfesorDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.ProfesorCasosDeUso
{
    public class ActualizarProfesorCasoDeUso
    {
        private readonly IProfesorRepositorio _profesorRepositorio;
        private readonly IMapper _mapper;

        public ActualizarProfesorCasoDeUso(IProfesorRepositorio profesorRepositorio, IMapper mapper)
        {
            _profesorRepositorio = profesorRepositorio;
            _mapper = mapper;
        }

        public async Task<ProfesorDTO?> Ejecutar(ActualizarProfesorDTO profesorDTO)
        {
            var profesorEntidad = _mapper.Map<Profesor>(profesorDTO);
            var profesorActualizado = await _profesorRepositorio.ActualizarAsync(profesorEntidad);
            return profesorActualizado == null ? null : _mapper.Map<ProfesorDTO>(profesorActualizado);
        }
    }
}
