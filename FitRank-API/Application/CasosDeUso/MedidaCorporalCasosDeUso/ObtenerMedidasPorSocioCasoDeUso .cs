using AutoMapper;
using FitRank_API.Application.DTOs.MedidaCorporalDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.MedidaCorporalCasosDeUso
{
    public class ObtenerMedidasPorSocioCasoDeUso
    {
        private readonly IMedidaCorporalRepositorio _medidaRepositorio;
        private readonly IMapper _mapper;

        public ObtenerMedidasPorSocioCasoDeUso(
            IMedidaCorporalRepositorio medidaRepositorio,
            IMapper mapper)
        {
            _medidaRepositorio = medidaRepositorio;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ObtenerMedidaCorporalDTO>> Ejecutar(long socioId)
        {
            
            var medidas = await _medidaRepositorio.ObtenerPorSocioAsync(socioId);

           
            var medidasOrdenadas = medidas
                .OrderByDescending(m => m.Fecha)
                .ToList();

           
            return _mapper.Map<IEnumerable<ObtenerMedidaCorporalDTO>>(medidasOrdenadas);
        }
    }
}
