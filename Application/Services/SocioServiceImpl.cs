using AutoMapper;
using FitRank_API.Application.DTOs.Logro;
using FitRank_API.Application.Interfaces;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.Services
{
    public class SocioServiceImpl : ISocioService
    {
        public readonly ISocioRepositorio _socioRepositorio;
        public readonly Mapper _mapper;

        public SocioServiceImpl(ISocioRepositorio socioRepositorio, IMapper mapper)
        {
            _socioRepositorio = socioRepositorio;
            _mapper = (Mapper)mapper;
        }

        public async Task<IReadOnlyList<LogroUsuarioDto>> MisLogrosAsync(int socioId, int gimnasioId)
        {
            var logros = await _socioRepositorio.MisLogrosAsync(socioId, gimnasioId);
            return _mapper.Map<IReadOnlyList<LogroUsuarioDto>>(logros);
        }
    }
}
