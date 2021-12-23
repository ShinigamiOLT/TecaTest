using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistencia.DataBase;
using Serilog;
using Servicios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TecaFrontEnd
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _env = env;
        }

        public IConfiguration Configuration { get; }
        private readonly IWebHostEnvironment _env;
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string StringConnection = "";
            string Scheme = "";

            if (_env.IsDevelopment())
            {
                StringConnection = Configuration.GetConnectionString("DefaultConnection");
                Scheme = Configuration["AppSettings:DefaultSchema"];
            }
            else
            {
                StringConnection = Configuration.GetConnectionString("DefaultConnection");
                Scheme = Configuration["AppSettings:DefaultSchema"];
            }
            services.AddDbContext<ApplicationDbContext>
         (
             opts =>
             {
                 opts.UseSqlServer(StringConnection, o =>
                 {
                     o.MigrationsHistoryTable("__MyMigrationsHistory", Scheme);
                     o.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                 });
                 opts.EnableSensitiveDataLogging();
             });

            services.AddControllersWithViews();
            services.AddAutoMapper(typeof(Startup));
            services.AddTransient<IClientesServicio, CClientesServicio>();
            services.AddTransient<ICuentaAhorroServicio, CCuentaAhorroServicio>();
            services.AddTransient<IOperacionCuentaServicio, COperacionCuentaServicio>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .Enrich.FromLogContext()
                .CreateLogger();
            loggerFactory.AddSerilog();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
