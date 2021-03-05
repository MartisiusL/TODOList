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
            var myTodos = _todosService.GetMyTodos (out var errorMessage);
            if (errorMessage != null)
                return BadRequest (new { message = errorMessage });
            return Ok (myTodos);
            }

        [Authorize (Roles = Role.User)]
        [HttpDelete ("remove")]
        public IActionResult RemoveTodo ([FromBody] TodoModel todo)
            {
            _todosService.RemoveMyTodo (todo.Id, out var errorMessage);
            if (errorMessage != null)
                return BadRequest (new { message = errorMessage });
            return Ok ();
            }

        [Authorize (Roles = Role.User)]
        [HttpPost ("add")]
        public IActionResult AddTodo ([FromBody] TodoModel todo)
            {
            _todosService.AddTodo (todo.Name, out var errorMessage);
            if (errorMessage != null)
                return BadRequest (new { message = errorMessage });
            return Ok ();
            }

        [Authorize (Roles = Role.User)]
        [HttpPut ("update")]
        public IActionResult UpdateTodo ([FromBody] TodoModel todo)
            {
            _todosService.UpdateTodo (todo, out var errorMessage);
            if (errorMessage != null)
                return BadRequest (new { message = errorMessage });
            return Ok ();
            }
        }
    }
