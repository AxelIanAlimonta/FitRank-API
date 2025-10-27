using AutoMapper;
using FitRank_API.Application.DTOs.MedidaCorporalDTOs;
using FitRank_API.Infrastructure.Interfaces;

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

        public async Task<ObtenerMedidaCorporalDTO?> Ejecutar(long id)
        {
            var entidad = await _repo.ObtenerPorIdAsync(id);
            return entidad != null ? _mapper.Map<ObtenerMedidaCorporalDTO>(entidad) : null;
        }
    }
}
