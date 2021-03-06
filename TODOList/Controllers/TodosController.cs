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

        /// <summary>
        /// Gets todos list for authorized user.
        /// </summary>
        /// <returns>Todos list.</returns>
        /// <response code="200">Todos successfully found.</response>
        /// <response code="400">User not found.</response>
        /// <response code="401">User is not authorized.</response>
        [HttpGet ("my")]
        public IActionResult GetMyTodos ()
            {
            var myTodos = _todosService.GetMyTodos (out var errorMessage);
            if (errorMessage != null)
                return BadRequest (new { message = errorMessage });
            return Ok (myTodos);
            }

        /// <summary>
        /// Removes todoItem for authorized user.
        /// </summary>
        /// <param name="todoId">Id of the todoItem to delete.</param>
        /// <returns>Ok result.</returns>
        /// <response code="200">TodoItem successfully removed.</response>
        /// <response code="400">TodoItem with given id not found.</response>
        /// <response code="401">User is not authorized.</response>
        [HttpDelete ("remove")]
        public IActionResult RemoveTodo ([FromQuery] int todoId)
            {
            _todosService.RemoveMyTodo (todoId, out var errorMessage);
            if (errorMessage != null)
                return BadRequest (new { message = errorMessage });
            return Ok ();
            }

        /// <summary>
        /// Creates a new todoItem for the authorized user.
        /// </summary>
        /// <param name="todoName">Name of new todoItem.</param>
        /// <returns>Ok result.</returns>
        /// <response code="200">TodoItem successfully added.</response>
        /// <response code="400">Bad request with an error.</response>
        /// <response code="401">User is not authorized.</response>
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

        /// <summary>
        /// Updates todoItem for authorized user.
        /// </summary>
        /// <param name="todoModel">Model of todoItem to update.</param>
        /// <returns>Ok result.</returns>
        /// <response code="200">TodoItem successfully updated.</response>
        /// <response code="400">Bad request with an error.</response>
        /// <response code="401">User is not authorized.</response>
        [HttpPut ("update")]
        public IActionResult UpdateTodo ([FromBody] TodoModel todoModel)
            {
            _todosService.UpdateTodo (todoModel, out var errorMessage);
            if (errorMessage != null)
                return BadRequest (new { message = errorMessage });
            return Ok ();
            }
        }
    }
