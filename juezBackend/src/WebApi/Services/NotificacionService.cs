using Application.Interfaces;
using Microsoft.AspNetCore.SignalR;
using WebApi.Hubs;

namespace WebApi.Services;

public class NotificacionService(IHubContext<JuezHub> hubContext) : INotificacionService
{
    public async Task NotificarNuevoVeredictoAsync(Guid usuarioId, object veredicto)
    {
        // Enviar al usuario específico (SignalR usa el NameIdentifier del Claim por defecto)
        await hubContext.Clients.User(usuarioId.ToString()).SendAsync("RecibirVeredicto", veredicto);
    }

    public async Task NotificarActualizacionScoreboardAsync(Guid competenciaId)
    {
        // Enviar a todos los que están viendo esa competencia
        await hubContext.Clients.Group(competenciaId.ToString()).SendAsync("ActualizarScoreboard");
    }

    public async Task NotificarAclaracionAsync(Guid competenciaId, Guid? usuarioId, object aclaracion, bool esGlobal)
    {
        if (esGlobal)
        {
            await hubContext.Clients.Group(competenciaId.ToString()).SendAsync("NuevaAclaracionGlobal", aclaracion);
        }
        else if (usuarioId.HasValue)
        {
            await hubContext.Clients.User(usuarioId.Value.ToString()).SendAsync("NuevaRespuestaPrivada", aclaracion);
        }
    }

    public async Task NotificarNuevaPreguntaAdminAsync(Guid competenciaId, object aclaracion)
    {
        await hubContext.Clients.Group("Admins").SendAsync("NuevaPreguntaEquipo", aclaracion);
    }
}
