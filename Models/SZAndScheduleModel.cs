using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public string LunchTimeBegin { get; private set; }
        public string LunchTimeEnd { get; private set; }
        public DateTime DTA { get; set; }
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

        public string LunchTime
        {
            get
            {
                return $"{LunchTimeBegin} - {LunchTimeEnd}";
            }
        }

        public string WorkTime
        {
            get
            {
                return $"{WorkBegin} - {WorkEnd}";
            }
        }

        public string TimeOff
        {
            get
            {
                if (TimeBegin != null && TimeEnd != null)
                    return $"{TimeBegin} - {TimeEnd}";
                else
                    return string.Empty;
            }
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

    public class ScheduleTemplateModel : INotifyPropertyChanged
    {
        public int ID_Template { get; set; }
        public string TemplateName { get; set; }
        public bool isFlexible { get; set; }
        public int RestingDaysCount { get; set; }
        public int WorkingDaysCount { get; set; }

        private string _workBegin { get; set; }
        private string _workEnd { get; set; }
        private string _lunchTimeBegin { get; set; }
        private string _lunchTimeEnd { get; set; }
        public double? WorkingHours { get; set; }

        public string Day { get; set; }
        private bool _isRestingDay { get; set; }

        public int ID_Day { get; set; }

        public override string ToString()
        {
            return $"{TemplateName}";
        }

        public string WorkBegin
        {
            get { return _workBegin; }
            set
            {
                if (_workBegin != value)
                {
                    _workBegin = value;
                    OnPropertyChanged(nameof(WorkBegin));
                }
            }
        }

        public string WorkEnd
        {
            get { return _workEnd; }
            set
            {
                if (_workEnd != value)
                {
                    _workEnd = value;
                    OnPropertyChanged(nameof(WorkEnd));
                }
            }
        }

        public string LunchTimeBegin
        {
            get { return _lunchTimeBegin; }
            set
            {
                _lunchTimeBegin = value;
                OnPropertyChanged(nameof(LunchTimeBegin));
            }
        }

        public string LunchTimeEnd
        {
            get { return _lunchTimeEnd; }
            set
            {
                _lunchTimeEnd = value;
                OnPropertyChanged(nameof(LunchTimeEnd));
            }
        }

        public bool isRestingDay
        {
            get { return _isRestingDay; }
            set
            {
                if (_isRestingDay != value)
                {
                    _isRestingDay = value;
                    OnPropertyChanged(nameof(isRestingDay));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}