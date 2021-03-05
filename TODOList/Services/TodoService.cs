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

        public TodoService(IUserService userService)
        {
        _userService = userService;
        }

        public void AddTodo(string todoName, out string errorMessage)
            {
            errorMessage = null;
            using (var context = new ApplicationDbContext ())
                {
                var user = _userService.GetCachedUser ();
                if (user is null)
                    {
                    errorMessage = "User is not authorized";
                    return;
                    }
                var newTodo = new TodoItem ()
                    {
                    Name = todoName,
                    User = context.User.FirstOrDefault (u => u.Id == user.Id),
                    IsDone = false
                    };
                context.TodoItem.Add (newTodo);

                context.SaveChanges ();
                }
            }

        public void RemoveMyTodo (int todoId, out string errorMessage)
            {
            errorMessage = null;
            using (var context = new ApplicationDbContext ())
                {
                var user = _userService.GetCachedUser ();
                if (user is null)
                    {
                    errorMessage = "User is not authorized";
                    return;
                    }
                var todoToRemove = new TodoItem () {Id = todoId, User = new User () {Id = user.Id}};
                context.TodoItem.Attach (todoToRemove);
                context.TodoItem.Remove (todoToRemove);
                context.SaveChanges ();
                }
            }

        public IEnumerable<TodoItem> GetMyTodos (out string errorMessage)
            {
            errorMessage = null;
            using (var context = new ApplicationDbContext ())
                {
                var user = _userService.GetCachedUser ();
                if (user is null)
                    {
                    errorMessage = "User is not authorized";
                    return null;
                    }
                return context.TodoItem.Where (u => u.User.Id == user.Id).ToList ();
                }
            }

        public void UpdateTodo (TodoModel todoModel, out string errorMessage)
            {
            errorMessage = null;
            using (var context = new ApplicationDbContext ())
                {
                var user = _userService.GetCachedUser ();
                if (user is null)
                    {
                    errorMessage = "User is not authorized";
                    return;
                    }
                var todoToUpdate = context.TodoItem.FirstOrDefault
                    (todo => todo.Id == todoModel.Id && todo.User.Id == user.Id);
                if (todoToUpdate != null)
                    {
                    if (todoModel.Name != null)
                        {
                        todoToUpdate.Name = todoModel.Name;
                        }
                    if (todoModel.ChangeIsDone)
                        {
                        todoToUpdate.IsDone = !todoToUpdate.IsDone;
                        }
                    context.SaveChanges ();
                    }
                }
            }

        public IEnumerable<TodoItem> GetUserTodoList (string userId)
            {
            using (var context = new ApplicationDbContext ())
                {
                return context.TodoItem.Where (u => u.User.Id == userId).ToList ();
                }
            }

        public void RemoveTodo (int todoId)
            {
            using (var context = new ApplicationDbContext ())
                {
                var todoToRemove = new TodoItem () { Id = todoId };
                context.TodoItem.Attach (todoToRemove);
                context.TodoItem.Remove (todoToRemove);
                context.SaveChanges ();
                }
            }
        }
    }
