using FitRank_API.Application.DTOs.Asistencia;
using FitRank_API.Infrastructure.Interfaces;

namespace FitRank_API.Application.CasosDeUso.AsistenciaCasosDeUso
{
    public class DetectarSociosInactivosCasoDeUso
    {
        private readonly IAsistenciaRepositorio _asistenciaRepositorio;
        private readonly IUsuarioRepositorio _usuarioRepositorio;

        public DetectarSociosInactivosCasoDeUso(
            IAsistenciaRepositorio asistenciaRepositorio,
            IUsuarioRepositorio usuarioRepositorio)
        {
            _asistenciaRepositorio = asistenciaRepositorio;
            _usuarioRepositorio = usuarioRepositorio;
        }

        public async Task<List<SocioInactivoDTO>> Ejecutar(int diasInactividad = 5)
        {
            var socios = await _usuarioRepositorio.ObtenerSociosActivosAsync();
            var resultado = new List<SocioInactivoDTO>();
            var fechaCorte = DateTime.Today.AddDays(-diasInactividad);

            foreach (var socio in socios)
            {
      
                var ultimaAsistencia = await _asistenciaRepositorio.ObtenerUltimaAsistenciaPorUsuarioAsync(socio.Id);

                if (ultimaAsistencia == null || ultimaAsistencia.Fecha < fechaCorte)
                {
                    resultado.Add(new SocioInactivoDTO
                    {
                        Id = socio.Id,
                        Nombre = socio.Nombre,
                        Apellido = socio.Apellido,
                        Email = socio.Email,
                        GimnasioId = socio.GimnasioId ?? 0,
                        DiasSinAsistir = ultimaAsistencia == null
                            ? (DateTime.Today - socio.FechaRegistro.Date).Days
                            : (DateTime.Today - ultimaAsistencia.Fecha.Date).Days,
                        Telefono=socio.Telefono
                    });
                }
            }
            // 🔹 HARDCODE PARA PRUEBA — Simula un socio inactivo de ejemplo
            resultado.Add(new SocioInactivoDTO
            {
                Id = 2,
                Nombre = "Ayelén",
                Apellido = "Quiroga",
                Email = "ayelenquiroga80@gmail.com",
                GimnasioId = 1,
                DiasSinAsistir = 8,
                Telefono="1130646462",
               
            });
            return resultado.OrderByDescending(s => s.DiasSinAsistir).ToList();
        }
    }
}
