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

        public async Task<ProfesorDTO?> Ejecutar(long id, ActualizarProfesorDTO dto)
        {
           
            var profesor = await _profesorRepositorio.ObtenerPorIdAsync(id);
            if (profesor == null)
                return null;

            _mapper.Map(dto, profesor);

           
            var profesorActualizado = await _profesorRepositorio.ActualizarAsync(profesor);

            
            return _mapper.Map<ProfesorDTO>(profesorActualizado);
        }
    }
}
