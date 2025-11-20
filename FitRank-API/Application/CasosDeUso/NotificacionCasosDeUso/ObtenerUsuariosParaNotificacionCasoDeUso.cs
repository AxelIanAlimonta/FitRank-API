using AutoMapper;
using FitRank_API.Application.DTOs.NotificacionDTOs;

using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.NotificacionCasoDeUso;

public class ObtenerUsuariosParaNotificacionCasoDeUso
{
    private readonly IUsuarioRepositorio _usuarioRepositorio;
    private readonly IMapper _mapper;

    public ObtenerUsuariosParaNotificacionCasoDeUso(
        IUsuarioRepositorio usuarioRepositorio,
        IMapper mapper)
    {
        _usuarioRepositorio = usuarioRepositorio;
        _mapper = mapper;
    }

    public async Task<List<UsuarioNotificacionDTO>> Ejecutar()
    {
        var usuarios = await _usuarioRepositorio.ObtenerTodosAsync();

        return _mapper.Map<List<UsuarioNotificacionDTO>>(usuarios);
    }
}
