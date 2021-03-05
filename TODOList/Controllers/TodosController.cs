using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TODOList.Entities;
using TODOList.Models;
using TODOList.Services;

namespace TODOList.Controllers
    {
    [Authorize (AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [ApiController]
    [Route ("[controller]")]
    public class TodosController : ControllerBase
        {
        private readonly ITodoService _todosService;

        public TodosController (ITodoService todosService)
            {
            _todosService = todosService;
            }

        [Authorize (Roles = Role.User)]
        [HttpGet ("my")]
        public IActionResult GetMyTodos ()
            {
            return Ok (_todosService.GetMyTodos ());
            }

        [Authorize (Roles = Role.User)]
        [HttpDelete ("remove")]
        public IActionResult RemoveTodo ([FromBody] TodoModel todo)
            {
            _todosService.RemoveMyTodo (todo.Id);
            return Ok ();
            }

        [Authorize (Roles = Role.User)]
        [HttpPost ("add")]
        public IActionResult AddTodo ([FromBody] TodoModel todo)
            {
            _todosService.AddTodo (todo.Name);
            return Ok ();
            }

        [Authorize (Roles = Role.User)]
        [HttpPost ("update")]
        public IActionResult UpdateTodo ([FromBody] TodoModel todo)
            {
            _todosService.UpdateTodo (todo);
            return Ok ();
            }
        }
    }
