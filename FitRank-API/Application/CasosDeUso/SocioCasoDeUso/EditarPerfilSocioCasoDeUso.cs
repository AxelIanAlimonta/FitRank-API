using FitRank_API.Application.DTOs.SocioDTOs;
using FitRank_API.Domain.Interfaces;

namespace FitRank_API.Application.CasosDeUso.SocioCasosDeUso
{
    public class EditarPerfilSocioCasoDeUso
    {
        private readonly ISocioRepositorio _repo;

        public EditarPerfilSocioCasoDeUso(ISocioRepositorio repo)
        {
            _repo = repo;
        }

        public virtual async Task<bool> Ejecutar(long socioId, EditarPerfilSocioDTO dto)
        {
            var socio = await _repo.ObtenerPorIdAsync(socioId);
            if (socio == null) return false;

            socio.Nombre = dto.Nombre;
            socio.Apellido = dto.Apellido;
            socio.Sexo = dto.Sexo;
            socio.FotoDePerfil = dto.FotoUrl;
            socio.Altura = dto.Altura;
            socio.Peso = dto.Peso;

            await _repo.ActualizarAsync(socio);
            return true;
        }
    }
}
