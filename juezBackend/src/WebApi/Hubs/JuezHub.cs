using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Hubs;

[Authorize]
public class JuezHub : Hub
{
    // Los clientes pueden unirse a un grupo específico de una competencia
    // para recibir actualizaciones del scoreboard o aclaraciones globales.
    public async Task UnirseACompetencia(Guid competenciaId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, competenciaId.ToString());
    }

    public async Task SalirDeCompetencia(Guid competenciaId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, competenciaId.ToString());
    }

    // Los administradores pueden unirse a un grupo especial para ver todo en tiempo real
    public async Task UnirseAMonitoreoAdmin()
    {
        if (Context.User?.IsInRole("Admin") == true)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
        }
    }

    public override async Task OnConnectedAsync()
    {
        // Lógica opcional al conectar
        await base.OnConnectedAsync();
    }
}
