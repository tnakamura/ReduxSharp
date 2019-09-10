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
            Store = new StoreBuilder<AppState>(new AppReducer())
                .UseInitialState(new AppState())
                .Build();
        }
    }
}
