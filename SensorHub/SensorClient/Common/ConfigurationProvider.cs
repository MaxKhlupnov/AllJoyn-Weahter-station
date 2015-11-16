using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RemoteMonitoring.Common.Configurations;

namespace SensorClient.Common
{
    public class ConfigurationProvider : IConfigurationProvider, IDisposable
    {
        readonly Dictionary<string, string> configuration = new Dictionary<string, string>();       
        const string ConfigToken = "config:";
        bool _disposed = false;

        public string GetConfigurationSettingValue(string configurationSettingName)
        {
            throw new NotImplementedException();
        }

        public string GetConfigurationSettingValueOrDefault(string configurationSettingName, string defaultValue)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
