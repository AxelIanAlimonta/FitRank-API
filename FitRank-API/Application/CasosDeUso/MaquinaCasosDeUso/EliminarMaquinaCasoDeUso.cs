using AutoMapper;
using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.CasosDeUso.MaquinaCasosDeUso
{
    public class EliminarMaquinaCasoDeUso
    {
        private readonly IMaquinaRepositorio _maquinaRepositorio;
        public EliminarMaquinaCasoDeUso(IMaquinaRepositorio maquinaRepositorio)
        {
            _maquinaRepositorio = maquinaRepositorio;
        }
        public virtual async Task<bool> Ejecutar(long id)
        {
            return await _maquinaRepositorio.EliminarMaquina(id);
        }
    }
}
