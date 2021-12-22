using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistencia.DataBase;
using Serilog;

namespace TecaCoreApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
           
            var webhost = CreateHostBuilder(args).Build();
            using (var scope = webhost.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var application = services.GetService<ApplicationDbContext>();
               application.Database.Migrate();                
              
            }

            webhost.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });      
                
        }
    }
}