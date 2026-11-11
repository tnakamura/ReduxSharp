using System.Windows;

namespace TodoList
{
    /// <summary>
    /// Interaction logic for the main window.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>Initializes the window and its view model.</summary>
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainViewModel();
        }
    }
}
