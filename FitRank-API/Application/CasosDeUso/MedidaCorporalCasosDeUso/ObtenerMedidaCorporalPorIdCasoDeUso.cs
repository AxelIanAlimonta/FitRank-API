using AutoMapper;
using FitRank_API.Application.DTOs.MedidaCorporalDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.CasosDeUso.MedidaCorporalCasosDeUso
{
    public class ObtenerMedidaCorporalPorIdCasoDeUso
    {
        private readonly IMedidaCorporalRepositorio _repo;
        private readonly IMapper _mapper;

        public ObtenerMedidaCorporalPorIdCasoDeUso(IMedidaCorporalRepositorio repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public virtual async Task<ObtenerMedidaCorporalDTO?> Ejecutar(long id)
        {
            var medidaCorporal = await _repo.ObtenerPorIdAsync(id);
            return medidaCorporal == null ? null : _mapper.Map<ObtenerMedidaCorporalDTO>(medidaCorporal);
        }
    }
}

