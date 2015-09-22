using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Windows.System.Threading;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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
     

        string mutexId = "WeatherStation";
        Mutex mutex;

        private List<AbstractSensor> _sensors;
        public List<AbstractSensor> Sensors
        {
            get { return this._sensors; }
            set { SetProperty(ref this._sensors, value); }
        }


        private double _humidity = 0.0;
        public double Humidity
        {
            get
            {
                return this._humidity;
            }
            set
            {
                SetProperty(ref this._humidity, value);                
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
                SetProperty( ref this._temperature, value);                
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
                SetProperty(ref _pressure, value);
            }
        }

        private ConnectTheDotsHelper connectHelper = null;

        public WeatherShieldViewModel()
        {

            this._sensors = new List<AbstractSensor>(0);
            connectHelper = ConnectTheDotsHelper.makeConnectTheDotsHelper();
            // Mutex will be used to ensure only one thread at a time is talking to the shield / isolated storage
            mutex = new Mutex(false, mutexId);

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
                // Notify the UI to do an update.
                var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
                await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>  {

                    List<AbstractSensor> TheSensors = new List<AbstractSensor>();

                    ConnectTheDotsMeasure measure = null;
                    if (this.humiditySensor != null)
                    {
                        measure = await this.humiditySensor.DoMeasure();
                        this.Humidity = measure.value;
                        connectHelper.sendMeasure(measure);
                        TheSensors.Add(this.humiditySensor);
                    }
                    if (this.temperatureSensor != null)
                    {
                       measure = await this.temperatureSensor.DoMeasure();
                       this.Temperature = measure.value;
                        connectHelper.sendMeasure(measure);
                        TheSensors.Add(this.temperatureSensor);
                    }
                    if (this.pressureSensor != null)
                    {
                        measure = await this.pressureSensor.DoMeasure();
                        this.Pressure = measure.value;
                        TheSensors.Add(this.pressureSensor);
                        connectHelper.sendMeasure(measure);
                    }

                    this.Sensors = TheSensors;
                });

            }, TimeSpan.FromSeconds(2));
        }




        public event PropertyChangedEventHandler PropertyChanged;

        private bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            if (object.Equals(storage, value)) return false;

            storage = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var eventHandler = this.PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
