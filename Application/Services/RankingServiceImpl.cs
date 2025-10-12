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
        private readonly CalculoDivisionService _calculoDivisionService;

        public RankingServiceImpl(IUsuarioRepository usuarioRepository,
                                  IPuntuacionDiariaRepository puntuacionDiariaRepository,
                                  IMapper mapper,
                                  FitRankDbContext context,
                                  CalculoDivisionService calculoDivisionService)
        {
            _usuarioRepository = usuarioRepository;
            _puntuacionDiariaRepository = puntuacionDiariaRepository;
            _mapper = mapper;
            _context = context;
            _calculoDivisionService = calculoDivisionService;
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
                        username = usuario.username,
                        TotalPuntos = item.PuntosTotales,
                        Nivel = usuario.nivel,



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

            var ranking = new List<MostrarRankingPorGrupoMuscular>();
            // Agrupamos por usuario y luego por grupo muscular
            var grupos = ejerciciosRealizados
                .GroupBy(er => new { er.UsuarioId, er.Ejercicio.GrupoMuscular });

                foreach(var grupo in grupos){
                   var usuario = grupo.First().Usuario;
                var grupoMuscular = grupo.Key.GrupoMuscular;

                string division = await _calculoDivisionService.CalcularDivisionPorGrupoAsync(usuario, grupoMuscular);

                ranking.Add(new MostrarRankingPorGrupoMuscular
                {
                    username = usuario.username,
                    Nivel = usuario.nivel,
                    GrupoMuscular = grupoMuscular,
                    TotalPuntos = grupo.Sum(er => er.PuntosObtenidos),
                    Nombre = grupo.First().Ejercicio.Nombre,
                    DivisionPorGrupo = division
                }); 
            }


            return ranking.OrderByDescending(r => r.TotalPuntos).ToList();
        }



    }
}







