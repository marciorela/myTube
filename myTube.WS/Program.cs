using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using myTube.Data;
using myTube.Data.Repositories;
using myTube.Services;
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

                    services.AddTransient<UsuarioRepository>();
                    services.AddTransient<CanalRepository>();
                    services.AddTransient<VideoRepository>();

                    services.AddSingleton<CheckChannelService>();
                    services.AddSingleton<CheckVideoService>();

                    services.AddSingleton<YoutubeServices>();

                    services.AddHostedService<WSCheck>();
                });
    }
}
