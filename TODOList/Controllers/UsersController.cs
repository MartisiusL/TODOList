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
        private readonly ApplicationDbContext _context;

        public UsersController (IUserService userService, UserManager<User> userManager, ApplicationDbContext context)
            {
            _userService = userService;
            _userManager = userManager;
            _context = context;
            }

        /// <summary>
        /// User authentication endpoint.
        /// </summary>
        /// <param name="model">Model for user authentication with username and password.</param>
        /// <returns>Signed in user.</returns>
        /// <response code="200">User successfully signed in.</response>
        /// <response code="401">Username or password was incorrect so user is not authorized.</response>
        [AllowAnonymous]
        [HttpPost ("authenticate")]
        public IActionResult Authenticate ([FromBody] AuthenticateModel model)
            {
            var user = _userService.Authenticate (model.Username, model.Password);

            if (user.Result == null)
                return Unauthorized (new { message = "Email or password is incorrect" });

            return Ok (user);
            }

        /// <summary>
        /// Opening endpoint which cleans the database and generates users and todos to the database.
        /// </summary>
        /// <returns>Ok result.</returns>
        /// <response code="200">Database entries successfully mocked.</response>
        [AllowAnonymous]
        [HttpGet ("generate")]
        public async Task<IActionResult> GenerateUsers ()
            {
            await MockDatabaseData ();
            return Ok ("Database Mocked");
            }

        private async Task MockDatabaseData ()
            {
            // Creates the database if not exists
            await _context.Database.EnsureDeletedAsync ();
            await _context.Database.EnsureCreatedAsync ();

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

            var newTodo = new TodoItem ()
                {
                Name = "First Todo",
                User = await _context.User.FirstOrDefaultAsync (user => user.Id == userMe.Id)
                };
            await _context.TodoItem.AddAsync (newTodo);

            newTodo = new TodoItem ()
                {
                Name = "Second Todo",
                User = await _context.User.FirstOrDefaultAsync (user => user.Id == userMe.Id)
                };
            await _context.TodoItem.AddAsync (newTodo);

            newTodo = new TodoItem ()
                {
                Name = "First Random",
                User = await _context.User.FirstOrDefaultAsync (user => user.Id == userRandom.Id)
                };
            await _context.TodoItem.AddAsync (newTodo);

            newTodo = new TodoItem ()
                {
                Name = "Second Random",
                User = await _context.User.FirstOrDefaultAsync (user => user.Id == userRandom.Id)
                };
            await _context.TodoItem.AddAsync (newTodo);
            await _context.SaveChangesAsync ();
            }
        }
    }
