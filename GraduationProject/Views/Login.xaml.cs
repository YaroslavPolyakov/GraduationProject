using System.Windows;

namespace GraduationProject.Views
{
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var mainWnd = new MainWindow();
            mainWnd.Show();
            this.Close();
        }
    }
}