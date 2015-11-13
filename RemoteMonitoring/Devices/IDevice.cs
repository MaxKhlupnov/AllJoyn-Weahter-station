using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using RemoteMonitoring.Common.Models;
using RemoteMonitoring.Telemetry;

namespace RemoteMonitoring.Devices
{
    /// <summary>
    /// Represents a device. Implementors may be written in managed code, or a managed wrapper
    /// around a native (C/C++) core.
    /// </summary>
    public interface IDevice
    {
        string DeviceID { get; set; }

        string HostName { get; set; }

        string PrimaryAuthKey { get; set; }

        dynamic DeviceProperties { get; set; }

        dynamic Commands { get; set; }

        List<ITelemetry> TelemetryEvents { get; }

        bool RepeatEventListForever { get; set; }

        void Init(InitialDeviceConfig config, dynamic initialDevice);


        Task SendDeviceInfo();

        dynamic GetDeviceInfo();

        Task StartAsync(CancellationToken token);
    }
}
