using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using RemoteMonitoring.Logging;
using RemoteMonitoring.Common.Configurations;
using RemoteMonitoring.Transport.Factory;
using RemoteMonitoring.Telemetry.Factory;
using RemoteMonitoring.Devices;
using RemoteMonitoring.Common.Repository;
using RemoteMonitoring.Serialization;
using RemoteMonitoring.Common.Models;
using RemoteMonitoring.Common.DeviceSchema;

using SensorClient.Common;
using SensorClient.DataModel.WeatherShield;

namespace SensorClient.DataModel
{
    /// <summary>
    /// Manages and coordinates all devices
    /// </summary>
    public class DeviceManager
    {

        Mutex _mutex;
        const string mutexId = "WeatherStationDeviceManager";

        private readonly CancellationToken _token;

        private readonly Dictionary<string, CancellationTokenSource> _cancellationTokens;

        // change this to inject a different logger
        private readonly ILogger _logger;
        private readonly ITransportFactory _transportFactory;
        private readonly IConfigurationProvider _configProvider;
        private readonly ITelemetryFactory _telemetryFactory;        
        private readonly IVirtualDeviceStorage _deviceConfiguration;

        private readonly Dictionary<string, IDevice> _devices;

        public DeviceManager(ILogger logger, CancellationToken token)
        {
            _logger = logger;

            _token = token;
            _mutex = new Mutex(true, mutexId);
            _cancellationTokens = new Dictionary<string, CancellationTokenSource>();
            _devices = new Dictionary<string, IDevice >();

            _configProvider = new ConfigurationProvider();
            _deviceConfiguration = new DeviceConfigTableStorage(_configProvider);

            var serializer = new JsonSerialize();
            _transportFactory = new IotHubTransportFactory(serializer, _logger, _configProvider);

            _telemetryFactory = new WeatherShieldTelemetryFactory(_logger);
        }
     
               
        public async void StartDeviceSensorAsync(AbstractSensor sensor, dynamic device)
        {
            if (device == null)
                return;

            bool hasMutex = false;
            try
            {
                hasMutex = _mutex.WaitOne(5000);
                // check if device already exists
                string deviceID = DeviceSchemaHelper.GetDeviceID(device);
                WeatherShieldDevice existingDevice;
                if (_devices.ContainsKey(deviceID))
                {
                    
                    var deviceCancellationToken = new CancellationTokenSource();
                    _cancellationTokens.Add(deviceID, deviceCancellationToken);

                    //Initialize device as a new
                    existingDevice = new WeatherShieldDevice(this._logger, this._transportFactory,
                        this._telemetryFactory, this._configProvider, deviceCancellationToken.Token);

                    InitialDeviceConfig config = await this._deviceConfiguration.GetDeviceAsync(deviceID);
                    if (config == null) {
                        config = new InitialDeviceConfig();
                        config.Key = string.Empty;
                        config.HostName = string.Empty;                        
                    }
                    existingDevice.Init(config, device);
                    
                    _devices.Add(deviceID, existingDevice);

                }
                else
                {
                    existingDevice = _devices[deviceID] as WeatherShieldDevice;
                }
                existingDevice.AddDeviceSensor(sensor);


                this._logger.LogInfo("Addedd sensor {0} dor device id {1}", new object[] { sensor.Title, existingDevice.DeviceID });

            }catch(Exception ex)
            {
                this._logger.LogError("Error adding {0} for device {1}. Error message: {2}", new object[] { sensor, device, ex.Message });
            }
            finally
                {
                    if (hasMutex)
                    {
                        _mutex.ReleaseMutex();
                    }
                }
}

        /// <summary>
        /// Starts all the devices in the list of devices in this class.
        /// 
        /// Note: This will not return until all devices have finished sending events,
        /// assuming no device has RepeatEventListForever == true
        /// </summary>
        public async Task StartDevicesAsync()
        {
            await Task.Run(async() => 
            {
                if (_devices == null || !_devices.Any())
                    return;

                var startDeviceTasks = new List<Task>();

                bool hasMutex = false;
                try
                {
                    hasMutex = _mutex.WaitOne(5000);
                    foreach (var device in _devices.Values)
                    {
                        var deviceCancellationToken = _cancellationTokens[device.DeviceID];
                        startDeviceTasks.Add(device.StartAsync(deviceCancellationToken.Token));
                    }

                    // wait here until all tasks complete
                    await Task.WhenAll(startDeviceTasks);
                }catch (Exception ex)
                {
                    this._logger.LogError("Error processing device task: {0}", new object[] {ex.Message });
                }
                finally
                {
                    if (hasMutex)
                    {
                        _mutex.ReleaseMutex();
                    }
                }
        }, _token);
        }

        /// <summary>
        /// Cancel the asynchronous tasks for the devices specified
        /// </summary>
        /// <param name="deviceIds"></param>
        public void StopDevices(List<string> deviceIds) 
        {
            foreach (var deviceId in deviceIds)
            {
                var cancellationToken = _cancellationTokens[deviceId];

                if (cancellationToken != null)
                {
                    cancellationToken.Cancel();                   
                    _cancellationTokens.Remove(deviceId);
                    _devices.Remove(deviceId);

                    _logger.LogInfo("********** STOPPED DEVICE : {0} ********** ", deviceId);
                }
            }   
        }

        /// <summary>
        /// Cancel the asynchronous tasks for all devices
        /// </summary>
        public void StopAllDevices() 
        {
            foreach (var cancellationToken in _cancellationTokens)
            {
                cancellationToken.Value.Cancel();
                _logger.LogInfo("********** STOPPED DEVICE : {0} ********** ", cancellationToken.Key);
            }

            _cancellationTokens.Clear();
        }
    }
}
