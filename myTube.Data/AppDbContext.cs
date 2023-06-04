using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using myTube.Domain.Entities;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace myTube.Data
{
    public class AppDbContext : DbContext
    {
        private readonly IConfiguration _config;
        private readonly ILogger<AppDbContext> _logger;

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Canal> Canais { get; set; }
        public DbSet<Filme> Filmes { get; set; }
        public DbSet<LogYoutube> LogYoutube { get; set; }

        public AppDbContext(IConfiguration config, ILogger<AppDbContext> logger)
        {
            _config = config;
            _logger = logger;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseMySql(_config["CONNECTION_STRING"], ServerVersion.Create(new Version(), ServerType.MariaDb));

//            optionsBuilder.UseMySql(_config["CONNECTION_STRING"], b => b.MigrationsAssembly("myTube.Data"));
            //optionsBuilder.UseMySql(_config.GetConnectionString("MySQL"), b => b.MigrationsAssembly("myTube.Data"));
            //optionsBuilder.UseSqlServer(_config.GetConnectionString("SQLServer"), b => b.MigrationsAssembly("myTube.Data"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<Usuario>().OwnsOne(e => e.Email);
        }
    }
}
