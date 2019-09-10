using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
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
            Store = new StoreBuilder<AppState>(new TodoManagerReducer())
                .UseInitialState(new AppState())
                .Build();
        }
    }
}
