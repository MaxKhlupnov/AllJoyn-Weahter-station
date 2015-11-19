using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RemoteMonitoring.Telemetry;
using RemoteMonitoring.Logging;
using RemoteMonitoring.Devices;

namespace SensorClient.DataModel.Telemetry
{
    public class StartupTelemetry : ITelemetry
    {

        private readonly ILogger _logger;
        private readonly IDevice _device;
        public bool TelemetryActive { get; set; }
        public StartupTelemetry(ILogger logger, IDevice device)
        {
            _logger = logger;
            _device = device;
            this.TelemetryActive = !string.IsNullOrWhiteSpace(device.HostName) && !string.IsNullOrWhiteSpace(device.PrimaryAuthKey);
        }

        public async Task SendEventsAsync(System.Threading.CancellationToken token, Func<object, Task> sendMessageAsync)
        {
            if (!token.IsCancellationRequested && TelemetryActive)
            {
                _logger.LogInfo("Sending initial data for device {0}", _device.DeviceID);
                await sendMessageAsync(_device.GetDeviceInfo());
            }
        }
    }
}
