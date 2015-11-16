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

        public StartupTelemetry(ILogger logger, IDevice device)
        {
            _logger = logger;
            _device = device;
        }

        public async Task SendEventsAsync(System.Threading.CancellationToken token, Func<object, Task> sendMessageAsync)
        {
            if (!token.IsCancellationRequested)
            {
                _logger.LogInfo("Sending initial data for device {0}", _device.DeviceID);
                await sendMessageAsync(_device.GetDeviceInfo());
            }
        }
    }
}
