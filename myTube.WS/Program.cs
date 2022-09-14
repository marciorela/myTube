using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MR.Log;
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
            MRLog.ConfigureLogMain();

            try
            {
                CreateHostBuilder(args).Build().Run();
            }
            finally
            {
                MRLog.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .MRConfigureLogService()
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<AppDbContext>();

                    services.AddSingleton<UsuarioRepository>();
                    services.AddSingleton<CanalRepository>();
                    services.AddSingleton<VideoRepository>();
                    services.AddSingleton<LogYoutubeRepository>();

                    services.AddSingleton<CheckChannelService>();
                    services.AddSingleton<CheckVideoService>();

                    services.AddSingleton<YoutubeServices>();

                    services.AddHostedService<WSCheck>();
                });
    }
}
