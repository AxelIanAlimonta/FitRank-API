using AutoMapper;
using FitRank_API.Application.DTOs.MedidaCorporalDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;

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

        public async Task<ObtenerMedidaCorporalDTO?> Ejecutar( long socioid ,ActualizarMedidaCorporalDTO dto)
        {
            var existente = await _repo.ObtenerPorIdAsync(dto.Id);
            if (existente == null)
                return null;

           
            if (existente.SocioId != socioid)
                throw new UnauthorizedAccessException("No estás autorizado para modificar esta medición.");

            _mapper.Map(dto, existente);
            await _repo.ActualizarAsync(existente);
            return _mapper.Map<ObtenerMedidaCorporalDTO>(existente);
        }
    }
}
