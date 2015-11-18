using System;
using System.Collections.Generic;
using Windows.Storage;
using Windows.Foundation.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RemoteMonitoring.Common.Configurations;

namespace SensorClient.Common
{
    public class ConfigurationProvider : IConfigurationProvider
    {               

        public string GetConfigurationSettingValue(string configurationSettingName)
        {
            var localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

            object value = localSettings.Values[configurationSettingName];
            if (value == null)
                return null;
            else
                return value.ToString();
        }

        public string GetConfigurationSettingValueOrDefault(string configurationSettingName, string defaultValue)
        {
            string configuredVal = GetConfigurationSettingValue(configurationSettingName);
            if (string.IsNullOrEmpty(configuredVal))
                return defaultValue;
            else
                return configuredVal;
        }

       
    }
}
