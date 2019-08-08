using GM.Api;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GM.WindowsUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        GMClient _client;

        GM.Api.GmConfiguration _globalConfig;

        GM.Api.ApiConfig _apiConfig;
        GM.Api.BrandClientInfo _clientCredentials;

        string _brand;
        string _brandDisplay;

        public MainWindow()
        {
            InitializeComponent();
            LoadConfiguration();

            if (string.IsNullOrEmpty(Properties.Settings.Default.SavedBrand))
            {
                var bw = new BrandWindow(_globalConfig);
                if (!bw.ShowDialog().GetValueOrDefault())
                {
                    MessageBox.Show("You must select a brand!");
                    App.Current.Shutdown();
                    return;
                }
                Properties.Settings.Default.SavedBrand = bw.SelectedBrand;
                Properties.Settings.Default.Save();
            }

            _brand = Properties.Settings.Default.SavedBrand;
            _brandDisplay = _brand.Substring(0, 1).ToUpperInvariant() + _brand.Substring(1);

            Title = _brandDisplay + " Vehicle Control";

            _clientCredentials = _globalConfig.brand_client_info[_brand];
            _apiConfig = (from f in _globalConfig.configs where f.name.Equals(_brand, StringComparison.OrdinalIgnoreCase) select f).FirstOrDefault();




            txtVin.Text = Properties.Settings.Default.SavedVin;
            txtPin.Password = Properties.Settings.Default.SavedPin;
            grpActions.IsEnabled = false;

        }

        void LoadConfiguration()
        {
            if (!File.Exists("config\\configuration.json"))
            {
                MessageBox.Show("You must extract the configuration file from the GM Android App's .apk and copy it to the config folder first.", "Missing configuration");
                App.Current.Shutdown();
                return;
            }

            try
            {
                _globalConfig = JsonConvert.DeserializeObject<GM.Api.GmConfiguration>(File.ReadAllText("Config\\configuration.json", Encoding.UTF8));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error reading config file: " + ex.ToString(), "Config read error");
                App.Current.Shutdown();
                return;
            }
        }



        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            var wind = new LoginWindow();
            wind.ShowDialog();
            if (wind.GMClient == null)
            {
                return;
            }

            _client = wind.GMClient;

            lblStatus.Content = "Connected";
            grpActions.IsEnabled = true;
        }

        private async void BtnLock_Click(object sender, RoutedEventArgs e)
        {
            grpActions.IsEnabled = false;
            btnLogin.IsEnabled = false;
            lblStatus.Content = "Locking (Please wait)";
            var success = await _client.LockDoor(txtVin.Text, txtPin.Password);
            if (success)
            {
                lblStatus.Content = "Locked Successfully";
            }
            else
            {
                lblStatus.Content = "Locking Failed";
            }
            
            grpActions.IsEnabled = true;
            btnLogin.IsEnabled = true;
        }

        private async void BtnUnlock_Click(object sender, RoutedEventArgs e)
        {
            grpActions.IsEnabled = false;
            btnLogin.IsEnabled = false;
            lblStatus.Content = "Unlocking (Please wait)";
            var success = await _client.UnlockDoor(txtVin.Text, txtPin.Password);
            if (success)
            {
                lblStatus.Content = "Unlocked Successfully";
            }
            else
            {
                lblStatus.Content = "Unlocking Failed";
            }
            grpActions.IsEnabled = true;
            btnLogin.IsEnabled = true;
        }

        private async void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            grpActions.IsEnabled = false;
            btnLogin.IsEnabled = false;
            lblStatus.Content = "Starting (Please wait)";
            var success = await _client.Start(txtVin.Text, txtPin.Password);
            if (success)
            {
                lblStatus.Content = "Started Successfully";
            }
            else
            {
                lblStatus.Content = "Starting Failed";
            }
            grpActions.IsEnabled = true;
            btnLogin.IsEnabled = true;
        }

        private async void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            grpActions.IsEnabled = false;
            btnLogin.IsEnabled = false;
            lblStatus.Content = "Stopping (Please wait)";
            var success = await _client.CancelStart(txtVin.Text, txtPin.Password);
            if (success)
            {
                lblStatus.Content = "Stopped Successfully";
            }
            else
            {
                lblStatus.Content = "Stopping Failed";
            }
            grpActions.IsEnabled = true;
            btnLogin.IsEnabled = true;
        }

        private async void BtnAlert_Click(object sender, RoutedEventArgs e)
        {
            grpActions.IsEnabled = false;
            btnLogin.IsEnabled = false;
            lblStatus.Content = "Alarming (Please wait)";
            var success = await _client.Alert(txtVin.Text, txtPin.Password);
            if (success)
            {
                lblStatus.Content = "Alarmed Successfully";
            }
            else
            {
                lblStatus.Content = "Alarming Failed";
            }
            grpActions.IsEnabled = true;
            btnLogin.IsEnabled = true;
        }

        private async void BtnCancelAlert_Click(object sender, RoutedEventArgs e)
        {
            grpActions.IsEnabled = false;
            btnLogin.IsEnabled = false;
            lblStatus.Content = "Stopping Alarm (Please wait)";
            var success = await _client.CancelAlert(txtVin.Text, txtPin.Password);
            if (success)
            {
                lblStatus.Content = "Alarmed Stopped Successfully";
            }
            else
            {
                lblStatus.Content = "Alarm Stopping Failed";
            }
            grpActions.IsEnabled = true;
            btnLogin.IsEnabled = true;
        }
    }
}
