using AutoMapper;
using FitRank_API.Application.DTOs.MaquinaDTOs;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.MaquinaCasosDeUso
{
    public class ActualizarMaquinaCasoDeUso
    {
        private readonly IMaquinaRepositorio _maquinaRepositorio;
        private readonly IMapper _mapper;
        public ActualizarMaquinaCasoDeUso(IMaquinaRepositorio maquinaRepositorio, IMapper mapper)
        {
            _maquinaRepositorio = maquinaRepositorio;
            _mapper = mapper;
        }
        public async Task<ObtenerMaquinaDTO?> Ejecutar(long id, ActualizarMaquinaDTO dto)
        {
            var maquinaExistente = await _maquinaRepositorio.ObtenerMaquinaPorId(id);
            if (maquinaExistente == null)
            {
                return null;
            }

            _mapper.Map(dto, maquinaExistente);
            var maquinaActualizada = await _maquinaRepositorio.ActualizarMaquina(maquinaExistente);
            return _mapper.Map<ObtenerMaquinaDTO>(maquinaActualizada);
        }
    }
}
