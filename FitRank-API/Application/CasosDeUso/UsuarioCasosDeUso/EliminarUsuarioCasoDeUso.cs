using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.UsuarioCasosDeUso
{
    public class EliminarUsuarioCasoDeUso
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;

        public EliminarUsuarioCasoDeUso(IUsuarioRepositorio usuarioRepositorio)
        {
            _usuarioRepositorio = usuarioRepositorio;
        }

        public async Task<bool> EjecutarAsync(long id)
        {
    
            var usuario = await _usuarioRepositorio.ObtenerPorIdAsync(id);

            if (usuario == null)
                return false;

        
            await _usuarioRepositorio.EliminarAsync(usuario);

            return true;
        }
    }
}
