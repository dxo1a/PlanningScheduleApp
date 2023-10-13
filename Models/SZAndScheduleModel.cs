using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace PlanningScheduleApp.Models
{
    public class SZAndScheduleModel
    {
        public int ID_Schedule { get; set; }
        public double? TotalHours { get; set; }
        public double? Cost { get; set; }
        public double? FreeHours { get; set; }
        public string Detail { get; set; }
        public string NUM { get; set; }
        public double? Count { get; set; }
        public string Product { get; set; }
        public double? WorkingHours { get; set; }
        public DateTime DTA { get; set; }
        public string FIO { get; set; }

        public string ScheduleFull
        {
            get
            {
                return $"{Product} {Detail} {NUM} {Cost} {Count} {WorkingHours} {DTA} {FreeHours}";
            }
        }


    }

    public class StaffModel
    {
        public int ID_Schedule { get; set; }
        public string Tabel { get; set; }
        public string FIO { get; set; }
        public string Subdivision { get; set; }
        public string Position { get; set; }
        public int STAFF_ID { get; set; }
        public DateTime DTA { get; set; }
        public double? WorkingHours { get; set; }
        public double? TotalHours { get; set; }
        public double? FreeHours { get; set; }
        public double AcceptableFreeHours { get; set; }

        public string StaffFIOTabel
        {
            get
            {
                return $"{FIO} ({Tabel})";
            }
        }

        public string StaffFull
        {
            get { return $"{FIO} {Tabel} {DTA} {WorkingHours}"; }
        }

        public override string ToString()
        {
            return $"{FIO} ({Tabel})";
        }
    }

    public class DepModel
    {
        public string Position { get; set; }
        public override string ToString()
        {
            return $"{Position}";
        }
    }
}