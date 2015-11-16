using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Windows.ApplicationModel.Core;


namespace SensorClient.DataModel.Telemetry
{
    /// <summary>
    /// Class to manage sensor data and attributes 
    /// </summary>
     
    public class SensorTelemetryData
    {        
       
        public string DeviceId { get; set; }
                 
        public string MeasureName { get; set; }
       
        public string UnitOfMeasure { get; set; }
       
        public string TimeCreated { get; set; }
       
        public double Value { get; set; }

  
        }
    }
