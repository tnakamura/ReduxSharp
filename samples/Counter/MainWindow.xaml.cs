using System.Windows;

namespace Counter
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            ViewModel = new MainViewModel();
        }

        MainViewModel ViewModel
        {
            get { return (MainViewModel)DataContext; }
            set { DataContext = value; }
        }

        void OnCountUp(object sender, RoutedEventArgs e)
        {
            ViewModel.CountUp();
        }

        void OnCountDown(object sender, RoutedEventArgs e)
        {
            ViewModel.CountDown();
        }
    }
}
