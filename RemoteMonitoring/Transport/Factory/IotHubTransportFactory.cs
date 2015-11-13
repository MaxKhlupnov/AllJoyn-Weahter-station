using RemoteMonitoring.Common.Configurations;
using RemoteMonitoring.Devices;
using RemoteMonitoring.Logging;
using RemoteMonitoring.Serialization;

namespace RemoteMonitoring.Transport.Factory
{
    public class IotHubTransportFactory : ITransportFactory
    {
        private ISerialize _serializer;
        private ILogger _logger;
        private IConfigurationProvider _configurationProvider;

        public IotHubTransportFactory(ISerialize serializer, ILogger logger,
            IConfigurationProvider configurationProvider)
        {
            _serializer = serializer;
            _logger = logger;
            _configurationProvider = configurationProvider;
        }

        public ITransport CreateTransport(IDevice device)
        {
            return new IoTHubTransport(_serializer, _logger, _configurationProvider, device);
        }
    }
}
