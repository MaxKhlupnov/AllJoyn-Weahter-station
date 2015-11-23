using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.ComponentModel;

using SensorClient.DataModel.Telemetry;

namespace SensorClient.DataModel
{
    public abstract class AbstractSensor : INotifyPropertyChanged
    {
        public delegate void SensorSessionLost(AbstractSensor sensor);
        public SensorSessionLost onSensorSessionLost;
        public string UniqueName { get; private set; }

        private string _title = string.Empty;
        public string Title {
            get
            {
                return this._title;
            }
            set
            {
                SetProperty(ref this._title, value);
            }
        }
        public string Location { get; set; }

        private SensorTelemetryData _maxValueMeasure = null;
        public SensorTelemetryData MaxValueMeasure {
            get
            {
                return this._maxValueMeasure;
            }
            set
            {
                SetProperty(ref this._maxValueMeasure, value);
            }
        }

        private SensorTelemetryData _minValueMeasure = null;
        public SensorTelemetryData MinValueMeasure {
            get
            {
                return this._minValueMeasure;
            }
            set
            {
                SetProperty(ref this._minValueMeasure, value);
            }
        }


        private SensorTelemetryData _lastMeasure = null;
        public SensorTelemetryData LastMeasure {
            get { return this._lastMeasure; }
            set { SetProperty(ref this._lastMeasure, value); }
        }

        public delegate void SessionLost(SensorTelemetryData sensor);
        public SessionLost OnSessionLost;

        public AbstractSensor(string UniqueName)
        {
            this.UniqueName = UniqueName;
        }

        protected abstract Task<SensorTelemetryData> ReadDataAsync();

        public async Task<SensorTelemetryData> DoMeasure()
        {
            
             SensorTelemetryData current = await ReadDataAsync();

            if (current == null)
                throw new Exception(String.Format("Sensor {0} id {1} measure return a null", Title, UniqueName));

             current.TimeCreated = DateTimeOffset.Now.ToLocalTime().ToString();

            // Update max and min values
            if (MaxValueMeasure == null || MaxValueMeasure.UnitOfMeasure != current.UnitOfMeasure || MaxValueMeasure.Value < current.Value)
                this.MaxValueMeasure = current;

            if (MinValueMeasure == null || MinValueMeasure.UnitOfMeasure != current.UnitOfMeasure || MinValueMeasure.Value > current.Value)
                this.MinValueMeasure = current;

            if (_lastMeasure == null || _lastMeasure.Value != current.Value)
                this.LastMeasure = current;
            

            return current;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] String propertyName = null)
        {
            if (object.Equals(storage, value)) return false;

            storage = value;
            this.OnPropertyChanged(propertyName);
            return true;
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var eventHandler = this.PropertyChanged;
            if (eventHandler != null)
            {
                eventHandler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
