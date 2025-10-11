namespace FitRank_API.Domain.Entities
{
    public class Rutina
    {
        public int Id { get; set; }
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public int FrecuenciaSemanal {  get; set; } // 1/2/3, cuantas veces se hace la rutina completa
        public List<BloqueRutina> bloques { get; set; } = new(); // fullBody, torso, etc.
    }
}
