using AutoMapper;
using FitRank_API.Application.DTOs.AdministradorDTOs;
using FitRank_API.Application.Interfaces;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.CasosDeUso.AdministradorCasosDeUso
{
    public class AgregarAdministradorCasoDeUso
    {
        private readonly IAdministradorRepositorio _repositorio;
        private readonly IMapper _mapper;
        private readonly IPasswordService _passwordService;

        public AgregarAdministradorCasoDeUso(IAdministradorRepositorio repositorio, IMapper mapper, IPasswordService passwordService)
        {
            _repositorio = repositorio;
            _mapper = mapper;
            _passwordService = passwordService;
        }

        public virtual async Task<Administrador> Ejecutar(AgregarAdministradorDTO dto)
        {
            var admin = _mapper.Map<Administrador>(dto);
            admin.Rol = "Admin";
            admin.EsActivado = true;
            admin.PasswordHash = _passwordService.HashPassword(dto.Password);

            await _repositorio.AgregarAsync(admin);
            return admin;
        }
    }
}

