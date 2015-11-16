using System;
using System.Threading.Tasks;
using RemoteMonitoring.Common.DeviceSchema;
using RemoteMonitoring.Common.Helpers;
using RemoteMonitoring.CommandProcessors;
using RemoteMonitoring.Transport;

using SensorClient.DataModel.WeatherShield;

namespace SensorClient.DataModel.WeatherShield.CommandProcessors
{
    /// <summary>
    /// Command processor to stop telemetry data
    /// </summary>
    public class StopCommandProcessor : CommandProcessor 
    {
        private const string STOP_TELEMETRY = "StopTelemetry";

        public StopCommandProcessor(WeatherShieldDevice device)
            : base(device)
        {

        }

        public async override Task<CommandProcessingResult> HandleCommandAsync(DeserializableCommand deserializableCommand)
        {
            if (deserializableCommand.CommandName == STOP_TELEMETRY)
            {
                var command = deserializableCommand.Command;

                try
                {
                    var device = Device as WeatherShieldDevice;
                    device.StopTelemetryData();
                    return CommandProcessingResult.Success;
                }
                catch (Exception)
                {
                    return CommandProcessingResult.RetryLater;
                }
            }
            else if (NextCommandProcessor != null)
            {
                return await NextCommandProcessor.HandleCommandAsync(deserializableCommand);
            }

            return CommandProcessingResult.CannotComplete;
        }
    }
}
