using GM.Api;
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
        public Brand? SelectedBrand { get; set; } = null;

        public BrandWindow()
        {
            InitializeComponent();

            var brandNames = Enum.GetNames(typeof(Brand));

            foreach (var brandName in brandNames.OrderBy((val) => val, StringComparer.OrdinalIgnoreCase))
            {
                lstBrands.Items.Add(brandName);
            }
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (lstBrands.SelectedItem == null) return;
            SelectedBrand = BrandHelpers.GetBrand((string)lstBrands.SelectedItem);
            this.Close();
        }
    }
}
