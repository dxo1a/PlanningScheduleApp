using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace PlanningScheduleApp.Converters
{
    public class MinutesConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && (double)value > 0)
            {
                int minutes = System.Convert.ToInt32(value);
                int hours = minutes / 60;
                int remainingMinutes = minutes % 60;

                string result = string.Empty;

                if (hours > 0)
                {
                    result += $"{hours} ч. ";
                }

                if (remainingMinutes > 0)
                {
                    result += $"{remainingMinutes} мин.";
                }

                return result;
            }
            else
            {
                return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
