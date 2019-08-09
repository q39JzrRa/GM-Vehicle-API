using GM.Api.Models;
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
using System.Windows.Shapes;

namespace GM.WindowsUI
{
    /// <summary>
    /// Interaction logic for BrandWindow.xaml
    /// </summary>
    public partial class BrandWindow : Window
    {

        GmConfiguration _config;

        public string SelectedBrand { get; set; } = null;


        public BrandWindow(GmConfiguration configuration)
        {
            _config = configuration;
            InitializeComponent();

            foreach (var brandName in _config.brand_client_info.Keys.OrderBy((val) => val, StringComparer.OrdinalIgnoreCase))
            {
                lstBrands.Items.Add(brandName.Substring(0, 1).ToUpperInvariant() + brandName.Substring(1));
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (lstBrands.SelectedItem == null) return;
            SelectedBrand = ((string)lstBrands.SelectedItem).ToLowerInvariant();
            this.Close();
        }
    }
}
