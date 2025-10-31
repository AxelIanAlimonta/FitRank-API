using System.ComponentModel.DataAnnotations;

namespace FitRank_API.Domain.Entities
{
    public class Invitacion
    {
        public long Id { get; set; }
        [Required]
        public long GimnasioId { get; set; } 
        public Gimnasio Gimnasio { get; set; }
        [Required]
        public string Email { get; set; } = string.Empty;
        public string DatosPrellenados { get; set; } = string.Empty; 
        public string MetodoPago { get; set; } = "Efectivo"; 
        public string? MpPaymentId { get; set; }  
        public DateTime CreadaEn { get; set; }
        public DateTime ExpiraEn { get; set; }  
        public string Estado { get; set; } = "Pendiente";  
        public long? UsuarioId { get; set; }
        public Socio? Usuario { get; set; }
        public DateTime? CuotaPagadaHasta { get; set; }  
    }
}
