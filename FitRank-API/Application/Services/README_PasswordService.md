# Servicio de Encriptación de Contraseñas

## ?? Descripción

El `PasswordService` es un servicio centralizado que se encarga de encriptar y verificar contraseñas en toda la aplicación. Implementa el patrón de **Inversión de Dependencias** (SOLID) para facilitar el mantenimiento y permitir cambios futuros en el algoritmo de encriptación sin afectar el resto del código.

## ?? Ventajas

### ? Centralización
- **Un solo punto de cambio**: Si necesitas cambiar el algoritmo de encriptación (por ejemplo, de BCrypt a Argon2), solo debes modificar la implementación del servicio.
- **Consistencia**: Todas las contraseñas se encriptan y verifican de la misma manera en toda la aplicación.

### ? Testabilidad
- Fácil de mockear en pruebas unitarias
- Permite probar lógica de negocio sin depender de la implementación real de encriptación

### ? Mantenibilidad
- Código más limpio y desacoplado
- Facilita la aplicación de principios SOLID

## ?? Uso

### Registro / Creación de Usuario

```csharp
public class RegistrarUsuarioCasoDeUso
{
    private readonly IPasswordService _passwordService;

    public RegistrarUsuarioCasoDeUso(IPasswordService passwordService)
    {
        _passwordService = passwordService;
    }

    public async Task<Usuario> Ejecutar(RegisterDTO dto)
    {
        var usuario = new Usuario
        {
            Email = dto.Email,
            PasswordHash = _passwordService.HashPassword(dto.Password)
        };
        
        // ... resto de la lógica
    }
}
```

### Login / Verificación de Contraseña

```csharp
public class LoginUsuarioCasoDeUso
{
    private readonly IPasswordService _passwordService;

    public LoginUsuarioCasoDeUso(IPasswordService passwordService)
    {
        _passwordService = passwordService;
    }

    public async Task<bool> Ejecutar(LoginDTO dto)
    {
        var usuario = await _usuarioRepo.ObtenerPorEmail(dto.Email);
        
        if (!_passwordService.VerifyPassword(dto.Password, usuario.PasswordHash))
            return false;
            
        // ... resto de la lógica
    }
}
```

## ?? Testing

### Ejemplo de Test con Mock

```csharp
public class RegistrarUsuarioCasoDeUsoTests
{
    private readonly Mock<IPasswordService> _mockPasswordService;
    
    public RegistrarUsuarioCasoDeUsoTests()
    {
        _mockPasswordService = new Mock<IPasswordService>();
    }
    
    [Fact]
    public async Task DebeEncriptarPasswordAlRegistrar()
    {
        // Arrange
        _mockPasswordService
            .Setup(p => p.HashPassword("Password123!"))
            .Returns("hashed_password_xyz");
        
        // Act
        var resultado = await _casoDeUso.Ejecutar(dto);
        
        // Assert
        _mockPasswordService.Verify(
            p => p.HashPassword("Password123!"), 
            Times.Once
        );
    }
}
```

## ?? Cambiar el Algoritmo de Encriptación

Si en el futuro necesitas cambiar de BCrypt a otro algoritmo (como Argon2, PBKDF2, etc.), simplemente:

1. Crea una nueva implementación de `IPasswordService`:

```csharp
public class Argon2PasswordService : IPasswordService
{
    public string HashPassword(string password)
    {
        // Implementación con Argon2
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        // Verificación con Argon2
    }
}
```

2. Cambia el registro en `Program.cs`:

```csharp
// Antes
builder.Services.AddScoped<IPasswordService, PasswordService>();

// Después
builder.Services.AddScoped<IPasswordService, Argon2PasswordService>();
```

¡Y listo! No necesitas modificar ningún caso de uso ni controlador. ??

## ?? Implementación Actual

Actualmente, el servicio utiliza **BCrypt** que es:
- ? Resistente a ataques de fuerza bruta
- ? Genera salt automático para cada contraseña
- ? Algoritmo unidireccional (no se puede desencriptar)
- ? Ampliamente probado y recomendado por OWASP

## ?? Seguridad

El servicio actual:
- ? **NO almacena contraseñas en texto plano**
- ? Genera un salt único por cada contraseña
- ? Usa un algoritmo de hashing lento para prevenir ataques de fuerza bruta
- ? El hash resultante es unidireccional (irreversible)

## ?? Casos de Uso que Utilizan este Servicio

- `LoginUsuarioCasoDeUso` - Verificación de contraseñas
- `RegistrarUsuarioCasoDeUso` - Encriptación al registrar usuarios
- `ActivarCuentaCasoDeUso` - Encriptación al activar cuenta
- `AgregarAdministradorCasoDeUso` - Encriptación al crear administradores
- `AgregarSocioCasoDeUso` - Encriptación al crear socios
- `AgregarProfesorCasoDeUso` - Encriptación al crear profesores
- `AgregarUsuarioConInvitacionCasoDeUso` - Encriptación al registrar con invitación

---

**Última actualización**: 2024
**Autor**: FitRank Development Team
