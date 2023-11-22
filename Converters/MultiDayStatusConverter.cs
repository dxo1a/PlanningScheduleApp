using PlanningScheduleApp.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace PlanningScheduleApp.Converters
{
    public class MultiDayStatusConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length < 2 || !(values[0] is DateTime) || !(values[1] is List<StatusInfo>))
                return string.Empty;

            DateTime date = (DateTime)values[0];
            List<StatusInfo> statusList = (List<StatusInfo>)values[1];

            var statusForDate = statusList.FirstOrDefault(s => s.Date == date);

            return statusForDate != null ? statusForDate.Status : "Н";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
