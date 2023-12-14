using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
        public string TABEL_ID { get; set; }

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

    public class StaffModel : INotifyPropertyChanged
    {
        public int ID_Schedule { get; set; }
        public int STAFF_ID { get; set; }
        public string TABEL_ID { get; set; }
        public string SHORT_FIO { get; set; }
        public string WorkBegin { get; set; }
        public string WorkEnd { get; set; }
        public string LunchTimeBegin { get; set; }
        public string LunchTimeEnd { get; set; }
        public DateTime DTA { get; set; }
        private double? _workingHours;
        public int? ID_Absence { get; set; }
        public string CauseAbsence { get; set; }
        public DateTime? DateBegin { get; set; }
        public DateTime? DateEnd { get; set; }
        public string TimeBegin { get; set; }
        public string TimeEnd { get; set; }

        public double? TotalHours { get; set; }
        public double? FreeHours { get; set; }
        private double _acceptableFreeHours { get; set; }

        public string Subdivision { get; set; }
        public string Position { get; set; }
        public string Company { get; set; }

        public List<DateAndSchedule> DatesAndSchedules { get; set; }

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

        public string AbsenceTime
        {
            get
            {
                if (TimeBegin != null && TimeEnd != null && TimeBegin != string.Empty && TimeEnd != string.Empty)
                    return $"{TimeBegin} - {TimeEnd}";
                else
                    return "Весь день";
            }
        }

        public string AbsenceDate
        {
            get
            {
                if (DateBegin != null && DateEnd != null)
                    if (DateBegin == DateEnd)
                        return $"{DateBegin:dd.MM.yyyy}";
                    else
                        return $"{DateBegin:dd.MM.yyyy} - {DateEnd:dd.MM.yyyy}";
                else
                    return string.Empty;
            }
        }

        public double? WorkingHours
        {
            get { return _workingHours; }
            set
            {
                if (_workingHours != value)
                {
                    _workingHours = value;
                    OnPropertyChanged(nameof(WorkingHours));
                }
            }
        }

        public double AcceptableFreeHours
        {
            get { return _acceptableFreeHours; }
            set
            {
                if (_acceptableFreeHours != value)
                {
                    _acceptableFreeHours = value;
                    OnPropertyChanged(nameof(AcceptableFreeHours));
                }
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            var other = (StaffModel)obj;

            return STAFF_ID == other.STAFF_ID
                && TABEL_ID == other.TABEL_ID
                && DTA == other.DTA
                && Position == other.Position;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + STAFF_ID.GetHashCode();
                hash = hash * 23 + (TABEL_ID?.GetHashCode() ?? 0);
                hash = hash * 23 + DTA.GetHashCode();
                return hash;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class Company
    {
        public string Name { get; set; }
        public List<Department> Departments { get; set; } = new List<Department>();
    }

    public class Department
    {
        public string Name { get; set; }
        public List<StaffModel> StaffMembers { get; set; } = new List<StaffModel>();
    }

    public class DateAndSchedule
    {
        public DateTime DTA { get; set; }
        public int ID_Schedule { get; set; }
        public int ID_Absence { get; set; }
        public string cellText { get; set; }
        public double? WorkingHours { get; set; }
        public string TimeBegin { get; set; }
        public string TimeEnd { get; set; }
        public DateTime? DateBegin { get; set; }
        public DateTime? DateEnd { get; set; }
        public string WorkBegin { get; set; }
        public string WorkEnd { get; set; }
        public string LunchTimeBegin { get; set; }
        public string LunchTimeEnd { get; set; }
        

        public override string ToString()
        {
            if (cellText == "Р")
                return "Р";
            else if (cellText == "Н")
                return "Н";
            else
                return "U";
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

    public class filterCMB
    {
        public int id { get; set; }
        public string filterName { get; set; } = "";
        public override string ToString() => $"{filterName}";
    }
}