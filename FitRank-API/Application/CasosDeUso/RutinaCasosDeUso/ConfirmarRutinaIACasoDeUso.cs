using FitRank_API.Application.DTOs.RutinaDTOs;
using FitRank_API.Domain.Entities;
using FitRank_API.Domain.Interfaces;
using System.Text.Json;

namespace FitRank_API.Application.CasosDeUso.RutinaCasosDeUso
{
    public class ConfirmarRutinaIACasoDeUso
    {
        private readonly IRutinaRepositorio _rutinaRepositorio;

        public ConfirmarRutinaIACasoDeUso(IRutinaRepositorio rutinaRepositorio)
        {
            _rutinaRepositorio = rutinaRepositorio;
        }

        public virtual async Task<ResultadoConfirmarRutinaDTO> EjecutarAsync(ConfirmarRutinaDTO body)
        {
            if (body is null || body.Rutina is null)
                return ResultadoConfirmarRutinaDTO.Fallo("Body vacío.");

            var validacion = await _rutinaRepositorio.ValidarReferenciasAsync(body);
            if (!validacion.Ok)
                return ResultadoConfirmarRutinaDTO.Fallo(validacion.Mensaje);

            string? snapJson = null;
            string? rulesJson = null;
            try
            {
                if (body.Rutina.InputSnapshot is not null)
                    snapJson = JsonSerializer.Serialize(body.Rutina.InputSnapshot);
                if (body.Rutina.RulesExplain is not null)
                    rulesJson = JsonSerializer.Serialize(body.Rutina.RulesExplain);
            }
            catch { }

            var rutina = new Rutina
            {
                Nombre = body.Rutina.Nombre,
                TipoCreacion = "IA",
                FechaCreacion = DateTime.UtcNow,
                Descripcion = $"{body.Rutina.Objetivo} · {body.Rutina.Division}",
                Activa = true,
                SocioId = body.SocioId,
                UsuarioId = body.UsuarioId,
                InputSnapshotJson = snapJson,
                RulesExplainJson = rulesJson,
                Sesiones = new List<Sesion>()
            };

            await _rutinaRepositorio.GuardarRutinaCompletaAsync(rutina, body.Rutina.SesionesPlan);

            return ResultadoConfirmarRutinaDTO.Exito(rutina.Id);
        }
    }
}
