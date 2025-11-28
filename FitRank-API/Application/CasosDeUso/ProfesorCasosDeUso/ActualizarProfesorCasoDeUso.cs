using AutoMapper;
using FitRank_API.Application.DTOs.ProfesorDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;

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

            // 🟣 Validación EMAIL
            if (profesor.Email != dto.Email) 
            {
                if (await _profesorRepositorio.ExisteEmailAsync(dto.Email))
                    throw new Exception("EMAIL_DUPLICADO");
            }

            // 🟣 Validación DNI
            if (profesor.Dni != dto.Dni) // solo validar si lo cambió
            {
                if (await _profesorRepositorio.ExisteDniAsync(dto.Dni))
                    throw new Exception("DNI_DUPLICADO");
            }

            // 🟣 Mapear cambios
            _mapper.Map(dto, profesor);

            var actualizado = await _profesorRepositorio.ActualizarAsync(profesor);

            return _mapper.Map<ProfesorDTO>(actualizado);
        }



    }
}
