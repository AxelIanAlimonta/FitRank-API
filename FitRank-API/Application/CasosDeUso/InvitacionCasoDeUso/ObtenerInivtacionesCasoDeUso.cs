using AutoMapper;
using FitRank_API.Application.DTOs.Invitacion;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.CasosDeUso.Invitacion
{
    public class ObtenerInvitacionesCasoDeUso
    {
        private readonly IInvitacionRepositorio _invitacionRepositorio;
        private readonly IMapper _mapper;

        public ObtenerInvitacionesCasoDeUso(IInvitacionRepositorio invitacionRepositorio, IMapper mapper)
        {
            _invitacionRepositorio = invitacionRepositorio;
            _mapper = mapper;
        }

        public virtual async Task<List<InvitacionListadoDTO>> Ejecutar(int gimnasioId)
        {
            var invitaciones = await _invitacionRepositorio.ObtenerTodasAsync(gimnasioId);

          
            var resultado = _mapper.Map<List<InvitacionListadoDTO>>(invitaciones);

            return resultado;

        }
    }
}
