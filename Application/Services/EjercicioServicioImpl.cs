using AutoMapper;
using FitRank_API.Application.DTOs.Rutina;
using FitRank_API.Application.Interfaces;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.Services
{
    public class EjercicioServicioImpl: IEjercicioServicio
    {
        private readonly IEjercicioRepositorio _repositorio;
        private readonly IMapper _mapper;

        public EjercicioServicioImpl(IEjercicioRepositorio repositorio, IMapper mapper)
        {
            _repositorio = repositorio;
            _mapper = mapper;
        }

        public async Task<List<EjercicioDTO>> ListarEjerciciosAsync()
        {
            var ejercicios = await _repositorio.ListarEjerciciosAsync();
            return _mapper.Map<List<EjercicioDTO>>(ejercicios);
        }
    }
}
