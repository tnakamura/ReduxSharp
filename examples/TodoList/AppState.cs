using System.Collections.Generic;
using System.Linq;
using System;
using ReduxSharp;

namespace TodoList
{
    /// <summary>Holds the application state.</summary>
    public class AppState
    {
        /// <summary>Gets or sets the todo collection state.</summary>
        public TodoManager TodoManager { get; set; } = new TodoManager();
    }

    /// <summary>Holds a todo item.</summary>
    public class Todo
    {
        /// <summary>Assigns a new identifier to the todo item.</summary>
        public Todo()
        {
            Id = Guid.NewGuid().ToString();
        }

        /// <summary>Gets or sets the todo identifier.</summary>
        public string Id { get; set; }

        /// <summary>Gets or sets whether the todo is complete.</summary>
        public bool Completed { get; set; }

        /// <summary>Gets or sets the todo text.</summary>
        public string Text { get; set; }
    }

    /// <summary>Holds the todo collection.</summary>
    public class TodoManager
    {
        /// <summary>Gets or sets the todo items.</summary>
        public List<Todo> Todos { get; set; } = new List<Todo>();
    }

    /// <summary>Adds a todo item.</summary>
    public class AddTodoAction
    {
        /// <summary>Creates an action with the text of the todo to add.</summary>
        public AddTodoAction(string todo)
        {
            Todo = todo;
        }

        /// <summary>Gets the text of the new todo.</summary>
        public string Todo { get; }
    }

    /// <summary>Toggles the completion of a todo item.</summary>
    public class CompleteTodoAction
    {
        /// <summary>Creates an action targeting the todo to toggle.</summary>
        public CompleteTodoAction(string todoId)
        {
            TodoId = todoId;
        }

        /// <summary>Gets the target todo identifier.</summary>
        public string TodoId { get; }
    }

    /// <summary>Deletes a todo item.</summary>
    public class DeleteTodoAction
    {
        /// <summary>Creates an action targeting the todo to delete.</summary>
        public DeleteTodoAction(string todoId)
        {
            TodoId = todoId;
        }

        /// <summary>Gets the target todo identifier.</summary>
        public string TodoId { get; }
    }

    /// <summary>Computes the next todo state from dispatched actions.</summary>
    public class TodoManagerReducer : IReducer<AppState>
    {
        /// <inheritdoc/>
        public AppState Invoke<TAction>(AppState state, TAction action)
        {
            switch (action)
            {
                case AddTodoAction addTodoAction:
                    return new AppState
                    {
                        TodoManager = AddTodo(state.TodoManager, addTodoAction)
                    };
                case CompleteTodoAction completeTodoAction:
                    return new AppState
                    {
                        TodoManager = CompleteTodo(state.TodoManager, completeTodoAction),
                    };
                case DeleteTodoAction deleteTodoAction:
                    return new AppState
                    {
                        TodoManager = DeleteTodo(state.TodoManager, deleteTodoAction),
                    };
            }
            return state;
        }

        static TodoManager AddTodo(TodoManager state, AddTodoAction action)
        {
            var newTodos = new List<Todo>(state.Todos);
            newTodos.Add(new Todo
            {
                Text = action.Todo,
            });
            return new TodoManager
            {
                Todos = newTodos,
            };
        }

        static TodoManager CompleteTodo(TodoManager state, CompleteTodoAction action)
        {
            var newTodos = state.Todos.Select(todo => new Todo
            {
                Id = todo.Id,
                Text = todo.Text,
                Completed = todo.Id == action.TodoId ? !todo.Completed : todo.Completed,
            }).ToList();
            return new TodoManager
            {
                Todos = newTodos,
            };
        }

        static TodoManager DeleteTodo(TodoManager state, DeleteTodoAction action)
        {
            var newTodos = state.Todos.Where(todo => todo.Id != action.TodoId).ToList();
            return new TodoManager
            {
                Todos = newTodos,
            };
        }
    }
}
