
using AutoMapper;
using FitRank_API.Application.DTOs.ProfesorDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.ProfesorCasosDeUso
{
    public class ObtenerTodosPorGimnasioCasoDeUso
    {
        private readonly IProfesorRepositorio _profesorRepositorio;
        private readonly IMapper _mapper;


      

        public async Task<List<ProfesorDTO>> Ejecutar(long gimnasioId)
        {
            var profesores = await _profesorRepositorio.ObtenerPorGimnasioAsync(gimnasioId);

            return _mapper.Map<List<ProfesorDTO>>(profesores);
        }

    }
}