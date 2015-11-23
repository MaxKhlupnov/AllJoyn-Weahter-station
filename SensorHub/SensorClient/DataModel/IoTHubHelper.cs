using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using WinRTXamlToolkit.Debugging;
using Microsoft.Azure.Devices.Client;
using SensorClient.DataModel.Telemetry;


namespace SensorClient.DataModel
{
    public class IoTHubHelper
    {

        // App Settings variables
        AppSettings localSettings = new AppSettings();
        private Mutex mutex;

        DeviceClient deviceClient = null;

        private string ConnectionString
        {
            get
            {
                return String.Format("HostName={0}.azure-devices.net;DeviceId={1};SharedAccessKey={2}",
                    localSettings.EventHubName, localSettings.DisplayName, localSettings.Key);
            }
        }

        private IoTHubHelper()
        {

        }

        public static IoTHubHelper initIoTHubHelper()
        {
            IoTHubHelper helper = new IoTHubHelper();

            helper.localSettings.EventHubName = "MtcIoTHub";
            helper.localSettings.DisplayName = "makhlupi";
            helper.localSettings.Key = "P1X+oqyUXWfAoxfr3M00VhIl4x+C+EuPkE9s//fW43o=";
            helper.localSettings.Organization = "Microsoft Technology Center";
            helper.localSettings.Location = "Moscow";
            helper.InitIoTHubConnection();

            helper.mutex = new Mutex(false, "SensorHub");

            return helper;
        }



        private async void InitIoTHubConnection()
        {
            try
            {
                // this.deviceClient = DeviceClient.CreateFromConnectionString(this.ConnectionString, TransportType.Http1);
                this.deviceClient = DeviceClient.CreateFromConnectionString("HostName=MtcIoTHub.azure-devices.net;DeviceId=makhlupi;SharedAccessKey=P1X+oqyUXWfAoxfr3M00VhIl4x+C+EuPkE9s//fW43o=",
                    TransportType.Http1);
                await deviceClient.OpenAsync();

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error IoT hub connection: {0}", ex.Message);
                throw ex;
            }
        }

        private SensorTelemetryData ApplySettingsToMeasure(SensorTelemetryData measure)
        {

            measure.displayname = this.localSettings.DisplayName;
            measure.location = this.localSettings.Location;
            measure.organization = this.localSettings.Organization;

            return measure;
        }

        /// <summary>
        /// Send message to Azure Event Hub using HTTP/REST API
        /// </summary>
        /// <param name="message"></param>
        public async void sendMeasure(SensorTelemetryData measure)
        {
            bool hasMutex = false;

            try
            {
                hasMutex = mutex.WaitOne(1000);
                if (hasMutex)
                {
                    if (this.deviceClient == null)
                        InitIoTHubConnection();

                    measure = ApplySettingsToMeasure(measure);
                    string message = measure.ToJson();
                    var msgBytes = new Message(Encoding.UTF8.GetBytes(message));
                    await this.deviceClient.SendEventAsync(msgBytes);
                    Debug.WriteLine("Sent: {0}", message);
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Exception when sending message:" + ex.Message);
            }
            finally
            {
                if (hasMutex)
                {
                    mutex.ReleaseMutex();
                }
            }
            

        }


        }
    }
