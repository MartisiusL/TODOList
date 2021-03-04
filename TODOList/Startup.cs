using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
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

            // configure DI for application services
            services.AddSingleton<IUserService, UserService> ();
            services.AddSingleton<ITodoService, TodoService> ();
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

            MockDatabaseData ();
            }

        private void MockDatabaseData()
            {
            using (var context = new TodosContext ())
                {
                // Creates the database if not exists
                context.Database.EnsureDeleted ();
                context.Database.EnsureCreated ();
                var userMe = new User ()
                    {
                        Email = "martisiuslukas97@gmail.com",
                        Password = "123456789abc",
                        Role = Role.User
                    };
                context.User.Add (userMe);

                var userAdmin = new User ()
                    {
                    Email = "admin@gmail.com",
                    Password = "987654321abc",
                    Role = Role.Admin
                    };
                context.User.Add (userAdmin);

                var userRandom = new User ()
                    {
                    Email = "randomuser@gmail.com",
                    Password = "987654321abc",
                    Role = Role.User
                    };
                context.User.Add (userRandom);


                var newTodo = new TodoItem ()
                    {
                    Name = "First Todo",
                    User = userMe
                    };
                context.TodoItem.Add (newTodo);

                newTodo = new TodoItem ()
                    {
                    Name = "Second Todo",
                    User = userMe
                    };
                context.TodoItem.Add (newTodo);

                newTodo = new TodoItem ()
                    {
                    Name = "First Random",
                    User = userRandom
                    };
                context.TodoItem.Add (newTodo);

                newTodo = new TodoItem ()
                    {
                    Name = "Second Random",
                    User = userRandom
                    };
                context.TodoItem.Add (newTodo);

                context.SaveChanges ();
                }
            }
        }
    }
