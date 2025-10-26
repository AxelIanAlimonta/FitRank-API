using AutoMapper;
using FitRank_API.Application.DTOs.ProfesorDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.ProfesorCasosDeUso
{
    public class AgregarProfesorCasoDeUso
    {
        private readonly IProfesorRepositorio _profesorRepositorio;
        private readonly IMapper _mapper;
        public AgregarProfesorCasoDeUso(IProfesorRepositorio profesorRepositorio, IMapper mapper)
        {
            _profesorRepositorio = profesorRepositorio;
            _mapper = mapper;
        }
        public async Task<ProfesorDTO> Ejecutar(AgregarProfesorDTO agregarProfesorDTO)
        {
            var profesorEntidad = _mapper.Map<Profesor>(agregarProfesorDTO);
            var profesorCreado = await _profesorRepositorio.AgregarAsync(profesorEntidad);
            return _mapper.Map<ProfesorDTO>(profesorCreado);
        }
    }
}
