using AutoMapper;
using FitRank_API.Application.DTOs.Rutina;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Persistence;

namespace FitRank_API.Application.Mappings
{
    public class RutinaProfile : Profile
    {
        public RutinaProfile()
        {
            // Rutina persistencia <-> dominio
            CreateMap<RutinaEntity, Rutina>().ReverseMap();

            // Rutina dominio <-> DTO
            CreateMap<Rutina, RutinaDTO>().ReverseMap();
            CreateMap<Rutina, CrearRutinaDTO>().ReverseMap();
            CreateMap<Rutina, ActualizarRutinaDTO>().ReverseMap();

            // Bloques
            CreateMap<BloqueRutinaEntity, BloqueRutina>().ReverseMap();
            CreateMap<BloqueRutina, BloqueRutinaDTO>().ReverseMap();

            // Dias de bloques
            CreateMap<BloqueDiaEntity, BloqueDia>().ReverseMap();
            CreateMap<BloqueDia, BloqueDiaDTO>().ReverseMap();

            // Dia real
            CreateMap<DiaEntity, Dia>().ReverseMap();

            // Ejercicios de bloques
            CreateMap<EjercicioBloqueEntity, EjercicioBloque>().ReverseMap();
            CreateMap<EjercicioBloque, EjercicioBloqueDTO>().ReverseMap();

            // Ejercicios reales
            CreateMap<EjercicioEntity, Ejercicio>().ReverseMap();

        }
    }
}
