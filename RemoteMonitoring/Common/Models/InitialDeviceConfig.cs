namespace RemoteMonitoring.Common.Models
{
    /// <summary>
    /// Device config that is read from a repository to init a set of devices
    /// in a single simulator for testing.
    /// </summary>
    public class InitialDeviceConfig
    {
                
        public string DeviceId { get; set; }
        public string DeviceName { get; set; }
        public string Location { get; set; }
        /// <summary>
        /// IoT Hub Transport type (Http/ AMQP/ MQTT)
        /// </summary>
        public string Transport { get; set; }
        /// <summary>
        /// IoT Hub  SAS Auth Key
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// IoT Hub HostName
        /// </summary>
        public string HostName { get; set; }
    }
}
