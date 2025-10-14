using System.Security.Principal;

namespace FitRank_API.Infrastructure.Persistence
{
    public class BloqueDiaEntity
    {
        public int Id { get; set; }
        public int IdBloqueRutina { get; set; }
        public int IdDia { get; set; }

        public BloqueRutinaEntity BloqueRutina { get; set; } // Evito la consulta
        public DiaEntity Dia { get; set; }                   // Evito la consulta
    }
}
