using System.Collections.Generic;
using System.Linq;
using TODOList.Entities;
using TODOList.Models;

namespace TODOList.Services
    {
    public interface ITodoService
        {
        public void AddTodo (string todoName);
        public void RemoveTodo (int todoId);
        public IEnumerable<TodoItem> GetAll ();
        public void UpdateTodo (TodoModel todo);
        }
    public class TodoService: ITodoService
        {
        private readonly IUserService _userService;

        public TodoService(IUserService userService)
        {
        _userService = userService;
        }

        public void AddTodo(string todoName)
            {
            using (var context = new TodosContext ())
                {
                var newTodo = new TodoItem ()
                    {
                    Name = todoName,
                    User = context.User.FirstOrDefault (user => user.Id == _userService.GetCachedUser ().Id),
                    IsDone = false
                    };
                context.TodoItem.Add (newTodo);

                context.SaveChanges ();
                }
            }

        public void RemoveTodo (int todoId)
            {
            using (var context = new TodosContext ())
                {
                var todoToRemove = new TodoItem () {Id = todoId};
                context.TodoItem.Attach (todoToRemove);
                context.TodoItem.Remove (todoToRemove);
                context.SaveChanges ();
                }
            }

        public IEnumerable<TodoItem> GetAll ()
            {
            using (var context = new TodosContext ())
                {
                return context.TodoItem.Where (u => u.User.Id == _userService.GetCachedUser ().Id).ToList ();
                }
            }

        public void UpdateTodo (TodoModel todoModel)
            {
            using (var context = new TodosContext ())
                {
                var todoToUpdate = context.TodoItem.FirstOrDefault
                    (todo => todo.Id == todoModel.Id && todo.User.Id == _userService.GetCachedUser ().Id);
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
        }
    }
