using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.mtcmoscow.SensorHub.Pressure;
using System.Runtime.Serialization;

using System.Threading.Tasks;
using SensorClient.DataModel.Telemetry;

namespace SensorClient.DataModel
{
    public enum PressureUnits {kPa, Pa, MmOfMercury, InchesOfMercury };
    public class PressureSensor : AbstractSensor
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
                SetProperty(ref this._units, value);
            }
        }

        private PressureConsumer consumer;
        public PressureSensor(PressureConsumer consumer, string UniqueName) : base(UniqueName)
        {
            this.consumer = consumer;
            this.Title = "Pressure";
            this.consumer.SessionLost += Consumer_SessionLost;
            this.Units = PressureUnits.kPa;
        }

        private void Consumer_SessionLost(PressureConsumer sender, Windows.Devices.AllJoyn.AllJoynSessionLostEventArgs args)
        {
            if (this.onSensorSessionLost != null)
                this.onSensorSessionLost.Invoke(this);
        }


        protected async override Task<SensorTelemetryData> ReadDataAsync()
        {
            SensorTelemetryData measure = new SensorTelemetryData();            
            

            if (this.Units == PressureUnits.kPa || this.Units == PressureUnits.Pa)
            {
                PressureGetPascalResult pResults = await this.consumer.GetPascalAsync();
                measure.Value = this.Units == PressureUnits.kPa ? pResults.Pascal / 1000 : pResults.Pascal;
                return measure;
            }
            else if (this.Units == PressureUnits.InchesOfMercury)
            {
                PressureGetInchesOfMercuryResult fResult = await this.consumer.GetInchesOfMercuryAsync();
                measure.Value = fResult.InchesOfMercury;
                return measure;
            }else if (this.Units == PressureUnits.InchesOfMercury)
            {
                PressureGetMmOfMercuryResult fResult = await this.consumer.GetMmOfMercuryAsync();
                measure.Value = fResult.MmOfMercury;
                return measure;
            }

            throw new ArgumentException(String.Format("Unknown Units {0} for Pressure", Units));
        }
    }
}
