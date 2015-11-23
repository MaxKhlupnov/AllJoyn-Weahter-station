using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RemoteMonitoring.Telemetry;
using RemoteMonitoring.Logging;
using RemoteMonitoring.Devices;
using System.Threading;


using SensorClient.DataModel;

namespace SensorClient.DataModel.Telemetry
{
        /// <summary>
        /// This class can be used if we decde send all measures as a separated data 
        /// </summary>
    public class SensorTelemetry : ITelemetry
    {
        private readonly ILogger _logger;
        private readonly string _deviceId;

        private const int REPORT_FREQUENCY_IN_SECONDS = 5;
        private const int PEAK_FREQUENCY_IN_SECONDS = 90;

        private AbstractSensor _sensor;

        public bool TelemetryActive { get; set; }


        public SensorTelemetry(ILogger logger, IDevice device, AbstractSensor sensor)
        {
            this._logger = logger;
            this._deviceId = device.DeviceID;
            this._sensor = sensor;
            this.TelemetryActive = !string.IsNullOrWhiteSpace(device.HostName) && !string.IsNullOrWhiteSpace(device.PrimaryAuthKey);
        }

        public async Task SendEventsAsync(CancellationToken token, Func<object, Task> sendMessageAsync)
        {
            // Read data from sensor
            if (this._sensor == null || this._sensor.LastMeasure == null || !this.TelemetryActive)
                return;

            var monitorData = this._sensor.LastMeasure;
            monitorData.DeviceId = this._deviceId;
            this._logger.LogInfo("Sending telemetry for device {0}; _sensor {1}; value: {2}", new object[] { _deviceId, _sensor.Title, monitorData.Value});
            while (!token.IsCancellationRequested)
            {
                if (TelemetryActive)
                {
                    try {
                        await sendMessageAsync(monitorData);
                    }catch(Exception ex)
                    {
                        this._logger.LogError("Error {0} sending telemetry for device {1}; _sensor {2}; value: {3}", 
                            new object[] {ex.Message, _deviceId, _sensor.Title, monitorData.Value });
                    }
                }
               
            }
            
        }
    }
}
