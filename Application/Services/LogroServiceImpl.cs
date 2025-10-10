using AutoMapper;
using FitRank_API.Application.DTOs.Logro;
using FitRank_API.Application.Interfaces;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.Services
{
    public class LogroServiceImpl : ILogroService
    {
        private readonly ILogroRepositorio _repo;
        private readonly IMapper _mapper;

        public LogroServiceImpl(ILogroRepositorio repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<LogroDto>> ListarActivosAsync(CancellationToken ct = default)
        {
            var list = await _repo.ListarActivosAsync(ct);
            return _mapper.Map<IReadOnlyList<LogroDto>>(list);
        }

        public async Task<IReadOnlyList<LogroUsuarioDto>> MisLogrosAsync(int socioId, CancellationToken ct = default)
        {
            var list = await _repo.MisLogrosAsync(socioId, ct);
            return _mapper.Map<IReadOnlyList<LogroUsuarioDto>>(list);
        }

        public Task OtorgarSiNoExisteAsync(int socioId, int logroId, CancellationToken ct = default)
        {
            return _repo.OtorgarSiNoExisteAsync(socioId, logroId, ct);
        }
        public Task SetActivoAsync(int logroId, bool activo, CancellationToken ct = default)
        {
            return _repo.SetActivoAsync(logroId, activo, ct);
        }
    }
}
