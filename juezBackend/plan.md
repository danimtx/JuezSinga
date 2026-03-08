# Plan de Implementación: Usuarios y Seguridad (JWT + Cuentas de Competencia)

Este documento detalla la estrategia para implementar el sistema de identidad, roles y generación masiva de cuentas para el JuezSinGa.

## 1. Capa de Dominio (Domain)
Responsabilidades: Definir la entidad de usuario y el modelo flexible de metadatos.

*   **Enum `RolUsuario`**: `Admin`, `Estudiante`, `Equipo`.
*   **Clase `MetadatosUsuario` (JSONB)**:
    *   Contendrá: `Universidad`, `Departamento`, `Pais`, `NombreEquipo`, `Integrantes`, `FotoUrl`.
*   **Entidad `Usuario`**:
    *   `Guid Id` (PK).
    *   `String UserName` (Único).
    *   `String PasswordHash`.
    *   `String Nombre`, `String Apellidos`, `String Correo`.
    *   `RolUsuario Rol`.
    *   `MetadatosUsuario Metadatos` (Mapeado como JSONB).
    *   `Date FechaRegistro`.
    *   `Bool EstaActivo`.

## 2. Capa de Aplicación (Application)
Responsabilidades: Casos de uso para autenticación y gestión.

*   **Servicios e Interfaces**:
    *   `IPasswordHasher`: Interfaz para encriptar y verificar contraseñas.
    *   `ITokenService`: Interfaz para generar los JWT.
*   **Casos de Uso**:
    *   `LoginCasoDeUso`: Valida credenciales y devuelve el JWT.
    *   `RegistroEstudianteCasoDeUso`: Permite que alumnos se registren solos.
    *   `GenerarEquiposMasivoCasoDeUso`: 
        1. Recibe una lista de datos básicos (NombreEquipo, Universidad, etc.).
        2. Genera el Hash de 5 caracteres (A-Z, 0-9).
        3. Crea el `UserName` (`singaDMTX-XXXXX`) y la Password (`TJ-XXXXX`).
        4. Guarda en la DB con el rol `Equipo`.
*   **DTOs**: `LoginPeticionDto`, `UsuarioRespuestaDto`, `CrearEquipoDto`.

## 3. Capa de Infraestructura (Infrastructure)
Responsabilidades: Implementaciones técnicas y persistencia.

*   **Seguridad**:
    *   Implementación de `BCrypt` para el hashing de contraseñas.
    *   Implementación de JWT con `System.IdentityModel.Tokens.Jwt`.
*   **Persistencia (EF Core + Npgsql)**:
    *   Configurar el mapeo de la propiedad `Metadatos` usando `.ToJson()` (forma Pro para JSONB en EF Core 9).
    *   Configurar índices únicos para `UserName`.
*   **Constantes**: Clase centralizada para el formato de usuario y reglas del hash.

## 4. Capa WebApi
Responsabilidades: Exponer los controladores y proteger los endpoints.

*   **`AuthController`**:
    *   `POST /api/Auth/Login`.
    *   `POST /api/Auth/Registro` (Para estudiantes).
*   **`UsuariosController`**:
    *   `POST /api/Usuarios/Equipos/Masivo` (Solo Admin).
    *   `GET /api/Usuarios/Equipos/Exportar` (Devuelve lista de credenciales generadas).
*   **Configuración**:
    *   Habilitar `Authentication` y `Authorization` en `Program.cs`.
    *   Configurar Swagger para que acepte el token Bearer (Botón de candado).

## 5. Reglas de Generación de Cuentas (Lógica Técnica)
*   **Longitud Hash**: 5 caracteres.
*   **Alfabeto**: `ABC...XYZ012...9` (Sin Ñ, sin tildes).
*   **Formato de Constante**: 
    *   `USER_PREFIX = "singaDMTX-"`
    *   `PASS_PREFIX = "TJ-"`

---
**Nota:** Al ser un proyecto público, se incluirán comentarios XML detallados y se mantendrá la Regla de Oro (idioma español en el código).
