using FitRank_API.Application.DTOs.ImagenDTOs;

namespace FitRank_API.Application.Interfaces
{
    public interface IImagenService
    {
        Task<ImagenUploadResponseDto> SubirImagenAsync(IFormFile archivo, string carpeta = "imagenes");
        Task<ImagenResponseDto> ObtenerImagenAsync(string key);
        Task<List<ImagenResponseDto>> ListarImagenesAsync(string? carpeta = null);
        Task<bool> EliminarImagenAsync(string key);
        Task<ImagenUploadResponseDto> ActualizarImagenAsync(string key, IFormFile archivo);
        string ObtenerUrlPublica(string key);
    }
}
