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

        /// <summary>
        /// Gets todos list for selected user.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <returns>Todos list.</returns>
        /// <response code="200">Todos successfully found.</response>
        /// <response code="401">User is not authorized.</response>
        [HttpGet ("todos/user")]
        public IActionResult GetUserTodoList ([FromQuery] string userId)
            {
            return Ok (_todosService.GetUserTodoList (userId));
            }

        /// <summary>
        /// Removed selected todoItem.
        /// </summary>
        /// <param name="todoId">TodoItem id.</param>
        /// <returns>Ok response.</returns>
        /// <response code="200">TodoItem successfully deleted.</response>
        /// <response code="401">User is not authorized.</response>
        [HttpDelete ("todos/remove")]
        public IActionResult RemoveUserTodo ([FromQuery] int todoId)
            {
            _todosService.RemoveTodo (todoId);
            return Ok ();
            }
        }
    }
