# Servicio de Almacenamiento de Imágenes con Cloudflare R2

Este proyecto incluye un servicio completo para gestionar imágenes utilizando Cloudflare R2 (compatible con la API de AWS S3).

## 📋 Configuración

### 1. Obtener credenciales de Cloudflare R2

1. Accede a tu dashboard de Cloudflare
2. Ve a R2 Object Storage
3. Crea un bucket si no tienes uno
4. Genera un API Token con permisos de lectura/escritura
5. Obtén las siguientes credenciales:
   - Access Key ID
   - Secret Access Key
   - Account ID
   - Bucket Name

### 2. Configurar appsettings.json

Actualiza el archivo `appsettings.json` con tus credenciales:

```json
{
  "CloudflareR2": {
    "AccessKey": "TU_ACCESS_KEY",
    "SecretKey": "TU_SECRET_KEY",
    "BucketName": "TU_BUCKET_NAME",
    "AccountId": "TU_ACCOUNT_ID",
    "PublicUrl": "https://TU_DOMINIO_PUBLICO.r2.dev"
  }
}
```

### 3. Configurar dominio público (opcional pero recomendado)

Para acceder a las imágenes públicamente:

1. En el dashboard de R2, selecciona tu bucket
2. Ve a Settings > Public Access
3. Configura un dominio público (puede ser un subdominio de tu sitio o el dominio R2.dev que Cloudflare proporciona)
4. Actualiza `PublicUrl` en appsettings.json con tu dominio

## 🚀 Uso del API

### Endpoints disponibles

#### 1. Subir una imagen

```http
POST /api/Imagen/subir?carpeta=imagenes
Content-Type: multipart/form-data

archivo: [archivo de imagen]
```

**Ejemplo con cURL:**
```bash
curl -X POST "https://localhost:7226/api/Imagen/subir?carpeta=perfil" \
  -F "archivo=@ruta/a/tu/imagen.jpg"
```

**Respuesta exitosa (200):**
```json
{
  "key": "perfil/550e8400-e29b-41d4-a716-446655440000.jpg",
  "url": "https://tu-dominio.r2.dev/perfil/550e8400-e29b-41d4-a716-446655440000.jpg",
  "nombreArchivo": "imagen.jpg",
  "tamanoBytes": 245678,
  "contentType": "image/jpeg",
  "fechaSubida": "2025-11-20T18:30:00Z"
}
```

#### 2. Obtener información de una imagen

```http
GET /api/Imagen/{key}
```

**Ejemplo:**
```http
GET /api/Imagen/perfil/550e8400-e29b-41d4-a716-446655440000.jpg
```

**Respuesta:**
```json
{
  "key": "perfil/550e8400-e29b-41d4-a716-446655440000.jpg",
  "url": "https://tu-dominio.r2.dev/perfil/550e8400-e29b-41d4-a716-446655440000.jpg",
  "nombreArchivo": "imagen.jpg",
  "tamanoBytes": 245678,
  "ultimaModificacion": "2025-11-20T18:30:00Z",
  "eTag": "\"d41d8cd98f00b204e9800998ecf8427e\""
}
```

#### 3. Listar todas las imágenes

```http
GET /api/Imagen?carpeta=perfil
```

**Respuesta:**
```json
[
  {
    "key": "perfil/imagen1.jpg",
    "url": "https://tu-dominio.r2.dev/perfil/imagen1.jpg",
    "nombreArchivo": "imagen1.jpg",
    "tamanoBytes": 245678,
    "ultimaModificacion": "2025-11-20T18:30:00Z",
    "eTag": "\"abc123\""
  },
  {
    "key": "perfil/imagen2.jpg",
    "url": "https://tu-dominio.r2.dev/perfil/imagen2.jpg",
    "nombreArchivo": "imagen2.jpg",
    "tamanoBytes": 187654,
    "ultimaModificacion": "2025-11-20T19:15:00Z",
    "eTag": "\"def456\""
  }
]
```

#### 4. Actualizar una imagen

```http
PUT /api/Imagen/{key}
Content-Type: multipart/form-data

archivo: [nuevo archivo de imagen]
```

**Ejemplo:**
```bash
curl -X PUT "https://localhost:7226/api/Imagen/perfil/550e8400.jpg" \
  -F "archivo=@nueva-imagen.jpg"
```

#### 5. Eliminar una imagen

```http
DELETE /api/Imagen/{key}
```

**Ejemplo:**
```http
DELETE /api/Imagen/perfil/550e8400-e29b-41d4-a716-446655440000.jpg
```

**Respuesta:**
```json
{
  "mensaje": "Imagen eliminada exitosamente",
  "key": "perfil/550e8400-e29b-41d4-a716-446655440000.jpg"
}
```

#### 6. Obtener URL pública

```http
GET /api/Imagen/url/{key}
```

**Respuesta:**
```json
{
  "key": "perfil/imagen.jpg",
  "url": "https://tu-dominio.r2.dev/perfil/imagen.jpg"
}
```

## 📁 Estructura de archivos creados

```
FitRank-API/
├── Application/
│   ├── DTOs/
│   │   ├── ImagenUploadResponseDto.cs
│   │   └── ImagenResponseDto.cs
│   ├── Interfaces/
│   │   └── IImagenService.cs
│   └── Services/
│       └── ImagenService.cs
└── Presentacion/
    └── Controllers/
        └── ImagenController.cs
```

## 🔒 Validaciones

El servicio incluye las siguientes validaciones:

- **Tipos de archivo permitidos**: `.jpg`, `.jpeg`, `.png`, `.gif`, `.webp`
- **Archivo vacío**: Retorna error 400 si el archivo está vacío
- **Nombres únicos**: Genera automáticamente nombres UUID para evitar colisiones
- **Organización por carpetas**: Permite organizar imágenes en carpetas lógicas

## 💡 Ejemplos de uso

### Ejemplo en Angular/TypeScript

```typescript
// Subir imagen
async subirImagen(archivo: File, carpeta: string = 'imagenes') {
  const formData = new FormData();
  formData.append('archivo', archivo);

  const response = await fetch(
    `${API_URL}/api/Imagen/subir?carpeta=${carpeta}`,
    {
      method: 'POST',
      body: formData,
      headers: {
        'Authorization': `Bearer ${token}`
      }
    }
  );

  return await response.json();
}

// Eliminar imagen
async eliminarImagen(key: string) {
  await fetch(`${API_URL}/api/Imagen/${key}`, {
    method: 'DELETE',
    headers: {
      'Authorization': `Bearer ${token}`
    }
  });
}
```

### Ejemplo con fetch en JavaScript

```javascript
// Subir imagen desde input file
const inputFile = document.getElementById('fileInput');
const archivo = inputFile.files[0];

const formData = new FormData();
formData.append('archivo', archivo);

fetch('https://localhost:7226/api/Imagen/subir?carpeta=perfil', {
  method: 'POST',
  body: formData
})
  .then(response => response.json())
  .then(data => {
    console.log('Imagen subida:', data.url);
  });
```

## 🔧 Características técnicas

- **Almacenamiento**: Cloudflare R2 (compatible con S3)
- **Generación de nombres**: UUID únicos para evitar colisiones
- **Metadata**: Se guarda el nombre original del archivo
- **URLs públicas**: Acceso directo a las imágenes mediante URLs
- **Organización**: Sistema de carpetas para organizar imágenes
- **CRUD completo**: Crear, leer, actualizar y eliminar

## 📝 Notas importantes

1. Las imágenes se nombran automáticamente con UUID para evitar conflictos
2. El nombre original del archivo se almacena en los metadatos
3. Las URLs son públicas si configuras tu bucket como público
4. Puedes organizar las imágenes en carpetas (ej: `perfil/`, `gimnasios/`, etc.)
5. El servicio mantiene el tipo de contenido original de la imagen

## 🚨 Solución de problemas

### Error: "Cloudflare R2 credentials are not configured properly"
- Verifica que hayas configurado todas las credenciales en `appsettings.json`
- Asegúrate de que las claves sean correctas

### Error 403: Forbidden
- Verifica los permisos del token de API en Cloudflare
- Asegúrate de que el token tenga permisos de lectura/escritura

### Las imágenes no se muestran
- Verifica que el bucket tenga acceso público configurado
- Confirma que la URL pública esté correctamente configurada en appsettings.json
