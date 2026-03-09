# Contexto del Proyecto: JuezSinGa (Backend)

## 1. Visión General
JuezSinGa es un motor de evaluación de alto rendimiento para programación competitiva (estilo ICPC). El sistema permite gestionar competencias (contests) en tiempo real, evaluando código fuente de forma segura contra casos de prueba ocultos, gestionando límites de tiempo/memoria y calculando rankings automáticos bajo reglas internacionales.

## 2. Stack Tecnológico
*   **Framework:** .NET 9 (C#).
*   **Arquitectura:** Clean Architecture (Domain, Application, Infrastructure, WebApi).
*   **Base de Datos:** PostgreSQL (Hospedado en Supabase) usando Entity Framework Core 9.
*   **Motor de Evaluación:** Judge0 CE (Integrado vía RapidAPI o Self-hosted).
*   **Seguridad:** JWT (JSON Web Tokens) con Refresh Tokens y Hashing BCrypt.
*   **Tiempo Real:** SignalR para veredictos, aclaraciones y actualización de Scoreboard.

## 3. Características Principales (PRD)
*   **Gestión de Identidad:** Sistema de roles (Admin, Estudiante, Equipo) con metadatos flexibles almacenados en formato JSONB.
*   **Generador de Competencia:** Creación masiva de cuentas de equipo con credenciales aleatorias seguras y formato personalizado (`singaDMTX-XXXXX`).
*   **Motor de Evaluación Paralelo:** Evaluación simultánea de múltiples casos de prueba usando semáforos de control para optimizar el uso de APIs externas (Quitar semaforos para prod y Judge0 CE esta corriendo en un servidor propio).
*   **Lógica ICPC:** Scoreboard automático que calcula problemas resueltos y penalizaciones por tiempo/errores.
*   **Scoreboard Freeze:** Capacidad de congelar el ranking público para añadir suspenso al final de la competencia.
*   **Sistema de Aclaraciones:** Chat bidireccional entre competidores y jueces con capacidad de anuncios globales.
*   **Resiliencia:** Middleware global de excepciones y limpieza automática de base de datos ante fallos de cuota en motores externos (Error 429 en caso de estar usando API con limites).

## 4. Estructura de Datos
*   **Identificadores:** Uso estricto de **GUID** para todos los IDs de base de datos.
*   **Unidades:** Tiempo en **Segundos**, Memoria en **Kilobytes**.
## 5. Motor de Evaluación: Judge0 CE
El núcleo de la evaluación técnica se basa en **Judge0 CE**, un motor de ejecución de código de código abierto y robusto.
*   **Repositorio Oficial:** [https://github.com/judge0/judge0.git](https://github.com/judge0/judge0.git)

### Estrategia de Despliegue (Desarrollo vs. Producción)
1.  **Fase de Desarrollo (Actual):**
    *   Se utiliza la API de Judge0 a través de **RapidAPI**.
    *   **Limitación:** El plan gratuito tiene cuotas mensuales y límites de velocidad (Rate Limit).
    *   **Control de Concurrencia:** El backend implementa un `SemaphoreSlim(2)` en los casos de uso de evaluación para evitar errores `429 (Too Many Requests)` al disparar múltiples peticiones en paralelo.

2.  **Fase de Producción (Recomendado):**
    *   **Instalación:** Desplegar Judge0 CE en un servidor local o VPS con **Ubuntu Linux** utilizando **Docker y Docker Compose**.
    *   **Ventajas:** Peticiones ilimitadas, latencia mínima, mayor seguridad y control total sobre los recursos del sistema.
    *   **Cambios en el Backend:**
        1.  **Configuración:** Actualizar `BaseUrl` en el archivo `appsettings.json` para que apunte a la IP del servidor local (ej: `http://192.168.1.50:2358`).
        2.  **Optimización:** Eliminar el `SemaphoreSlim` en `EvaluarEnvioCompetenciaUseCase.cs` para permitir que `Task.WhenAll` envíe todos los casos de prueba de forma masiva y simultánea, aprovechando toda la potencia del hardware propio.

