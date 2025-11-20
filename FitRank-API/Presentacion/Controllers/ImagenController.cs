using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using FitRank_API.Application.Interfaces;
using FitRank_API.Application.DTOs.ImagenDTOs;

namespace FitRank_API.Presentacion.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImagenController : ControllerBase
    {
        private readonly IImagenService _imagenService;
        private readonly ILogger<ImagenController> _logger;

        public ImagenController(IImagenService imagenService, ILogger<ImagenController> logger)
        {
            _imagenService = imagenService;
            _logger = logger;
        }

        /// <summary>
        /// Sube una imagen a Cloudflare R2
        /// </summary>
        /// <param name="archivo">Archivo de imagen</param>
        /// <param name="carpeta">Carpeta destino (opcional, default: imagenes)</param>
        /// <returns>Información de la imagen subida</returns>
        [HttpPost("subir")]
        [ProducesResponseType(typeof(ImagenUploadResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SubirImagen([FromForm] IFormFile archivo, [FromQuery] string carpeta = "imagenes")
        {
            try
            {
                if (archivo == null || archivo.Length == 0)
                {
                    return BadRequest(new { mensaje = "No se proporcionó ningún archivo" });
                }

                var resultado = await _imagenService.SubirImagenAsync(archivo, carpeta);
                return Ok(resultado);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Error de validación al subir imagen");
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al subir imagen");
                return StatusCode(500, new { mensaje = "Error interno al subir la imagen" });
            }
        }

        /// <summary>
        /// Obtiene información de una imagen específica
        /// </summary>
        /// <param name="key">Key de la imagen en R2</param>
        /// <returns>Información de la imagen</returns>
        [HttpGet("{*key}")]
        [ProducesResponseType(typeof(ImagenResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ObtenerImagen(string key)
        {
            try
            {
                var imagen = await _imagenService.ObtenerImagenAsync(key);
                return Ok(imagen);
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogWarning(ex, "Imagen no encontrada: {Key}", key);
                return NotFound(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener imagen: {Key}", key);
                return StatusCode(500, new { mensaje = "Error interno al obtener la imagen" });
            }
        }

        /// <summary>
        /// Lista todas las imágenes, opcionalmente filtradas por carpeta
        /// </summary>
        /// <param name="carpeta">Carpeta para filtrar (opcional)</param>
        /// <returns>Lista de imágenes</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<ImagenResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ListarImagenes([FromQuery] string? carpeta = null)
        {
            try
            {
                var imagenes = await _imagenService.ListarImagenesAsync(carpeta);
                return Ok(imagenes);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar imágenes");
                return StatusCode(500, new { mensaje = "Error interno al listar las imágenes" });
            }
        }

        /// <summary>
        /// Elimina una imagen
        /// </summary>
        /// <param name="key">Key de la imagen a eliminar</param>
        /// <returns>Confirmación de eliminación</returns>
        [HttpDelete("{*key}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> EliminarImagen(string key)
        {
            try
            {
                var eliminado = await _imagenService.EliminarImagenAsync(key);
                
                if (!eliminado)
                {
                    return NotFound(new { mensaje = $"La imagen con key '{key}' no fue encontrada" });
                }

                return Ok(new { mensaje = "Imagen eliminada exitosamente", key });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar imagen: {Key}", key);
                return StatusCode(500, new { mensaje = "Error interno al eliminar la imagen" });
            }
        }

        /// <summary>
        /// Actualiza una imagen existente
        /// </summary>
        /// <param name="key">Key de la imagen a actualizar</param>
        /// <param name="archivo">Nuevo archivo de imagen</param>
        /// <returns>Información de la imagen actualizada</returns>
        [HttpPut("{*key}")]
        [ProducesResponseType(typeof(ImagenUploadResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ActualizarImagen(string key, [FromForm] IFormFile archivo)
        {
            try
            {
                if (archivo == null || archivo.Length == 0)
                {
                    return BadRequest(new { mensaje = "No se proporcionó ningún archivo" });
                }

                var resultado = await _imagenService.ActualizarImagenAsync(key, archivo);
                return Ok(resultado);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Error de validación al actualizar imagen");
                return BadRequest(new { mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar imagen: {Key}", key);
                return StatusCode(500, new { mensaje = "Error interno al actualizar la imagen" });
            }
        }

        /// <summary>
        /// Obtiene la URL pública de una imagen
        /// </summary>
        /// <param name="key">Key de la imagen</param>
        /// <returns>URL pública</returns>
        [HttpGet("url/{*key}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public IActionResult ObtenerUrlPublica(string key)
        {
            var url = _imagenService.ObtenerUrlPublica(key);
            return Ok(new { key, url });
        }
    }
}
