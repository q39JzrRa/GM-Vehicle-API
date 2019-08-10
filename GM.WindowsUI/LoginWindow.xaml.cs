using GM.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace GM.WindowsUI
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        GMClientBase _client;

        public bool Success { get; private set; } = false;

        public LoginWindow(GMClientBase client)
        {
            _client = client;
            InitializeComponent();
        }

        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            var success = await _client.Login(txtUsername.Text, txtPassword.Password);

            if (success)
            {
                Success = true;
                this.Close();
                return;
            }

            MessageBox.Show("Login failed");

        }
    }
}
