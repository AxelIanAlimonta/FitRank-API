namespace FitRank_API.Application.DTOs.RutinaDTOs
{
    public class ResultadoConfirmarRutinaDTO
    {
        public bool Ok { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public long RutinaId { get; set; }

        public static ResultadoConfirmarRutinaDTO Exito(long id) =>
            new() { Ok = true, RutinaId = id, Mensaje = "Rutina guardada correctamente." };

        public static ResultadoConfirmarRutinaDTO Fallo(string msg) =>
            new() { Ok = false, Mensaje = msg };
    }
}
