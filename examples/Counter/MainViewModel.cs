using System;
using System.ComponentModel;

namespace Counter
{
    public class MainViewModel : IObserver<AppState>, INotifyPropertyChanged
    {
        public MainViewModel()
        {
            App.Store.Subscribe(this);
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
            App.Store.Dispatch(CountUpAction.Instance);
        }

        public void CountDown()
        {
            App.Store.Dispatch(CountDownAction.Instance);
        }

        public void OnCompleted()
        {
            // 何もしない
        }

        public void OnError(Exception error)
        {
            // 何もしない
        }

        public void OnNext(AppState value)
        {
            Count = value.Counter.Count;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
