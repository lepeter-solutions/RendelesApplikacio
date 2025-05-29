using System.Windows;
using System.Windows.Navigation;

namespace OrderManagementApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Load the MainPage into the Frame when the application starts
            MainFrame.Navigate(new MainPage());
        }
    }
}