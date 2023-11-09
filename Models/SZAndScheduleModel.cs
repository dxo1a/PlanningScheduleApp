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
        public string DetailNum { get; set; }
        public string DetailName { get; set; }
        public string NUM { get; set; }
        public string PP { get; set; }
        public double? Count { get; set; }
        public string Product { get; set; }
        public double? WorkingHours { get; set; }
        public DateTime DTA { get; set; }
        public string FIO { get; set; }

        public string Detail
        {
            get
            {
                return $"{DetailNum} {DetailName}";
            }
        }

        public string ScheduleFull
        {
            get
            {
                return $"{Product} {Detail} {NUM} {PP} {Cost} {Count} {WorkingHours} {DTA} {FreeHours}";
            }
        }


    }

    public class StaffModel
    {
        public int ID_Schedule { get; set; }
        public int STAFF_ID { get; private set; }
        public string TABEL_ID { get; set; }
        public string SHORT_FIO { get; private set; }
        public string WorkBegin { get; private set; }
        public string WorkEnd { get; private set; }
        public DateTime DTA { get; set; }
        public double? LunchTime { get; private set; }
        public double? WorkingHours { get; set; }
        public string CauseAbsence { get; private set; }
        public DateTime? DateBegin { get; private set; }
        public DateTime? DateEnd { get; private set; }
        public string CauseTimeOff { get; private set; }
        public string TimeBegin { get; private set; }
        public string TimeEnd { get; private set; }

        public double? TotalHours { get; set; }
        public double? FreeHours { get; set; }
        public double AcceptableFreeHours { get; set; }

        public string Subdivision { get; set; }
        public string Position { get; set; }

        public string StaffForSearch
        {
            get
            {
                return $"{STAFF_ID} {TABEL_ID.Trim()} {SHORT_FIO} {CauseAbsence} {DTA}";
            }
        }

        public override string ToString()
        {
            if (TABEL_ID != null)
                return $"{SHORT_FIO} ({TABEL_ID.Trim()})";
            else
                return $"{SHORT_FIO}";
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

    public class ScheduleTemplateModel
    {
        public int ID_Template { get; set; }
        public string TemplateName { get; set; }
        public bool isFlexible { get; set; }
        public int RestingDaysCount { get; set; }
        public int WorkingDaysCount { get; set; }

        public string WorkBegin { get; set; }
        public string WorkEnd { get; set; }
        public double? LunchTime { get; set; }
        public double? WorkingHours { get; set; }

        public string Day { get; set; }
        public bool isRestingDay { get; set; }

        public override string ToString()
        {
            return $"{TemplateName}";
        }
    }
}