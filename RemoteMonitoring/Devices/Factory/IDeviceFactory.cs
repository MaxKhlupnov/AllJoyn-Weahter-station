using RemoteMonitoring.Common.Configurations;
using RemoteMonitoring.Common.Models;
using RemoteMonitoring.Telemetry.Factory;
using RemoteMonitoring.Transport.Factory;

namespace RemoteMonitoring.Devices.Factory
{
    public interface IDeviceFactory
    {
        IDevice CreateDevice(Logging.ILogger logger, ITransportFactory transportFactory,
            ITelemetryFactory telemetryFactory, IConfigurationProvider configurationProvider, InitialDeviceConfig config);
    }
}
