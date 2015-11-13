using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using RemoteMonitoring.Devices;
using RemoteMonitoring.Transport;

namespace RemoteMonitoring.CommandProcessors
{
    public class PingDeviceProcessor : CommandProcessor
    {
        public PingDeviceProcessor(IDevice device)
            : base(device)
        {
            
        }

        public async override Task<CommandProcessingResult> HandleCommandAsync(DeserializableCommand deserializableCommand)
        {
            if (deserializableCommand.CommandName == "PingDevice")
            {
                var command = deserializableCommand.Command;

                try
                {
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
