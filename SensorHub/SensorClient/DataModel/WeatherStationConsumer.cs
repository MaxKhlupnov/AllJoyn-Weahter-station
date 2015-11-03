using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Devices.AllJoyn;
using Windows.Devices;

using com.mtcmoscow.SensorHub.Humidity;
using com.mtcmoscow.SensorHub.Temperature;
using com.mtcmoscow.SensorHub.Pressure;


namespace SensorClient.DataModel
{
    public class WeatherStationConsumer
    {
        
        public delegate void HumidityWatcherSessionStarted(HumiditySensor humidity);
        public HumidityWatcherSessionStarted HumiditySensorSessionStarted;
        HumidityWatcher humidityWatcher = null;
        private AllJoynBusAttachment humidityBusAttachment = new AllJoynBusAttachment();

        public delegate void TemperatureWatcherSessionStarted(TemperatureSensor Temperature);
        public TemperatureWatcherSessionStarted TemperatureSensorSessionStarted;
        TemperatureWatcher temperatureWatcher = null;
        private AllJoynBusAttachment temperatureBusAttachment = new AllJoynBusAttachment();

        public delegate void PressureWatcherSessionStarted(PressureSensor Pressure);
        public PressureWatcherSessionStarted PerssureSensorSessionStarted;
        PressureWatcher pressureWatcher = null;

        public WeatherStationConsumer()
        {
            this.humidityWatcher = new HumidityWatcher(humidityBusAttachment);

            this.humidityWatcher.Added += HumidityWatcher_Added;
            this.humidityWatcher.Start();

            this.temperatureWatcher = new TemperatureWatcher(temperatureBusAttachment);
            this.temperatureWatcher.Added += TemperatureWatcher_Added;
            this.temperatureWatcher.Start();

            this.pressureWatcher = new PressureWatcher(new AllJoynBusAttachment());
            this.pressureWatcher.Added += PressureWatcher_Added;
            this.pressureWatcher.Start();

        }


        private async void PressureWatcher_Added(PressureWatcher sender, AllJoynServiceInfo args)
        {            
            PressureJoinSessionResult joinResult = await PressureConsumer.JoinSessionAsync(args, sender);

            if (joinResult.Status == AllJoynStatus.Ok)
            {
                PressureSensor newSensor = new PressureSensor(joinResult.Consumer, args.UniqueName);
                
                var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
                await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                    if (PerssureSensorSessionStarted != null)
                        PerssureSensorSessionStarted.Invoke(newSensor);
                });

            }
            else
            {
                throw new Exception(String.Format("Joining the session went wrong for PressureSensor {0}", args.UniqueName));
            }
        }

        private async void TemperatureWatcher_Added(TemperatureWatcher sender, AllJoynServiceInfo args)
        {
            TemperatureJoinSessionResult joinResult = await TemperatureConsumer.JoinSessionAsync(args, sender);
            AllJoynAboutDataView view = await AllJoynAboutDataView.GetDataBySessionPortAsync(args.UniqueName, this.temperatureBusAttachment, args.SessionPort);
            
            if (joinResult.Status == AllJoynStatus.Ok)
            {
                TemperatureSensor newSensor = new TemperatureSensor(joinResult.Consumer, args.UniqueName);

                var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
                await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                    if (TemperatureSensorSessionStarted != null)
                        TemperatureSensorSessionStarted.Invoke(newSensor);
                });

            }
            else
            {
                throw new Exception(String.Format("Joining the session went wrong for TemperatureSensor {0}", args.UniqueName));
            }
        }

        private async void HumidityWatcher_Added(HumidityWatcher sender, AllJoynServiceInfo args)
        {

         HumidityJoinSessionResult joinResult = await HumidityConsumer.JoinSessionAsync(args, sender);
         AllJoynAboutDataView view = await AllJoynAboutDataView.GetDataBySessionPortAsync(args.UniqueName, this.humidityBusAttachment, args.SessionPort);

            if (joinResult.Status == AllJoynStatus.Ok)
            {
                HumiditySensor newSensor = new HumiditySensor(joinResult.Consumer, args.UniqueName);

                var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
                await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                    if (HumiditySensorSessionStarted != null)
                        HumiditySensorSessionStarted.Invoke(newSensor);
                });

            }
            else
            {
                throw new Exception(String.Format("Joining the session went wrong for HumiditySensor {0}", args.UniqueName));
            }
        }
    }
}
