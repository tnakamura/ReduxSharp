using System;
using System.ComponentModel;
using System.Reactive.Linq;

namespace RxCounter
{
    public class MainViewModel : INotifyPropertyChanged
    {
        readonly IDisposable _subscription;

        public MainViewModel()
        {
            _subscription = App.Store
                .Select(s => s.Counter)
                .Subscribe(s =>
                {
                    Count = s.Count;
                });
        }

        int count = 0;

        public int Count
        {
            get { return count; }
            set
            {
                if (count != value)
                {
                    count = value;
                    OnPropertyChanged(nameof(Count));
                }
            }
        }

        public void CountUp()
        {
            App.Store.DispatchAsync(CountUpAction.Instance);
        }

        public void CountDown()
        {
            App.Store.DispatchAsync(CountDownAction.Instance);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
