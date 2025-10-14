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

            CreateMap<Ejercicio, EjercicioDTO>();
            CreateMap<Dia, DiaDTO>();

            // Rutina <-> ActualizarRutinaDTO
            CreateMap<ActualizarRutinaDTO, Rutina>();
            CreateMap<Rutina, ActualizarRutinaDTO>();

            // Bloques
            CreateMap<ActualizarBloqueRutinaDTO, BloqueRutina>();
            CreateMap<BloqueRutina, ActualizarBloqueRutinaDTO>();

            // Dias de bloques
            CreateMap<ActualizarBloqueDiaDTO, BloqueDia>();
            CreateMap<BloqueDia, ActualizarBloqueDiaDTO>();

            // Ejercicios de bloques
            CreateMap<ActualizarEjercicioBloqueDTO, EjercicioBloque>();
            CreateMap<EjercicioBloque, ActualizarEjercicioBloqueDTO>();

            // DTOs de creación
            CreateMap<CrearRutinaDTO, Rutina>();
            CreateMap<CrearBloqueRutinaDTO, BloqueRutina>();
            CreateMap<CrearEjercicioBloqueDTO, EjercicioBloque>();

        }
    }
}
