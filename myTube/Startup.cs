using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using myTube.Data;
using myTube.Data.Repositories;
using myTube.Services;
using Microsoft.AspNetCore.Http;
using myTube.Automapper;
using AutoMapper;
using System.Globalization;
using Microsoft.AspNetCore.Localization;

namespace myTube
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddDbContext<AppDbContext>();

            services.AddScoped<UsuarioRepository>();
            services.AddScoped<CanalRepository>();
            services.AddScoped<VideoRepository>();
            services.AddScoped<UsuarioService>();

            services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme =
                options.DefaultSignInScheme =
                options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            }).AddCookie(options => options.LoginPath = "/auth/login");

            var config = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CanalProfile>();
                cfg.AddProfile<UsuarioProfile>();
            });
            IMapper mapper = config.CreateMapper();
            services.AddSingleton(mapper);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            var culture = "pt-BR";
            //var culture = "en-US";
            //{            RequestLocalizationOptions localizationOptions = new RequestLocalizationOptions
            //            {
            //                SupportedCultures = new List<CultureInfo> { new CultureInfo(culture) },
            //                SupportedUICultures = new List<CultureInfo> { new CultureInfo(culture) },
            //                DefaultRequestCulture = new RequestCulture(culture)
            //            };
            //            app.UseRequestLocalization(localizationOptions);

            var cultureInfo = new CultureInfo(culture, true);

            //cultureInfo.DateTimeFormat.AMDesignator = "a.m.";
            //cultureInfo.DateTimeFormat.DateSeparator = "-";
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Video}/{action=Index}/{id?}");
            });
        }
    }
}
