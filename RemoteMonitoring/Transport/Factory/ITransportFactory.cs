using RemoteMonitoring.Devices;

namespace RemoteMonitoring.Transport.Factory
{
    public interface ITransportFactory
    {
        ITransport CreateTransport(IDevice device);
    }
}
