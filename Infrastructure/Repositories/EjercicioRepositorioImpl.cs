using AutoMapper;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Infrastructure.Repositories
{
    public class EjercicioRepositorioImpl: IEjercicioRepositorio
    {
        private readonly FitRankDbContext _context;
        private readonly IMapper _mapper;

        public EjercicioRepositorioImpl(FitRankDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<Ejercicio>> ListarEjerciciosAsync()
        {
            var ejerciciosEntity = await _context.Ejercicios.ToListAsync();
            return _mapper.Map<List<Ejercicio>>(ejerciciosEntity); //mapea a mi entidad de dominio, es decir, Ejercicio.
        }
    }
}
