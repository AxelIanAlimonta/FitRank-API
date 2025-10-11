using AutoMapper;
using FitRank_API.Application.DTOs.Logro;
using FitRank_API.Application.Interfaces;
using FitRank_API.Domain.Entities;
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

        public async Task<int> CrearLogroAsync(LogroCreateDto logroDto)
        {
            var logroEntity = _mapper.Map<Logro>(logroDto);
            return await _repo.CrearLogroAsync(logroEntity);
        }
        public async Task<IReadOnlyList<LogroDto>> ListarAsync()
        {
            var logros = await _repo.ListarAsync();
            return _mapper.Map<List<LogroDto>>(logros);
        }

        public async Task SetActivoAsync(int logroId, bool activo)
        {
            await _repo.SetActivoAsync(logroId, activo);
        }

        public async Task<LogroDto?> ObtenerPorIdAsync(int logroId)
        {
            var logro = await _repo.ObtenerPorIdAsync(logroId);
            return logro == null ? null : _mapper.Map<LogroDto>(logro);
        }
    }
}
