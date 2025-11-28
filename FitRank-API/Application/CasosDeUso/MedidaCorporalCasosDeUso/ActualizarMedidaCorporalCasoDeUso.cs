using AutoMapper;
using FitRank_API.Application.DTOs.MedidaCorporalDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.CasosDeUso.MedidaCorporalCasosDeUso
{
    public class ActualizarMedidaCorporalCasoDeUso
    {
        private readonly IMedidaCorporalRepositorio _repo;
        private readonly IMapper _mapper;

        public ActualizarMedidaCorporalCasoDeUso(IMedidaCorporalRepositorio repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public virtual async Task<ObtenerMedidaCorporalDTO?> Ejecutar(ActualizarMedidaCorporalDTO dto)
        {
            var existente = await _repo.ActualizarAsync(_mapper.Map<MedidaCorporal>(dto));
            return existente == null ? null : _mapper.Map<ObtenerMedidaCorporalDTO>(existente);
        }
    }
}
