using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.ComponentModel;
using Windows.ApplicationModel.Core;
using System.Threading.Tasks;
using Windows.Devices.AllJoyn;
using com.mtcmoscow.SensorHub.Humidity;
using System.Runtime.Serialization;
using SensorClient.DataModel.Telemetry;

namespace SensorClient.DataModel
{    
    public class HumiditySensor : AbstractSensor
    {
        private HumidityConsumer consumer;
       
        public HumiditySensor(HumidityConsumer consumer, string UniqueName) : base(UniqueName)
        {                   
            this.consumer = consumer;
            this.Title = "Humidity";
            this.consumer.SessionLost += Consumer_SessionLost;            
        }

        private void Consumer_SessionLost(HumidityConsumer sender, AllJoynSessionLostEventArgs args)
        {
            ///TODO: Do something if we lost connection with sensor
        }
           

        protected async override Task<SensorTelemetryData> ReadDataAsync()
        {
            HumidityGetRHResult RHResult = await this.consumer.GetRHAsync();
            SensorTelemetryData measure = new SensorTelemetryData();
            measure.UnitOfMeasure = "RH%";
            measure.Value = RHResult.RH;            
            return measure;
        }

    }
}
