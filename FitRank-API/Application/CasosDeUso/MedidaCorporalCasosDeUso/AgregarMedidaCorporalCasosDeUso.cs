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

        public virtual async Task<ObtenerMedidaCorporalDTO> Ejecutar(AgregarMedidaCorporalDTO dto)
        {
            var creado = await _repo.AgregarAsync(_mapper.Map<MedidaCorporal>(dto));
            return _mapper.Map<ObtenerMedidaCorporalDTO>(creado);

        }
    }
}
