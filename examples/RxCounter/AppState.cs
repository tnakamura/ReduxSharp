namespace RxCounter
{
    /// <summary>Holds the application state.</summary>
    public class AppState
    {
        /// <summary>Gets or sets the counter state.</summary>
        public CounterState Counter { get; set; } = new CounterState();
    }

    /// <summary>Holds the counter value.</summary>
    public class CounterState
    {
        /// <summary>Gets or sets the counter value.</summary>
        public int Count { get; set; } = 0;
    }
}
