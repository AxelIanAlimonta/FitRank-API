using AutoMapper;
using FitRank_API.Domain.Entities;
using FitRank_API.Infrastructure.Persistence;

namespace FitRank_API.Application.Mappings
{
    public class EjercicioProfile: Profile
    {
        public EjercicioProfile()
        {
            // De Entity a Dominio
            CreateMap<EjercicioEntity, Ejercicio>();

            // De Dominio a Entity (para persistir o actualizar)
            CreateMap<Ejercicio, EjercicioEntity>();
        }
    }
}
