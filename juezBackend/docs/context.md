# Contexto del Proyecto: JuezSinGa (Backend)

## 1. Visión General
JuezSinGa es una API RESTful desarrollada para funcionar como el motor de evaluación de un juez de programación competitiva universitario (estilo ICPC) para la Universidad Privada Domingo Savio. El sistema recibe código fuente de los estudiantes, lo compila y lo ejecuta de forma segura contra casos de prueba ocultos para emitir un veredicto oficial (Accepted, Time Limit Exceeded, Compilation Error, etc.).

## 2. Stack Tecnológico
* **Framework:** .NET 9 (C#).
* **Arquitectura:** Clean Architecture (Domain, Application, Infrastructure, WebApi) (Usar Patrones de diseño en casos que se lo requiera).
* **Base de Datos:** PostgreSQL (usando Entity Framework Core).
* **Motor de Evaluación:** Judge0 CE. 
  * *Fase actual (MVP):* Consumo de la API de Judge0 a través de RapidAPI.
  * *Fase de producción:* Despliegue en contenedores Docker nativos en Linux aislando recursos con cgroups v1.
  * *Repo *

## 3. Dinámica del Equipo y Reglas de Código
El desarrollo se lleva a cabo en un equipo varias personas con un flujo estricto de Git. Todo el código pasa por revisión mediante Pull Requests antes de hacer merge a la rama principal. 
Por lo tanto, el código generado debe:
* Ser extremadamente legible y seguir principios SOLID.
* Respetar estrictamente las dependencias de la Arquitectura Limpia (Application no puede depender de Infrastructure, el Dominio no puede tener dependencias externas).
* No dejar credenciales quemadas (hardcodeadas) en el código; usar siempre `appsettings.json` o variables de entorno.
* Usar inyección de dependencias nativa de .NET.

## 4. Dominio Principal (Core)
El sistema girará en torno a simular un entorno estricto de competencias. Las entidades principales a modelar incluyen:
* **Submission (Envío):** Rastrea el código enviado, el lenguaje (C++, C#, Python), el tiempo de ejecución, el consumo de memoria y el token de Judge0.
* **Problem (Problema):** Define los límites estrictos de tiempo (`TimeLimit`) y memoria (`MemoryLimit`).
* **TestCase (Caso de Prueba):** Entradas y salidas esperadas, fuertemente enfocadas en casos "ocultos" para evitar trampas.
* **Enums de Veredicto:** Representación exacta de los estados (In Queue, Processing, Accepted, Wrong Answer, TLE, MLE, CE).

## 5. Objetivo Actual del Agente
Actuar como un desarrollador Senior en .NET. Recibirás instrucciones para generar entidades, casos de uso, repositorios o controladores. Tu tarea es generar el código exacto, indicando en qué capa y carpeta de la estructura debe ubicarse cada archivo, sin romper la abstracción del motor externo (Judge0).

## comandos para la migracion

1. dotnet ef migrations add Inicial -p src/Infrastructure -s src/WebApi
2. dotnet ef database update -p src/Infrastructure -s src/WebApi