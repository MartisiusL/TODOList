using System;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TODOList.Entities;
using TODOList.Models;
using TODOList.Services;

namespace TODOList.Controllers
    {
    [Authorize (AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = Role.User)]
    [ApiController]
    [Route ("[controller]")]
    public class TodosController : ControllerBase
        {
        private readonly ITodoService _todosService;

        public TodosController (ITodoService todosService)
            {
            _todosService = todosService;
            }

        [HttpGet ("my")]
        public IActionResult GetMyTodos ()
            {
            var myTodos = _todosService.GetMyTodos (out var errorMessage);
            if (errorMessage != null)
                return BadRequest (new { message = errorMessage });
            return Ok (myTodos);
            }

        [HttpDelete ("remove")]
        public IActionResult RemoveTodo ([FromQuery] int todoId)
            {
            _todosService.RemoveMyTodo (todoId, out var errorMessage);
            if (errorMessage != null)
                return BadRequest (new { message = errorMessage });
            return Ok ();
            }

        [HttpPost ("add")]
        public IActionResult AddTodo ([FromQuery] string todoName)
            {
            if (String.IsNullOrEmpty (todoName))
                {
                return BadRequest (new { message = "Name cannot be null" });
                }
            _todosService.AddTodo (todoName, out var errorMessage);
            if (errorMessage != null)
                return BadRequest (new { message = errorMessage });
            return Ok ();
            }

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
