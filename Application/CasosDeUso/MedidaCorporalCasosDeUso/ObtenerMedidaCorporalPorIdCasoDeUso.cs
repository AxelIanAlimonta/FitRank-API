using AutoMapper;
using FitRank_API.Application.DTOs.MedidaCorporalDTOs;
using FitRank_API.Domain.Entities;
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

        public async Task<ObtenerMedidaCorporalDTO?> Ejecutar(long socioId, long id)
        {
            var medida = await _repo.ObtenerPorIdAsync(id);
            if (medida == null)
                return null;

            if (medida.SocioId != socioId)
                throw new UnauthorizedAccessException("No estás autorizado para acceder a esta medición.");

            return _mapper.Map<ObtenerMedidaCorporalDTO>(medida);
        }
    }
}

