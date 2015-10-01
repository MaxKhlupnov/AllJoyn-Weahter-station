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

namespace SensorClient.DataModel
{
    public class WeatherShieldViewModel 
    {

        private WeatherStationConsumer weatherStationConsumer;
        
     

        string mutexId = "WeatherStation";
        Mutex mutex;

       
        public SensorsCollection<AbstractSensor> Sensors{ get; set; }



        private IoTHubHelper connectHelper = null;

        public WeatherShieldViewModel()
        {

            this.Sensors = new SensorsCollection<AbstractSensor>();
            connectHelper = IoTHubHelper.initIoTHubHelper();
            // Mutex will be used to ensure only one thread at a time is talking to the shield / isolated storage
            mutex = new Mutex(true, mutexId);

            // Mutex will be used to ensure only one thread at a time is talking to the shield / isolated storage
            // mutex = new Mutex(false, mutexId);

            this.weatherStationConsumer = new WeatherStationConsumer();
            this.weatherStationConsumer.HumiditySensorSessionStarted += SensorStarted;
            this.weatherStationConsumer.TemperatureSensorSessionStarted += SensorStarted;
            this.weatherStationConsumer.PerssureSensorSessionStarted += SensorStarted;

            StartReadThread();
        }

        private async void SensorStarted(AbstractSensor sensor)
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
                                    ConnectTheDotsMeasure measure = await sensor.DoMeasure();
                                    connectHelper.sendMeasure(measure);
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

    }
}
