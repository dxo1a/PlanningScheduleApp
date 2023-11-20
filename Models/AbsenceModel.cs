using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanningScheduleApp.Models
{
    public class AbsenceModel
    {
        //public int ID_Absence { get; set; }
        //public int id_Staff { get; set; }
        //public DateTime DateBegin { get; set; }
        //public DateTime DateEnd { get; set; }
        //private string _timeBegin { get; set; }
        //private string _timeEnd { get; set; }

        public int ID_AbsenceRef { get; set; }
        public string Cause { get; set; }
        public bool Type { get; set; }

        //public string TimeBegin => _timeBegin ?? string.Empty;
        //public string TimeEnd => _timeEnd ?? string.Empty;

        public override string ToString()
        {
            return $"{Cause}";
        }
    }
}
