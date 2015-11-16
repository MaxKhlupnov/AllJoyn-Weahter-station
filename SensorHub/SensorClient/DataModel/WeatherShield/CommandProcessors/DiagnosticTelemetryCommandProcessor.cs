using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using RemoteMonitoring.Common.DeviceSchema;
using RemoteMonitoring.Common.Helpers;
using RemoteMonitoring.CommandProcessors;
using RemoteMonitoring.Transport;

using SensorClient.DataModel.WeatherShield;

namespace SensorClient.DataModel.WeatherShield.CommandProcessors
{
    /// <summary>
    /// Command processor to handle activating external temperature
    /// </summary>
    public class DiagnosticTelemetryCommandProcessor : CommandProcessor
    {
        private const string DIAGNOSTIC_TELEMETRY = "DiagnosticTelemetry";

        public DiagnosticTelemetryCommandProcessor(WeatherShieldDevice device)
            : base(device)
        {

        }

        public async override Task<CommandProcessingResult> HandleCommandAsync(DeserializableCommand deserializableCommand)
        {
            if (deserializableCommand.CommandName == DIAGNOSTIC_TELEMETRY)
            {
                var command = deserializableCommand.Command;

                try
                {
                    var device = Device as WeatherShieldDevice;
                    if (device != null)
                    {
                        dynamic parameters = WireCommandSchemaHelper.GetParameters(command);
                        if (parameters != null)
                        {
                            dynamic activeAsDynamic = 
                                ReflectionHelper.GetNamedPropertyValue(
                                    parameters, 
                                    "Active", 
                                    usesCaseSensitivePropertyNameMatch: true,
                                    exceptionThrownIfNoMatch: true);

                            if (activeAsDynamic != null)
                            {
                                var active = Convert.ToBoolean(activeAsDynamic.ToString());

                                if (active != null)
                                {
                                    device.DiagnosticTelemetry(active);
                                    return CommandProcessingResult.Success;
                                }
                                else
                                {
                                    // Active is not a boolean.
                                    return CommandProcessingResult.CannotComplete;
                                }
                            }
                            else
                            {
                                // Active is a null reference.
                                return CommandProcessingResult.CannotComplete;
                            }
                        }
                        else
                        {
                            // parameters is a null reference.
                            return CommandProcessingResult.CannotComplete;
                        }
                    }
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
