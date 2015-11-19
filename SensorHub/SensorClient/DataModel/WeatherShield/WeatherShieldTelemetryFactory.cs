using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RemoteMonitoring.Telemetry.Factory;
using RemoteMonitoring.Logging;
using RemoteMonitoring.Devices;

using SensorClient.DataModel.Telemetry;
using SensorClient.DataModel;

namespace SensorClient.DataModel.WeatherShield
{
    public class WeatherShieldTelemetryFactory : ITelemetryFactory
    {
        private readonly ILogger _logger;

        public WeatherShieldTelemetryFactory(ILogger logger)
        {
            _logger = logger;
        }



        public object PopulateDeviceWithTelemetryEvents(IDevice device)
        {

            var startupTelemetry = new StartupTelemetry(_logger, device);
            device.TelemetryEvents.Add(startupTelemetry);

            return startupTelemetry;
        }
    }
}
