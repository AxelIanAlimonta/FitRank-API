using AutoMapper;
using FitRank_API.Application.DTOs.Rankig;
using FitRank_API.Application.DTOs.Ranking;
using FitRank_API.Application.Interfaces;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Interfaces;
using FitRank_API.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace FitRank_API.Application.Services
{


    public class RankingServiceImpl : IRankingService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IPuntuacionDiariaRepository _puntuacionDiariaRepository;
        private readonly IMapper _mapper;
        private readonly FitRankDbContext _context;

        public RankingServiceImpl(IUsuarioRepository usuarioRepository,
                                  IPuntuacionDiariaRepository puntuacionDiariaRepository,
                                  IMapper mapper,
                                  FitRankDbContext context)
        {
            _usuarioRepository = usuarioRepository;
            _puntuacionDiariaRepository = puntuacionDiariaRepository;
            _mapper = mapper;
            _context = context;
        }


        public async Task<List<MostrarRankingDTO>> MostrarRankingAsync()
        {

            var puntuacionesPorUsuario = await _context.PuntuacionesDiarias
                .GroupBy(pd => pd.UsuarioId)
                .Select(g => new
                {
                    UsuarioId = g.Key,
                    PuntosTotales = g.Sum(pd => pd.Puntos)
                })
                .OrderByDescending(x => x.PuntosTotales)
                .ToListAsync();

            var ranking = new List<MostrarRankingDTO>();

            foreach (var item in puntuacionesPorUsuario)
            {

                var usuario = await _usuarioRepository.GetByIdAsync(item.UsuarioId);
                if (usuario != null)
                {
                    ranking.Add(new MostrarRankingDTO
                    {
                        userName = usuario.Username,
                        TotalPuntos = item.PuntosTotales,
                        Nivel = usuario.Nivel,



                    });
                }
            }

            return ranking;
        }




        public async Task<List<MostrarRankingPorGrupoMuscular>> MostrarRankingPorGrupoMuscularAsync()
        {
            // Traemos todos los ejercicios realizados con el usuario y el ejercicio
            var ejerciciosRealizados = await _context.EjerciciosRealizados
                .Include(er => er.Ejercicio)
                .Include(er => er.Usuario)
                .ToListAsync();

            // Agrupamos por usuario y luego por grupo muscular
            var ranking = ejerciciosRealizados
                .GroupBy(er => new { er.UsuarioId, er.Ejercicio.GrupoMuscular })
                .Select(g => new MostrarRankingPorGrupoMuscular
                {
                    userName = g.First().Usuario.Username,
                    Nivel = g.First().Usuario.Nivel.ToString(),
                    GrupoMuscular = g.Key.GrupoMuscular,
                    TotalPuntos = g.Sum(er => er.PuntosObtenidos),
                    Nombre= g.First().Ejercicio.Nombre
                })
                .OrderByDescending(r => r.TotalPuntos)
                .ToList();

            return ranking;
        }



    }
}







