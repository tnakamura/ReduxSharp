using System.Windows;

namespace RxCounter
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

            ViewModel = new MainViewModel();
        }

        MainViewModel ViewModel
        {
            get
            {
                return (MainViewModel)DataContext;
            }
            set
            {
                DataContext = value;
            }
        }

        void OnCountUp(object sender, RoutedEventArgs eventArgs)
        {
            ViewModel.CountUp();
        }

        void OnCountDown(object sender, RoutedEventArgs eventArgs)
        {
            ViewModel.CountDown();
        }
    }
}
