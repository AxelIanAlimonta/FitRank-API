using Org.BouncyCastle.Bcpg.OpenPgp;

namespace FitRank_API.Domain.Entities;

public class Logro
{
    public long Id { get; set; }
    public string NombreClave { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public string Categoria { get; set; }
    public string Imagen { get; set; }
    public int Puntos { get; set; }
    public bool Estado { get; set; } = true;

    public ICollection<LogroGimnasio> GimnasiosConfigurados { get; set; }
    = new List<LogroGimnasio>();

    public ICollection<LogroSocio> LogrosOtorgados { get; set; }
        = new List<LogroSocio>();
}
