﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.mtcmoscow.SensorHub.Pressure;
using System.Runtime.Serialization;

using System.Threading.Tasks;

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
                this._units = value;
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
            ///TODO: Do something if we lost connection with sensor
        }


        protected async override Task<ConnectTheDotsMeasure> ReadDataAsync()
        {
            ConnectTheDotsMeasure measure = new ConnectTheDotsMeasure(this.UniqueName, this.Title, Units.ToString());
            measure.timecreated = DateTimeOffset.Now.ToLocalTime().ToString();
            

            if (this.Units == PressureUnits.kPa || this.Units == PressureUnits.Pa)
            {
                PressureGetPascalResult pResults = await this.consumer.GetPascalAsync();
                measure.value = this.Units == PressureUnits.kPa ? pResults.Pascal / 1000 : pResults.Pascal;
                return measure;
            }
            else if (this.Units == PressureUnits.InchesOfMercury)
            {
                PressureGetInchesOfMercuryResult fResult = await this.consumer.GetInchesOfMercuryAsync();
                measure.value = fResult.InchesOfMercury;
                return measure;
            }else if (this.Units == PressureUnits.InchesOfMercury)
            {
                PressureGetMmOfMercuryResult fResult = await this.consumer.GetMmOfMercuryAsync();
                measure.value = fResult.MmOfMercury;
                return measure;
            }

            throw new ArgumentException(String.Format("Unknown Units {0} for Pressure", Units));
        }
    }
}
