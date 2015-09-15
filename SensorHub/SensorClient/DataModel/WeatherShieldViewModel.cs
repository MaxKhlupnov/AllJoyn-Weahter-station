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
    public class WeatherShieldViewModel : INotifyPropertyChanged
    {

        private WeatherStationConsumer weatherStationConsumer;
        private HumiditySensor humiditySensor;        
        private TemperatureSensor temperatureSensor;
        private PressureSensor pressureSensor;

        public event PropertyChangedEventHandler PropertyChanged;

        //  string mutexId = "WeatherStation";
        //  Mutex mutex

        private double _humidity = 0.0;
        public double Humidity
        {
            get
            {
                return this._humidity;
            }
            set
            {
                this._humidity = value;
                RaisePropertyChanged("Humidity");
            }
        }

        private double _temperature = 0.0;
        public double Temperature
        {
            get
            {
                return this._temperature;
            }
            set
            {
                this._temperature = value;
                RaisePropertyChanged("Temperature");
            }
        }


        private double _pressure = 0.0;
        public double Pressure
        {
            get
            {
                return this._pressure;
            }
            set
            {
                this._pressure = value;
                RaisePropertyChanged("Pressure");
            }
        }
        public WeatherShieldViewModel()
        {
            // Mutex will be used to ensure only one thread at a time is talking to the shield / isolated storage
            // mutex = new Mutex(false, mutexId);

            this.weatherStationConsumer = new WeatherStationConsumer();
            this.weatherStationConsumer.HumiditySensorSessionStarted += HumiditySensorStarted;
            this.weatherStationConsumer.TemperatureSensorSessionStarted += TemperatureSensorStarted;
            this.weatherStationConsumer.PerssureSensorSessionStarted += PressureSensorStarted;

            StartReadThread();
        }

        private void PressureSensorStarted(PressureSensor sensor)
        {
            this.pressureSensor = sensor;
        }

        private void TemperatureSensorStarted(TemperatureSensor sensor)
        {
            this.temperatureSensor = sensor;
        }

        private void HumiditySensorStarted(HumiditySensor sensor)
        {
            this.humiditySensor = sensor;
        }

        private void StartReadThread()
        {

            // Create a timer-initiated ThreadPool task to read data from AllJoyn
            ThreadPoolTimer readerTimer = ThreadPoolTimer.CreatePeriodicTimer(async (source) =>
            {

                int Status = -1; //TODO: Status < 0 means = an Error
                if (this.humiditySensor != null)
                    Status = await this.humiditySensor.ReadDataAsync();
                if (this.temperatureSensor != null)
                    Status = await this.temperatureSensor.ReadDataAsync();
                if (this.pressureSensor != null)
                    Status = await this.pressureSensor.ReadDataAsync();

                // Notify the UI to do an update.
                var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
                await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                    if (this.humiditySensor != null)
                        this.Humidity = this.humiditySensor.value;
                    if (this.temperatureSensor != null)
                        this.Temperature = this.temperatureSensor.value;
                    if (this.pressureSensor != null)
                        this.Pressure = this.pressureSensor.value;

                });

            }, TimeSpan.FromSeconds(2));


        }

        private void RaisePropertyChanged(string propertyName)
        {

            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
