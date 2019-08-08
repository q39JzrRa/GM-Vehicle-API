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
        public GMClient GMClient { get; set; }

        public LoginWindow()
        {
            InitializeComponent();

            //txtUsername.Text = Properties.Settings.Default.

        }

        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            if (GMClient == null)
            {
                GMClient = helpers.CreateClient();
            }

            var success = await GMClient.Login(txtUsername.Text, txtPassword.Password);

            if (success)
            {
                this.Close();
                return;
            }

            MessageBox.Show("Login failed");

        }
    }
}
