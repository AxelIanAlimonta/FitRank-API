using AutoMapper;
using FitRank_API.Application.DTOs.MaquinaDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.MaquinaCasosDeUso
{
    public class ObtenerMaquinaPorIdCasoDeUso
    {
        private readonly IMaquinaRepositorio _maquinaRepositorio;
        private readonly IMapper _mapper;
        public ObtenerMaquinaPorIdCasoDeUso(IMaquinaRepositorio maquinaRepositorio, IMapper mapper)
        {
            _maquinaRepositorio = maquinaRepositorio;
            _mapper = mapper;
        }
        public async Task<ObtenerMaquinaDTO?> Ejecutar(long id)
        {
            var maquina = await _maquinaRepositorio.ObtenerMaquinaPorId(id);
            if (maquina == null)
            {
                return null;
            }
            return _mapper.Map<ObtenerMaquinaDTO>(maquina);
        }
    }
}
