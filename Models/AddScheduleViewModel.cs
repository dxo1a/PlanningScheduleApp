using System.ComponentModel;

namespace PlanningScheduleApp.Models
{
    public class AddScheduleViewModel
    {
        public double _workingHours;

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

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
