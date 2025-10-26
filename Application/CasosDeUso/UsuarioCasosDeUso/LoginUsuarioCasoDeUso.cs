using AutoMapper;
using BCrypt.Net;
using FitRank_API.Application.CasosDeUso.UsuarioCasosDeUso;
using FitRank_API.Application.DTOs.UsuarioDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.UsuarioCasosDeUso
{
    public class LoginUsuarioCasoDeUso
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly GenerarTokenCasoDeUso _generarTokenCasoDeUso;
        private readonly IMapper _mapper;

      public LoginUsuarioCasoDeUso(IUsuarioRepositorio usuarioRepositorio, GenerarTokenCasoDeUso generarTokenCasoDeUso, IMapper mapper)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _generarTokenCasoDeUso = generarTokenCasoDeUso;
            _mapper = mapper;
        }
        public async Task<AuthResponseDTO?> Ejecutar(LoginDTO dto)
        {
           
            var usuario = await _usuarioRepositorio.ObtenerPorCondicionAsync(u => u.Email == dto.Email);

            if (usuario == null || string.IsNullOrEmpty(usuario.PasswordHash))
                return null;

            
            if (!BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash))
                return null;

            if (!usuario.EsActivado)
                return null; 
           
            var token = _generarTokenCasoDeUso.Ejecutar(usuario);

            
            var usuarioDto = _mapper.Map<UsuarioAuthDTO>(usuario);

            
            return new AuthResponseDTO
            {
                Token = token,
                User = usuarioDto
            };
        }
    }
}
