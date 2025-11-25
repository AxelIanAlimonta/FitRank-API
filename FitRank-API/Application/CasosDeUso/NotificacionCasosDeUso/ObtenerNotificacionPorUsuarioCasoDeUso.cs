using AutoMapper;
using FitRank_API.Application.DTOs.NotificacionDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.NotificacionCasosDeUso;

public class ObtenerNotificacionPorUsuarioCasoDeUso
{
    private readonly INotificacionRepositorio _repo;
    private readonly IMapper _mapper;

    public ObtenerNotificacionPorUsuarioCasoDeUso(INotificacionRepositorio repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public virtual async Task<IEnumerable<ObtenerNotificacionDTO>> Ejecutar(long usuarioId)
    {
        var notificaciones = await _repo.ObtenerPorUsuarioAsync(usuarioId);
        return _mapper.Map<IEnumerable<ObtenerNotificacionDTO>>(notificaciones);
    }
}
