using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System;

namespace TodoList
{
    /// <summary>Exposes application state and commands to the main window.</summary>
    public class MainViewModel : ViewModel, IObserver<AppState>
    {
        IDisposable _subscription;

        /// <summary>Subscribes the main view model to store updates.</summary>
        public MainViewModel()
        {
            _subscription = App.Store.Subscribe(this);
        }

        string _newTodo;

        /// <summary>Gets or sets the text of the todo to add.</summary>
        public string NewTodo
        {
            get
            {
                return _newTodo;
            }
            set
            {
                SetProperty(ref _newTodo, value);
            }
        }

        /// <summary>Gets the todo items.</summary>
        public ObservableCollection<TodoViewModel> Todos { get; } = new ObservableCollection<TodoViewModel>();

        ICommand _addCommand;

        /// <summary>Gets the command that adds a todo.</summary>
        public ICommand AddCommand
        {
            get
            {
                return _addCommand = _addCommand
                    ?? new DelegateCommand(
                        _ =>
                        {
                            App.Store.Dispatch(new AddTodoAction(NewTodo));
                            NewTodo = string.Empty;
                        },
                        _ => !string.IsNullOrEmpty(NewTodo));
            }
        }

        /// <inheritdoc/>
        public void OnNext(AppState value)
        {
            // TODO: Update only the difference.
            Todos.Clear();
            foreach (Todo todo in value.TodoManager.Todos)
            {
                Todos.Add(new TodoViewModel
                {
                    Id = todo.Id,
                    Text = todo.Text,
                });
            }
        }

        /// <inheritdoc/>
        public void OnError(Exception error)
        {
        }

        /// <inheritdoc/>
        public void OnCompleted()
        {
        }
    }

    /// <summary>Exposes a todo item and its commands to the view.</summary>
    public class TodoViewModel : ViewModel
    {
        /// <summary>Creates a view model for a todo item.</summary>
        public TodoViewModel()
        {
        }

        /// <summary>Gets or sets the todo identifier.</summary>
        public string Id { get; set; }

        string _text;

        /// <summary>Gets or sets the todo text.</summary>
        public string Text
        {
            get
            {
                return _text;
            }
            set
            {
                SetProperty(ref _text, value);
            }
        }

        ICommand _completeCommand;

        /// <summary>Gets the command that toggles todo completion.</summary>
        public ICommand CompleteCommand
        {
            get
            {
                return _completeCommand = _completeCommand
                    ?? new DelegateCommand(_ =>
                    {
                        App.Store.Dispatch(new CompleteTodoAction(Id));
                    });
            }
        }

        ICommand _deleteCommand;

        /// <summary>Gets the command that deletes the todo.</summary>
        public ICommand DeleteCommand
        {
            get
            {
                return _deleteCommand = _deleteCommand
                    ?? new DelegateCommand(_ =>
                    {
                        App.Store.Dispatch(new DeleteTodoAction(Id));
                    });
            }
        }
    }

    /// <summary>Provides property change notifications for view models.</summary>
    public class ViewModel : INotifyPropertyChanged
    {
        /// <summary>Initializes property change notification support.</summary>
        protected ViewModel()
        {
        }

        /// <summary>Updates a property backing field and raises a change notification if its value differs.</summary>
        protected bool SetProperty<T>(ref T store, T value, [CallerMemberName] string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(store, value))
            {
                store = value;
                OnPropertyChanged(propertyName);
                return true;
            }
            return false;
        }

        /// <summary>Raises the property change notification for the supplied name.</summary>
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;
    }

    /// <summary>Invokes callbacks to execute a command and determine its availability.</summary>
    public class DelegateCommand : ICommand
    {
        Action<object> _execute;

        Func<object, bool> _canExecute;

        /// <summary>Creates a command that always permits execution.</summary>
        public DelegateCommand(Action<object> execute)
            : this(execute, _ => true)
        {
        }

        /// <summary>Creates a command with execution and availability callbacks.</summary>
        public DelegateCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>Occurs when command availability may have changed.</summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
            }
            remove
            {
                CommandManager.RequerySuggested -= value;
            }
        }

        /// <inheritdoc/>
        public bool CanExecute(object parameter)
        {
            return _canExecute(parameter);
        }

        /// <inheritdoc/>
        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}
