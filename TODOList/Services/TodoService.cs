using System;
using System.Collections.Generic;
using System.Linq;
using TODOList.Entities;
using TODOList.Models;

namespace TODOList.Services
    {
    public interface ITodoService
        {
        public void AddTodo (string todoName, out string errorMessage);
        public void RemoveMyTodo (int todoId, out string errorMessage);
        public IEnumerable<TodoItem> GetMyTodos (out string errorMessage);
        public void UpdateTodo (TodoModel todo, out string errorMessage);

        public IEnumerable<TodoItem> GetUserTodoList (string userId);
        public void RemoveTodo (int todoId);
        }
    public class TodoService: ITodoService
        {
        private readonly IUserService _userService;
        private readonly ApplicationDbContext _context;

        public TodoService(IUserService userService, ApplicationDbContext context)
        {
        _userService = userService;
        _context = context;
        }

        public void AddTodo(string todoName, out string errorMessage)
            {
            errorMessage = null;
            var user = _userService.GetCachedUser ();
            if (user is null)
                {
                errorMessage = "User is not authorized";
                return;
                }
            var newTodo = new TodoItem ()
                {
                Name = todoName,
                User = _context.User.FirstOrDefault (u => u.Id == user.Id),
                IsDone = false
                };
            _context.TodoItem.Add (newTodo);

            _context.SaveChanges ();
            }

        public void RemoveMyTodo (int todoId, out string errorMessage)
            {
            errorMessage = null;
            var user = _userService.GetCachedUser ();
            if (user is null)
                {
                errorMessage = "User is not authorized";
                return;
                }
            var todoToRemove = new TodoItem () {Id = todoId, User = new User () {Id = user.Id}};
            //_context.TodoItem.Attach (todoToRemove);
            _context.TodoItem.Remove (todoToRemove);
            TrySaveContext (_context);
            }

        public IEnumerable<TodoItem> GetMyTodos (out string errorMessage)
            {
            errorMessage = null;
            var user = _userService.GetCachedUser ();
            if (user is null)
                {
                errorMessage = "User is not authorized";
                return null;
                }
            var myTodos = _context.TodoItem.Where (u => u.User.Id == user.Id).ToList ();
            foreach (var todo in myTodos)
                {
                todo.User = null;
                }

            return myTodos;
            }

        public void UpdateTodo (TodoModel todoModel, out string errorMessage)
            {
            errorMessage = null;
            var user = _userService.GetCachedUser ();
            if (user is null)
                {
                errorMessage = "User is not authorized.";
                return;
                }
            var todoToUpdate = _context.TodoItem.FirstOrDefault
                (todo => todo.Id == todoModel.Id && todo.User.Id == user.Id);
            if (todoToUpdate is null)
                {
                errorMessage = "Todo not found.";
                return;
                }
            if (todoModel.Name != null)
                {
                todoToUpdate.Name = todoModel.Name;
                }
            if (todoModel.ChangeIsDone)
                {
                todoToUpdate.IsDone = !todoToUpdate.IsDone;
                }
            _context.SaveChanges ();
            }

        public IEnumerable<TodoItem> GetUserTodoList (string userId)
            {
            return _context.TodoItem.Where (u => u.User.Id == userId).ToList ();
            }

        public void RemoveTodo (int todoId)
            {
            var todoToRemove = new TodoItem () { Id = todoId };
            _context.TodoItem.Attach (todoToRemove);
            _context.TodoItem.Remove (todoToRemove);
            TrySaveContext (_context);
            }

        private void TrySaveContext (ApplicationDbContext context)
            {
            try
                {
                context.SaveChanges ();
                }
            catch (Exception ex)
                {

                }
            }
        }
    }
