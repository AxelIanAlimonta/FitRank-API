using AutoMapper;

using FitRank_API.Application.DTOs.UsuarioDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.UsuarioCasosDeUso
{
    public class ObtenerUsuarioPorIdCasoDeUso
    {
        private readonly IUsuarioRepositorio _usuarioRepositorio;
        private readonly IMapper _mapper;

        public ObtenerUsuarioPorIdCasoDeUso(IUsuarioRepositorio usuarioRepositorio, IMapper mapper)
        {
            _usuarioRepositorio = usuarioRepositorio;
            _mapper = mapper;
        }

        public virtual async Task<UsuarioAuthDTO?> EjecutarAsync(long id)
        {
            var usuario = await _usuarioRepositorio.ObtenerPorIdAsync(id);

            
            if (usuario == null)
                return null;

         
            var dto = _mapper.Map<UsuarioAuthDTO>(usuario);

            return dto;
        }
    }
}
