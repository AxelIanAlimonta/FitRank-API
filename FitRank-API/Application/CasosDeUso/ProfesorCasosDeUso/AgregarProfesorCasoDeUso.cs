using AutoMapper;
using FitRank_API.Application.DTOs.ProfesorDTOs;
using FitRank_API.Application.Interfaces;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.CasosDeUso.ProfesorCasosDeUso
{
    public class AgregarProfesorCasoDeUso
    {
        private readonly IProfesorRepositorio _profesorRepositorio;
        private readonly IMapper _mapper;
        private readonly IPasswordService _passwordService;

        public AgregarProfesorCasoDeUso(IProfesorRepositorio profesorRepositorio, IMapper mapper, IPasswordService passwordService)
        {
            _profesorRepositorio = profesorRepositorio;
            _mapper = mapper;
            _passwordService = passwordService;
        }

        public async Task<ProfesorDTO> Ejecutar(AgregarProfesorDTO dto)
        {
            if (await _profesorRepositorio.ExisteEmailAsync(dto.Email))
                throw new Exception("EMAIL_DUPLICADO");

            if (await _profesorRepositorio.ExisteDniAsync(dto.Dni))
                throw new Exception("DNI_DUPLICADO");

            var profesor = _mapper.Map<Profesor>(dto);

            profesor.Rol = "Profesor";
            profesor.EsActivado = true;
            profesor.PasswordHash = _passwordService.HashPassword(dto.Password);
            profesor.GimnasioId = dto.GimnasioId;

            var creado = await _profesorRepositorio.AgregarAsync(profesor);
            return _mapper.Map<ProfesorDTO>(creado);
        }
    }
}
