namespace RxCounter
{
    public class AppState
    {
        public CounterState Counter { get; set; } = new CounterState();
    }

    public class CounterState
    {
        public int Count { get; set; } = 0;
    }
}
