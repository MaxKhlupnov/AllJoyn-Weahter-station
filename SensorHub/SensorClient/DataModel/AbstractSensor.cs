using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace SensorClient.DataModel
{
    public abstract class AbstractSensor
    {        
        public string UniqueName { get; private set; }
        public string Title { get; set; }
        public string Location { get; set; }
       
        public ConnectTheDotsMeasure MaxValueMeasure { get; set; }
        public ConnectTheDotsMeasure MinValueMeasure { get; set; }
        public ConnectTheDotsMeasure LastMeasure { get; set; }

        public delegate void SessionLost(ConnectTheDotsMeasure sensor);
        public SessionLost OnSessionLost;

        public AbstractSensor(string UniqueName)
        {
            this.UniqueName = UniqueName;
        }

        protected abstract Task<ConnectTheDotsMeasure> ReadDataAsync();

        public async Task<ConnectTheDotsMeasure> DoMeasure()
        {
            
             ConnectTheDotsMeasure current = await ReadDataAsync();
            
            // Update max and min values
            if (MaxValueMeasure == null || MaxValueMeasure.unitofmeasure != current.unitofmeasure || MaxValueMeasure.value < current.value)
                this.MaxValueMeasure = current;

            if (MinValueMeasure == null || MinValueMeasure.unitofmeasure != current.unitofmeasure || MinValueMeasure.value > current.value)
                this.MinValueMeasure = current;

            this.LastMeasure = current;

            return current;
        }

    }
}
