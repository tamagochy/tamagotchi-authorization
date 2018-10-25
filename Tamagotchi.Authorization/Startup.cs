using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Swagger;
using Microsoft.IdentityModel.Tokens;
using Tamagotchi.Authorization.Core;
using Tamagotchi.Authorization.Models;

namespace Tamagotchi.Authorization
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IHostingEnvironment Environment { get; }
        public object CompatibilityVersion { get; private set; }

        public Startup(IConfiguration configuration, IHostingEnvironment env)
        {
            Configuration = configuration;
            Environment = env;
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<UserContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("LocalDB")));
            var appInfo = Configuration.GetSection("AppInfo");
            //services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            //      .AddJwtBearer(options =>
            //      {
            //          options.RequireHttpsMetadata = false;
            //          options.TokenValidationParameters = new TokenValidationParameters
            //          {
            //              // будет ли валидироваться время существования
            //              ValidateLifetime = true,
            //              // валидация ключа безопасности
            //              ValidateIssuerSigningKey = true,
            //              // установка ключа безопасности
            //              IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appInfo.GetSection("SecretKey").Value))
            //          };
            //      });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Title = "TamagotchiAuth",
                    Version = "v1",
                    Description = "Application Documentation",
                    TermsOfService = "None",
                    Contact = new Contact { Name = "Sergey Alekseev", Url = "github.com/itine" },
                    License = new License { Name = "MIT", Url = "https://en.wikipedia.org/wiki/MIT_License" }
                });
            });
            services.AddMvc();
            services.AddScoped<IUserRepository, UserRepository>();
            services.Configure<AppInfo>(appInfo);
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            UpdateDatabase(app);
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseDeveloperExceptionPage();
            app.UseMvc();
            // Enable middleware to serve generated Swagger as a JSON endpoint
            app.UseSwagger();
            //Enable middleware to serve swagger - ui assets(HTML, JS, CSS etc.)
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "TamagotchiAuth");
            });
            app.UseSwagger();
        }

        private static void UpdateDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<UserContext>())
                {
                    context.Database.Migrate();
                }
            }
        }
    }
}
