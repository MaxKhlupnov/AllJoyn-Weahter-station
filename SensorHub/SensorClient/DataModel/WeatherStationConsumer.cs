using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Devices.AllJoyn;
using Windows.Devices;
using SensorClient.Factory;

using com.mtcmoscow.SensorHub.Humidity;
using com.mtcmoscow.SensorHub.Temperature;
using com.mtcmoscow.SensorHub.Pressure;


using RemoteMonitoring.Devices;

//using org.alljoyn.ControlPanel.ControlPanel;


namespace SensorClient.DataModel
{
    public class WeatherStationConsumer
    {
        
        public delegate void HumidityWatcherSessionStarted(HumiditySensor humidity, dynamic device);
        public HumidityWatcherSessionStarted HumiditySensorSessionStarted;
        HumidityWatcher humidityWatcher = null;
        private AllJoynBusAttachment humidityBusAttachment = new AllJoynBusAttachment();

        public delegate void TemperatureWatcherSessionStarted(TemperatureSensor Temperature, dynamic device);
        public TemperatureWatcherSessionStarted TemperatureSensorSessionStarted;
        TemperatureWatcher temperatureWatcher = null;
        private AllJoynBusAttachment temperatureBusAttachment = new AllJoynBusAttachment();

        public delegate void PressureWatcherSessionStarted(PressureSensor Pressure, dynamic device);
        public PressureWatcherSessionStarted PerssureSensorSessionStarted;
        PressureWatcher pressureWatcher = null;
        private AllJoynBusAttachment pressureBusAttachment = new AllJoynBusAttachment();

        /*
        ControlPanelWatcher ControlPanelWatcher = null;
        private AllJoynBusAttachment controlPanelWatcherBusAttachment = new AllJoynBusAttachment();
        */

        public WeatherStationConsumer()
        {
            this.humidityWatcher = new HumidityWatcher(this.humidityBusAttachment);

            this.humidityWatcher.Added += HumidityWatcher_Added;
            this.humidityWatcher.Start();

            this.temperatureWatcher = new TemperatureWatcher(this.temperatureBusAttachment);
            this.temperatureWatcher.Added += TemperatureWatcher_Added;
            this.temperatureWatcher.Start();

            this.pressureWatcher = new PressureWatcher(this.pressureBusAttachment);
            this.pressureWatcher.Added += PressureWatcher_Added;
            this.pressureWatcher.Start();

        /*    this.ControlPanelWatcher = new ControlPanelWatcher(controlPanelWatcherBusAttachment);
            this.ControlPanelWatcher.Added += ControlPanelWatcher_Added;
            this.ControlPanelWatcher.Start();*/

        }

  /*      private async void ControlPanelWatcher_Added(ControlPanelWatcher sender, AllJoynServiceInfo args)
        {
            ControlPanelJoinSessionResult joinResult = await ControlPanelConsumer.JoinSessionAsync(args, sender);
            AllJoynAboutDataView view = await AllJoynAboutDataView.GetDataBySessionPortAsync(args.UniqueName, 
                this.controlPanelWatcherBusAttachment, args.SessionPort);

            if (joinResult.Status == AllJoynStatus.Ok)
            {
                

            }

        }*/

        private async void PressureWatcher_Added(PressureWatcher sender, AllJoynServiceInfo args)
        {            
            PressureJoinSessionResult joinResult = await PressureConsumer.JoinSessionAsync(args, sender);
 //           AllJoynAboutDataView view = await AllJoynAboutDataView.GetDataBySessionPortAsync(args.UniqueName, this.pressureBusAttachment, args.SessionPort);
 //           var device = SensorClient.Factory.AllJoynDeviceFactory.GetAllJoynDevice(view);

            if (joinResult.Status == AllJoynStatus.Ok)
            {
                PressureSensor newSensor = new PressureSensor(joinResult.Consumer, args.UniqueName);
                
                var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
                await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
  //                  if (PerssureSensorSessionStarted != null)
 //                       PerssureSensorSessionStarted.Invoke(newSensor, device);
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

              var device = SensorClient.Factory.AllJoynDeviceFactory.GetAllJoynDevice(view);

            if (joinResult.Status == AllJoynStatus.Ok)
            {
                TemperatureSensor newSensor = new TemperatureSensor(joinResult.Consumer, args.UniqueName);
                
                var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
                await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                    if (TemperatureSensorSessionStarted != null)
                        TemperatureSensorSessionStarted.Invoke(newSensor, device);
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
  //       AllJoynAboutDataView view = await AllJoynAboutDataView.GetDataBySessionPortAsync(args.UniqueName, this.humidityBusAttachment, args.SessionPort);
  //         var device = SensorClient.Factory.AllJoynDeviceFactory.GetAllJoynDevice(view);

            if (joinResult.Status == AllJoynStatus.Ok)
            {
                HumiditySensor newSensor = new HumiditySensor(joinResult.Consumer, args.UniqueName);

                var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
                await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
 //                   if (HumiditySensorSessionStarted != null)
                     //   HumiditySensorSessionStarted.Invoke(newSensor, device);
                });

            }
            else
            {
                throw new Exception(String.Format("Joining the session went wrong for HumiditySensor {0}", args.UniqueName));
            }
        }
    }
}
