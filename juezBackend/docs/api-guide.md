# Guía de la API: JuezSinGa

Esta guía detalla los endpoints disponibles para el desarrollo del Frontend.
**Base URL:** `https://localhost:7041`
**Header Obligatorio:** `Authorization: Bearer {token}` (para endpoints protegidos).

## 1. Autenticación (Pública)

| Método | Endpoint | Rol | Descripción | Body (JSON) |
| :--- | :--- | :--- | :--- | :--- |
| `POST` | `/api/Auth/Login` | Público | Inicia sesión. | `{ "userName", "password" }` |
| `POST` | `/api/Auth/Registro` | Público | Registro de estudiantes. | `{ "userName", "password", "nombre", "apellidos", "correo", "universidad", "departamento" }` |
| `POST` | `/api/Auth/RefreshToken` | Público | Renueva el AccessToken. | `{ "tokenExpirado", "refreshToken" }` |

---

## 2. Usuarios y Perfil

| Método | Endpoint | Rol | Descripción | Body / Params |
| :--- | :--- | :--- | :--- | :--- |
| `GET` | `/api/Usuarios/Perfil` | Cualquiera | Obtiene datos del usuario logueado. | (Extraído del Token) |
| `PUT` | `/api/Usuarios/Perfil` | Admin, Estudiante | Actualiza nombre, apellidos y metadatos. | `{ "nombre", "apellidos", "correo", "universidad", "departamento", "pais", "fotoUrl" }` |
| `PUT` | `/api/Usuarios/Perfil/Password` | Admin, Estudiante | Cambia la contraseña. | `{ "passwordActual", "passwordNueva" }` |
| `GET` | `/api/Usuarios` | Solo Admin | Lista usuarios con filtro opcional. | `?rol={1:Admin, 2:Estudiante, 3:Equipo}` |

---

## 3. Problemas

| Método | Endpoint | Rol | Descripción | Body / Params |
| :--- | :--- | :--- | :--- | :--- |
| `GET` | `/api/Problemas` | Cualquiera | Lista resumen (ID y Título). | - |
| `GET` | `/api/Problemas/{id}` | Cualquiera | Detalle completo y casos públicos. | `id` (GUID) |
| `POST` | `/api/Problemas` | Solo Admin | Crea un nuevo problema. | `{ "titulo", "descripcion", "limiteTiempo", "limiteMemoria", "unidad" }` |
| `PUT` | `/api/Problemas/{id}` | Solo Admin | Edita un problema existente. | Igual al POST |
| `DELETE` | `/api/Problemas/{id}` | Solo Admin | Borrado lógico. | `id` (GUID) |
| `POST` | `/api/Problemas/{id}/casos/sincronizar` | Solo Admin | Reemplaza todos los casos de prueba. | `[{ "entrada", "salidaEsperada", "esOculto" }]` |

---

## 4. Competencias (Contests)

| Método | Endpoint | Rol | Descripción | Body / Params |
| :--- | :--- | :--- | :--- | :--- |
| `GET` | `/api/Competencias` | Cualquiera | Lista de contests disponibles. | - |
| `GET` | `/api/Competencias/{id}` | Cualquiera | Detalle y problemas (si ya inició). | `id` (GUID) |
| `POST` | `/api/Competencias` | Solo Admin | Crea un contest. | `{ "titulo", "descripcion", "fechaInicio", "fechaFin", "fechaCongelamiento", "verVeredictoDuranteFreeze", "esPublica" }` |
| `POST` | `/api/Competencias/{id}/problemas` | Solo Admin | Asigna problemas al contest. | `[{ "problemaId", "letra", "colorGlobo" }]` |
| `POST` | `/api/Competencias/{id}/equipos/masivo` | Solo Admin | Genera e inscribe equipos. | `[{ "nombreEquipo", "integrantes", "universidad" }]` |
| `GET` | `/api/Competencias/{id}/scoreboard` | Cualquiera | Ranking bajo reglas ICPC. | `id` (GUID) |

---

## 5. Envíos y Veredictos

| Método | Endpoint | Rol | Descripción | Body / Params |
| :--- | :--- | :--- | :--- | :--- |
| `POST` | `/api/Envios/Problema` | Cualquiera | Envía solución a evaluar. | `{ "problemaId", "codigoFuente", "lenguajeId" }` |
| `GET` | `/api/Envios/Resultado/{id}` | Dueño/Admin | Obtiene el veredicto consolidado. | `id` (GUID del Envío) |
| `GET` | `/api/Envios` | Cualquiera | Historial de envíos. | `?competenciaId={guid}` (opcional) |
| `POST` | `/api/Envios/Prueba` | **DEBUG/ADMIN** | Ejecución rápida sin DB. | (Ver Swagger) |

---

## 6. Tiempo Real (SignalR Hub)

**URL del Hub:** `https://localhost:7041/hubs/juez?access_token={token}`

**Eventos a Escuchar en React:**
*   `RecibirVeredicto`: `{ "envioId", "veredicto", "casosPasados", "totalCasos" }`
*   `ActualizarScoreboard`: (No envía datos, solo indica que hay que refrescar el GET del Scoreboard).
*   `NuevaAclaracionGlobal`: `{ "aclaracionId", "mensaje" }`
*   `NuevaPreguntaEquipo`: (Para el Admin).

---

## 7. Manejo de Errores

Todos los errores no controlados devuelven un código HTTP correspondiente y un JSON:
```json
{
  "error": "Mensaje legible del error",
  "detail": "Detalles técnicos (opcional)"
}
```
*   **401:** Token inválido o expirado.
*   **403:** No tienes el rol necesario (ej: intentas crear un problema siendo Estudiante).
*   **404:** El recurso (GUID) no existe.
*   **429:** Límite de la API de Judge0 alcanzado.
