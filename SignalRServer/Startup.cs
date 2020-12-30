using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using SignalRServer.Hubs;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SignalRServer
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors();
            services.AddSignalR();
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCors(builder =>
            {
                builder.WithOrigins("null")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
            });

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context =>
                {
                    await context.Response.WriteAsync("Este é o chat server, use o cliente!");
                });

                endpoints.MapHub<ChatHub>("/chatHub");
            });

        }

    }
}
