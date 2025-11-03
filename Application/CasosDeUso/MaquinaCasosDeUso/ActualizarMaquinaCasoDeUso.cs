using AutoMapper;
using FitRank_API.Application.DTOs.MaquinaDTOs;
using FitRank_API.Domain.Entities;
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
        public virtual async Task<ObtenerMaquinaDTO?> Ejecutar(ActualizarMaquinaDTO dto)
        {
            var maquinaActualizada = await _maquinaRepositorio.ActualizarMaquina(_mapper.Map<Maquina>(dto));
            return _mapper.Map<ObtenerMaquinaDTO>(maquinaActualizada);

        }
    }
}
