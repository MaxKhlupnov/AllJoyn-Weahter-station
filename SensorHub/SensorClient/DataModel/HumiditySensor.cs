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

namespace SensorClient.DataModel
{
    public class HumiditySensor : ConnectTheDotsSensor
    {
        private HumidityConsumer consumer;

        public HumiditySensor(HumidityConsumer consumer, string UniqueName) 
        {
            base.guid = UniqueName;
            base.measurename = "Humidity";
            base.unitofmeasure = "RH%";
            this.consumer = consumer;
            this.consumer.SessionLost += Consumer_SessionLost;            
        }

        private void Consumer_SessionLost(HumidityConsumer sender, AllJoynSessionLostEventArgs args)
        {
            ///TODO: Do something if we lost connection with sensor
        }
           

        public async Task<int> ReadDataAsync()
        {
            HumidityGetRHResult RHResult = await this.consumer.GetRHAsync();
            base.value = RHResult.RH;
            base.timecreated = DateTimeOffset.Now.ToLocalTime().ToString();            
            return RHResult.Status;
        }

    }
}
