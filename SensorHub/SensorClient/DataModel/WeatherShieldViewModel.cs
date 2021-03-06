﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Windows.System.Threading;
using System.ComponentModel;
using System.Windows;
using Windows.ApplicationModel.Core;
using System.Diagnostics;

using SensorClient.Common;

using RemoteMonitoring.Logging;

namespace SensorClient.DataModel
{
    public class WeatherShieldViewModel 
    {
        private const int SendTelemetryPeriod_Sec = 5; //60 * 60 * 5; 

        private static WeatherStationConsumer weatherStationConsumer;
        private static ILogger logger;
        static CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();


        public SensorsCollection<AbstractSensor> Sensors{ get; set; }

        public DeviceManager deviceManager = null;

        //  private IoTHubHelper connectHelper = null;

        private static Mutex mutex = new Mutex(true, "temporaryUIMutex");

        public WeatherShieldViewModel()
        {
            if (deviceManager == null)
            {
                logger = new TraceLogger();
                deviceManager = new DeviceManager(logger, cancellationTokenSource.Token);
                RunAsync();
            }

            Sensors = new SensorsCollection<AbstractSensor>();

            weatherStationConsumer = new WeatherStationConsumer();
            weatherStationConsumer.HumiditySensorSessionStarted += deviceManager.StartDeviceSensorAsync;
            weatherStationConsumer.TemperatureSensorSessionStarted += deviceManager.StartDeviceSensorAsync;
            weatherStationConsumer.PerssureSensorSessionStarted += deviceManager.StartDeviceSensorAsync;

            weatherStationConsumer.HumiditySensorSessionStarted += SensorStarted;
            weatherStationConsumer.TemperatureSensorSessionStarted += SensorStarted;
            weatherStationConsumer.PerssureSensorSessionStarted += SensorStarted;

            StartUpdateUIThread();

        }




        private void RunAsync()
        {
           ThreadPoolTimer readerTimer = ThreadPoolTimer.CreatePeriodicTimer(async (source) =>
            {
                while (!cancellationTokenSource.Token.IsCancellationRequested)
                {

                    logger.LogInfo("Sending devices telemery starts..");
                    try
                    {
                        await deviceManager.StartDevicesAsync();
                        await Task.Delay(TimeSpan.FromSeconds(SendTelemetryPeriod_Sec), cancellationTokenSource.Token);                    
 
                        
                    }
                    catch (TaskCanceledException) { }
            }
            }, TimeSpan.FromSeconds(SendTelemetryPeriod_Sec));
        }

     


        private async void SensorStarted(AbstractSensor sensor, dynamic device)
        {
           
               bool hasMutex = false;
                try
                {
                    await sensor.DoMeasure();
                    hasMutex = mutex.WaitOne(1000);
                    if (hasMutex)
                    {
                        Sensors.Add(sensor);
                    }
                }
                finally
                {
                    if (hasMutex)
                    {
                        mutex.ReleaseMutex();
                    }
                }
        }

        private void StartUpdateUIThread()
        {

            // Create a timer-initiated ThreadPool task to read data from AllJoyn
                 ThreadPoolTimer readerTimer = ThreadPoolTimer.CreatePeriodicTimer(async (source) =>
                 {
                     // Notify the UI to do an update.

                     var dispatcher = CoreApplication.MainView.CoreWindow.Dispatcher;
                    await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                     {
                         bool hasMutex = false;
                         try
                         {
                             hasMutex = mutex.WaitOne(100);
                             // We have exlusive access to the mutex so can safely read the transfer file
                             if (hasMutex)
                             {
                                 try {
                                     foreach (AbstractSensor sensor in this.Sensors)
                                     {
                                         await sensor.DoMeasure();
                                         //connectHelper.sendMeasure(measure);
                                     }
                                 }catch(Exception ex)
                                 {                               
                                     Debug.WriteLine(ex.Message);
                                 }
                             }
                         }
                         finally
                         {
                             if (hasMutex)
                             {
                                 mutex.ReleaseMutex();
                             }
                         }
                     });


                 }, TimeSpan.FromSeconds(2));
               
        }

    }
}
