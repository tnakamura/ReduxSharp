using ReduxSharp;
using System.Windows;

namespace RxCounter
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        public static IStore<AppState> Store { get; }

        static App()
        {
            var reducer = new CombinedReducer<AppState>(new AppReducer());
            Store = new StoreBuilder<AppState>(reducer)
                .UseInitialState(new AppState())
                .Build();
        }
    }
}
