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
using Microsoft.OpenApi.Models;
using TODOList.Configuration;
using TODOList.Entities;
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
            services.Configure<JwtTokenConfiguration> (Configuration.GetSection (JwtTokenConfiguration.SectionName));
            services.Configure<EmailConfiguration> (Configuration.GetSection (EmailConfiguration.SectionName));
            services.Configure<MySqlConfiguration> (Configuration.GetSection (MySqlConfiguration.SectionName));

            ConfigureJwtAuthentication (services);

            var mySqlConfigurationSection = Configuration.GetSection (MySqlConfiguration.SectionName);
            var mySqlConfiguration = mySqlConfigurationSection.Get<MySqlConfiguration> ();
            services.AddDbContext<ApplicationDbContext> (
                options => options.UseMySQL (mySqlConfiguration.ConnectionString));

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

            ConfigureSwagger (services);

            // configure DI for application services
            services.AddScoped<IUserService, UserService> ();
            services.AddScoped<ITodoService, TodoService> ();
            services.AddScoped<IEmailSenderService, EmailSenderService> ();
            }

        private void ConfigureJwtAuthentication (IServiceCollection services)
            {
            var jwtTokenConfigurationSection = Configuration.GetSection (JwtTokenConfiguration.SectionName);
            var jwtTokenConfiguration = jwtTokenConfigurationSection.Get<JwtTokenConfiguration> ();
            var key = Encoding.ASCII.GetBytes (jwtTokenConfiguration.Secret);
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
            }

        private void ConfigureSwagger (IServiceCollection services)
            {
            services.AddSwaggerGen (c =>
                {
                c.SwaggerDoc ("v1", new OpenApiInfo
                    {
                    Title = "TODO API",
                    Version = "v1"
                    });

                // add JWT Authentication
                var securityScheme = new OpenApiSecurityScheme
                    {
                    Name = "JWT Authentication",
                    Description = "Enter JWT Bearer token **_only_**",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                        {
                        Id = JwtBearerDefaults.AuthenticationScheme,
                        Type = ReferenceType.SecurityScheme
                        }
                    };
                c.AddSecurityDefinition (securityScheme.Reference.Id, securityScheme);
                c.AddSecurityRequirement (new OpenApiSecurityRequirement
                    {
                        {securityScheme, new string[] { }}
                    });
                });
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

            app.UseSwagger ();
            app.UseSwaggerUI (c =>
                {
                c.SwaggerEndpoint ("/swagger/v1/swagger.json", "API V1");
                });
            }
        }
    }
