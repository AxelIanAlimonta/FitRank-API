using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace FitRank_API.Application.Hubs
{
    [Authorize]
    public class NotificacionesHub : Hub
    {
        public override async Task OnConnectedAsync()
        {
            

            foreach (var c in Context.User.Claims)
            {
                Console.WriteLine($"{c.Type} = {c.Value}");
            }

            

            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var gimnasioId = Context.User?.FindFirst(ClaimTypes.GroupSid)?.Value;

            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
                
            }
            else
            {
                Console.WriteLine("⚠ No se pudo encontrar el UserId para SignalR");
            }

            if (!string.IsNullOrEmpty(gimnasioId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"gimnasio-{gimnasioId}");
                Console.WriteLine($"💜 Conectado a grupo gimnasio-{gimnasioId}");
            }
            else
            {
                Console.WriteLine("⚠ No se pudo encontrar el GimnasioId en los claims");
            }

            await base.OnConnectedAsync();
        }


        public async Task EnviarActualizacionTema(long gimnasioId, object tema)
        {
            await Clients.Group($"gimnasio-{gimnasioId}")
                .SendAsync("ThemeUpdated", tema);

            Console.WriteLine($"🎨 Tema enviado al grupo gimnasio-{gimnasioId}");
        }

    }
}
