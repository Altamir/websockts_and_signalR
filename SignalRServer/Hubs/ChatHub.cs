using Microsoft.AspNetCore.SignalR;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRServer.Hubs
{
    public class ChatHub : Hub
    {
        public override Task OnConnectedAsync()
        {
            Console.WriteLine($"Conectado {Context.ConnectionId}");
            Clients.Client(Context.ConnectionId).SendAsync("receiveConnId", Context.ConnectionId);
            return base.OnConnectedAsync();
        }

        public async Task SendMensageAsync(string message)
        {
            var routObj = JsonConvert.DeserializeObject<dynamic>(message);
            string toCliente = routObj.Para;
            Console.WriteLine($"Mensagem recebida: {Context.ConnectionId}");

            if (string.IsNullOrWhiteSpace(toCliente))
            {
                await Clients.All.SendAsync("receiveMessage", message);
            }
            else
            {
                await Clients.Client(toCliente).SendAsync("receiveMessage", message);
            }
        }
    }
}
