using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanningScheduleApp.Models
{
    public class ScheduleModel
    {
        public DateTime? DTA { get; set; }
        public double WorkingHours { get; set; }
        public double? TotalHours { get; set; }

        public string ScheduleFull
        {
            get
            {
                return $"{DTA} {WorkingHours}";
            }
        }
    }
}
