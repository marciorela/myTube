using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using myTube.Data;
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
                    ServiceData.AddDbContext(services);
                    services.AddHostedService<WSCheckNewVideos>();
                });
    }
}
