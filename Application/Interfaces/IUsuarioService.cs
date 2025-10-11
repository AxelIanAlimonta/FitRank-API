using FitRank_API.Application.DTOs.Usuario;

namespace FitRank_API.Application.Interfaces
{
    public interface IUsuarioService
    {
      
        Task  <UsuarioDTO> GetUsuarioByIdAsync(int usuarioId);
        Task <UsuariorRespuestaDto> CrearUsuarioAsync(CrearUsuarioDTO usuarioDto);
        Task<UsuarioDTO> BuscarUsuarioPorDni(int dni);


    }
}
