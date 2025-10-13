using AutoMapper;
using FitRank_API.Application.DTOs.Logro;
using FitRank_API.Application.Interfaces;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.Services
{
    public class GimnasioServiceImpl : IGimnasioService
    {
        private readonly IGimnasioRepositorio _repo;
        private readonly IMapper _mapper;

        public GimnasioServiceImpl(IGimnasioRepositorio repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<LogroDto>> ListarLogrosActivosAsync(int idGimnasio)
        {
            var logros = await _repo.ListarLogrosActivosAsync(idGimnasio);
            return _mapper.Map<IReadOnlyList<LogroDto>>(logros);
        }

        public Task OtorgarLogroAsync(int socioId, int logroId, int gimnasioId)
            => _repo.OtorgarLogroAsync(socioId, logroId, gimnasioId);

        public Task SetEstadoLogroAsync(int idGimnasio, int logroId, bool activo)
            => _repo.SetEstadoLogro(idGimnasio, logroId, activo);
    }
}
