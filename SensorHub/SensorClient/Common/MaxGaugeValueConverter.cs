using System;
using Windows.UI.Xaml.Data;

namespace SensorClient.Common
{
    /// <summary>
    /// Value converter that calculate Maximum Gauage Value base d on sensor measure.
    /// </summary>
    public sealed class MaxGaugeValueConverter : IValueConverter
    {
   
        public object Convert(object value, Type targetType, object parameter, string language)
        {           
            if (value is double)
            {
                // Add 20% to the gauage maximum scale
                return ((double)value) + 0.2 * ((double)value);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
       
    }
}
