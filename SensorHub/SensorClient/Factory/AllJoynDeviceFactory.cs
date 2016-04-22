using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using RemoteMonitoring.Common.DeviceSchema;
using RemoteMonitoring.Common.Models;
using RemoteMonitoring.Devices;

using Windows.Devices.AllJoyn;

namespace SensorClient.Factory
{
    public static class AllJoynDeviceFactory
    {
        public const string OBJECT_TYPE_DEVICE_INFO = "DeviceInfo";

        public const string VERSION_1_0 = "1.0";

        private const int MAX_COMMANDS_SUPPORTED = 6;

       private const bool IS_SIMULATED_DEVICE = false;

 /*        private static List<string> DefaultDeviceNames = new List<string>{
            "SampleDevice001", 
            "SampleDevice002", 
            "SampleDevice003", 
            "SampleDevice004"
        };*/

        private class Location
        {
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            
            public Location(double latitude, double longitude)
            {
                Latitude = latitude;
                Longitude = longitude;
            }

        }

        private static List<Location> _possibleDeviceLocations = new List<Location>{
            new Location(47.659159, -122.141515),  // Microsoft Red West Campus, Building A
            new Location(47.593307, -122.332165),  // 800 Occidental Ave S, Seattle, WA 98134
            new Location(47.617025, -122.191285),  // 11111 NE 8th St, Bellevue, WA 98004
            new Location(47.583582, -122.130622)  // 3003 160th Ave SE Bellevue, WA 98008
        };

        public static dynamic GetAllJoynDevice(AllJoynAboutDataView deviceDataView)
        {

            var device = DeviceSchemaHelper.BuildDeviceStructure(deviceDataView.DeviceId, IS_SIMULATED_DEVICE);

            AssignDeviceProperties(deviceDataView, device);
            AssignCommands(device);
            device.ObjectType = OBJECT_TYPE_DEVICE_INFO;
            device.Version = VERSION_1_0;
            device.IsSimulatedDevice = IS_SIMULATED_DEVICE;

          
           // 

            return device;
        }

 /*       public static dynamic GetSampleDevice(Random randomNumber, SecurityKeys keys)
        {
            string deviceId = 
                string.Format(
                    CultureInfo.InvariantCulture,
                    "00000-DEV-{0}C-{1}LK-{2}D-{3}",
                    MAX_COMMANDS_SUPPORTED, 
                    randomNumber.Next(99999),
                    randomNumber.Next(99999),
                    randomNumber.Next(99999));

            dynamic device = DeviceSchemaHelper.BuildDeviceStructure(deviceId, false);
            device.ObjectName = "IoT Device Description";

            AssignDeviceProperties(deviceId, device);
            AssignCommands(device);

            return device;
        }*/

        private static void AssignDeviceProperties(AllJoynAboutDataView deviceDataView, dynamic device)
        {
            dynamic deviceProperties = DeviceSchemaHelper.GetDeviceProperties(device);
            
            deviceProperties.HubEnabledState = false;
            deviceProperties.Manufacturer = deviceDataView.Manufacturer;
            deviceProperties.ModelNumber = deviceDataView.ModelNumber;
            deviceProperties.SerialNumber = deviceDataView.DeviceId;
                           
            ulong version = 0;
            if (!ulong.TryParse(deviceDataView.SoftwareVersion, out version))
            {
                deviceProperties.FirmwareVersion = String.Empty;
            }
            else
            {
                deviceProperties.FirmwareVersion = String.Format(CultureInfo.InvariantCulture, "{0}.{1}.{2}.{3}",
                            (version & 0xFFFF000000000000) >> 48,
                            (version & 0x0000FFFF00000000) >> 32,
                            (version & 0x00000000FFFF0000) >> 16,
                            version & 0x000000000000FFFF);
            }

            deviceProperties.Platform = deviceDataView.HardwareVersion;

            /*deviceProperties.Processor = "ARM";
            deviceProperties.InstalledRAM = "No data";

            // Choose a location between the 3 above and set Lat and Long for device properties
            int chosenLocation = GetIntBasedOnString(deviceDataView.DeviceId + "Location", _possibleDeviceLocations.Count);
            deviceProperties.Latitude = _possibleDeviceLocations[chosenLocation].Latitude;
            deviceProperties.Longitude = _possibleDeviceLocations[chosenLocation].Longitude;*/
        }

        private static int GetIntBasedOnString(string input, int maxValueExclusive)
        {
            int hash = input.GetHashCode();

            //Keep the result positive
            if(hash < 0)
            {
                hash = -hash;
            }

            return hash % maxValueExclusive;
        }

        private static void AssignCommands(dynamic device)
        {
            dynamic command = CommandSchemaHelper.CreateNewCommand("PingDevice");
            CommandSchemaHelper.AddCommandToDevice(device, command);
            
            command = CommandSchemaHelper.CreateNewCommand("StartTelemetry");
            CommandSchemaHelper.AddCommandToDevice(device, command);
            
            command = CommandSchemaHelper.CreateNewCommand("StopTelemetry");
            CommandSchemaHelper.AddCommandToDevice(device, command);
            
            command = CommandSchemaHelper.CreateNewCommand("ChangeSetPointTemp");
            CommandSchemaHelper.DefineNewParameterOnCommand(command, "SetPointTemp", "double");
            CommandSchemaHelper.AddCommandToDevice(device, command);
            
            command = CommandSchemaHelper.CreateNewCommand("DiagnosticTelemetry");
            CommandSchemaHelper.DefineNewParameterOnCommand(command, "Active", "boolean");
            CommandSchemaHelper.AddCommandToDevice(device, command);
            
            command = CommandSchemaHelper.CreateNewCommand("ChangeDeviceState");
            CommandSchemaHelper.DefineNewParameterOnCommand(command, "DeviceState", "string");
            CommandSchemaHelper.AddCommandToDevice(device, command);
        }

  /*      public static List<string> GetDefaultDeviceNames()
        {
            long milliTime = DateTime.Now.Millisecond;
            return DefaultDeviceNames.Select(r => string.Concat(r, "_" + milliTime)).ToList();
        }*/
    }
}
