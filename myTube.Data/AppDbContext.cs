using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using myTube.Domain.Entities;
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

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Canal> Canais { get; set; }
        public DbSet<Filme> Filmes { get; set; }
        public DbSet<LogYoutube> LogYoutube { get; set; }

        public AppDbContext(IConfiguration config)
        {
            _config = config;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseMySql(_config.GetConnectionString("MySQL"), b => b.MigrationsAssembly("myTube.Data"));
            //optionsBuilder.UseSqlServer(_config.GetConnectionString("SQLServer"), b => b.MigrationsAssembly("myTube.Data"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Entity<Usuario>().OwnsOne(e => e.Email);
        }
    }
}
