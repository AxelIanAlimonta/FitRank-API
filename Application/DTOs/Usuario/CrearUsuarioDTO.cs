using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.DTOs.Usuario
{
    public class CrearUsuarioDTO
    {

        public string nombre { get; set; }
        public string apellidos { get; set; }
        public int dni { get; set; }
        public DateTime fechaNacimiento { get; set; }
        public string telefono { get; set; }
        public string estado { get; set; }
        public string correo { get; set; }
        public int alturaCm { get; set; }
        public double pesoKg { get; set; }

        public string nivel { get; set; }

        public string username { get; set; }

        public string email { get; set; }

    }
}
