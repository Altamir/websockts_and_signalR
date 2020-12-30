
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace websocketserver.Middleware
{
    public static class WebSocketServerMiddlewareExtension
    {
        public static IApplicationBuilder UseWebSocketServer(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<WebSocketServerMiddleware>();
        }

        public static IServiceCollection AddWebSocketManager(this IServiceCollection services)
        {
            return services.AddSingleton<WebSocketConnectionManager>();
        }
    }
}
