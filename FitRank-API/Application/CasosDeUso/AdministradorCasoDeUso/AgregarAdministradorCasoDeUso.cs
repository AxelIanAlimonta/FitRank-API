
using AutoMapper;
using FitRank_API.Application.DTOs.AdministradorDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.AdministradorCasosDeUso
{
    public class AgregarAdministradorCasoDeUso
    {
        private readonly IAdministradorRepositorio _repositorio;
        private readonly IMapper _mapper;

        public AgregarAdministradorCasoDeUso(IAdministradorRepositorio repositorio, IMapper mapper)
        {
            _repositorio = repositorio;
            _mapper = mapper;
        }

        public virtual async Task<Administrador> Ejecutar(AgregarAdministradorDTO dto)
        {
            var admin = _mapper.Map<Administrador>(dto);
            admin.Rol = "Admin";
            admin.EsActivado = true;
            admin.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            await _repositorio.AgregarAsync(admin);
            return admin;
        }
    }
}

