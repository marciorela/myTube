using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using myTube.Data;
using myTube.Data.Repositories;
using myTube.Services.Youtube;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace myTube.WS
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddTransient<AppDbContext>();

                    services.AddScoped<UsuarioRepository>();
                    services.AddScoped<CanalRepository>();
                    services.AddScoped<VideoRepository>();

                    services.AddScoped<YoutubeServices>();
                    services.AddHostedService<WSCheckNewVideos>();
                    services.AddHostedService<WSValidateChannel>();
                });
    }
}
