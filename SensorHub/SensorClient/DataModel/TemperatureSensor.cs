using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using com.mtcmoscow.SensorHub.Temperature;

namespace SensorClient.DataModel
{
    public enum TemperatureUnits { Celsius, Fahrenheits };
    public class TemperatureSensor : AbstractSensor
    {
        private TemperatureUnits _units;
        public TemperatureUnits Units
        {
            get
            {
                return this._units;
            }
            set
            {
                SetProperty(ref this._units, value);                
            }
        }

        private TemperatureConsumer consumer;
        public TemperatureSensor(TemperatureConsumer consumer, string UniqueName) : base(UniqueName)
        {                       
            this.consumer = consumer;
            this.Title = "Temperature";
            this.consumer.SessionLost += Consumer_SessionLost;
            this.Units = TemperatureUnits.Celsius;
        }

        private void Consumer_SessionLost(TemperatureConsumer sender, Windows.Devices.AllJoyn.AllJoynSessionLostEventArgs args)
        {
            ///TODO: Do something if we lost connection with sensor
        }


        protected async override Task<ConnectTheDotsMeasure> ReadDataAsync()
        {
            ConnectTheDotsMeasure measure = new ConnectTheDotsMeasure(this.UniqueName, this.Title, Units.ToString());
            measure.timecreated = DateTimeOffset.Now.ToLocalTime().ToString();
            
            if (this.Units == TemperatureUnits.Celsius)
            {
                TemperatureGetCelsiusResult cResults =   await this.consumer.GetCelsiusAsync();
                measure.value = cResults.Celsius;
                return measure;
            }else if (this.Units == TemperatureUnits.Fahrenheits)
            {
                TemperatureGetFahrenheitsResult fResult = await this.consumer.GetFahrenheitsAsync();
                measure.value = fResult.Fahrenheits;
                return measure;
            }

            throw new ArgumentException(String.Format("Unknown Units {0} for Temperature", Units));
        }
    }
}
