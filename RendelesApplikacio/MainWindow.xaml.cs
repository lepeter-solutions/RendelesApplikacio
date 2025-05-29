using System.Windows;
using System.Windows.Navigation;

namespace OrderManagementApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MainFrame.Navigate(new MainPage());
        }
    }
}