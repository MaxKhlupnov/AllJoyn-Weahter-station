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

namespace SensorClient.DataModel
{
    public class WeatherShieldViewModel 
    {

        private WeatherStationConsumer weatherStationConsumer;
        
     

        string mutexId = "WeatherStation";
        Mutex mutex;

       
        public SensorsCollection<AbstractSensor> Sensors{ get; set; }



        private ConnectTheDotsHelper connectHelper = null;

        public WeatherShieldViewModel()
        {

            this.Sensors = new SensorsCollection<AbstractSensor>();
            connectHelper = ConnectTheDotsHelper.makeConnectTheDotsHelper();
            // Mutex will be used to ensure only one thread at a time is talking to the shield / isolated storage
            mutex = new Mutex(false, mutexId);

            // Mutex will be used to ensure only one thread at a time is talking to the shield / isolated storage
            // mutex = new Mutex(false, mutexId);

            this.weatherStationConsumer = new WeatherStationConsumer();
            this.weatherStationConsumer.HumiditySensorSessionStarted += SensorStarted;
            this.weatherStationConsumer.TemperatureSensorSessionStarted += SensorStarted;
            this.weatherStationConsumer.PerssureSensorSessionStarted += SensorStarted;

            StartReadThread();
        }

        private void SensorStarted(AbstractSensor sensor)
        {
            lock (this.Sensors)
            {
                this.Sensors.Add(sensor);              
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
                    lock (this.Sensors)
                    {
                        // We have exlusive access to the mutex so can safely read the transfer file
                        foreach (AbstractSensor sensor in this.Sensors)
                            sensor.DoMeasure();
                    }
                });
            

            }, TimeSpan.FromSeconds(2));
        }

    }
}
