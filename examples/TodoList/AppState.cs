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

    public class AddTodoAction
    {
        public AddTodoAction(string todo)
        {
            Todo = todo;
        }

        public string Todo { get; }
    }

    public class CompleteTodoAction
    {
        public CompleteTodoAction(string todoId)
        {
            TodoId = todoId;
        }

        public string TodoId { get; }
    }

    public class DeleteTodoAction
    {
        public DeleteTodoAction(string todoId)
        {
            TodoId = todoId;
        }

        public string TodoId { get; }
    }

    public class TodoManagerReducer : IReducer<AppState>
    {
        public AppState Invoke<TAction>(AppState state, TAction action)
        {
            switch (action)
            {
                case AddTodoAction a:
                    return new AppState
                    {
                        TodoManager = AddTodo(state.TodoManager, a)
                    };
                case CompleteTodoAction a:
                    return new AppState
                    {
                        TodoManager = CompleteTodo(state.TodoManager, a),
                    };
                case DeleteTodoAction a:
                    return new AppState
                    {
                        TodoManager = DeleteTodo(state.TodoManager, a),
                    };
            }
            return state;
        }

        static TodoManager AddTodo(TodoManager state, AddTodoAction action)
        {
            var newTodos = new List<Todo>(state.Todos);
            newTodos.Add(new Todo() { Text = action.Todo });
            return new TodoManager()
            {
                Todos = newTodos,
            };
        }

        static TodoManager CompleteTodo(TodoManager state, CompleteTodoAction action)
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

        static TodoManager DeleteTodo(TodoManager state, DeleteTodoAction action)
        {
            var newTodos = state.Todos.Where(t => t.Id != action.TodoId).ToList();
            return new TodoManager()
            {
                Todos = newTodos,
            };
        }
    }
}
