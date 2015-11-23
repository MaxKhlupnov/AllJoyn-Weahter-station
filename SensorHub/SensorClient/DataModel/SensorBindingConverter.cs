using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using SensorClient.DataModel.Telemetry;


namespace SensorClient.DataModel
{
   public class SensorBindingConverter: IValueConverter
    {  
            public object Convert(object value, Type targetType, object parameter, string language)
            {
                SensorTelemetryData s = value as SensorTelemetryData;
                return "Hello!";// (MainPage.Current.Scenarios.IndexOf(s) + 1) + ") " + s.Title;
            }

            public object ConvertBack(object value, Type targetType, object parameter, string language)
            {
                return true;
            }        
    }
}
