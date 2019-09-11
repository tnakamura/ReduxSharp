using System.Windows;
using ReduxSharp;

namespace TodoList
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        public static IStore<AppState> Store { get; }

        static App()
        {
            Store = new Store<AppState>(new TodoManagerReducer(), new AppState());
        }
    }
}
