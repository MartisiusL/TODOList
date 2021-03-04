using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TODOList.Entities;
using TODOList.Services;

namespace TODOList.Controllers
    {
    [Authorize]
    [ApiController]
    [Route ("[controller]")]
    public class AdminController : ControllerBase
        {
        private readonly ITodoService _todosService;

        public AdminController (ITodoService todosService)
            {
            _todosService = todosService;
            }

        [Authorize (Roles = Role.Admin)]
        [HttpGet ("todos/user")]
        public IActionResult GetUserTodoList ([FromQuery] int userId)
            {
            return Ok (_todosService.GetUserTodoList (userId));
            }

        [Authorize (Roles = Role.Admin)]
        [HttpDelete ("todos/remove")]
        public IActionResult RemoveUserTodo ([FromQuery] int todoId)
            {
            _todosService.RemoveTodo (todoId);
            return Ok ();
            }
        }
    }
