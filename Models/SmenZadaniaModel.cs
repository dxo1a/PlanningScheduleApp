using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanningScheduleApp.Models
{
    public class SmenZadaniaModel
    {
        public int ID_Schedule { get; set; }
        public double? TotalHours { get; set; }
        public double? Cost { get; set; }
        public double? FreeHours { get; set; }
        public string DetailNum { get; set; }
        public string DetailName { get; set; }
        public string NUM { get; set; }
        public string PP { get; set; }
        public double? Count { get; set; }
        public string Product { get; set; }
        public double? WorkingHours { get; set; }
        public DateTime DTA { get; set; }
        public string SHORT_FIO { get; set; }

        public string Detail
        {
            get
            {
                return $"{DetailNum} {DetailName}";
            }
        }

        public double? TotalCost
        {
            get { return Cost * Count; }
        }

        public string ScheduleFull
        {
            get
            {
                return $"{Product} {Detail} {NUM} {PP} {Cost} {Count} {WorkingHours} {DTA} {FreeHours}";
            }
        }
    }
}
