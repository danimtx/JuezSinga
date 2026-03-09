namespace Application.Interfaces;

public interface INotificacionService
{
    // Notificar al usuario específico sobre su veredicto
    Task NotificarNuevoVeredictoAsync(Guid usuarioId, object veredicto);

    // Notificar a todos sobre un cambio en el scoreboard de una competencia
    Task NotificarActualizacionScoreboardAsync(Guid competenciaId);

    // Notificar a un usuario (o a todos si es global) sobre una aclaración
    Task NotificarAclaracionAsync(Guid competenciaId, Guid? usuarioId, object aclaracion, bool esGlobal);

    // Notificar a los administradores sobre una nueva pregunta
    Task NotificarNuevaPreguntaAdminAsync(Guid competenciaId, object aclaracion);
}
