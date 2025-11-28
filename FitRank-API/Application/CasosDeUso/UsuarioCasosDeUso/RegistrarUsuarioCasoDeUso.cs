using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using FitRank_API.Application.DTOs.UsuarioDTOs;
using FitRank_API.Application.Interfaces;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace FitRank_API.Application.CasosDeUso.UsuarioCasosDeUso
{
    public class RegistrarUsuarioCasoDeUso
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly GenerarTokenCasoDeUso _generarToken;
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;
        private readonly IPasswordService _passwordService;

        public RegistrarUsuarioCasoDeUso(IUsuarioRepositorio usuarioRepositorio, GenerarTokenCasoDeUso generarToken, IConfiguration config, IMapper mapper, IPasswordService passwordService)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _generarToken = generarToken;
            _config = config;
            _mapper = mapper;
            _passwordService = passwordService;
        }

        public virtual async Task<AuthResponseDTO?> Ejecutar(RegisterDTO dto)
        {
            
            if (await _usuarioRepositorio.ExistePorEmailAsync(dto.Email))
                return null;

            var user = _mapper.Map<Usuario>(dto);

            user.PasswordHash = _passwordService.HashPassword(dto.Password);
            user.Rol = string.IsNullOrEmpty(dto.Rol) ? "User" : dto.Rol;
            user.Estado = "Activo";

 
            await _usuarioRepositorio.AgregarAsync(user);

            var token = _generarToken.Ejecutar(user);

         
            var userDto = _mapper.Map<UsuarioAuthDTO>(user);

            return new AuthResponseDTO { Token = token, User = userDto };
        }
    }
}


