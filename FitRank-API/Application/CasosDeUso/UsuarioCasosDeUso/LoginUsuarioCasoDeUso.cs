using AutoMapper;
using BCrypt.Net;
using FitRank_API.Application.DTOs.UsuarioDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.CasosDeUso.UsuarioCasosDeUso
{
    public class LoginUsuarioCasoDeUso
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IMapper _mapper;

        public LoginUsuarioCasoDeUso(IUsuarioRepositorio usuarioRepositorio, IMapper mapper)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _mapper = mapper;
        }

        public virtual async Task<(Usuario entidad, UsuarioAuthDTO dto)?> Ejecutar(LoginDTO dto)
        {
            var usuario = await _usuarioRepositorio.ObtenerPorCondicionAsync(u => u.Email == dto.Email);

            if (usuario == null)
                return null;

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, usuario.PasswordHash))
                return null;

            if (!usuario.EsActivado)
                return null;

            // mapear a DTO para que el frontend reciba lo que necesita
            var usuarioDto = _mapper.Map<UsuarioAuthDTO>(usuario);

            return (usuario, usuarioDto);
        }
    }
}
