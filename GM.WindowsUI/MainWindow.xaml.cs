using GM.Api;
using GM.Api.Models;
using GM.Api.Tokens;
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
        GMClientBase _client;
        
        Brand _brand;

        Vehicle[] _vehicles = null;

        //Vehicle _selectedVehicle;

        public MainWindow()
        {
            InitializeComponent();
            LoadBrand();
            CreateClient();
            grpActions.IsEnabled = false;
        }


        void CreateClient()
        {
            if (string.IsNullOrEmpty(Properties.Settings.Default.DeviceId))
            {
                Properties.Settings.Default.DeviceId = Guid.NewGuid().ToString();
                Properties.Settings.Default.Save();
            }

            //todo: maybe the client reads the config and takes the brand and device id as param?
            _client = new GMClientNoKey(Properties.Settings.Default.DeviceId, _brand, Properties.Settings.Default.TokenSignerUrl);
            _client.TokenUpdateCallback = TokenUpdateHandler;

            if (!string.IsNullOrEmpty(Properties.Settings.Default.LoginData))
            {
                LoginData ld = null;

                try
                {
                    ld = JsonConvert.DeserializeObject<LoginData>(Properties.Settings.Default.LoginData);
                }
                catch (Exception ex)
                {
                    Properties.Settings.Default.LoginData = null;
                    Properties.Settings.Default.Save();
                }

                if (ld != null)
                {
                    _client.LoginData = ld;
                }
            }

        }


        async Task TokenUpdateHandler(LoginData loginData)
        {
            if (loginData != null)
            {
                Properties.Settings.Default.LoginData = JsonConvert.SerializeObject(loginData);
                Properties.Settings.Default.Save();
            }
        }


        void LoadBrand()
        {
            if (string.IsNullOrEmpty(Properties.Settings.Default.Brand))
            {
                var bw = new BrandWindow();
                bw.ShowDialog();

                if (!bw.SelectedBrand.HasValue)
                {
                    MessageBox.Show("You must select a brand!");
                    Environment.Exit(100);
                    return;
                }

                Properties.Settings.Default.Brand = bw.SelectedBrand.Value.GetName();
                Properties.Settings.Default.Save();
            }

            _brand = BrandHelpers.GetBrand(Properties.Settings.Default.Brand);

            Title = _brand.GetDisplayName() + " Vehicle Control";
        }


        async Task LoadVehicles()
        {


            IEnumerable<Vehicle> vehicles = null;
            try
            {
                vehicles = await _client.GetVehicles();
            }
            catch (Exception)
            {
                throw;
            }

            if (vehicles == null)
            {
                MessageBox.Show("There are no vehicles on your account!");
                return;
            }

            _vehicles = vehicles.ToArray();

            foreach (var vehicle in _vehicles)
            {
                cmbVehicle.Items.Add($"{vehicle.Year} {vehicle.Model} ({vehicle.Vin})");
            }

            if (!string.IsNullOrEmpty(Properties.Settings.Default.Vin))
            {
                bool found = false;
                for (int i = 0; i < _vehicles.Length; i++)
                {
                    if (_vehicles[i].Vin.Equals(Properties.Settings.Default.Vin, StringComparison.OrdinalIgnoreCase))
                    {
                        found = true;
                        cmbVehicle.SelectedIndex = i;
                        break;
                    }
                }
                if (!found)
                {
                    cmbVehicle.SelectedIndex = 0;
                }
            }
            else
            {
                cmbVehicle.SelectedIndex = 0;
            }


            
        }


        private async void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            var wind = new LoginWindow(_client);
            wind.ShowDialog();
            if (!wind.Success)
            {
                return;
            }

            await LoadVehicles();
            lblStatus.Content = "Connected";
            grpActions.IsEnabled = true;
            btnLogin.IsEnabled = false;
        }


        async Task<bool> HandleUpgrade()
        {
            if (!_client.IsUpgraded)
            {
                if (string.IsNullOrEmpty(txtPin.Password))
                {
                    MessageBox.Show("OnStar PIN required");
                    return false;
                }

                var result = await _client.UpgradeLogin(txtPin.Password);
                if (!result)
                {
                    MessageBox.Show("Login upgrade failed!");
                    return false;
                }
            }
            return true;
        }

        private async void BtnLock_Click(object sender, RoutedEventArgs e)
        {
            if (!await HandleUpgrade()) return;

            grpActions.IsEnabled = false;
            btnLogin.IsEnabled = false;
            lblStatus.Content = "Locking (Please wait)";
            var success = await _client.LockDoor();
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
            if (!await HandleUpgrade()) return;
            grpActions.IsEnabled = false;
            btnLogin.IsEnabled = false;
            lblStatus.Content = "Unlocking (Please wait)";
            var success = await _client.UnlockDoor();
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
            if (!await HandleUpgrade()) return;
            grpActions.IsEnabled = false;
            btnLogin.IsEnabled = false;
            lblStatus.Content = "Starting (Please wait)";
            var success = await _client.Start();
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
            if (!await HandleUpgrade()) return;
            grpActions.IsEnabled = false;
            btnLogin.IsEnabled = false;
            lblStatus.Content = "Stopping (Please wait)";
            var success = await _client.CancelStart();
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
            if (!await HandleUpgrade()) return;
            grpActions.IsEnabled = false;
            btnLogin.IsEnabled = false;
            lblStatus.Content = "Alarming (Please wait)";
            var success = await _client.Alert();
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
            if (!await HandleUpgrade()) return;
            grpActions.IsEnabled = false;
            btnLogin.IsEnabled = false;
            lblStatus.Content = "Stopping Alarm (Please wait)";
            var success = await _client.CancelAlert();
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

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (await _client.RefreshToken())
            {
                await LoadVehicles();
                lblStatus.Content = "Connected";
                grpActions.IsEnabled = true;
                btnLogin.IsEnabled = false;
            }

        }

        private void CmbVehicle_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_vehicles == null || _vehicles.Length == 0 || cmbVehicle.SelectedIndex < 0)
            {
                _client.ActiveVehicle = null;
                return;
            }

            _client.ActiveVehicle = _vehicles[cmbVehicle.SelectedIndex];

            Properties.Settings.Default.Vin = _client.ActiveVehicle.Vin;
            Properties.Settings.Default.Save();

            //todo: populate available actions
            //todo: update client state instead of local variable?

        }

        private async void BtnDiagnostics_Click(object sender, RoutedEventArgs e)
        {
            if (!await HandleUpgrade()) return;
            grpActions.IsEnabled = false;
            btnLogin.IsEnabled = false;
            lblStatus.Content = "Getting Diagnostics (Please Wait)...";
            var details = await _client.GetDiagnostics();
            txtOutput.Text = JsonConvert.SerializeObject(details, Formatting.Indented);
            lblStatus.Content = "Getting Diagnostics Complete";
            grpActions.IsEnabled = true;
            btnLogin.IsEnabled = true;
        }
    }
}
