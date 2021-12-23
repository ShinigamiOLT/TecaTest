using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Modelos.Identity;
using TecaCoreApi.Configuraciones;
using Persistencia.DataBase;
using Serilog;
using Servicios;
using NETCore.MailKit.Extensions;
using NETCore.MailKit.Infrastructure.Internal;

namespace TecaCoreApi
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

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "TecaCoreApi", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = @"Encabezado de autorización JWT utilizando el esquema Bearer.
    Ingrese 'Bearer' [espacio] y luego su token en la entrada de texto a continuaci�n.
      Ejemplo: 'Bearer 12345abcdef'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
                c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });
            services.AddControllers(options => { options.Conventions.Add(new GroupingByNamespaceConvention()); });
            services.AddCors();
            //services.AddControllers().AddNewtonsoftJson(options =>
            //    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            // );
            services.AddControllers(options => { options.Conventions.Add(new GroupingByNamespaceConvention()); });

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
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultEmailProvider;
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders()
                .AddTokenProvider<CustomDataProtectorTokenProvider<ApplicationUser>>(StringConnection);

            // Set token life span to 24 hours
            services.Configure<DataProtectionTokenProviderOptions>(o => { o.TokenLifespan = TimeSpan.FromHours(24); }
            );

            services.Configure<IdentityOptions>(options =>
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequiredLength = 6;
                    options.Password.RequiredUniqueChars = 1;
                    options.SignIn.RequireConfirmedEmail = true;
                }

            );
            services.AddAutoMapper(typeof(Startup));
            //mail Kit
            var mailKitOptions = Configuration.GetSection("Email").Get<MailKitOptions>();
            services.AddMailKit(config => { config.UseMailKit(mailKitOptions); }
            );

            var ClaveSecreta = Configuration.GetValue<string>("SecretKey");
            var key = Encoding.ASCII.GetBytes(ClaveSecreta);
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
            ConfigurandoInyeccionDependencia(services);

        
            services.AddAutoMapper(typeof(Startup));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,
            ILoggerFactory loggerFactory)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                    {
                        string swaggerJsonBasePath = string.IsNullOrWhiteSpace(c.RoutePrefix) ? "." : "..";
                        c.SwaggerEndpoint($"{swaggerJsonBasePath}/swagger/v1/swagger.json", "TecaCoreApi v1");
                        c.DocumentTitle = "TECA :: API";
                    }

                );
            }

            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .Enrich.FromLogContext()
                .CreateLogger();
            loggerFactory.AddSerilog();
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseRouting();
            var urlAceptadas = Configuration
                .GetSection("MisHost").Value.Split(",");
            app.UseCors(builder => builder
                .WithOrigins(urlAceptadas)
                .SetIsOriginAllowedToAllowWildcardSubdomains()
                .AllowAnyMethod()
                .AllowAnyHeader()

            );
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        private static void ConfigurandoInyeccionDependencia(IServiceCollection services)
        {
            // services.AddTransient<,>();
            services.AddTransient<ITokenServicio, TokenServicio>();
            services.AddTransient<IMasterServicio, CMasterServicio>();
            services.AddTransient<IClientesServicio, CClientesServicio>();
            services.AddTransient<ICuentaAhorroServicio, CCuentaAhorroServicio>(); 
            services.AddTransient<IOperacionCuentaServicio, COperacionCuentaServicio>();

        }
    }

    public class GroupingByNamespaceConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            var controllerNamespace = controller.ControllerType.Namespace;
            var apiVersion = controllerNamespace.Split(".").Last().ToLower();
            if (!apiVersion.StartsWith("v"))
            {
                apiVersion = "v1";
            }

            controller.ApiExplorer.GroupName = apiVersion;
        }
    }
}