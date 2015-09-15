using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.mtcmoscow.SensorHub.Pressure;

using System.Threading.Tasks;

namespace SensorClient.DataModel
{
    public enum PressureUnits {kPa, Pa, MmOfMercury, InchesOfMercury };
    public class PressureSensor : ConnectTheDotsSensor
    {
        private PressureUnits _units;
        public PressureUnits Units
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

        private PressureConsumer consumer;
        public PressureSensor(PressureConsumer consumer, string UniqueName)
        {
            base.guid = UniqueName;
            base.measurename = "Pressure";
            this.consumer = consumer;
            this.consumer.SessionLost += Consumer_SessionLost;
            this.Units = PressureUnits.kPa;
        }

        private void Consumer_SessionLost(PressureConsumer sender, Windows.Devices.AllJoyn.AllJoynSessionLostEventArgs args)
        {
            ///TODO: Do something if we lost connection with sensor
        }


        public async Task<int> ReadDataAsync()
        {
            base.timecreated = DateTimeOffset.Now.ToLocalTime().ToString();

            if (this.Units == PressureUnits.kPa || this.Units == PressureUnits.Pa)
            {
                PressureGetPascalResult pResults = await this.consumer.GetPascalAsync();
                base.value = this.Units == PressureUnits.kPa ? pResults.Pascal / 1000 : pResults.Pascal;
                return pResults.Status;
            }
            else if (this.Units == PressureUnits.InchesOfMercury)
            {
                PressureGetInchesOfMercuryResult fResult = await this.consumer.GetInchesOfMercuryAsync();
                base.value = fResult.InchesOfMercury;
                return fResult.Status;
            }else if (this.Units == PressureUnits.InchesOfMercury)
            {
                PressureGetMmOfMercuryResult fResult = await this.consumer.GetMmOfMercuryAsync();
                base.value = fResult.MmOfMercury;
                return fResult.Status;
            }

            return -1;
        }
    }
}
