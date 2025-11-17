using AutoMapper;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.DificultadCasosDeUso
{
    public class EliminarDificultadCasoDeUso
    {

        private readonly IDificultadRepositorio _dificultadRepositorio;
        private readonly IMapper _mapper;

        public EliminarDificultadCasoDeUso(IDificultadRepositorio dificultadRepositorio, IMapper mapper)
        {
            _dificultadRepositorio = dificultadRepositorio;
            _mapper = mapper;
        }

        public async Task Ejecutar(int id)
        {
            await _dificultadRepositorio.EliminarAsync(id);
        }
    }
}
