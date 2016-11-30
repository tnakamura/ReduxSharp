using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReduxSharp;

namespace TodoList
{
    public class AppState
    {
        public TodoManager TodoManager { get; set; } = new TodoManager();
    }

    public class Todo
    {
        public Todo()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string Id { get; set; }

        public bool Completed { get; set; }

        public string Text { get; set; }
    }

    public class TodoManager
    {
        public List<Todo> Todos { get; set; } = new List<Todo>();
    }

    public class AddTodoAction : IAction
    {
        public AddTodoAction(string todo)
        {
            Todo = todo;
        }

        public string Todo { get; }
    }

    public class CompleteTodoAction : IAction
    {
        public CompleteTodoAction(string todoId)
        {
            TodoId = todoId;
        }

        public string TodoId { get; }
    }

    public class DeleteTodoAction : IAction
    {
        public DeleteTodoAction(string todoId)
        {
            TodoId = todoId;
        }

        public string TodoId { get; }
    }

    public class TodoManagerReducer : IReducer<AppState>
    {
        public AppState Invoke(AppState state, IAction action)
        {
            if (action is AddTodoAction)
            {
                state.TodoManager = AddTodo(state.TodoManager, (AddTodoAction)action);
            }
            else if (action is CompleteTodoAction)
            {
                state.TodoManager = CompleteTodo(state.TodoManager, (CompleteTodoAction)action);
            }
            else if (action is DeleteTodoAction)
            {
                state.TodoManager = DeleteTodo(state.TodoManager, (DeleteTodoAction)action);
            }

            return state;
        }

        private TodoManager AddTodo(TodoManager state, AddTodoAction action)
        {
            var newTodos = new List<Todo>(state.Todos);
            newTodos.Add(new Todo() { Text = action.Todo });
            return new TodoManager()
            {
                Todos = newTodos,
            };
        }

        private TodoManager CompleteTodo(TodoManager state, CompleteTodoAction action)
        {
            var newTodos = state.Todos.Select(t => new Todo
            {
                Id = t.Id,
                Text = t.Text,
                Completed = t.Id == action.TodoId ? !t.Completed : t.Completed,
            }).ToList();
            return new TodoManager()
            {
                Todos = newTodos,
            };
        }

        private TodoManager DeleteTodo(TodoManager state, DeleteTodoAction action)
        {
            var newTodos = state.Todos.Where(t => t.Id != action.TodoId).ToList();
            return new TodoManager()
            {
                Todos = newTodos,
            };
        }
    }
}
