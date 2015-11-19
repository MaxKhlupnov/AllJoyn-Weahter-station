using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;

using RemoteMonitoring.Common.Configurations;
using RemoteMonitoring.Common.Models;
using RemoteMonitoring.Common.Repository;

//using Microsoft.WindowsAzure.Storage;
//using Microsoft.WindowsAzure.Storage.Table;
using SQLite.Net;
///using SQLite.Net.Attributes;
using RemoteMonitoring.Common.Helpers;

namespace SensorClient.Common
{
    public class DeviceConfigTableStorage : IVirtualDeviceStorage
    {
        private readonly string _storageConnectionString;
        private const string _deviceTableName = "Devices";

        SQLiteConnection conn;

        public DeviceConfigTableStorage(IConfigurationProvider configProvider)
        {
            string path = Path.Combine(Windows.Storage.ApplicationData.Current.LocalFolder.Path, "db.SensorClientConfig");
            _storageConnectionString = configProvider.GetConfigurationSettingValueOrDefault("device.StorageConnectionString",
                path);
            conn = new SQLite.Net.SQLiteConnection(new SQLite.Net.Platform.WinRT.SQLitePlatformWinRT(), path);
            if (conn.GetTableInfo(_deviceTableName).Count == 0)
            {
                conn.CreateTable<InitialDeviceConfig>();
            }
            // _deviceTableName = configProvider.GetConfigurationSettingValueOrDefault("device.DeviceConfigTableName", );
        }

        public async Task<List<InitialDeviceConfig>> GetDeviceListAsync()
        {
            List<InitialDeviceConfig> devices = new List<InitialDeviceConfig>();
           
            var query = conn.Table<InitialDeviceConfig>();
            foreach (var device in query)
            {              
                devices.Add(device);
            }
            return devices;           
        }

        public Task<InitialDeviceConfig> GetDeviceAsync(string deviceId)
        {
            var query = conn.Table<InitialDeviceConfig>().Where(device => device.DeviceId.Equals(deviceId));
            return Task.FromResult(query.FirstOrDefault());
        }

        public Task<InitialDeviceConfig> GetDevice(string deviceId, string hostName)
        {
            var query = conn.Table<InitialDeviceConfig>().Where(device => device.DeviceId.Equals(deviceId) && device.HostName.Equals(hostName));

            return Task.FromResult(query.FirstOrDefault());
        }

        public async Task<bool> RemoveDeviceAsync(string deviceId)
        {

            return conn.Table<InitialDeviceConfig>().Delete(device => device.DeviceId.Equals(deviceId)) > 0;
        }

        public async Task AddOrUpdateDeviceAsync(InitialDeviceConfig deviceConfig)
        {
            conn.InsertOrReplace(deviceConfig, typeof(InitialDeviceConfig));
        }
      
    }
}
