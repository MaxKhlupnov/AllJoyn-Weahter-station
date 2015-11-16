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
        }

        public async Task SendEventsAsync(CancellationToken token, Func<object, Task> sendMessageAsync)
        {
            // Read data from sensor
            var monitorData = await this._sensor.DoMeasure();
            monitorData.DeviceId = this._deviceId;

            while (!token.IsCancellationRequested)
            {
                if (TelemetryActive)
                {
                    await sendMessageAsync(monitorData);
                }
               
            }

             await Task.Delay(TimeSpan.FromSeconds(REPORT_FREQUENCY_IN_SECONDS), token);
        }
    }
}
