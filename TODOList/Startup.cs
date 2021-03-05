using System;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using TODOList.Entities;
using TODOList.Helpers;
using TODOList.Models;
using TODOList.Services;

namespace TODOList
    {
    public class Startup
        {
        public Startup (IConfiguration configuration)
            {
            Configuration = configuration;
            }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices (IServiceCollection services)
            {
            services.AddCors ();
            services.AddControllers ();
            services.AddHttpContextAccessor ();

            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection ("AppSettings");
            services.Configure<AppSettings> (appSettingsSection);

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings> ();
            var key = Encoding.ASCII.GetBytes (appSettings.Secret);
            services.AddAuthentication (x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer (x =>
                {
                    x.RequireHttpsMetadata = false;
                    x.SaveToken = true;
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey (key),
                        ValidateIssuer = false,
                        ValidateAudience = false
                    };
                });

            services.AddDbContext<ApplicationDbContext> (
                options => options.UseMySQL ("server=localhost;database=todos;user=root;password=martisius"));

            services.AddIdentity<User, IdentityRole> (opt =>
                    {
                    opt.Password.RequiredLength = 12;
                    opt.Password.RequireDigit = false;
                    opt.Password.RequireUppercase = false;
                    opt.Password.RequireNonAlphanumeric = false;
                    opt.User.RequireUniqueEmail = true;
                    })
                .AddEntityFrameworkStores<ApplicationDbContext> ()
                .AddDefaultTokenProviders ();

            services.Configure<DataProtectionTokenProviderOptions> (opt =>
                opt.TokenLifespan = TimeSpan.FromMinutes (10));

            var emailConfig = Configuration
                .GetSection ("EmailConfiguration")
                .Get<EmailConfiguration> ();
            services.AddSingleton (emailConfig);
            services.AddScoped<IEmailSender, EmailSender> ();

            // configure DI for application services
            services.AddScoped<IUserService, UserService> ();
            services.AddScoped<ITodoService, TodoService> ();
            }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure (IApplicationBuilder app, IWebHostEnvironment env)
            {
            if (env.IsDevelopment ())
                {
                app.UseDeveloperExceptionPage ();
                }

            app.UseHttpsRedirection ();

            app.UseRouting ();

            app.UseCors (x => x
                .AllowAnyOrigin ()
                .AllowAnyMethod ()
                .AllowAnyHeader ());

            app.UseAuthentication ();
            app.UseAuthorization ();

            app.UseEndpoints (endpoints => {
                endpoints.MapControllers ();
            });
            }
        }
    }
