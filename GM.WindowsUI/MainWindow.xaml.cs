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
        GMClient _client;
        

        GmConfiguration _globalConfig;

        ApiConfig _apiConfig;
        BrandClientInfo _clientCredentials;

        string _brand;
        string _brandDisplay;


        Vehicle[] _vehicles = null;

        Vehicle _selectedVehicle;

        public MainWindow()
        {
            InitializeComponent();
            LoadConfiguration();
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
            _client = new GMClient(_clientCredentials.client_id, Properties.Settings.Default.DeviceId, _clientCredentials.client_secret, _apiConfig.url);
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
                var bw = new BrandWindow(_globalConfig);
                bw.ShowDialog();

                if (string.IsNullOrEmpty(bw.SelectedBrand))
                {
                    MessageBox.Show("You must select a brand!");
                    Environment.Exit(100);
                    return;
                }

                Properties.Settings.Default.Brand = bw.SelectedBrand;
                Properties.Settings.Default.Save();
            }

            _brand = Properties.Settings.Default.Brand;
            _brandDisplay = _brand.Substring(0, 1).ToUpperInvariant() + _brand.Substring(1);

            Title = _brandDisplay + " Vehicle Control";

            _clientCredentials = _globalConfig.brand_client_info[_brand];
            _apiConfig = (from f in _globalConfig.configs where f.name.Equals(_brand, StringComparison.OrdinalIgnoreCase) select f).FirstOrDefault();
        }

        void LoadConfiguration()
        {
            if (!Directory.Exists("apk")) Directory.CreateDirectory("apk");

            var fn = (from f in Directory.EnumerateFiles("apk") where System.IO.Path.GetExtension(f).Equals(".apk", StringComparison.OrdinalIgnoreCase) select f).FirstOrDefault();

            if (string.IsNullOrEmpty(fn))
            {
                MessageBox.Show("You must copy the Android app's .apk file to the apk folder first.", "Missing apk");
                Environment.Exit(100);
                return;
            }

            try
            {
                _globalConfig = JsonConvert.DeserializeObject<GmConfiguration>(GM.SettingsReader.ReadUtility.Read(Properties.Resources.a, Properties.Resources.gm, File.OpenRead(fn)));
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error reading config file: " + ex.ToString(), "Config read error");
                Environment.Exit(100);
                return;
            }
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
                cmbVehicle.Items.Add($"{vehicle.year} {vehicle.model} ({vehicle.vin})");
            }

            if (!string.IsNullOrEmpty(Properties.Settings.Default.Vin))
            {
                bool found = false;
                for (int i = 0; i < _vehicles.Length; i++)
                {
                    if (_vehicles[i].vin.Equals(Properties.Settings.Default.Vin, StringComparison.OrdinalIgnoreCase))
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

        private async void BtnLock_Click(object sender, RoutedEventArgs e)
        {
            grpActions.IsEnabled = false;
            btnLogin.IsEnabled = false;
            lblStatus.Content = "Locking (Please wait)";
            var success = await _client.LockDoor(_selectedVehicle.vin, txtPin.Password);
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
            var success = await _client.UnlockDoor(_selectedVehicle.vin, txtPin.Password);
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
            var success = await _client.Start(_selectedVehicle.vin, txtPin.Password);
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
            var success = await _client.CancelStart(_selectedVehicle.vin, txtPin.Password);
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
            var success = await _client.Alert(_selectedVehicle.vin, txtPin.Password);
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
            var success = await _client.CancelAlert(_selectedVehicle.vin, txtPin.Password);
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
                _selectedVehicle = null;
                return;
            }

            _selectedVehicle = _vehicles[cmbVehicle.SelectedIndex];

            Properties.Settings.Default.Vin = _selectedVehicle.vin;
            Properties.Settings.Default.Save();

            //todo: populate available actions
            //todo: update client state instead of local variable?

        }
    }
}
