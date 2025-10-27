using AutoMapper;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.MaquinaCasosDeUso
{
    public class EliminarMaquinaCasoDeUso
    {
        private readonly IMaquinaRepositorio _maquinaRepositorio;
        private readonly IMapper _mapper;
        public EliminarMaquinaCasoDeUso(IMaquinaRepositorio maquinaRepositorio, IMapper mapper)
        {
            _maquinaRepositorio = maquinaRepositorio;
            _mapper = mapper;
        }
        public async Task<bool> Ejecutar(long id)
        {
            return await _maquinaRepositorio.EliminarMaquina(id);
        }
    }
}
