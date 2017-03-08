using System;
using System.ComponentModel;

namespace RxCounter
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            App.Store.Subscribe(state =>
            {
                Count = state.Counter.Count;
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
            App.Store.Dispatch(CountUpAction.Instance);
        }

        public void CountDown()
        {
            App.Store.Dispatch(CountDownAction.Instance);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
