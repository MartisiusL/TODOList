using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TODOList.Entities;
using TODOList.Services;

namespace TODOList.Controllers
    {
    [Authorize (AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Role.Admin)]
    [ApiController]
    [Route ("[controller]")]
    public class AdminController : ControllerBase
        {
        private readonly ITodoService _todosService;

        public AdminController (ITodoService todosService)
            {
            _todosService = todosService;
            }

        [HttpGet ("todos/user")]
        public IActionResult GetUserTodoList ([FromQuery] string userId)
            {
            return Ok (_todosService.GetUserTodoList (userId));
            }

        [HttpDelete ("todos/remove")]
        public IActionResult RemoveUserTodo ([FromQuery] int todoId)
            {
            _todosService.RemoveTodo (todoId);
            return Ok ();
            }
        }
    }
