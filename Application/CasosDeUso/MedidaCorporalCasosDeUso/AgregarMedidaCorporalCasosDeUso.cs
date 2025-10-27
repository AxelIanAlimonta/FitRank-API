using AutoMapper;
using FitRank_API.Application.DTOs.MedidaCorporalDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.MedidaCorporalCasosDeUso
{
    public class AgregarMedidaCorporalCasoDeUso
    {
        private readonly IMedidaCorporalRepositorio _repo;
        private readonly IMapper _mapper;

        public AgregarMedidaCorporalCasoDeUso(IMedidaCorporalRepositorio repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<ObtenerMedidaCorporalDTO> Ejecutar(long socioId, AgregarMedidaCorporalDTO dto)
        {
            var entidad = _mapper.Map<MedidaCorporal>(dto);
            entidad.SocioId = socioId;
            var guardada = await _repo.AgregarAsync(entidad);
            return _mapper.Map<ObtenerMedidaCorporalDTO>(guardada);
        }
    }
}
