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
            if (value is DateTime date)
            {
                CultureInfo russianCulture = new CultureInfo("ru-RU");
                //string formattedDate = date.ToString("dd.MM.yyyy", russianCulture);
                string dayOfWeek = russianCulture.DateTimeFormat.GetDayName(date.DayOfWeek);

                return $"{dayOfWeek}";
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
