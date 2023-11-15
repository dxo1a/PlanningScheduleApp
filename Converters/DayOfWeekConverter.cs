using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace PlanningScheduleApp.Converters
{
    public class DayOfWeekConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string day)
            {
                switch (day)
                {
                    case "Monday": return "Понедельник";
                    case "Tuesday": return "Вторник";
                    case "Wednesday": return "Среда";
                    case "Thursday": return "Четверг";
                    case "Friday": return "Пятница";
                    case "Saturday": return "Суббота";
                    case "Sunday": return "Воскресенье";
                    default: return day;
                }
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
