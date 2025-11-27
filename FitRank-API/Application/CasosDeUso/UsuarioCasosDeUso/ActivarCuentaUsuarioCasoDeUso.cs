using FitRank_API.Application.Interfaces;
using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.CasosDeUso.UsuarioCasosDeUso
{
    public class ActivarCuentaCasoDeUso
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IPasswordService _passwordService;

        public ActivarCuentaCasoDeUso(IUsuarioRepositorio usuarioRepositorio, IPasswordService passwordService)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _passwordService = passwordService;
        }

        public virtual async Task<string?> Ejecutar(string token, string nuevaPassword)
        {
           
            var usuario = await _usuarioRepositorio.ObtenerPorCondicionAsync(
                u => u.TokenRecuperacion == token &&
                     u.TokenExpira > DateTime.UtcNow &&
                     !u.EsActivado
            );

            if (usuario == null)
                return null; 

           
            usuario.PasswordHash = _passwordService.HashPassword(nuevaPassword);
            usuario.EsActivado = true;
            usuario.TokenRecuperacion = null;
            usuario.TokenExpira = null;

           
            if (string.IsNullOrEmpty(usuario.NombreUsuario))
                usuario.NombreUsuario = usuario.Email.Split('@')[0];

            
            await _usuarioRepositorio.ActualizarAsync(usuario);

           
            return usuario.Email;
        }
    }
}
