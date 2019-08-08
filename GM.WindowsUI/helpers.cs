using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GM.Api;

namespace GM.WindowsUI
{
    static class helpers
    {








        public static GMClient CreateClient()
        {
            //Generate device ID if not yet created
            if (string.IsNullOrEmpty(Properties.Settings.Default.DeviceId))
            {
                Properties.Settings.Default.DeviceId = Guid.NewGuid().ToString();
                Properties.Settings.Default.Save();
            }

            //var client = new GMClient(Properties.Settings.Default.ProviderClientId, Properties.Settings.Default.DeviceId, Properties.Settings.Default.ProviderClientSecret, Properties.Settings.Default.ProviderApiUrl);

            //return client;
            return null;
        }


    }
}
