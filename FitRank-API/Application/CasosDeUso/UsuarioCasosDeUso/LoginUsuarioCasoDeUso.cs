using AutoMapper;
using FitRank_API.Application.DTOs.UsuarioDTOs;
using FitRank_API.Application.Interfaces;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.CasosDeUso.UsuarioCasosDeUso
{
    public class LoginUsuarioCasoDeUso
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IMapper _mapper;
        private readonly IPasswordService _passwordService;

        public LoginUsuarioCasoDeUso(IUsuarioRepositorio usuarioRepositorio, IMapper mapper, IPasswordService passwordService)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _mapper = mapper;
            _passwordService = passwordService;
        }

        public virtual async Task<(Usuario entidad, UsuarioAuthDTO dto)?> Ejecutar(LoginDTO dto)
        {
            var usuario = await _usuarioRepositorio.ObtenerPorCondicionAsync(u => u.Email == dto.Email);

            if (usuario == null)
                return null;

            if (!_passwordService.VerifyPassword(dto.Password, usuario.PasswordHash))
                return null;

            if (!usuario.EsActivado)
                return null;

            var usuarioDto = _mapper.Map<UsuarioAuthDTO>(usuario);

            return (usuario, usuarioDto);
        }
    }
}
