using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlanningScheduleApp.Models
{
    public class DateViewModel : INotifyPropertyChanged
    {
        private DateTime _date;
        private DateTime _todayDate;
        private double _workingHours;
        private bool _isDateMatching;
        private bool _isTodayDateMatching;

        public DateTime Date
        {
            get { return _date; }
            set
            {
                if (_date != value)
                {
                    _date = value;
                    OnPropertyChanged(nameof(Date));
                }
            }
        }

        public DateTime TodayDate
        {
            get { return _todayDate; }
            set
            {
                if (_todayDate != value)
                {
                    _todayDate = value;
                    OnPropertyChanged(nameof(TodayDate));
                }
            }
        }

        public double WorkingHours
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

        public bool IsDateMatching
        {
            get { return _isDateMatching; }
            set
            {
                if (_isDateMatching != value)
                {
                    _isDateMatching = value;
                    OnPropertyChanged(nameof(IsDateMatching));
                }
            }
        }

        public bool IsTodayDateMatching
        {
            get { return _isTodayDateMatching; }
            set
            {
                if (_isTodayDateMatching != value)
                {
                    _isTodayDateMatching = value;
                    OnPropertyChanged(nameof(IsTodayDateMatching));
                }
            }
        }

        public DateViewModel(DateTime date)
        {
            Date = date;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
