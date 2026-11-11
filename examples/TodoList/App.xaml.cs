using System.Windows;
using ReduxSharp;

namespace TodoList
{
    /// <summary>
    /// Interaction logic for the application.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>Gets the shared application store.</summary>
        public static IStore<AppState> Store { get; }

        static App()
        {
            Store = new StoreBuilder<AppState>(new AppState()).Build(new TodoManagerReducer());
        }
    }
}
