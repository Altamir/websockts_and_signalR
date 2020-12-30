using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.WebSockets;
using System.Threading;
using websocketserver.Middleware;

namespace websocketserver
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddWebSocketManager();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseWebSockets();
            app.UseWebSocketServer();

            app.Run(async (context) =>
            {
                Console.WriteLine("Segundo hanlder");
                await context.Response.WriteAsync("Ola do custon request delegate");
            });
        }
    }
}
