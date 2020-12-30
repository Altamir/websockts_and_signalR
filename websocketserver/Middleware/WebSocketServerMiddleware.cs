using System;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Threading;
using Newtonsoft.Json;
using System.Linq;

namespace websocketserver.Middleware
{
    public class WebSocketServerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly WebSocketConnectionManager _manager;

        public WebSocketServerMiddleware(RequestDelegate next, WebSocketConnectionManager manager)
        {
            this._next = next ?? throw new ArgumentNullException(nameof(next));
            this._manager = manager ?? throw new ArgumentNullException(nameof(manager));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            Console.WriteLine("Primeiro hanlder");
            WriteRequestPara(context);
            if (context.WebSockets.IsWebSocketRequest)
            {
                WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
                Console.WriteLine("WebSockect connectado");

                string ConnID = _manager.AddSocket(webSocket);
                await SendConnIdAsync(webSocket, ConnID);

                await ReceiveMassage(webSocket, async (result, buffer) =>
                {
                    if (result.MessageType == WebSocketMessageType.Text)
                    {
                        var msg = Encoding.UTF8.GetString(buffer, 0, result.Count);
                        Console.WriteLine("Message received");
                        Console.WriteLine($"Message: {msg}");
                        await RouteJsonMassageAsync(msg);
                    }
                    else if (result.MessageType == WebSocketMessageType.Close)
                    {
                        string id = _manager.AllSockects.FirstOrDefault((s) => s.Value == webSocket).Key;
                        Console.WriteLine("Recebeu Close Message");
                        if (_manager.AllSockects.TryRemove(id, out WebSocket sock))
                            await sock.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
                        return;
                    }
                });
            }
            else
            {
                await _next(context);
            }
        }

        private async Task SendConnIdAsync(WebSocket socket, string connId)
        {
            var buffer = Encoding.UTF8.GetBytes($"ConnID: {connId}");
            await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public void WriteRequestPara(HttpContext context)
        {
            Console.WriteLine($"Context show:");
            Console.WriteLine($"Request Method: {context.Request.Method}");
            Console.WriteLine($"Request Protocol: {context.Request.Protocol}");

            if (context.Request.Headers != null)
            {
                foreach (var h in context.Request.Headers)
                {
                    Console.WriteLine($"{h.Key} : {h.Value}");
                }
            }
            Console.WriteLine($"Context show end");
        }

        private async Task ReceiveMassage(WebSocket socket, Action<WebSocketReceiveResult, byte[]> handlerMessage)
        {
            var buffer = new byte[1024 * 4];
            while (socket.State == WebSocketState.Open)
            {
                var result = await socket.ReceiveAsync(
                    buffer: new ArraySegment<byte>(buffer),
                    cancellationToken: CancellationToken.None);
                handlerMessage(result, buffer);
            }
        }

        public async Task RouteJsonMassageAsync(string message)
        {
            var routObj = JsonConvert.DeserializeObject<dynamic>(message);

            if (Guid.TryParse(routObj.Para.ToString(), out Guid guiOutPut))
            {
                Console.WriteLine("Para alguem");
                var sck = _manager.AllSockects.FirstOrDefault((x) => x.Key == routObj.Para.ToString());
                if (sck.Value != null)
                {
                    if (sck.Value.State == WebSocketState.Open)
                    {
                        await sck.Value.SendAsync(
                           Encoding.UTF8.GetBytes(routObj.Mensagem.ToString()),
                           WebSocketMessageType.Text,
                           true,
                           CancellationToken.None);
                    }
                    else
                    {
                        Console.WriteLine($"{routObj.Para.ToString()} Connection is Close");
                    }
                }
            }
            else
            {
                Console.WriteLine("Brodcast");
                foreach (var item in _manager.AllSockects)
                {
                    if (item.Value.State == WebSocketState.Open)
                    {
                        await item.Value.SendAsync(
                            Encoding.UTF8.GetBytes(routObj.Mensagem.ToString()),
                            WebSocketMessageType.Text,
                            true,
                            CancellationToken.None);
                    }
                }
            }
        }
    }
}
