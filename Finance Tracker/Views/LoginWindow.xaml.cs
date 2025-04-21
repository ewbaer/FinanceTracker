using System.Windows;
using FinanceTracker.ViewModels;

namespace FinanceTracker.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            var vm = new LoginViewModel();
            if (vm.Authenticate(UsernameBox.Text, PasswordBox.Password))
            {
                var main = new MainWindow(UsernameBox.Text);
                main.Show();
                this.Close(); 
            }
            else
            {
                MessageBox.Show("Invalid login.");
            }
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                Login_Click(sender, e);
            }
        }
    }
}
