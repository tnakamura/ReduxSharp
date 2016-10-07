using System.Windows;

namespace RxCounter
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

        private MainViewModel ViewModel
        {
            get { return (MainViewModel)DataContext; }
            set { DataContext = value; }
        }

        private void OnCountUp(object sender, RoutedEventArgs e)
        {
            ViewModel.CountUp();
        }

        private void OnCountDown(object sender, RoutedEventArgs e)
        {
            ViewModel.CountDown();
        }
    }
}
