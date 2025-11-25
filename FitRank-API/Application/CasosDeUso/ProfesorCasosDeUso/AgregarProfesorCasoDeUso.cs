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
        public virtual async Task<ProfesorDTO> Ejecutar(AgregarProfesorDTO dto)
        {
            var profesor = _mapper.Map<Profesor>(dto);

            profesor.Rol = "Profesor";
            profesor.EsActivado = true;
            profesor.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // 🔹 Asignar el gimnasio (viene desde el DTO)
            profesor.GimnasioId = dto.GimnasioId;

            var profesorCreado = await _profesorRepositorio.AgregarAsync(profesor);
            return _mapper.Map<ProfesorDTO>(profesorCreado);
        }

    }
}
