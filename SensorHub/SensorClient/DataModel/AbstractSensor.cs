using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.ComponentModel;


namespace SensorClient.DataModel
{
    public abstract class AbstractSensor : INotifyPropertyChanged
    {        
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

        private ConnectTheDotsMeasure _maxValueMeasure = null;
        public ConnectTheDotsMeasure MaxValueMeasure {
            get
            {
                return this._maxValueMeasure;
            }
            set
            {
                SetProperty(ref this._maxValueMeasure, value);
            }
        }

        private ConnectTheDotsMeasure _minValueMeasure = null;
        public ConnectTheDotsMeasure MinValueMeasure {
            get
            {
                return this._minValueMeasure;
            }
            set
            {
                SetProperty(ref this._minValueMeasure, value);
            }
        }


        private ConnectTheDotsMeasure _lastMeasure = null;
        public ConnectTheDotsMeasure LastMeasure {
            get { return this._lastMeasure; }
            set { SetProperty(ref this._lastMeasure, value); }
        }

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
