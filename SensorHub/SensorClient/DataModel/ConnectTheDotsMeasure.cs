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


namespace SensorClient.DataModel
{
    /// <summary>
    /// Class to manage sensor data and attributes 
    /// </summary>
     
    public class ConnectTheDotsMeasure 
    {
        
       
        public string guid { get; set; }
       
        public string displayname { get; set; }
       
        public string organization { get; set; }
       
        public string location { get; set; }
       
        public string measurename { get; set; }
       
        public string unitofmeasure { get; set; }
       
        public string timecreated { get; set; }
       
        public double value { get; set; }

        /// <summary>
        /// Default parameterless constructor needed for serialization of the objects of this class
        /// </summary>
        public ConnectTheDotsMeasure()
        {
        }

        /// <summary>
        /// Construtor taking parameters guid, measurename and unitofmeasure
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="measurename"></param>
        /// <param name="unitofmeasure"></param>
        public ConnectTheDotsMeasure(string guid, string measurename, string unitofmeasure)
        {
            this.guid = guid;
            this.measurename = measurename;
            this.unitofmeasure = unitofmeasure;
        }

        /// <summary>
        /// ToJson function is used to convert sensor data into a JSON string to be sent to Azure Event Hub
        /// </summary>
        /// <returns>JSon String containing all info for sensor data</returns>
        public string ToJson()
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ConnectTheDotsMeasure));
            MemoryStream ms = new MemoryStream();
            ser.WriteObject(ms, this as ConnectTheDotsMeasure);
            string json = Encoding.UTF8.GetString(ms.ToArray(), 0, (int)ms.Length);

            return json;
        }
       
       

        }
    }
