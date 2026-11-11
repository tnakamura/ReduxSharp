using System.ComponentModel;
using System.Reactive.Linq;
using System;

namespace RxCounter
{
    /// <summary>Exposes application state and commands to the main window.</summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        readonly IDisposable _subscription;

        /// <summary>Subscribes the main view model to store updates.</summary>
        public MainViewModel()
        {
            _subscription = App.Store
                .Select(state => state.Counter)
                .Subscribe(counter =>
                {
                    Count = counter.Count;
                });
        }

        int _count = 0;

        /// <summary>Gets or sets the counter value.</summary>
        public int Count
        {
            get
            {
                return _count;
            }
            set
            {
                if (_count != value)
                {
                    _count = value;
                    OnPropertyChanged(nameof(Count));
                }
            }
        }

        /// <summary>Dispatches an increment action.</summary>
        public void CountUp()
        {
            App.Store.Dispatch(CountUpAction.Instance);
        }

        /// <summary>Dispatches a decrement action.</summary>
        public void CountDown()
        {
            App.Store.Dispatch(CountDownAction.Instance);
        }

        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
