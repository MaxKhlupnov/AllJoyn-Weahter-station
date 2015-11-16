using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RemoteMonitoring.Devices;
using RemoteMonitoring.Logging;
using RemoteMonitoring.Common.Configurations;
using RemoteMonitoring.Transport.Factory;
using RemoteMonitoring.Telemetry.Factory;
using RemoteMonitoring.CommandProcessors;

using SensorClient.DataModel.Telemetry;
using SensorClient.DataModel.WeatherShield.CommandProcessors;

namespace SensorClient.DataModel.WeatherShield
{
    public class WeatherShieldDevice : DeviceBase, ITelemetryFactory
    {
        
        public SensorsCollection<AbstractSensor> DeviceSensors { get; set; }

        public WeatherShieldDevice(ILogger logger, ITransportFactory transportFactory, ITelemetryFactory telemetryFactory, 
            IConfigurationProvider configurationProvider): base(logger, transportFactory, telemetryFactory, configurationProvider)
        {
            this.DeviceSensors = new SensorsCollection<AbstractSensor>();
        }

        /// <summary>
        /// Builds up the set of commands that are supported by this device
        /// </summary>
        protected override void InitCommandProcessors()
        {
            var pingDeviceProcessor = new PingDeviceProcessor(this);
            var startCommandProcessor = new StartCommandProcessor(this);
            var stopCommandProcessor = new StopCommandProcessor(this);
            var diagnosticTelemetryCommandProcessor = new DiagnosticTelemetryCommandProcessor(this);
           var changeSetPointTempCommandProcessor = new ChangeSetPointTempCommandProcessor(this);
            var changeDeviceStateCommmandProcessor = new ChangeDeviceStateCommandProcessor(this);

            pingDeviceProcessor.NextCommandProcessor = startCommandProcessor;
            startCommandProcessor.NextCommandProcessor = stopCommandProcessor;
            stopCommandProcessor.NextCommandProcessor = diagnosticTelemetryCommandProcessor;
            diagnosticTelemetryCommandProcessor.NextCommandProcessor = changeSetPointTempCommandProcessor;
            changeSetPointTempCommandProcessor.NextCommandProcessor = changeDeviceStateCommmandProcessor;

            RootCommandProcessor = pingDeviceProcessor;
        }

        public void StartTelemetryData()
        {
            var remoteMonitorTelemetry = (SensorTelemetry)_telemetryController;
            remoteMonitorTelemetry.TelemetryActive = true;
            Logger.LogInfo("Device {0}: Telemetry has started", DeviceID);
        }

        public void StopTelemetryData()
        {
            var remoteMonitorTelemetry = (SensorTelemetry)_telemetryController;
            remoteMonitorTelemetry.TelemetryActive = false;
            Logger.LogInfo("Device {0}: Telemetry has stopped", DeviceID);
        }

        public void ChangeSetPointTemp(double setPointTemp)
        {
            var remoteMonitorTelemetry = (SensorTelemetry)_telemetryController;
          //  remoteMonitorTelemetry.ChangeSetPointTemperature(setPointTemp);
            Logger.LogInfo("Device {0} temperature changed to {1}", DeviceID, setPointTemp);
        }

        public async void ChangeDeviceState(string deviceState)
        {
            // simply update the DeviceState property and send updated device info packet
            DeviceProperties.DeviceState = deviceState;
            await SendDeviceInfo();
            Logger.LogInfo("Device {0} in {1} state", DeviceID, deviceState);
        }

        public void DiagnosticTelemetry(bool active)
        {
            var remoteMonitorTelemetry = (SensorTelemetry)_telemetryController;
            //remoteMonitorTelemetry.ActivateExternalTemperature = active;
            string externalTempActive = active ? "on" : "off";
            Logger.LogInfo("Device {0}: External Temperature: {1}", DeviceID, externalTempActive);
        }

        public void ChangeSetPointTemperature(double newSetPointTemperature)
        {
          //  _temperatureGenerator.ShiftSubsequentData(newSetPointTemperature);
        }

        public void AddDeviceSensor(AbstractSensor sensor)
        {
           var existingSensor = this.DeviceSensors.FirstOrDefault<AbstractSensor>(sn => sn.UniqueName.Equals(sensor.UniqueName));

            if (existingSensor != null)
            {
                this.Logger.LogWarning("Overwrite sensor {0} uniquename {1} for deviceid {2}", sensor.Title, sensor.UniqueName, this.DeviceID);
                existingSensor = sensor;
            }
            else
            {
                this.DeviceSensors.Add(sensor);
                var monitorTelemetry = new SensorTelemetry(this.Logger, this, sensor);
                this.TelemetryEvents.Add(monitorTelemetry);
            }
        }

        public void RemoveDeviceSensor(AbstractSensor sensor)
        {
            var existingSensor = this.DeviceSensors.FirstOrDefault<AbstractSensor>(sn => sn.UniqueName.Equals(sensor.UniqueName));
            if (existingSensor != null)
            {               
                this.DeviceSensors.Remove(existingSensor);                
            }
        }
        public object PopulateDeviceWithTelemetryEvents()
        {
            return PopulateDeviceWithTelemetryEvents(this);
        }
        public object PopulateDeviceWithTelemetryEvents(IDevice device)
        {

            var startupTelemetry = new StartupTelemetry(this.Logger, device);
            device.TelemetryEvents.Add(startupTelemetry);                        

            return startupTelemetry;
        }

    }
}
