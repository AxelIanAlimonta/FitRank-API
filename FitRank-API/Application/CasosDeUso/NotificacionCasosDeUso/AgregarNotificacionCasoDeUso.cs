using AutoMapper;
using FitRank_API.Application.DTOs.NotificacionDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.NotificacionCasosDeUso
{
    public class AgregarNotificacionCasoDeUso
    {
        private readonly INotificacionRepositorio _repo;
        private readonly IMapper _mapper;

        public AgregarNotificacionCasoDeUso(INotificacionRepositorio repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public virtual async Task<ObtenerNotificacionDTO?> Ejecutar(AgregarNotificacionDTO dto)
        {
            var notificacion = _mapper.Map<Notificacion>(dto);
            var notificacionCreada = await _repo.AgregarAsync(notificacion);
            return _mapper.Map<ObtenerNotificacionDTO>(notificacionCreada);

        }
    }
}
