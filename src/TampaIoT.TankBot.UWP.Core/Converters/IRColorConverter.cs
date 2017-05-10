using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TampaIoT.TankBot.Core.Interfaces;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace TampaIoT.TankBot.UWP.Core.Converters
{
    public class IRColorConverter : IValueConverter
    {
        public SolidColorBrush _greenBrush = new SolidColorBrush(Colors.Green);
        public SolidColorBrush _redBrush = new SolidColorBrush(Colors.Red);
        public SolidColorBrush _grayBrush = new SolidColorBrush(Colors.Gray);
        public SolidColorBrush _silverBrush = new SolidColorBrush(Colors.Silver);

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value is ISensor)
            {
                var sensor = value as ISensor;
                if (sensor.IsOnline)
                {
                    return sensor.Value.ToLower() == "on" ?  _redBrush : _greenBrush;
                }
                else
                {
                    return _grayBrush;
                }
            }
            else
            {
                return _silverBrush;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
