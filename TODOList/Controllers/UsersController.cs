using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TODOList.Entities;
using TODOList.Models;
using TODOList.Services;

namespace TODOList.Controllers
    {
    [Authorize (AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route ("[controller]")]
    public class UsersController : ControllerBase
        {
        private IUserService _userService;
        private readonly UserManager<User> _userManager;

        public UsersController (IUserService userService, UserManager<User> userManager)
            {
            _userService = userService;
            _userManager = userManager;
            }

        [AllowAnonymous]
        [HttpPost ("authenticate")]
        public IActionResult Authenticate ([FromBody] AuthenticateModel model)
            {
            var user = _userService.Authenticate (model.Username, model.Password);

            if (user == null)
                return BadRequest (new { message = "Email or password is incorrect" });

            return Ok (user);
            }

        [AllowAnonymous]
        [HttpGet ("generate")]
        public async Task<IActionResult> GenerateUsers ()
            {
            await MockDatabaseData ();
            return Ok ("Database Mocked");
            }

        private async Task MockDatabaseData ()
            {
            await using (var context = new ApplicationDbContext ())
                {
                // Creates the database if not exists
                await context.Database.EnsureDeletedAsync ();
                await context.Database.EnsureCreatedAsync ();
                await context.SaveChangesAsync ();
                }
            
            var userMe = new User ()
                {
                UserName = "martisiuslukas97@gmail.com",
                Email = "martisiuslukas97@gmail.com",
                NormalizedEmail = "martisiuslukas97@gmail.com",
                Role = Role.User
                };
            await _userManager.CreateAsync (userMe, "123456789abc");

            var userAdmin = new User ()
                {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                NormalizedEmail = "admin@gmail.com",
                Role = Role.Admin
                };
            await _userManager.CreateAsync (userAdmin, "123456789abc");

            var userRandom = new User ()
                {
                UserName = "randomuser@gmail.com",
                Email = "randomuser@gmail.com",
                NormalizedEmail = "randomuser@gmail.com",
                Role = Role.User
                };
            await _userManager.CreateAsync (userRandom, "123456789abc");

            await using (var context = new ApplicationDbContext ())
                {
                var newTodo = new TodoItem ()
                    {
                    Name = "First Todo",
                    User = await context.User.FirstOrDefaultAsync (user => user.Id == userMe.Id)
                    };
                await context.TodoItem.AddAsync (newTodo);

                newTodo = new TodoItem ()
                    {
                    Name = "Second Todo",
                    User = await context.User.FirstOrDefaultAsync (user => user.Id == userMe.Id)
                    };
                await context.TodoItem.AddAsync (newTodo);

                newTodo = new TodoItem ()
                    {
                    Name = "First Random",
                    User = await context.User.FirstOrDefaultAsync (user => user.Id == userRandom.Id)
                    };
                await context.TodoItem.AddAsync (newTodo);

                newTodo = new TodoItem ()
                    {
                    Name = "Second Random",
                    User = await context.User.FirstOrDefaultAsync (user => user.Id == userRandom.Id)
                    };
                await context.TodoItem.AddAsync (newTodo);
                await context.SaveChangesAsync ();
                }

            }
        }
    }
