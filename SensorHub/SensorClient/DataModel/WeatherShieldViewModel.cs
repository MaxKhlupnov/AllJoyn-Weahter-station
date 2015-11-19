using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Windows.System.Threading;
using System.ComponentModel;
using System.Windows;
using Windows.ApplicationModel.Core;
using WinRTXamlToolkit.Debugging;
using RemoteMonitoring.Devices;
using SensorClient.DataModel.Telemetry;
using SensorClient.DataModel.WeatherShield;
using SensorClient.Common;

namespace SensorClient.DataModel
{
    public static class WeatherShieldViewModel 
    {
        

        private static WeatherStationConsumer weatherStationConsumer;

        static CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();


        public static SensorsCollection<AbstractSensor> Sensors{ get; set; }

        public static DeviceManager deviceManager = null;

      //  private IoTHubHelper connectHelper = null;

       

        public static void Init()
        {
            if (deviceManager == null)
            {
                var logger = new TraceLogger();
                deviceManager = new DeviceManager(logger, cancellationTokenSource.Token);
                RunAsync();
            }

            Sensors = new SensorsCollection<AbstractSensor>();

            weatherStationConsumer = new WeatherStationConsumer();
            weatherStationConsumer.HumiditySensorSessionStarted += deviceManager.StartDeviceSensorAsync;
            weatherStationConsumer.TemperatureSensorSessionStarted += deviceManager.StartDeviceSensorAsync;
            weatherStationConsumer.PerssureSensorSessionStarted += deviceManager.StartDeviceSensorAsync;                                  
        }


        

        private static void RunAsync()
        {
           ThreadPoolTimer readerTimer = ThreadPoolTimer.CreatePeriodicTimer(async (source) =>
            {
                while (!cancellationTokenSource.Token.IsCancellationRequested)
            {
                DC.Trace("Running");
                try
                {
                    await deviceManager.StartDevicesAsync();
                    await Task.Delay(TimeSpan.FromMinutes(5), cancellationTokenSource.Token);
                }
                catch (TaskCanceledException) { }
            }
            }, TimeSpan.FromSeconds(60));
        }

        /*   
        private WeatherShieldViewModel()
        {

            this.Sensors = new SensorsCollection<AbstractSensor>();
                      
     
            this.weatherStationConsumer = new WeatherStationConsumer();
            this.weatherStationConsumer.HumiditySensorSessionStarted += SensorStarted;
            this.weatherStationConsumer.TemperatureSensorSessionStarted += SensorStarted;
            this.weatherStationConsumer.PerssureSensorSessionStarted += SensorStarted;

            RunAsync().Wait();           
            
        }


        private async void SensorStarted(AbstractSensor sensor, IDevice device)
        {
           
               bool hasMutex = false;
                try
                {
                    await sensor.DoMeasure();
                    hasMutex = mutex.WaitOne(1000);
                    if (hasMutex)
                    {
                        this.Sensors.Add(sensor);
                        DC.Trace("Addedd sensor {0} with id {1}", new object[] { sensor.Title, sensor.UniqueName });
                    }
                }
                finally
                {
                    if (hasMutex)
                    {
                        mutex.ReleaseMutex();
                    }
                }
        }
        private void StartReadThread()
             {

                 // Create a timer-initiated ThreadPool task to read data from AllJoyn
               ThreadPoolTimer readerTimer = ThreadPoolTimer.CreatePeriodicTimer(async (source) =>
                 {
                     // Notify the UI to do an update.

                     var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
                    await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                     {
                         bool hasMutex = false;
                         try
                         {
                             hasMutex = mutex.WaitOne(100);
                             // We have exlusive access to the mutex so can safely read the transfer file
                             if (hasMutex)
                             {
                                 try {
                                     foreach (AbstractSensor sensor in this.Sensors)
                                     {
                                         SensorTelemetryData measure = await sensor.DoMeasure();
                                         //connectHelper.sendMeasure(measure);
                                     }
                                 }catch(Exception ex)
                                 {                               
                                     Debug.WriteLine(ex.Message);
                                 }
                             }
                         }
                         finally
                         {
                             if (hasMutex)
                             {
                                 mutex.ReleaseMutex();
                             }
                         }
                     });


                 }, TimeSpan.FromSeconds(10));

             }

         */
    }
}
