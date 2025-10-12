using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Enums;

namespace FitRank_API.Application.DTOs.EjercicioRealizado
{
    public class EjercicioRealizadoDTOEntrada
    {

        public int UsuarioId { get; set; }
        public int EjercicioId { get; set; }
        public int Series { get; set; }
        public int Repeticiones { get; set; }
        public double Peso { get; set; }
      
        public string Observacion { get; set; }
        
    }
}
