using AutoMapper;
using FitRank_API.Application.DTOs.MaquinaDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.MaquinaCasosDeUso
{
    public class AgregarMaquinaCasoDeUso
    {
        private readonly IMaquinaRepositorio _maquinaRepositorio;
        private readonly IMapper _mapper;
        public AgregarMaquinaCasoDeUso(IMaquinaRepositorio maquinaRepositorio, IMapper mapper)
        {
            _maquinaRepositorio = maquinaRepositorio;
            _mapper = mapper;
        }
        public virtual async Task<ObtenerMaquinaDTO> Ejecutar(AgregarMaquinaDTO crearMaquinaDTO)
        {
            var maquinaEntidad = _mapper.Map<Maquina>(crearMaquinaDTO);
            var maquinaCreada = await _maquinaRepositorio.AgregarMaquina(maquinaEntidad);
            return _mapper.Map<ObtenerMaquinaDTO>(maquinaCreada);
        }


    }
}
