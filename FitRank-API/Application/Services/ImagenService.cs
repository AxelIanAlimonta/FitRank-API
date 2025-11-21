using Amazon.S3;
using Amazon.S3.Model;
using FitRank_API.Application.DTOs.ImagenDTOs;
using FitRank_API.Application.Interfaces;

namespace FitRank_API.Application.Services
{
    public class ImagenService : IImagenService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;
        private readonly string _publicUrl;
        private readonly ILogger<ImagenService> _logger;

        public ImagenService(IAmazonS3 s3Client, IConfiguration configuration, ILogger<ImagenService> logger)
        {
            _s3Client = s3Client;
            _logger = logger;
            _bucketName = configuration["CloudflareR2:BucketName"] 
                ?? throw new InvalidOperationException("BucketName no configurado");
            _publicUrl = configuration["CloudflareR2:PublicUrl"] 
                ?? throw new InvalidOperationException("PublicUrl no configurado");
            
            _logger.LogInformation("ImagenService inicializado - Bucket: {Bucket}, PublicUrl: {Url}", _bucketName, _publicUrl);
        }

        public async Task<ImagenUploadResponseDto> SubirImagenAsync(IFormFile archivo, string carpeta = "imagenes")
        {
            try
            {
                _logger.LogInformation("Intentando subir imagen: {FileName}, Tamaño: {Size} bytes", archivo?.FileName, archivo?.Length);
                
                if (archivo == null || archivo.Length == 0)
                {
                    throw new ArgumentException("El archivo no puede estar vacío");
                }

                // Validar tipo de archivo
                var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();
                
                if (!extensionesPermitidas.Contains(extension))
                {
                    throw new ArgumentException($"Tipo de archivo no permitido. Extensiones permitidas: {string.Join(", ", extensionesPermitidas)}");
                }

                // Generar nombre único
                var nombreArchivo = $"{Guid.NewGuid()}{extension}";
                var key = $"{carpeta}/{nombreArchivo}";

                _logger.LogInformation("Subiendo a R2 - Bucket: {Bucket}, Key: {Key}", _bucketName, key);

                using var memoryStream = new MemoryStream();
                await archivo.CopyToAsync(memoryStream);
                memoryStream.Position = 0;
                
                var putRequest = new PutObjectRequest
                {
                    BucketName = _bucketName,
                    Key = key,
                    InputStream = memoryStream,
                    ContentType = archivo.ContentType,
                    UseChunkEncoding = false,
                    Metadata =
                    {
                        ["x-amz-meta-original-name"] = archivo.FileName,
                        ["x-amz-meta-upload-date"] = DateTime.UtcNow.ToString("o")
                    }
                };

                var response = await _s3Client.PutObjectAsync(putRequest);

                _logger.LogInformation("Respuesta de R2: StatusCode={StatusCode}", response.HttpStatusCode);

                if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception($"Error al subir la imagen. StatusCode: {response.HttpStatusCode}");
                }

                return new ImagenUploadResponseDto
                {
                    Key = key,
                    Url = ObtenerUrlPublica(key),
                    NombreArchivo = archivo.FileName,
                    TamanoBytes = archivo.Length,
                    ContentType = archivo.ContentType,
                    FechaSubida = DateTime.UtcNow
                };
            }
            catch (AmazonS3Exception ex)
            {
                _logger.LogError(ex, "Error de AWS S3: StatusCode={StatusCode}, ErrorCode={ErrorCode}, Message={Message}", 
                    ex.StatusCode, ex.ErrorCode, ex.Message);
                throw new Exception($"Error al comunicarse con R2: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error general al subir imagen");
                throw;
            }
        }

        public async Task<ImagenResponseDto> ObtenerImagenAsync(string key)
        {
            try
            {
                var request = new GetObjectMetadataRequest
                {
                    BucketName = _bucketName,
                    Key = key
                };

                var response = await _s3Client.GetObjectMetadataAsync(request);

                return new ImagenResponseDto
                {
                    Key = key,
                    Url = ObtenerUrlPublica(key),
                    NombreArchivo = response.Metadata["x-amz-meta-original-name"] ?? Path.GetFileName(key),
                    TamanoBytes = response.ContentLength,
                    UltimaModificacion = response.LastModified,
                    ETag = response.ETag
                };
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                throw new FileNotFoundException($"La imagen con key '{key}' no fue encontrada");
            }
        }

        public async Task<List<ImagenResponseDto>> ListarImagenesAsync(string? carpeta = null)
        {
            var request = new ListObjectsV2Request
            {
                BucketName = _bucketName,
                Prefix = carpeta != null ? $"{carpeta}/" : null
            };

            var response = await _s3Client.ListObjectsV2Async(request);
            var imagenes = new List<ImagenResponseDto>();

            foreach (var obj in response.S3Objects)
            {
                // Obtener metadata de cada objeto
                var metadataRequest = new GetObjectMetadataRequest
                {
                    BucketName = _bucketName,
                    Key = obj.Key
                };

                try
                {
                    var metadata = await _s3Client.GetObjectMetadataAsync(metadataRequest);
                    
                    imagenes.Add(new ImagenResponseDto
                    {
                        Key = obj.Key,
                        Url = ObtenerUrlPublica(obj.Key),
                        NombreArchivo = metadata.Metadata["x-amz-meta-original-name"] ?? Path.GetFileName(obj.Key),
                        TamanoBytes = obj.Size ?? 0,
                        UltimaModificacion = obj.LastModified,
                        ETag = obj.ETag
                    });
                }
                catch
                {
                    // Si no se puede obtener metadata, agregar info básica
                    imagenes.Add(new ImagenResponseDto
                    {
                        Key = obj.Key,
                        Url = ObtenerUrlPublica(obj.Key),
                        NombreArchivo = Path.GetFileName(obj.Key),
                        TamanoBytes = obj.Size ?? 0,
                        UltimaModificacion = obj.LastModified,
                        ETag = obj.ETag
                    });
                }
            }

            return imagenes;
        }

        public async Task<bool> EliminarImagenAsync(string key)
        {
            try
            {
                var deleteRequest = new DeleteObjectRequest
                {
                    BucketName = _bucketName,
                    Key = key
                };

                var response = await _s3Client.DeleteObjectAsync(deleteRequest);
                return response.HttpStatusCode == System.Net.HttpStatusCode.NoContent 
                    || response.HttpStatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (AmazonS3Exception ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }
        }

        public async Task<ImagenUploadResponseDto> ActualizarImagenAsync(string key, IFormFile archivo)
        {
            // Primero eliminar la imagen anterior
            await EliminarImagenAsync(key);

            // Extraer la carpeta del key original
            var carpeta = Path.GetDirectoryName(key)?.Replace("\\", "/") ?? "imagenes";

            // Subir la nueva imagen
            return await SubirImagenAsync(archivo, carpeta);
        }

        public string ObtenerUrlPublica(string key)
        {
            // Cloudflare R2 con dominio público configurado
            return $"{_publicUrl.TrimEnd('/')}/{key}";
        }
    }
}
