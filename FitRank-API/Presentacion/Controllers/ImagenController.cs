using Microsoft.AspNetCore.Mvc;
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
        /// <param name="request">Datos de la imagen</param>
        /// <param name="carpeta">Carpeta destino (opcional, default: imagenes)</param>
        /// <returns>Información de la imagen subida</returns>
        [HttpPost("subir")]
        [ProducesResponseType(typeof(ImagenUploadResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SubirImagen([FromForm] SubirImagenRequestDto request, [FromQuery] string carpeta = "imagenes")
        {
            if (request?.Archivo == null || request.Archivo.Length == 0)
            {
                return BadRequest(new { Mensaje = "No se proporcionó ningún archivo." });
            }

            if (string.IsNullOrWhiteSpace(carpeta))
            {
                return BadRequest(new { Mensaje = "El nombre de la carpeta no puede estar vacío." });
            }

            try
            {
                var resultado = await _imagenService.SubirImagenAsync(request.Archivo, carpeta);
                return Ok(resultado);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Error de validación al subir imagen");
                return BadRequest(new { Mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al subir imagen");
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
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
            if (string.IsNullOrWhiteSpace(key))
            {
                return BadRequest(new { Mensaje = "El key de la imagen no puede estar vacío." });
            }

            try
            {
                var imagen = await _imagenService.ObtenerImagenAsync(key);
                return Ok(imagen);
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogWarning(ex, "Imagen no encontrada: {Key}", key);
                return NotFound(new { Mensaje = "Imagen no encontrada." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener imagen: {Key}", key);
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
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
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
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
            if (string.IsNullOrWhiteSpace(key))
            {
                return BadRequest(new { Mensaje = "El key de la imagen no puede estar vacío." });
            }

            try
            {
                var eliminado = await _imagenService.EliminarImagenAsync(key);
                
                if (!eliminado)
                {
                    return NotFound(new { Mensaje = "Imagen no encontrada." });
                }

                return Ok(new { Mensaje = "Imagen eliminada exitosamente.", Key = key });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar imagen: {Key}", key);
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        /// <summary>
        /// Actualiza una imagen existente
        /// </summary>
        /// <param name="key">Key de la imagen a actualizar</param>
        /// <param name="request">Nuevo archivo de imagen</param>
        /// <returns>Información de la imagen actualizada</returns>
        [HttpPut("{*key}")]
        [ProducesResponseType(typeof(ImagenUploadResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ActualizarImagen(string key, [FromForm] SubirImagenRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return BadRequest(new { Mensaje = "El key de la imagen no puede estar vacío." });
            }

            if (request?.Archivo == null || request.Archivo.Length == 0)
            {
                return BadRequest(new { Mensaje = "No se proporcionó ningún archivo." });
            }

            try
            {
                var resultado = await _imagenService.ActualizarImagenAsync(key, request.Archivo);
                return Ok(resultado);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Error de validación al actualizar imagen");
                return BadRequest(new { Mensaje = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar imagen: {Key}", key);
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }

        /// <summary>
        /// Obtiene la URL pública de una imagen
        /// </summary>
        /// <param name="key">Key de la imagen</param>
        /// <returns>URL pública</returns>
        [HttpGet("url/{*key}")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult ObtenerUrlPublica(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return BadRequest(new { Mensaje = "El key de la imagen no puede estar vacío." });
            }

            try
            {
                var url = _imagenService.ObtenerUrlPublica(key);
                return Ok(new { Key = key, Url = url });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener URL pública: {Key}", key);
                return StatusCode(500, new { Mensaje = "Error interno del servidor." });
            }
        }
    }
}
