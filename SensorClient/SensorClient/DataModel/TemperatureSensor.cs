using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using com.mtcmoscow.SensorHub.Temperature;

namespace SensorClient.DataModel
{
    public enum TemperatureUnits { Celsius, Fahrenheits };
    public class TemperatureSensor : ConnectTheDotsSensor
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
                unitofmeasure = value.ToString();
                this._units = value;
            }
        }

        private TemperatureConsumer consumer;
        public TemperatureSensor(TemperatureConsumer consumer, string UniqueName)
        {
            base.guid = UniqueName;
            base.measurename = "Temperature";
            this.consumer = consumer;
            this.consumer.SessionLost += Consumer_SessionLost;
            this.Units = TemperatureUnits.Celsius;
        }

        private void Consumer_SessionLost(TemperatureConsumer sender, Windows.Devices.AllJoyn.AllJoynSessionLostEventArgs args)
        {
            ///TODO: Do something if we lost connection with sensor
        }


        public async Task<int> ReadDataAsync()
        {
            base.timecreated = DateTimeOffset.Now.ToLocalTime().ToString();

            if (this.Units == TemperatureUnits.Celsius)
            {
                TemperatureGetCelsiusResult cResults =   await this.consumer.GetCelsiusAsync();
                base.value = cResults.Celsius;
                return cResults.Status;
            }else if (this.Units == TemperatureUnits.Fahrenheits)
            {
                TemperatureGetFahrenheitsResult fResult = await this.consumer.GetFahrenheitsAsync();
                base.value = fResult.Fahrenheits;
                return fResult.Status;
            }

            return -1;
        }
    }
}
