using FitRank_API.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using FitRank_API.Domain.Entities;  
namespace FitRank_API.Application.CasosDeUso.UsuarioCasosDeUso
{
    public class ValidarTokenActivacionCasoDeUso
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;

        public ValidarTokenActivacionCasoDeUso(IUsuarioRepositorio usuarioRepositorio)
        {
            _usuarioRepositorio = usuarioRepositorio;
        }

        public async Task<bool> Ejecutar(string token)
        {
            var usuario = await _usuarioRepositorio.ObtenerPorCondicionAsync(
                u => u.TokenRecuperacion == token &&
                     u.TokenExpira > DateTime.UtcNow &&
                     !u.EsActivado
            );

            return usuario != null;
        }
    }
}
