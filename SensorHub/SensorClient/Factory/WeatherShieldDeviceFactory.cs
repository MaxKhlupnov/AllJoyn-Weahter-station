using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using SensorClient.Common;

using RemoteMonitoring.Devices;
using RemoteMonitoring.Logging;
using RemoteMonitoring.Common.Configurations;
using RemoteMonitoring.Transport.Factory;
using RemoteMonitoring.Telemetry.Factory;
using RemoteMonitoring.Serialization;



namespace SensorClient.DataModel
{
    public class WeatherShieldDeviceFactory 
    {
        // change this to inject a different logger
        private readonly ILogger _logger;
        private readonly ITransportFactory _transportFactory;
        private readonly IConfigurationProvider _configProvider;
        private readonly ITelemetryFactory _telemetryFactory;

        internal WeatherShieldDeviceFactory()
        {
            this._logger = new TraceLogger();
            this._configProvider = new ConfigurationProvider();

            var serializer = new JsonSerialize();
            this._transportFactory  = new IotHubTransportFactory(serializer, this._logger, this._configProvider);
        }

    }
}
