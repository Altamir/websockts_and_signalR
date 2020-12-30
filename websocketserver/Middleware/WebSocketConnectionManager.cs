using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace websocketserver.Middleware
{
    public class WebSocketConnectionManager
    {

        ConcurrentDictionary<string, WebSocket> _sockets = new ConcurrentDictionary<string, WebSocket>();

        public ConcurrentDictionary<string, WebSocket> AllSockects => _sockets;

        public string AddSocket(WebSocket socket)
        {
            string ConnId = Guid.NewGuid().ToString();

            _sockets.TryAdd(ConnId, socket);
            Console.WriteLine($"Connection add {ConnId}");
            return ConnId;
        }
    }
}
