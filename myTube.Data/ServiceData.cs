using Microsoft.Extensions.DependencyInjection;
using myTube.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myTube.Data
{
    public static class ServiceData
    {
        public static void AddDbContext(IServiceCollection services)
        {
            services.AddSingleton<AppDbContext>();

            services.AddSingleton<UsuarioRepository>();
            services.AddSingleton<CanalRepository>();
            services.AddSingleton<FilmeRepository>();
        }
    }
}
