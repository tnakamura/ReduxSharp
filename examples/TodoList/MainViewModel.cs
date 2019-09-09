using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Windows.Input;
using ReduxSharp;

namespace TodoList
{
    public class MainViewModel : ViewModel, IObserver<AppState>
    {
        IDisposable _subscription;

        public MainViewModel()
        {
            _subscription = App.Store.Subscribe(this);
        }

        string _newTodo;

        public string NewTodo
        {
            get { return _newTodo; }
            set { SetProperty(ref _newTodo, value); }
        }

        public ObservableCollection<TodoViewModel> Todos { get; } = new ObservableCollection<TodoViewModel>();

        ICommand _addCommand;

        public ICommand AddCommand
        {
            get
            {
                return _addCommand = _addCommand
                    ?? new DelegateCommand(
                        _ =>
                        {
                            App.Store.DispatchAsync(new AddTodoAction(NewTodo));
                            NewTodo = string.Empty;
                        },
                        _ => !string.IsNullOrEmpty(NewTodo));
            }
        }

        public void OnNext(AppState value)
        {
            // TODO: Update only the difference.
            Todos.Clear();
            foreach(var todo in value.TodoManager.Todos)
            {
                Todos.Add(new TodoViewModel()
                {
                    Id = todo.Id,
                    Text = todo.Text,
                });
            }
        }

        public void OnError(Exception error)
        {
        }

        public void OnCompleted()
        {
        }
    }

    public class TodoViewModel : ViewModel
    {
        public TodoViewModel()
        {
        }

        public string Id { get; set; }

        string _text;

        public string Text
        {
            get { return _text; }
            set { SetProperty(ref _text, value); }
        }

        ICommand _completeCommand;

        public ICommand CompleteCommand
        {
            get
            {
                return _completeCommand = _completeCommand
                    ?? new DelegateCommand(_ =>
                    {
                        App.Store.DispatchAsync(new CompleteTodoAction(Id));
                    });
            }
        }

        ICommand _deleteCommand;

        public ICommand DeleteCommand
        {
            get
            {
                return _deleteCommand = _deleteCommand
                    ?? new DelegateCommand(_ =>
                    {
                        App.Store.DispatchAsync(new DeleteTodoAction(Id));
                    });
            }
        }
    }

    public class ViewModel : INotifyPropertyChanged
    {
        protected ViewModel() { }

        protected bool SetProperty<T>(ref T store, T value, [CallerMemberName]string propertyName = null)
        {
            if (!EqualityComparer<T>.Default.Equals(store, value))
            {
                store = value;
                OnPropertyChanged(propertyName);
                return true;
            }
            return false;
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    public class DelegateCommand : ICommand
    {
        Action<object> _execute;

        Func<object, bool> _canExecute;

        public DelegateCommand(Action<object> execute)
            : this(execute, _ => true)
        {
        }

        public DelegateCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute(parameter);
        }

        public void Execute(object parameter)
        {
            _execute(parameter);
        }
    }
}
