using FitRank_API.Application.DTOs.EjercicioRealizado;
using FitRank_API.Domain.Entities;

namespace FitRank_API.Application.Interfaces
{
    public interface IEjercicioRealizado
    {
        Task<IEnumerable<EjercicioRealizadoDTOSalida>> GetByUsuarioAsync(int usuarioId);


        Task<EjercicioRealizadoDTOSalida> RegistrarEjercicioAsync(EjercicioRealizadoDTOEntrada dto);
    }

}

